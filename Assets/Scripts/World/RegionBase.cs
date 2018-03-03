using Game.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.World
{
    [RequireComponent(typeof(UniqueId))]
    public abstract class RegionBase : MonoBehaviour, IRegionEventHandler
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
        private float localRenderDistanceFar;

        [SerializeField]
        [HideInInspector]
        private float localRenderDistanceInactive;

        private Transform myTransform;
        private SuperRegion superRegion;
        private UniqueId uniqueId;

        private bool isInitialized;
        protected eSubSceneState[] subSceneStates = new eSubSceneState[Enum.GetValues(typeof(eSubSceneLayer)).Length];
        private eRegionMode currentRegionMode = eRegionMode.Inactive;
        private eSubSceneMode currentSubSceneMode;
        private bool hasSubSceneModeChanged = false;
        private float cameraDistance;

        private List<Vector3> boundsCorners;

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

        public Bounds BoundingBox { get { return new Bounds(boundsCentre, boundsSize); } }

        public string Id { get { if (!uniqueId) { uniqueId = GetComponent<UniqueId>(); } return uniqueId.Id; } }

        public SuperRegion SuperRegion { get { return superRegion; } }

        public float RenderDistanceFar { get { return overrideRenderDistances ? localRenderDistanceFar : superRegion.World.RenderDistanceFar; } }

        public float RenderDistanceInactive { get { return overrideRenderDistances ? localRenderDistanceInactive : superRegion.World.RenderDistanceInactive; } }

        public eSubSceneMode CurrentSubSceneMode { get { return currentSubSceneMode; } }

        public float CameraDistance { get { return cameraDistance; } }

        #endregion properties

        //========================================================================================

        #region abstract

        public abstract List<eSubSceneMode> AvailableSubSceneModes { get; }

        protected abstract eSubSceneMode InitialSubSceneMode { get; }

        #endregion abstract

        //========================================================================================

        #region monobehaviour methods

        // #if UNITY_EDITOR
        //         private void Awake()
        //         {
        //             if(instanceId == 0)
        //             {
        //                 instanceId = GetInstanceID();

        //                 if (string.IsNullOrEmpty(id))
        //                 {
        //                     id = Guid.NewGuid().ToString();
        //                 }

        //                 //"save" changes
        //                 UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        //             }
        //             else if(!Application.isPlaying && instanceId != GetInstanceID())
        //             {
        //                 instanceId = GetInstanceID();
        //                 id = Guid.NewGuid().ToString();

        //                 //"save" changes
        //                 UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        //             }
        //         }
        // #endif

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            float part = localRenderDistanceFar * 0.2f;
            if (part < 1)
            {
                part = 1;
            }
            else if (part - (int)part > 0)
            {
                part = (int)part + 1;
            }

            if (localRenderDistanceInactive < localRenderDistanceFar + part)
            {
                localRenderDistanceInactive = localRenderDistanceFar + part;
            }
        }
#endif

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (drawBounds && Application.isEditor)
            {
                Gizmos.color = boundsColour;
                var bounds = BoundingBox;
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }
#endif

        #endregion monobehaviour methods

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

            currentSubSceneMode = InitialSubSceneMode;

            //initializing SubScene states array
            foreach (var subSceneLayer in Enum.GetValues(typeof(eSubSceneLayer)).Cast<eSubSceneLayer>())
            {
                int index = (int)subSceneLayer;
                subSceneStates[index] = eSubSceneState.Unloaded;
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

            isInitialized = true;
        }

        public List<SubSceneJob> UpdateRegion(Transform cameraTransform, List<Vector3> teleportPositions)
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
                foreach (var subScene in SubScenes)
                {
                    if (subScene.SubSceneMode != currentSubSceneMode)
                    {
                        result.Add(CreateUnloadSubSceneJob(subScene.SubSceneMode, subScene.SubSceneLayer));
                    }
                }

                for (int i = 0; i < subSceneStates.Length; i++)
                {
                    if (subSceneStates[i] == eSubSceneState.Loading)
                    {
                        foreach (var value in Enum.GetValues(typeof(eSubSceneMode)).Cast<eSubSceneMode>())
                        {
                            result.Add(CreateUnloadSubSceneJob(value, (eSubSceneLayer)i));
                        }
                    }
                }

                for (int i = 0; i < subSceneStates.Length; i++)
                {
                    subSceneStates[i] = eSubSceneState.Unloaded;
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
            cameraDistance = float.MaxValue;

            if (bounds.Contains(cameraPosition))
            {
                cameraDistance = 0;
            }
            else
            {
                cameraDistance = (bounds.ClosestPoint(cameraPosition) - cameraPosition).magnitude;

                foreach (var teleportPosition in teleportPositions)
                {
                    if (bounds.Contains(teleportPosition))
                    {
                        cameraDistance = 0;
                        break;
                    }
                    else
                    {
                        float dist = (bounds.ClosestPoint(teleportPosition) - teleportPosition).magnitude;
                        dist *= superRegion.World.SecondaryPositionDistanceModifier;

                        if (dist < cameraDistance)
                        {
                            cameraDistance = dist;
                        }
                    }
                }
            }

            //compute distance and switch mode
            if (!isVisible)
            {
                //Debug.Log("Region \"" + name + "\": is not visible!");
                if (currentRegionMode != eRegionMode.Inactive)
                {
                    result = SwitchMode(eRegionMode.Inactive);
                }
            }
            else
            {
                switch (currentRegionMode)
                {
                    case eRegionMode.Near:
                        if (cameraDistance > RenderDistanceInactive * 1.1f)
                        {
                            result = SwitchMode(eRegionMode.Inactive);
                        }
                        else if (cameraDistance > RenderDistanceFar * 1.1f)
                        {
                            result = SwitchMode(eRegionMode.Far);
                        }
                        break;
                    case eRegionMode.Far:
                        if (cameraDistance > RenderDistanceInactive * 1.1f)
                        {
                            result = SwitchMode(eRegionMode.Inactive);
                        }
                        else if (cameraDistance < RenderDistanceFar)
                        {
                            result = SwitchMode(eRegionMode.Near);
                        }
                        break;
                    case eRegionMode.Inactive:
                        if (cameraDistance < RenderDistanceFar)
                        {
                            result = SwitchMode(eRegionMode.Near);
                        }
                        else if (cameraDistance < RenderDistanceInactive)
                        {
                            result = SwitchMode(eRegionMode.Far);
                        }
                        break;
                }
            }

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
                if (subScene.SubSceneMode == currentSubSceneMode && subScene.SubSceneLayer == subSceneLayer)
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
        /// <param name="subSceneMode"></param>
        /// <param name="subSceneList"></param>
        /// <returns></returns>
        public Transform GetSubSceneRoot(eSubSceneLayer subSceneLayer, eSubSceneMode subSceneMode, List<SubScene> subSceneList = null)
        {
            if (subSceneList == null)
            {
                subSceneList = GetAllSubScenes();
            }

            foreach (var subScene in subSceneList)
            {
                if (subScene.SubSceneMode == subSceneMode && subScene.SubSceneLayer == subSceneLayer)
                {
                    return subScene.transform;
                }
            }

            return null;
        }

        #endregion public methods       

        //========================================================================================

        protected void ChangeSubSceneMode(eSubSceneMode newMode)
        {
            if (newMode != currentSubSceneMode && AvailableSubSceneModes.Contains(newMode))
            {
                currentSubSceneMode = newMode;
                hasSubSceneModeChanged = true;
            }
        }

        //========================================================================================

        #region private methods      

        private List<SubSceneJob> SwitchMode(eRegionMode newMode)
        {
            var result = new List<SubSceneJob>();

            switch (newMode)
            {
                case eRegionMode.Near:
                    //load
                    result.Add(CreateLoadSubSceneJob(currentSubSceneMode, eSubSceneLayer.Always));
                    result.Add(CreateLoadSubSceneJob(currentSubSceneMode, eSubSceneLayer.Near));

                    //unload
                    result.Add(CreateUnloadSubSceneJob(currentSubSceneMode, eSubSceneLayer.Far));

                    break;
                case eRegionMode.Far:
                    //load
                    result.Add(CreateLoadSubSceneJob(currentSubSceneMode, eSubSceneLayer.Always));
                    result.Add(CreateLoadSubSceneJob(currentSubSceneMode, eSubSceneLayer.Far));

                    //unload
                    result.Add(CreateUnloadSubSceneJob(currentSubSceneMode, eSubSceneLayer.Near));

                    break;
                case eRegionMode.Inactive:
                    //unload
                    result.Add(CreateUnloadSubSceneJob(currentSubSceneMode, eSubSceneLayer.Always));
                    result.Add(CreateUnloadSubSceneJob(currentSubSceneMode, eSubSceneLayer.Far));
                    result.Add(CreateUnloadSubSceneJob(currentSubSceneMode, eSubSceneLayer.Near));
                    break;
            }

            currentRegionMode = newMode;

            result.RemoveAll(item => item == null);
            return result;
        }

        private SubSceneJob CreateLoadSubSceneJob(eSubSceneMode subSceneMode, eSubSceneLayer subSceneLayer)
        {
            int index = (int)subSceneLayer;
            eSubSceneState state = subSceneStates[index];

            //Debug.LogWarningFormat("RegionBase \"{0}\": CreateSubSceneLoadJob: mode={1}; type={2}; currentState={3}", name, subSceneMode, subSceneType, state);

            if (state == eSubSceneState.Loaded || state == eSubSceneState.Loading)
            {
                return null;
            }
            else
            {
                subSceneStates[index] = eSubSceneState.Loading;
                return new SubSceneJob(this, subSceneMode, subSceneLayer, eSubSceneJobType.Load, OnSubSceneJobDone);
            }
        }

        private SubSceneJob CreateUnloadSubSceneJob(eSubSceneMode subSceneMode, eSubSceneLayer subSceneLayer)
        {
            int index = (int)subSceneLayer;
            eSubSceneState state = subSceneStates[index];

            if (state == eSubSceneState.Unloaded || state == eSubSceneState.Unloading)
            {
                return null;
            }
            else
            {
                subSceneStates[index] = eSubSceneState.Unloading;
                return new SubSceneJob(this, subSceneMode, subSceneLayer, eSubSceneJobType.Unload, OnSubSceneJobDone);
            }
        }

        private void OnSubSceneJobDone(SubSceneJob subSceneJob)
        {
            if (subSceneJob.SubSceneMode != currentSubSceneMode)
            {
                return;
            }

            int index = (int)subSceneJob.SubSceneLayer;

            if (subSceneJob.JobType == eSubSceneJobType.Load)
            {
                if (subSceneStates[index] == eSubSceneState.Loading)
                {
                    subSceneStates[index] = eSubSceneState.Loaded;
                }
                else
                {
                    Debug.LogWarningFormat("SubScene \"{0} {1} {2}\" was loaded but should be unloading ({3})", name, subSceneJob.SubSceneMode, subSceneJob.SubSceneLayer, subSceneStates[index]);
                }

                //initializing all WorldObjects
                var worldObjects = GetComponentsInChildren<IWorldObject>(true);
                for (int i = 0; i < worldObjects.Length; i++)
                {
                    worldObjects[i].Initialize(superRegion.World.GameController, superRegion.Type != eSuperRegionType.Centre);
                }
            }
            else if (subSceneJob.JobType == eSubSceneJobType.Unload)
            {
                if (subSceneStates[index] == eSubSceneState.Unloading)
                {
                    subSceneStates[index] = eSubSceneState.Unloaded;
                }
                else
                {
                    Debug.LogWarningFormat("SubScene \"{0} {1} {2}\" was unloaded but should be loading ({3})", name, subSceneJob.SubSceneMode, subSceneJob.SubSceneLayer, subSceneStates[index]);
                }
            }
        }

#if UNITY_EDITOR
        void IRegionEventHandler.CreateSubScene(eSubSceneMode subSceneMode, eSubSceneLayer subSceneLayer)
        {
            string subScenePath = WorldUtility.GetSubScenePath(gameObject.scene.path, Id, subSceneMode, subSceneLayer);
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
                var rootGO = new GameObject(WorldUtility.GetSubSceneRootName(subSceneMode, subSceneLayer), typeof(SubScene));
                rootGO.GetComponent<SubScene>().Initialize(subSceneMode, subSceneLayer);

                var root = rootGO.transform;
                root.SetParent(transform, false);
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif

#if UNITY_EDITOR
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

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif

#if UNITY_EDITOR
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