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
    public abstract class RegionBase : UniqueIdOwner
    {
        //###############################################################

        // -- CONSTANTS

        [SerializeField, HideInInspector] private Vector3 boundsCentre;
        [SerializeField, HideInInspector] private Vector3 boundsSize;

        [SerializeField, HideInInspector] private bool overrideRenderDistances;
        [SerializeField, HideInInspector] private float localRenderDistanceNear;
        [SerializeField, HideInInspector] private float localRenderDistanceMedium;
        [SerializeField, HideInInspector] private float localRenderDistanceFar;

        [SerializeField, HideInInspector] bool drawBounds;
        [SerializeField, HideInInspector] Color boundsColour = Color.green;

        [SerializeField, HideInInspector] private bool doNotDuplicate;

        //###############################################################
        //###############################################################

        // -- ATTRIBUTES

        public float PlayerDistance { get; private set; }

        protected WorldController WorldController;

        private Transform myTransform;
        private List<Vector3> boundsCorners;

        private Dictionary<eSubSceneVariant, Dictionary<eSubSceneLayer, List<SubSceneJob>>> SubSceneJobLists;

        private eRegionMode currentRegionMode;
        private eSubSceneVariant currentSubSceneVariant;

        private bool isInitialized;
        private bool hasSubSceneModeChanged;
        private bool firstJobDone;
        private bool validateSubScenes;



        public Bounds BoundingBox { get { return new Bounds(boundsCentre + transform.position, boundsSize); } }

        public float RenderDistanceNear { get { return overrideRenderDistances ? localRenderDistanceNear : WorldController.RenderDistanceNear; } }

        public float RenderDistanceMedium { get { return overrideRenderDistances ? localRenderDistanceMedium : WorldController.RenderDistanceMedium; } }

        public float RenderDistanceFar { get { return overrideRenderDistances ? localRenderDistanceFar : WorldController.RenderDistanceFar; } }

        public eSubSceneVariant CurrentSubSceneVariant { get { return currentSubSceneVariant; } }



        public bool DoNotDuplicate { get { return doNotDuplicate; } }



        public abstract List<eSubSceneVariant> AvailableSubSceneVariants { get; }

        protected abstract eSubSceneVariant InitialSubSceneVariant { get; }


        //###############################################################
        //###############################################################

        // -- INITIALIZATION     

        public virtual void Initialize(WorldController world_controller)
        {
            if (isInitialized)
            {
                return;
            }

            myTransform = transform;
            WorldController = world_controller;

            currentRegionMode = eRegionMode.Inactive;
            currentSubSceneVariant = InitialSubSceneVariant;

            SubSceneJobLists = new Dictionary<eSubSceneVariant, Dictionary<eSubSceneLayer, List<SubSceneJob>>>();
            foreach (var sub_scene_variant in AvailableSubSceneVariants)
            {
                var job_dictionary = new Dictionary<eSubSceneLayer, List<SubSceneJob>>();

                foreach (var sub_scene_layer in Enum.GetValues(typeof(eSubSceneLayer)).Cast<eSubSceneLayer>())
                {
                    job_dictionary.Add(sub_scene_layer, new List<SubSceneJob>());
                }

                SubSceneJobLists.Add(sub_scene_variant, job_dictionary);
            }

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

            foreach (var variant in AvailableSubSceneVariants)
            {
                if (variant != currentSubSceneVariant)
                {
                    foreach (var layer in Enum.GetValues(typeof(eSubSceneLayer)).Cast<eSubSceneLayer>())
                    {
                        CreateUnloadSubSceneJob(variant, layer);
                    }
                }
            }

            isInitialized = true;
        }

        //###############################################################
        //###############################################################

        // -- INQUIRIES

        /// <summary>
        /// Gets the current state of a specific SubScene.
        /// </summary>
        /// <param name="sub_scene_variant"></param>
        /// <param name="sub_scene_layer"></param>
        /// <returns></returns>
        public eSubSceneState GetSubSceneState(eSubSceneVariant sub_scene_variant, eSubSceneLayer sub_scene_layer)
        {
            var sub_scene_root_transform = GetSubSceneRoot(sub_scene_variant, sub_scene_layer);
            var sub_scene_job_list = SubSceneJobLists[sub_scene_variant][sub_scene_layer];

            //if (sub_scene_job_list.Count > 1)
            //{
            //    Debug.LogWarningFormat("Region {0}: There are {1} jobs pending for SubScene {2} {3}!", this.name, sub_scene_job_list.Count, sub_scene_variant, sub_scene_layer);
            //}

            SubSceneJob last_job = null;
            int last_job_index = sub_scene_job_list.Count - 1;

            while (last_job == null && last_job_index >= 0)
            {
                var job = sub_scene_job_list[last_job_index];

                if ((job.CurrentState & (eSubSceneJobState.Aborted)) > 0)
                {
                    last_job = job;
                }
                else
                {
                    last_job_index--;
                }
            }

            if (sub_scene_root_transform != null && sub_scene_root_transform.gameObject.activeSelf)
            {
                if (sub_scene_job_list.Count == 0 || last_job == null)
                {
                    return eSubSceneState.Loaded;
                }
                else if (last_job.JobType == eSubSceneJobType.Unload)
                {
                    return eSubSceneState.Unloading;
                }
                else
                {
                    Debug.LogWarningFormat("Region {0}: SubScene {1} {2} is loaded and the type of the last job is {3}", this.name, sub_scene_variant, sub_scene_layer, last_job.JobType);

                    return eSubSceneState.Loaded;
                }
            }
            else
            {
                if (sub_scene_job_list.Count == 0 || last_job == null)
                {
                    return eSubSceneState.Unloaded;
                }
                if (last_job.JobType == eSubSceneJobType.Load)
                {
                    return eSubSceneState.Loading;
                }
                else
                {
                    Debug.LogWarningFormat("Region {0}: SubScene {1} {2} is loaded and the type of the last job is {3}", this.name, sub_scene_variant, sub_scene_layer, last_job.JobType);

                    return eSubSceneState.Unloaded;
                }
            }
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
        /// Returns the Transform of the SubScene of chosen variant and layer.
        /// </summary>
        /// <param name="sub_scene_variant"></param>
        /// <param name="sub_scene_layer"></param>
        /// <returns></returns>
        public Transform GetSubSceneRoot(eSubSceneVariant sub_scene_variant, eSubSceneLayer sub_scene_layer)
        {
            var subSceneList = GetAllSubScenes();

            foreach (var subScene in subSceneList)
            {
                if (subScene.SubSceneVariant == sub_scene_variant && subScene.SubSceneLayer == sub_scene_layer)
                {
                    return subScene.transform;
                }
            }

            return null;
        }

        public int GetJobIndex(SubSceneJob job)
        {
            return SubSceneJobLists[job.SubSceneVariant][job.SubSceneLayer].IndexOf(job);
        }

        //###############################################################
        //###############################################################

        // -- OPERATIONS

        /// <summary>
        /// Updates the mode of the Region (far, medium, near) depending on the position of the player.
        /// </summary>
        /// <param name="player_position"></param>
        /// <param name="teleport_positions"></param>
        /// <returns></returns>
        public void UpdateRegion(Vector3 player_position, List<Vector3> teleport_positions)
        {
            if (!isInitialized)
            {
                return;
            }

            CleanSubSceneJobLists();

            PlayerDistance = ComputePlayerDistance(player_position, teleport_positions);

            var desired_mode = ComputeDesiredRegionMode();

            if (currentRegionMode != desired_mode)
            {
                SwitchMode(desired_mode);
            }
        }

        /// <summary>
        /// Sets the mode of the Region to "Inactive".
        /// </summary>
        /// <returns></returns>
        public void UnloadAll()
        {
            SwitchMode(eRegionMode.Inactive);
        }

        /// <summary>
        /// Editor method used to switch drawing of the region bounds on and off.
        /// </summary>
        /// <param name="drawBounds"></param>
        public void SetDrawBounds(bool drawBounds)
        {
            this.drawBounds = drawBounds;
        }

        /// <summary>
        /// Editor method that adjusts the size and centre of the region bounds to encompass all contained renderers.
        /// </summary>
        public void AdjustBounds()
        {
            Bounds bounds = new Bounds();
            List<Renderer> renderers = GetComponentsInChildren<Renderer>().ToList();
            Vector3 position = Vector3.zero;

            renderers.RemoveAll(item => item is LineRenderer || item is ParticleSystemRenderer || item is TrailRenderer);

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

            boundsCentre = bounds.center - transform.position;
            boundsSize = bounds.size;
        }

        /// <summary>
        /// 
        /// </summary>
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

            if (localRenderDistanceMedium < localRenderDistanceNear + part)
            {
                localRenderDistanceMedium = localRenderDistanceNear + part;
            }

            //validate render distance far
            part = localRenderDistanceMedium * 0.2f;
            if (part < 1)
            {
                part = 1;
            }
            else if (part - (int)part > 0)
            {
                part = (int)part + 1;
            }

            if (localRenderDistanceFar < localRenderDistanceMedium + part)
            {
                localRenderDistanceFar = localRenderDistanceMedium + part;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="new_variant"></param>
        protected void SwitchVariant(eSubSceneVariant new_variant)
        {
            var previous_mode = currentRegionMode;

            SwitchMode(eRegionMode.Inactive);

            currentSubSceneVariant = new_variant;

            SwitchMode(previous_mode);       
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDrawGizmos()
        {
            if (Application.isPlaying && isInitialized && WorldController.ShowRegionMode)
            {
                var bounds = BoundingBox;
                Color colour = new Color(0, 0, 0, 0);

                switch (currentRegionMode)
                {
                    case eRegionMode.Near:
                        colour = WorldController.ModeNearColor;
                        break;
                    case eRegionMode.Medium:
                        colour = WorldController.ModeMediumColor;
                        break;
                    case eRegionMode.Far:
                        colour = WorldController.ModeFarColor;
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

        /// <summary>
        /// Removes finished or aborted jobs from the lists.
        /// </summary>
        private void CleanSubSceneJobLists()
        {
            foreach (var variant_group in SubSceneJobLists)
            {
                foreach (var layer_pair in variant_group.Value)
                {
                    layer_pair.Value.RemoveAll(item => item.CurrentState == eSubSceneJobState.Aborted || item.CurrentState == eSubSceneJobState.Successfull);
                }
            }
        }

        /// <summary>
        /// Computes the distance between the player and the bounds of the region.
        /// Takes into consideration the positions the player could potentially teleport to.
        /// </summary>
        /// <param name="player_position"></param>
        /// <param name="teleport_positions"></param>
        /// <returns></returns>
        private float ComputePlayerDistance(Vector3 player_position, List<Vector3> teleport_positions)
        {
            float distance = float.MaxValue;
            if (BoundingBox.Contains(player_position))
            {
                distance = 0;
            }
            else
            {
                distance = (BoundingBox.ClosestPoint(player_position) - player_position).magnitude;

                foreach (var teleportPosition in teleport_positions)
                {
                    if (BoundingBox.Contains(teleportPosition))
                    {
                        distance = 0;
                        break;
                    }
                    else
                    {
                        float dist = (BoundingBox.ClosestPoint(teleportPosition) - teleportPosition).magnitude;
                        dist *= WorldController.SecondaryPositionDistanceModifier;

                        if (dist < distance)
                        {
                            distance = dist;
                        }
                    }
                }
            }

            return distance;
        }

        /// <summary>
        /// Computes the desired mode for the region.
        /// Takes into consideration the current mode in order to avoid flickering.
        /// </summary>
        /// <returns></returns>
        private eRegionMode ComputeDesiredRegionMode()
        {
            //MODE NEAR
            //when the player is inside a region it is always active, this is important to keep teleport destinations loaded
            if (currentRegionMode != eRegionMode.Near && PlayerDistance == 0)
            {
                //Debug.LogFormat("{0} {1}: mode switch AAA! dist={2}",superRegion.Type, name, playerDistance);
                //result.AddRange(SwitchRegionMode(eRegionMode.Near));
                return eRegionMode.Near;
            }
            else if (currentRegionMode != eRegionMode.Near && PlayerDistance < RenderDistanceNear)
            {
                //Debug.LogFormat("{0} {1}: mode switch BBB! dist={2}", superRegion.Type, name, playerDistance);
                //result.AddRange(SwitchRegionMode(eRegionMode.Near));
                return eRegionMode.Near;
            }
            //****************************************
            //MODE MEDIUM
            else if (currentRegionMode != eRegionMode.Medium && PlayerDistance > RenderDistanceNear * 1.1f && PlayerDistance < RenderDistanceMedium)
            {
                //Debug.LogFormat("{0} {1}: mode switch CCC! dist={2}", superRegion.Type, name, playerDistance);
                //result.AddRange(SwitchRegionMode(eRegionMode.Medium));
                return eRegionMode.Medium;
            }
            //****************************************
            // MODE FAR
            else if (currentRegionMode != eRegionMode.Far && PlayerDistance > RenderDistanceMedium * 1.1f && PlayerDistance < RenderDistanceFar)
            {
                //Debug.LogFormat("{0} {1}: mode switch DDD! dist={2}", superRegion.Type, name, playerDistance);
                //result.AddRange(SwitchRegionMode(eRegionMode.Far));
                return eRegionMode.Far;
            }
            //****************************************
            // MODE INACTIVE
            //else if (currentRegionMode != eRegionMode.Inactive && !isVisible && playerDistance > 0)
            //{
            //    //Debug.LogFormat("{0} {1}: mode switch EEE! dist={2}", superRegion.Type, name, playerDistance);
            //    //result.AddRange(SwitchRegionMode(eRegionMode.Inactive));
            //    return eRegionMode.Inactive;
            //}
            else if (currentRegionMode != eRegionMode.Inactive && PlayerDistance > RenderDistanceFar * 1.1f)
            {
                //Debug.LogFormat("{0} {1}: mode switch FFF! dist={2}", superRegion.Type, name, playerDistance);
                //result.AddRange(SwitchRegionMode(eRegionMode.Inactive));
                return eRegionMode.Inactive;
            }

            //Debug.LogWarning("RegionBase: ComputeDesiredRegionMode: no valid case!");
            return currentRegionMode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="new_region_mode"></param>
        /// <returns></returns>
        private void SwitchMode(eRegionMode new_region_mode)
        {
            if (currentRegionMode == new_region_mode)
            {
                Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAA");
            }

            switch (new_region_mode)
            {
                case eRegionMode.Near:
                    //load
                    CreateLoadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Always);
                    CreateLoadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Medium);
                    CreateLoadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Near);

                    //unload
                    CreateUnloadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Far);

                    break;
                case eRegionMode.Medium:
                    //load
                    CreateLoadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Always);
                    CreateLoadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Medium);

                    //unload
                    CreateUnloadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Near);
                    CreateUnloadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Far);

                    break;
                case eRegionMode.Far:
                    //load
                    CreateLoadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Always);
                    CreateLoadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Far);

                    //unload
                    CreateUnloadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Near);
                    CreateUnloadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Medium);

                    break;
                case eRegionMode.Inactive:
                    //unload
                    CreateUnloadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Near);
                    CreateUnloadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Medium);
                    CreateUnloadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Far);
                    CreateUnloadSubSceneJob(currentSubSceneVariant, eSubSceneLayer.Always);

                    break;
            }

            currentRegionMode = new_region_mode;
        }

        /// <summary>
        /// Creates a SubSceneJob for loading the chosen SubScene. Does not create the job if it is not necessary.
        /// </summary>
        /// <param name="sub_scene_variant"></param>
        /// <param name="sub_scene_layer"></param>
        /// <returns></returns>
        private void CreateLoadSubSceneJob(eSubSceneVariant sub_scene_variant, eSubSceneLayer sub_scene_layer)
        {
            eSubSceneState sub_scene_state = GetSubSceneState(sub_scene_variant, sub_scene_layer);

            //Debug.LogWarningFormat("RegionBase \"{0}\": CreateSubSceneLoadJob: mode={1}; type={2}; currentState={3}", name, subSceneMode, subSceneType, state);

            if ((sub_scene_state & (eSubSceneState.Loaded | eSubSceneState.Loading)) > 0)
            {
                return;
            }

            if (sub_scene_state == eSubSceneState.Unloading)
            {
                var job_list = SubSceneJobLists[sub_scene_variant][sub_scene_layer];
                var last_job = job_list[job_list.Count - 1];

                if (last_job.CurrentState != eSubSceneJobState.Active && last_job.JobType == eSubSceneJobType.Unload)
                {
                    last_job.CurrentState = eSubSceneJobState.Aborted;

                    if ((GetSubSceneState(sub_scene_variant, sub_scene_layer) & (eSubSceneState.Loaded | eSubSceneState.Loading)) > 0)
                    {
                        return;
                    }
                    else
                    {
                        Debug.LogErrorFormat("Region: CreateLoadSubSceneJob: unload job was aborted but the state of the SubScene is {0}", GetSubSceneState(sub_scene_variant, sub_scene_layer));
                    }
                }
            }

            var new_job = new SubSceneJob(this, sub_scene_variant, sub_scene_layer, eSubSceneJobType.Load);

            SubSceneJobLists[sub_scene_variant][sub_scene_layer].Add(new_job);
            WorldController.AddSubSceneJob(new_job);
        }

        /// <summary>
        /// Creates a SubSceneJob for unloading the chosen SubScene. Does not create the job if it is not necessary.
        /// </summary>
        /// <param name="sub_scene_variant"></param>
        /// <param name="sub_scene_layer"></param>
        /// <returns></returns>
        private void CreateUnloadSubSceneJob(eSubSceneVariant sub_scene_variant, eSubSceneLayer sub_scene_layer)
        {
            eSubSceneState sub_scene_state = GetSubSceneState(sub_scene_variant, sub_scene_layer);

            if ((sub_scene_state & (eSubSceneState.Unloaded | eSubSceneState.Unloading)) > 0)
            {
                return;
            }

            if (sub_scene_state == eSubSceneState.Loading)
            {
                var job_list = SubSceneJobLists[sub_scene_variant][sub_scene_layer];
                var last_job = job_list[job_list.Count - 1];

                if (last_job.CurrentState != eSubSceneJobState.Active && last_job.JobType == eSubSceneJobType.Load)
                {
                    last_job.CurrentState = eSubSceneJobState.Aborted;

                    if ((GetSubSceneState(sub_scene_variant, sub_scene_layer) & (eSubSceneState.Unloaded | eSubSceneState.Unloading)) > 0)
                    {
                        return;
                    }
                    else
                    {
                        Debug.LogErrorFormat("Region: CreateLoadSubSceneJob: load job was aborted but the state of the SubScene is {0}", GetSubSceneState(sub_scene_variant, sub_scene_layer));
                    }
                }
            }

            var new_job = new SubSceneJob(this, sub_scene_variant, sub_scene_layer, eSubSceneJobType.Unload);

            SubSceneJobLists[sub_scene_variant][sub_scene_layer].Add(new_job);
            WorldController.AddSubSceneJob(new_job);
        }

        //###############################################################
    }
} //end of namespace