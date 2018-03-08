using Game.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.World
{
    [RequireComponent(typeof(UniqueId))]
    public abstract class RegionBase : UniqueIdOwner, IRegionEventHandler
    {
        //========================================================================================

        #region member variables

        [SerializeField]
        [HideInInspector]
        private Vector3 boundsCentre;

        [SerializeField]
        [HideInInspector]
        private Vector3 boundsSize;

        [SerializeField]
        [HideInInspector]
        private bool overrideRenderDistances;

        [SerializeField]
        [HideInInspector]
        private float localRenderDistanceNear;

        [SerializeField]
        [HideInInspector]
        private float localRenderDistanceAlways;

        [SerializeField]
        [HideInInspector]
        private float localRenderDistanceFar;

        private Transform myTransform;
        private SuperRegion superRegion;

        private List<Vector3> boundsCorners;

        protected Dictionary<eSubSceneVariant, Dictionary<eSubSceneLayer, eSubSceneState>> subSceneStates;
        private eRegionMode currentRegionMode;
        private eSubSceneVariant currentSubSceneVariant;

        private float playerDistance;
        private List<SubSceneJob> currentJobs = new List<SubSceneJob>();

        private bool isInitialized;
        private bool hasSubSceneModeChanged;
        private bool firstJobDone;
        private bool validateSubScenes;

#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        bool drawBounds;
#endif

#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        Color boundsColour = Color.green;
#endif

        #endregion member variables

        //========================================================================================

        #region properties

        public Bounds BoundingBox { get { return new Bounds(boundsCentre + transform.position, boundsSize); } }

        public SuperRegion SuperRegion { get { return superRegion; } }

        public float RenderDistanceNear { get { return overrideRenderDistances ? localRenderDistanceNear : superRegion.World.RenderDistanceNear; } }

        public float RenderDistanceAlways { get { return overrideRenderDistances ? localRenderDistanceAlways : superRegion.World.RenderDistanceAlways; } }

        public float RenderDistanceFar { get { return overrideRenderDistances ? localRenderDistanceFar : superRegion.World.RenderDistanceFar; } }

        public eSubSceneVariant CurrentSubSceneVariant { get { return currentSubSceneVariant; } }

        public float CameraDistance { get { return playerDistance; } }

        #endregion properties

        //========================================================================================

        #region abstract

        public abstract List<eSubSceneVariant> AvailableSubSceneVariants { get; }

        protected abstract eSubSceneVariant InitialSubSceneVariant { get; }

        #endregion abstract        

        //========================================================================================

        #region public methods

        /// <summary>
        /// Destroys ALL children of the Region.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        public virtual void Initialize(SuperRegion superRegion)
        {
            if (isInitialized)
            {
                return;
            }

            myTransform = transform;
            this.superRegion = superRegion;

            currentRegionMode = eRegionMode.Inactive;
            currentSubSceneVariant = InitialSubSceneVariant;

            //initializing SubScene states dictionary
            subSceneStates = new Dictionary<eSubSceneVariant, Dictionary<eSubSceneLayer, eSubSceneState>>();
            foreach (var subSceneVariant in AvailableSubSceneVariants)
            {
                subSceneStates.Add(subSceneVariant, new Dictionary<eSubSceneLayer, eSubSceneState>());

                foreach (var subSceneLayer in Enum.GetValues(typeof(eSubSceneLayer)).Cast<eSubSceneLayer>())
                {
                    subSceneStates[subSceneVariant].Add(subSceneLayer, eSubSceneState.Unloaded);
                }
            }

            //computing corner positions
            Bounds bounds = BoundingBox;
            boundsCorners = new List<Vector3>(){
                new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z),
                new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z),
                new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z),
                new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z),
                new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z),
                new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z),
                new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z),
                new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z)
            };

            StartCoroutine(ValidateSubScenesCR());
            isInitialized = true;
        }

        public List<SubSceneJob> UpdateRegion(Transform cameraTransform, Vector3 playerPosition, List<Vector3> teleportPositions)
        {
            if (!isInitialized)
            {
                return null;
            }

            Bounds bounds = BoundingBox;
            Vector3 cameraPosition = cameraTransform.position;
            var SubScenes = GetAllSubScenes();

            bool isVisible = false;
            var result = new List<SubSceneJob>();

            //handling SubScene mode change
            if (hasSubSceneModeChanged)
            {
                Debug.Log("Region \"" + name + "\": mode has changed!");

                foreach (var subSceneVariant in subSceneStates.Keys)
                {
                    if (subSceneVariant != currentSubSceneVariant)
                    {
                        foreach (var subSceneLayer in Enum.GetValues(typeof(eSubSceneLayer)).Cast<eSubSceneLayer>())
                        {
                            var subSceneState = subSceneStates[subSceneVariant][subSceneLayer];
                            if (subSceneState == eSubSceneState.Loading || subSceneState == eSubSceneState.Loaded)
                            {
                                result.Add(CreateUnloadSubSceneJob(subSceneVariant, subSceneLayer));
                            }
                        }
                    }
                }

                hasSubSceneModeChanged = false;
            }

            //check if visible
            foreach (var corner in boundsCorners)
            {
                Vector3 vectorToCorner = corner - cameraPosition;

                if (Vector3.Angle(vectorToCorner, cameraTransform.forward) < 90)
                {
                    isVisible = true;
                    break;
                }
            }

            //compute distance
            playerDistance = float.MaxValue;
            if (bounds.Contains(playerPosition))
            {
                playerDistance = 0;
            }
            else
            {
                playerDistance = (bounds.ClosestPoint(playerPosition) - playerPosition).magnitude;

                foreach (var teleportPosition in teleportPositions)
                {
                    if (bounds.Contains(teleportPosition))
                    {
                        playerDistance = 0;
                        break;
                    }
                    else
                    {
                        float dist = (bounds.ClosestPoint(teleportPosition) - teleportPosition).magnitude;
                        dist *= superRegion.World.SecondaryPositionDistanceModifier;

                        if (dist < playerDistance)
                        {
                            playerDistance = dist;
                        }
                    }
                }
            }

            //****************************************
            //switch mode
            //****************************************
            //MODE NEAR
            //when the player is inside a region it is always active, this is important to keep teleport destinations loaded
            if (currentRegionMode != eRegionMode.Near && playerDistance == 0)
            {
                //Debug.LogFormat("{0} {1}: mode switch AAA! dist={2}",superRegion.Type, name, playerDistance);
                result.AddRange(SwitchRegionMode(eRegionMode.Near));
            }
            else if (currentRegionMode != eRegionMode.Near && isVisible && playerDistance < RenderDistanceNear)
            {
                //Debug.LogFormat("{0} {1}: mode switch BBB! dist={2}", superRegion.Type, name, playerDistance);
                result.AddRange(SwitchRegionMode(eRegionMode.Near));
            }
            //****************************************
            //MODE ALWAYS
            else if (currentRegionMode != eRegionMode.Always && isVisible && playerDistance > RenderDistanceNear * 1.1f && playerDistance < RenderDistanceAlways)
            {
                //Debug.LogFormat("{0} {1}: mode switch CCC! dist={2}", superRegion.Type, name, playerDistance);
                result.AddRange(SwitchRegionMode(eRegionMode.Always));
            }
            //****************************************
            // MODE FAR
            else if (currentRegionMode != eRegionMode.Far && isVisible && playerDistance > RenderDistanceAlways * 1.1f && playerDistance < RenderDistanceFar)
            {
                //Debug.LogFormat("{0} {1}: mode switch DDD! dist={2}", superRegion.Type, name, playerDistance);
                result.AddRange(SwitchRegionMode(eRegionMode.Far));
            }
            //****************************************
            // MODE INACTIVE
            else if (currentRegionMode != eRegionMode.Inactive && !isVisible && playerDistance > 0)
            {
                //Debug.LogFormat("{0} {1}: mode switch EEE! dist={2}", superRegion.Type, name, playerDistance);
                result.AddRange(SwitchRegionMode(eRegionMode.Inactive));
            }
            else if (currentRegionMode != eRegionMode.Inactive && playerDistance > RenderDistanceFar * 1.1f)
            {
                //Debug.LogFormat("{0} {1}: mode switch FFF! dist={2}", superRegion.Type, name, playerDistance);
                result.AddRange(SwitchRegionMode(eRegionMode.Inactive));
            }
            //****************************************

            //checks if all the SubScenes are loaded
            if (validateSubScenes && firstJobDone && currentJobs.Count == 0)
            {
                if (currentRegionMode == eRegionMode.Near)
                {
                    if (subSceneStates[currentSubSceneVariant][eSubSceneLayer.Near] != eSubSceneState.Loaded)
                    {
                        Debug.LogWarningFormat("{0} {1}: SubScene Near should be loaded but isn't! currentState={2}",
                            superRegion.Type,
                            name,
                            subSceneStates[currentSubSceneVariant][eSubSceneLayer.Near]
                        );
                        result.Add(CreateLoadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Near));
                    }
                }

                if (currentRegionMode == eRegionMode.Far)
                {
                    if (subSceneStates[currentSubSceneVariant][eSubSceneLayer.Far] != eSubSceneState.Loaded)
                    {
                        Debug.LogWarningFormat("{0} {1}: SubScene Far should be loaded but isn't! currentState={2}",
                            superRegion.Type,
                            name,
                            subSceneStates[currentSubSceneVariant][eSubSceneLayer.Far]
                        );
                        result.Add(CreateLoadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Far));
                    }
                }

                if (currentRegionMode != eRegionMode.Inactive)
                {
                    if (subSceneStates[currentSubSceneVariant][eSubSceneLayer.Always] != eSubSceneState.Loaded)
                    {
                        Debug.LogWarningFormat("{0} {1}: SubScene Always should be loaded but isn't! currentState={2}",
                            superRegion.Type,
                            name,
                            subSceneStates[currentSubSceneVariant][eSubSceneLayer.Always]
                        );
                        result.Add(CreateLoadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Always));
                    }
                }
            } //end of SubScene validation

            //Debug.LogFormat("{0}: jobCount={1}", name, currentJobs);
            validateSubScenes = false;
            return result;
        }

        /// <summary>
        /// Returns a list with the Transforms of all the SubScenes.
        /// </summary>
        /// <returns></returns>
        public List<Transform> GetAllSubSceneRoots()
        {
            var result = new List<Transform>();

            foreach (Transform child in (isInitialized ? myTransform : transform))
            {
                if (child.GetComponent<SubScene>())
                {
                    result.Add(child);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a list of all the SubScenes.
        /// </summary>
        /// <returns></returns>
        public List<SubScene> GetAllSubScenes()
        {
            var result = new List<SubScene>();

            foreach (Transform child in (isInitialized ? myTransform : transform))
            {
                var subScene = child.GetComponent<SubScene>();
                if (subScene)
                {
                    result.Add(subScene);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the Transform of the SubScene of current mode and chosen type.
        /// </summary>
        /// <param name="subSceneLayer"></param>
        /// <param name="subSceneList"></param>
        /// <returns></returns>
        public Transform GetSubSceneRoot(eSubSceneLayer subSceneLayer, List<SubScene> subSceneList = null)
        {
            if (subSceneList == null)
            {
                subSceneList = GetAllSubScenes();
            }

            foreach (var subScene in subSceneList)
            {
                if (subScene.SubSceneVariant == currentSubSceneVariant && subScene.SubSceneLayer == subSceneLayer)
                {
                    return subScene.transform;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the Transform of the SubScene of chosen mode and type.
        /// </summary>
        /// <param name="subSceneLayer"></param>
        /// <param name="subSceneVariant"></param>
        /// <param name="subSceneList"></param>
        /// <returns></returns>
        public Transform GetSubSceneRoot(eSubSceneLayer subSceneLayer, eSubSceneVariant subSceneVariant, List<SubScene> subSceneList = null)
        {
            if (subSceneList == null)
            {
                subSceneList = GetAllSubScenes();
            }

            foreach (var subScene in subSceneList)
            {
                if (subScene.SubSceneVariant == subSceneVariant && subScene.SubSceneLayer == subSceneLayer)
                {
                    return subScene.transform;
                }
            }

            return null;
        }

        #endregion public methods       

        //========================================================================================

        #region monobehaviour methods

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            //validate render distance near
            if (localRenderDistanceNear < 10)
            {
                localRenderDistanceNear = 10;
            }

            //validate render distance always
            float part = localRenderDistanceNear * 0.2f;
            if (part < 1)
            {
                part = 1;
            }
            else if (part - (int)part > 0)
            {
                part = (int)part + 1;
            }

            if (localRenderDistanceAlways < localRenderDistanceNear + part)
            {
                localRenderDistanceAlways = localRenderDistanceNear + part;
            }

            //validate render distance far
            part = localRenderDistanceAlways * 0.2f;
            if (part < 1)
            {
                part = 1;
            }
            else if (part - (int)part > 0)
            {
                part = (int)part + 1;
            }

            if (localRenderDistanceFar < localRenderDistanceAlways + part)
            {
                localRenderDistanceFar = localRenderDistanceAlways + part;
            }
        }
#endif

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if ( Application.isPlaying && isInitialized && superRegion.World.ShowRegionMode)
            {
                var bounds = BoundingBox;
                Color colour = new Color(0, 0, 0, 0);

                switch (currentRegionMode)
                {
                    case eRegionMode.Near:
                        colour = superRegion.World.ModeNearColor;
                        break;
                    case eRegionMode.Always:
                        colour = superRegion.World.ModeAlwaysColor;
                        break;
                    case eRegionMode.Far:
                        colour = superRegion.World.ModeFarColor;
                        break;
                    case eRegionMode.Inactive:
                        break;
                }

                Gizmos.color = colour;
                Gizmos.DrawCube(bounds.center, bounds.size);
            }
            else if (!Application.isPlaying && drawBounds)
            {
                Gizmos.color = boundsColour;
                var bounds = BoundingBox;
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }
#endif

        #endregion monobehaviour methods

        //========================================================================================

        protected void ChangeSubSceneMode(eSubSceneVariant newVariant)
        {
            if (newVariant != currentSubSceneVariant && AvailableSubSceneVariants.Contains(newVariant))
            {
                currentSubSceneVariant = newVariant;
                hasSubSceneModeChanged = true;
            }
        }

        //========================================================================================

        #region private methods      

        private List<SubSceneJob> SwitchRegionMode(eRegionMode newRegionMode)
        {
            var result = new List<SubSceneJob>();

            if (currentRegionMode == newRegionMode)
            {
                return result;
            }

            switch (newRegionMode)
            {
                case eRegionMode.Near:
                    //load
                    result.Add(CreateLoadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Always));
                    result.Add(CreateLoadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Near));

                    //unload
                    result.Add(CreateUnloadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Far));

                    break;
                case eRegionMode.Always:
                    //load
                    result.Add(CreateLoadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Always));

                    //unload
                    result.Add(CreateUnloadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Far));
                    result.Add(CreateUnloadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Near));
                    break;
                case eRegionMode.Far:
                    //load
                    result.Add(CreateLoadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Far));

                    //unload
                    result.Add(CreateUnloadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Near));
                    result.Add(CreateUnloadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Always));
                    break;
                case eRegionMode.Inactive:
                    //unload
                    result.Add(CreateUnloadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Always));
                    result.Add(CreateUnloadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Far));
                    result.Add(CreateUnloadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Near));
                    break;
            }

            currentRegionMode = newRegionMode;

            result.RemoveAll(item => item == null);
            return result;
        }

        /// <summary>
        /// Creates a SubSceneJob for loading the chosen SubScene. Checks if the job is necessary and returns null if it is not.
        /// </summary>
        /// <param name="subSceneVariant"></param>
        /// <param name="subSceneLayer"></param>
        /// <returns></returns>
        private SubSceneJob CreateLoadSubSceneJob(eSubSceneVariant subSceneVariant, eSubSceneLayer subSceneLayer)
        {
            int index = (int)subSceneLayer;
            eSubSceneState subSceneState = subSceneStates[subSceneVariant][subSceneLayer];

            //Debug.LogWarningFormat("RegionBase \"{0}\": CreateSubSceneLoadJob: mode={1}; type={2}; currentState={3}", name, subSceneMode, subSceneType, state);

            if (subSceneState == eSubSceneState.Loaded || subSceneState == eSubSceneState.Loading)
            {
                return null;
            }
            else
            {
                subSceneStates[subSceneVariant][subSceneLayer] = eSubSceneState.Loading;
                var newJob = new SubSceneJob(this, subSceneVariant, subSceneLayer, eSubSceneJobType.Load, OnSubSceneJobFinished);
                currentJobs.Add(newJob);
                return newJob;
            }
        }

        /// <summary>
        /// Creates a SubSceneJob for unloading the chosen SubScene. Checks if the job is necessary and returns null if it is not.
        /// </summary>
        /// <param name="subSceneVariant"></param>
        /// <param name="subSceneLayer"></param>
        /// <returns></returns>
        private SubSceneJob CreateUnloadSubSceneJob(eSubSceneVariant subSceneVariant, eSubSceneLayer subSceneLayer)
        {
            int index = (int)subSceneLayer;
            eSubSceneState state = subSceneStates[subSceneVariant][subSceneLayer];

            if (state == eSubSceneState.Unloaded || state == eSubSceneState.Unloading)
            {
                return null;
            }
            else
            {
                subSceneStates[subSceneVariant][subSceneLayer] = eSubSceneState.Unloading;
                var newJob = new SubSceneJob(this, subSceneVariant, subSceneLayer, eSubSceneJobType.Unload, OnSubSceneJobFinished);
                currentJobs.Add(newJob);
                return newJob;
            }
        }

        /// <summary>
        /// Callback called by the WorldController when a job is done or discarded.
        /// </summary>
        /// <param name="subSceneJob"></param>
        /// <param name="jobDone"></param>
        private void OnSubSceneJobFinished(SubSceneJob subSceneJob, bool jobDone)
        {
            currentJobs.Remove(subSceneJob);
            //Debug.LogFormat("{0} {1}: job done! remaining={2}", superRegion.Type, name, currentJobs.Count);

            if (jobDone)
            {
                firstJobDone = true;
                int index = (int)subSceneJob.SubSceneLayer;

                if (subSceneJob.JobType == eSubSceneJobType.Load)
                {
                    subSceneStates[subSceneJob.SubSceneVariant][subSceneJob.SubSceneLayer] = eSubSceneState.Loaded;
                }
                else if (subSceneJob.JobType == eSubSceneJobType.Unload)
                {
                    subSceneStates[subSceneJob.SubSceneVariant][subSceneJob.SubSceneLayer] = eSubSceneState.Unloaded;
                }
            }
        }

        /// <summary>
        /// Coroutine used to schedule validations.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ValidateSubScenesCR()
        {
            validateSubScenes = true;

            yield return new WaitForSecondsRealtime(1f);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor method that creates a SubScene object and initializes it.
        /// </summary>
        /// <param name="subSceneVariant"></param>
        /// <param name="subSceneLayer"></param>
        void IRegionEventHandler.CreateSubScene(eSubSceneVariant subSceneVariant, eSubSceneLayer subSceneLayer)
        {
            string subScenePath = WorldUtility.GetSubScenePath(gameObject.scene.path, UniqueId, subSceneVariant, subSceneLayer);
            string subScenePathFull = WorldUtility.GetFullPath(subScenePath);

            if (GetSubSceneRoot(subSceneLayer) != null)
            {
                return;
            }
            else if (System.IO.File.Exists(subScenePathFull))
            {
                Debug.LogFormat("SubScene \"{0}\" already exists but is not loaded!", subScenePath);
                return;
            }
            else
            {
                var rootGO = new GameObject(WorldUtility.GetSubSceneRootName(subSceneVariant, subSceneLayer), typeof(SubScene));
                rootGO.GetComponent<SubScene>().Initialize(subSceneVariant, subSceneLayer);

                var root = rootGO.transform;
                root.SetParent(transform, false);
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif

#if UNITY_EDITOR
        /// <summary>
        /// Editor method that adjusts the size and centre of the region bounds to encompass all contained renderers.
        /// </summary>
        void IRegionEventHandler.AdjustBounds()
        {
            Bounds bounds = new Bounds();
            var renderers = GetComponentsInChildren<Renderer>(true);
            Vector3 position = Vector3.zero;

            if (renderers.Count() == 0)
            {
                boundsCentre = transform.position;
                boundsSize = Vector3.zero;
            }
            else
            {
                foreach (var renderer in renderers)
                {
                    position += renderer.bounds.center;
                }

                bounds.center = position / renderers.Count();

                foreach (var renderer in renderers)
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            boundsCentre = bounds.center;
            boundsSize = bounds.size;

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif

#if UNITY_EDITOR
        /// <summary>
        /// Editor method used to switch drawing of the region bounds on and off.
        /// </summary>
        /// <param name="drawBounds"></param>
        void IRegionEventHandler.SetDrawBounds(bool drawBounds)
        {
            this.drawBounds = drawBounds;

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif

        #endregion private methods

        //========================================================================================
    }
} //end of namespace