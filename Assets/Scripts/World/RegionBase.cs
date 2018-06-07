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

        //###############################################################

        // -- ATTRIBUTES

        public SubSceneVariant CurrentSubSceneVariant { get; private set; }
        public float PlayerDistance { get; private set; }

        protected WorldController WorldController;

        private Transform myTransform;
        private Dictionary<SubSceneVariant, Dictionary<SubSceneLayer, List<SubSceneJob>>> SubSceneJobLists;
        private RegionMode currentRegionMode;

        private bool isInitialized;
        private bool hasSubSceneModeChanged;
        private bool firstJobDone;
        private bool validateSubScenes;

        public abstract List<SubSceneVariant> AvailableSubSceneVariants { get; }
        protected abstract SubSceneVariant InitialSubSceneVariant { get; }

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

            currentRegionMode = RegionMode.Inactive;
            CurrentSubSceneVariant = InitialSubSceneVariant;

            SubSceneJobLists = new Dictionary<SubSceneVariant, Dictionary<SubSceneLayer, List<SubSceneJob>>>();
            foreach (var sub_scene_variant in AvailableSubSceneVariants)
            {
                var job_dictionary = new Dictionary<SubSceneLayer, List<SubSceneJob>>();

                foreach (var sub_scene_layer in Enum.GetValues(typeof(SubSceneLayer)).Cast<SubSceneLayer>())
                {
                    job_dictionary.Add(sub_scene_layer, new List<SubSceneJob>());
                }

                SubSceneJobLists.Add(sub_scene_variant, job_dictionary);
            }

            foreach (var variant in AvailableSubSceneVariants)
            {
                if (variant != CurrentSubSceneVariant)
                {
                    foreach (var layer in Enum.GetValues(typeof(SubSceneLayer)).Cast<SubSceneLayer>())
                    {
                        CreateUnloadSubSceneJob(variant, layer);
                    }
                }
            }

            isInitialized = true;
        }

        //###############################################################

        // -- INQUIRIES

        /// <summary>
        /// The Bounding Box of the Region.
        /// </summary>
        public Bounds BoundingBox { get { return new Bounds(boundsCentre + transform.position, boundsSize); } }

        /// <summary>
        /// The distance at which the near mode is activated.
        /// </summary>
        public float RenderDistanceNear { get { return overrideRenderDistances ? localRenderDistanceNear : WorldController.RenderDistanceNear; } }

        /// <summary>
        /// The distance at which the medium mode is activated.
        /// </summary>
        public float RenderDistanceMedium { get { return overrideRenderDistances ? localRenderDistanceMedium : WorldController.RenderDistanceMedium; } }

        /// <summary>
        /// The distance at which the far mode is activated.
        /// </summary>
        public float RenderDistanceFar { get { return overrideRenderDistances ? localRenderDistanceFar : WorldController.RenderDistanceFar; } }

        /// <summary>
        /// Gets the current state of a specific SubScene.
        /// </summary>
        /// <param name="sub_scene_variant"></param>
        /// <param name="sub_scene_layer"></param>
        /// <returns></returns>
        public SubSceneState GetSubSceneState(SubSceneVariant sub_scene_variant, SubSceneLayer sub_scene_layer)
        {
            CleanSubSceneJobLists();

            var sub_scene_root_transform = GetSubSceneRoot(sub_scene_variant, sub_scene_layer);
            var sub_scene_job_list = SubSceneJobLists[sub_scene_variant][sub_scene_layer];

            /*
             * There are currently jobs for the SubScene.
             */
            if (sub_scene_job_list.Count > 0)
            {
                var last_job = sub_scene_job_list[sub_scene_job_list.Count - 1];

                switch (last_job.JobType)
                {
                    case SubSceneJobType.Load:
                        return SubSceneState.Loading;
                    case SubSceneJobType.Unload:
                        return SubSceneState.Unloading;
                    default:
                        Debug.LogError("ERROR!");
                        return SubSceneState.Unloaded;
                }
            }
            /*
             * The Subscene is loaded.
             */
            else if (sub_scene_root_transform != null && sub_scene_root_transform.gameObject.activeSelf)
            {
                return SubSceneState.Loaded;
            }
            /*
             * The Subscene is unloaded.
             */
            else
            {
                return SubSceneState.Unloaded;
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
        public Transform GetSubSceneRoot(SubSceneVariant sub_scene_variant, SubSceneLayer sub_scene_layer)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
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

            //if (currentRegionMode != desired_mode)
            //{
            SwitchMode(desired_mode);
            //}
        }

        /// <summary>
        /// Sets the mode of the Region to "Inactive".
        /// </summary>
        /// <returns></returns>
        public void UnloadAll()
        {
            SwitchMode(RegionMode.Inactive);
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
        protected void SwitchVariant(SubSceneVariant new_variant)
        {
            var previous_mode = currentRegionMode;

            SwitchMode(RegionMode.Inactive);

            CurrentSubSceneVariant = new_variant;

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
                    case RegionMode.Near:
                        colour = WorldController.ModeNearColor;
                        break;
                    case RegionMode.Medium:
                        colour = WorldController.ModeMediumColor;
                        break;
                    case RegionMode.Far:
                        colour = WorldController.ModeFarColor;
                        break;
                    case RegionMode.Inactive:
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
                    layer_pair.Value.RemoveAll(item => item.CurrentState == SubSceneJobState.Aborted || item.CurrentState == SubSceneJobState.Successfull);
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
            List<Vector3> position_list = teleport_positions;
            position_list.Add(player_position);

            foreach (var position in position_list)
            {
                if (BoundingBox.Contains(position))
                {
                    return 0;
                }
                else
                {
                    float new_distance = (BoundingBox.ClosestPoint(position) - position).magnitude;

                    if (new_distance < distance)
                    {
                        distance = new_distance;
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
        private RegionMode ComputeDesiredRegionMode()
        {
            // MODE NEAR
            if (PlayerDistance < RenderDistanceNear)
            {
                return RegionMode.Near;
            }

            // Switch NEAR to MEDIUM
            if (currentRegionMode == RegionMode.Near && PlayerDistance > (RenderDistanceNear * 1.1f) && PlayerDistance < RenderDistanceMedium)
            {
                return RegionMode.Medium;
            }

            //MODE MEDIUM
            else if (PlayerDistance > RenderDistanceNear && PlayerDistance < RenderDistanceMedium)
            {
                return RegionMode.Medium;
            }

            // Switch MEDIUM to FAR
            else if (currentRegionMode == RegionMode.Medium && PlayerDistance > (RenderDistanceMedium * 1.1f) && PlayerDistance < RenderDistanceFar)
            {
                return RegionMode.Far;
            }

            // MODE FAR
            else if (PlayerDistance > RenderDistanceMedium && PlayerDistance < RenderDistanceFar)
            {
                return RegionMode.Far;
            }

            // Switch FAR to INACTIVE

            else if (currentRegionMode == RegionMode.Far && PlayerDistance > (RenderDistanceFar * 1.1f))
            {
                return RegionMode.Inactive;
            }

            // MODE INACTIVE
            else
            {
                Debug.LogErrorFormat("RegionBase {0}: ComputeDesiredRegionMode: error! mode={1} distance={2}", this.name, currentRegionMode.ToString(), PlayerDistance);
                return RegionMode.Inactive;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="new_region_mode"></param>
        /// <returns></returns>
        private void SwitchMode(RegionMode new_region_mode)
        {
            switch (new_region_mode)
            {
                case RegionMode.Near:
                    //load
                    CreateLoadSubSceneJob(CurrentSubSceneVariant, SubSceneLayer.Always);
                    CreateLoadSubSceneJob(CurrentSubSceneVariant, SubSceneLayer.Medium);
                    CreateLoadSubSceneJob(CurrentSubSceneVariant, SubSceneLayer.Near);

                    //unload
                    CreateUnloadSubSceneJob(CurrentSubSceneVariant, SubSceneLayer.Far);

                    break;
                case RegionMode.Medium:
                    //load
                    CreateLoadSubSceneJob(CurrentSubSceneVariant, SubSceneLayer.Always);
                    CreateLoadSubSceneJob(CurrentSubSceneVariant, SubSceneLayer.Medium);

                    //unload
                    CreateUnloadSubSceneJob(CurrentSubSceneVariant, SubSceneLayer.Near);
                    CreateUnloadSubSceneJob(CurrentSubSceneVariant, SubSceneLayer.Far);

                    break;
                case RegionMode.Far:
                    //load
                    CreateLoadSubSceneJob(CurrentSubSceneVariant, SubSceneLayer.Always);
                    CreateLoadSubSceneJob(CurrentSubSceneVariant, SubSceneLayer.Far);

                    //unload
                    CreateUnloadSubSceneJob(CurrentSubSceneVariant, SubSceneLayer.Near);
                    CreateUnloadSubSceneJob(CurrentSubSceneVariant, SubSceneLayer.Medium);

                    break;
                case RegionMode.Inactive:
                    //unload
                    CreateUnloadSubSceneJob(CurrentSubSceneVariant, SubSceneLayer.Near);
                    CreateUnloadSubSceneJob(CurrentSubSceneVariant, SubSceneLayer.Medium);
                    CreateUnloadSubSceneJob(CurrentSubSceneVariant, SubSceneLayer.Far);
                    CreateUnloadSubSceneJob(CurrentSubSceneVariant, SubSceneLayer.Always);

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
        private void CreateLoadSubSceneJob(SubSceneVariant sub_scene_variant, SubSceneLayer sub_scene_layer)
        {
            SubSceneState sub_scene_state = GetSubSceneState(sub_scene_variant, sub_scene_layer);

            /*
             * If the sub scene is loaded or being loaded, no new load job is created.
             */
            if ((sub_scene_state & (SubSceneState.Loaded | SubSceneState.Loading)) > 0)
            {
                return;
            }

            /*
             * If the sub scene is marked for unloading but the job has not started yet, the unload job is aborted.
             */
            if (sub_scene_state == SubSceneState.Unloading)
            {
                var job_list = SubSceneJobLists[sub_scene_variant][sub_scene_layer];
                var last_job = job_list[job_list.Count - 1];

                if (last_job.CurrentState == SubSceneJobState.Pending && last_job.JobType == SubSceneJobType.Unload)
                {
                    last_job.CurrentState = SubSceneJobState.Aborted;

#if UNITY_EDITOR
                    var new_subscene_state = GetSubSceneState(sub_scene_variant, sub_scene_layer);
                    if ((new_subscene_state & (SubSceneState.Loaded | SubSceneState.Loading)) > 0)
                    {
                        Debug.LogErrorFormat("Region {0}: CreateLoadSubsceneJob: unload job was aborted!", this.name);
                        return;
                    }
                    else
                    {
                        Debug.LogErrorFormat("Region {0}: CreateLoadSubsceneJob: unload job was aborted but the state of the SubScene is {1}", this.name, new_subscene_state);
                    }
#endif
                }
            }

            var new_job = new SubSceneJob(this, sub_scene_variant, sub_scene_layer, SubSceneJobType.Load);

            SubSceneJobLists[sub_scene_variant][sub_scene_layer].Add(new_job);
            WorldController.AddSubSceneJob(new_job);
        }

        /// <summary>
        /// Creates a SubSceneJob for unloading the chosen SubScene. Does not create the job if it is not necessary.
        /// </summary>
        /// <param name="sub_scene_variant"></param>
        /// <param name="sub_scene_layer"></param>
        /// <returns></returns>
        private void CreateUnloadSubSceneJob(SubSceneVariant sub_scene_variant, SubSceneLayer sub_scene_layer)
        {
            SubSceneState sub_scene_state = GetSubSceneState(sub_scene_variant, sub_scene_layer);

            /*
             * If the sub scene is unloaded or being unloaded, no new unload job is created.
             */
            if ((sub_scene_state & (SubSceneState.Unloaded | SubSceneState.Unloading)) > 0)
            {
                return;
            }

            /*
             * If the sub scene is marked for loading but the job has not started yet, the load job is aborted.
             */
            if (sub_scene_state == SubSceneState.Loading)
            {
                var job_list = SubSceneJobLists[sub_scene_variant][sub_scene_layer];
                var last_job = job_list[job_list.Count - 1];

                if (last_job.CurrentState == SubSceneJobState.Pending && last_job.JobType == SubSceneJobType.Load)
                {
                    last_job.CurrentState = SubSceneJobState.Aborted;

#if UNITY_EDITOR
                    var new_subscene_state = GetSubSceneState(sub_scene_variant, sub_scene_layer);
                    if ((new_subscene_state & (SubSceneState.Unloaded | SubSceneState.Unloading)) > 0)
                    {
                        Debug.LogErrorFormat("Region {0}: CreateUnloadSubsceneJob: load job was aborted!", this.name);
                        return;
                    }
                    else
                    {
                        Debug.LogErrorFormat("Region {0}: CreateUnloadSubSceneJob: load job was aborted but the state of the SubScene is {0}", this.name, new_subscene_state);
                    }
#endif
                }
            }

            var new_job = new SubSceneJob(this, sub_scene_variant, sub_scene_layer, SubSceneJobType.Unload);

            SubSceneJobLists[sub_scene_variant][sub_scene_layer].Add(new_job);
            WorldController.AddSubSceneJob(new_job);
        }

        //###############################################################
    }
} //end of namespace