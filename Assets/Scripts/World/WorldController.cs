using Game.GameControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.World
{
    public class WorldController : MonoBehaviour
    {
        public static readonly Dictionary<eSuperRegionType, Vector3> SUPERREGION_OFFSETS = new Dictionary<eSuperRegionType, Vector3>()
        {
            {eSuperRegionType.Centre, Vector3.zero },
            {eSuperRegionType.North,        new Vector3(0,0,1) },
            {eSuperRegionType.NorthUp,      new Vector3(0,1,1) },
            {eSuperRegionType.Up,           new Vector3(0,1,0) },
            {eSuperRegionType.SouthUp,      new Vector3(0,1,-1) },
            {eSuperRegionType.South,        new Vector3(0,0,-1) },
            {eSuperRegionType.SouthDown,    new Vector3(0,-1,-1) },
            {eSuperRegionType.Down,         new Vector3(0,-1,0) },
            {eSuperRegionType.NorthDown,    new Vector3(0,-1,1) }
        };

        //###############################################################

        // -- CONSTANTS

        [SerializeField, HideInInspector] private Vector3 worldSize;

        [SerializeField, HideInInspector] private float renderDistanceNear;
        [SerializeField, HideInInspector] private float renderDistanceMedium;
        [SerializeField, HideInInspector] private float renderDistanceFar;

        [SerializeField, HideInInspector] private float preTeleportOffset;
        [SerializeField, HideInInspector] private float secondaryPositionDistanceModifier;

        [SerializeField, HideInInspector] private bool drawBounds;
        [SerializeField, HideInInspector] private bool drawRegionBounds;

        [SerializeField, HideInInspector] private bool showRegionMode;
        [SerializeField, HideInInspector] private Color modeNearColor = Color.red;
        [SerializeField, HideInInspector] private Color modeMediumColor = Color.yellow;
        [SerializeField, HideInInspector] private Color modeFarColor = Color.green;

        [SerializeField, HideInInspector] private bool editorSubScenesLoaded;

        [SerializeField, HideInInspector] private bool unloadInvisibleRegions = false; //should the regions behind the player be unloaded?
        [SerializeField, HideInInspector] private float invisibilityAngle = 90; //if the angle between camera forward and a region is higher than this, the region is invisible.

        [SerializeField, HideInInspector] private int debugResultCount = 10;

        //###############################################################
        //###############################################################

        // -- ATTRIBUTES

        public Vector3 WorldSize { get { return worldSize; } }

        public float RenderDistanceNear { get { return renderDistanceNear; } }
        public float RenderDistanceMedium { get { return renderDistanceMedium; } }
        public float RenderDistanceFar { get { return renderDistanceFar; } }

        public float SecondaryPositionDistanceModifier { get { return secondaryPositionDistanceModifier; } }

        public bool ShowRegionMode { get { return showRegionMode; } }
        public Color ModeNearColor { get { return modeNearColor; } }
        public Color ModeMediumColor { get { return modeMediumColor; } }
        public Color ModeFarColor { get { return modeFarColor; } }

        public bool EditorSubScenesLoaded
        {
            get { return editorSubScenesLoaded; }
#if UNITY_EDITOR
            set
            {
                editorSubScenesLoaded = value;
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }
#endif
        }

        public bool UnloadInvisibleRegions { get { return unloadInvisibleRegions; } }
        public float InvisibilityAngle { get { return invisibilityAngle; } }





        public IGameControllerBase GameController { get; private set; }
        public eWorldControllerState CurrentState { get; private set; }


        private List<RegionBase> RegionList = new List<RegionBase>();
        private List<SubSceneJob> SubSceneJobsList = new List<SubSceneJob>();

        private bool isInitialized = false;
        private bool isJobRunning = false;
        private int CurrentRegionIndex;

        private List<LoadingMeasurement> loadingDebug = new List<LoadingMeasurement>();

        //###############################################################
        //###############################################################

        // -- INITIALIZATION

        /// <summary>
        /// Initializes the GameController.
        /// </summary>
        /// <param name="gameController"></param>
        public void Initialize(IGameControllerBase gameController)
        {
            GameController = gameController;
            RegionList.Clear();

            CurrentState = eWorldControllerState.Deactivated;

            var initialRegions = GetComponentsInChildren<RegionBase>().ToList();

            foreach (var region in initialRegions)
            {
                region.Initialize(this);
                RegionList.Add(region);
            }

            //if (editorSubScenesLoaded)    //no streaming
            //{
            //    foreach (var region in initialRegions)
            //    {
            //        foreach (var child in region.GetAllSubSceneRoots())    //deactivating all subScenes
            //        {
            //            child.gameObject.SetActive(false);
            //        }
            //    }
            //}

            isInitialized = true;
        }

        //###############################################################
        //###############################################################

        // -- INQUIRIES

        //###############################################################
        //###############################################################

        // -- OPERATIONS

        /// <summary>
        /// Activates the WorldController.
        /// </summary>
        public void Activate()
        {
            if (!isInitialized)
            {
                return;
            }
            else if (CurrentState != eWorldControllerState.Deactivated)
            {
                return;
            }

            CurrentState = eWorldControllerState.Activating;
            CurrentRegionIndex = 0;

            foreach (var region in RegionList)
            {
                UpdateRegion(region);
            }
            //SubSceneJobsList.Clear();
            //foreach (var region in RegionList)
            //{
            //    SubSceneJobsList.AddRange(UpdateRegion(region));
            //}
            //SubSceneJobsList = SubSceneJobsList.OrderBy(item => item.Priority).ToList();

            StartCoroutine(ActivationCR());
        }

        /// <summary>
        /// Deactivates the WorldController.
        /// </summary>
        public void Deactivate()
        {
            if (!isInitialized)
            {
                return;
            }
            else if (CurrentState != eWorldControllerState.Activated)
            {
                return;
            }

            CurrentState = eWorldControllerState.Deactivating;

            foreach (var region in RegionList)
            {
                region.UnloadAll();
            }

            //SubSceneJobsList.Clear();
            //foreach (var region in RegionList)
            //{
            //    SubSceneJobsList.AddRange(region.UnloadAll());
            //}
            //SubSceneJobsList.RemoveAll(item => item == null);

            StartCoroutine(DeactivationCR());
        }

        public void AddSubSceneJob(SubSceneJob job)
        {
            if (!SubSceneJobsList.Contains(job))
            {
                SubSceneJobsList.Add(job);
            }
        }

        public void RemoveSubSceneJob(SubSceneJob job)
        {
            SubSceneJobsList.Remove(job);
        }


        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (!isInitialized)
            {
                return;
            }

            //***********************************************
            //updating world -> creating new jobs
            if (CurrentState == eWorldControllerState.Activated)
            {
                //updating one super region, getting a list of new jobs
                UpdateRegion(RegionList[CurrentRegionIndex]);

                CurrentRegionIndex++;
                if (CurrentRegionIndex == RegionList.Count)
                {
                    CurrentRegionIndex = 0;
                }
            }

            //***********************************************
            //executing jobs
            if (!isJobRunning && SubSceneJobsList.Count > 0)
            {
                UpdateSubSceneJobQueue();

                var newJob = SubSceneJobsList[0];
                SubSceneJobsList.RemoveAt(0);

                switch (newJob.JobType)
                {
                    case eSubSceneJobType.Load:
                        StartCoroutine(LoadSubSceneCR(newJob));
                        break;
                    case eSubSceneJobType.Unload:
                        StartCoroutine(UnloadSubSceneCR(newJob));
                        break;
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            //validate render distance near
            if (renderDistanceNear < 10)
            {
                renderDistanceNear = 10;
            }

            //validate render distance always
            float part = renderDistanceNear * 0.2f;
            if (part < 1)
            {
                part = 1;
            }
            else if (part - (int)part > 0)
            {
                part = (int)part + 1;
            }

            if (renderDistanceMedium < renderDistanceNear + part)
            {
                renderDistanceMedium = renderDistanceNear + part;
            }

            //validate render distance far
            part = renderDistanceMedium * 0.2f;
            if (part < 1)
            {
                part = 1;
            }
            else if (part - (int)part > 0)
            {
                part = (int)part + 1;
            }

            if (renderDistanceFar < renderDistanceMedium + part)
            {
                renderDistanceFar = renderDistanceMedium + part;
            }

            //
            if (preTeleportOffset < 1)
            {
                preTeleportOffset = 1;
            }

            //
            if (secondaryPositionDistanceModifier < 0)
            {
                secondaryPositionDistanceModifier = 0;
            }

            //
            invisibilityAngle = Mathf.Clamp(invisibilityAngle, 0, 360);

            //
            if (debugResultCount < 1)
            {
                debugResultCount = 1;
            }
        }
#endif

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (drawBounds && Application.isEditor)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(transform.position, worldSize);
            }
        }
#endif

        private void OnDestroy()
        {
            if (loadingDebug.Count == 0)
            {
                return;
            }

            Debug.Log("#######################################################");
            Debug.Log("SubScenes - average loading times");

            loadingDebug = loadingDebug.OrderByDescending(i => i.AverageLoadingTime).ToList();

            for (int i = 0; i < loadingDebug.Count; i++)
            {
                var mes = loadingDebug[i];
                Debug.LogFormat("{0}.  {1}  {2}  {3}  {4}", i, mes.regionName, mes.subSceneLayer, mes.subSceneVariant, mes.AverageLoadingTime);
            }

            Debug.Log("#######################################################");

            Debug.Log("#######################################################");
            Debug.Log("SubScenes - average initialization times");

            loadingDebug = loadingDebug.OrderByDescending(i => i.AverageInitializationTime).ToList();

            for (int i = 0; i < loadingDebug.Count; i++)
            {
                var mes = loadingDebug[i];
                Debug.LogFormat("{0}.  {1}  {2}  {3}  {4}", i, mes.regionName, mes.subSceneLayer, mes.subSceneVariant, mes.AverageInitializationTime);
            }

            Debug.Log("#######################################################");

            //Debug.Log("#######################################################");
            //Debug.Log("SubScenes - average move times");

            //loadingDebug = loadingDebug.OrderByDescending(i => i.AverageMoveTime).ToList();

            //for (int i = 0; i < Mathf.Min(debugResultCount, loadingDebug.Count); i++)
            //{
            //    var mes = loadingDebug[i];
            //    Debug.LogFormat("{0}.  {1}  {2}  {3}  {4}", i, mes.regionName, mes.subSceneLayer, mes.subSceneVariant, mes.AverageMoveTime);
            //}

            //Debug.Log("#######################################################");

            //Debug.Log("#######################################################");
            //Debug.Log("SubScenes - highest loading times");

            //loadingDebug = loadingDebug.OrderByDescending(i => i.highestLoadingTime).ToList();

            //for (int i = 0; i < Mathf.Min(debugResultCount, loadingDebug.Count); i++)
            //{
            //    var mes = loadingDebug[i];
            //    Debug.LogFormat("{0}.  {1}  {2}  {3}  {4}", i, mes.regionName, mes.subSceneLayer, mes.subSceneVariant, mes.highestLoadingTime);
            //}

            //Debug.Log("#######################################################");

            //Debug.Log("#######################################################");
            //Debug.Log("SubScenes - highest initialization times");

            //loadingDebug = loadingDebug.OrderByDescending(i => i.highestInitializationTime).ToList();

            //for (int i = 0; i < Mathf.Min(debugResultCount, loadingDebug.Count); i++)
            //{
            //    var mes = loadingDebug[i];
            //    Debug.LogFormat("{0}.  {1}  {2}  {3}  {4}", i, mes.regionName, mes.subSceneLayer, mes.subSceneVariant, mes.highestInitializationTime);
            //}

            //Debug.Log("#######################################################");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator ActivationCR()
        {
            yield return null;

            while (SubSceneJobsList.Count > 0 /*&& SubSceneJobsList[0].Priority <= renderDistanceMedium + (renderDistanceFar - renderDistanceMedium)*/)
            {
                yield return null;
            }

            CurrentState = eWorldControllerState.Activated;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator DeactivationCR()
        {
            yield return null;

            while (isJobRunning && SubSceneJobsList.Count > 0)
            {
                yield return null;
            }

            CurrentState = eWorldControllerState.Deactivated;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        private void UpdateRegion(RegionBase region)
        {
            var camera_transform = GameController.CameraController.transform;
            var player_position = GameController.PlayerController.CharController.MyTransform.position;
            var teleport_positions = new List<Vector3>();
            var half_size = worldSize * 0.5f;

            //identifying teleport positions
            if (player_position.y > half_size.y - preTeleportOffset)
            {
                var teleport_position = player_position;
                teleport_position.y = -half_size.y;
                teleport_positions.Add(teleport_position);
            }
            else if (player_position.y < -half_size.y + preTeleportOffset)
            {
                var teleport_position = player_position;
                teleport_position.y = half_size.y;
                teleport_positions.Add(teleport_position);
            }

            if (player_position.z > half_size.z - preTeleportOffset)
            {
                var teleport_position = player_position;
                teleport_position.z = -half_size.z;
                teleport_positions.Add(teleport_position);
            }
            else if (player_position.z < -half_size.z + preTeleportOffset)
            {
                var teleport_position = player_position;
                teleport_position.z = half_size.z;
                teleport_positions.Add(teleport_position);
            }

            region.UpdateRegion(camera_transform, player_position, teleport_positions);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateSubSceneJobQueue()
        {
            ////removing jobs that are already in the queue
            //var unnecessaryNewJobs = new List<SubSceneJob>();
            //foreach (var job in newJobs)
            //{
            //    var existingJob = SubSceneJobsList.Where(item =>
            //        item.JobType == job.JobType &&
            //        item.Region.UniqueId == job.Region.UniqueId &&
            //        item.SubSceneLayer == job.SubSceneLayer &&
            //        item.SubSceneVariant == job.SubSceneVariant
            //    ).FirstOrDefault();

            //    if (existingJob != null)
            //    {
            //        unnecessaryNewJobs.Add(job);
            //    }
            //}

            //foreach (var unnecessaryJob in unnecessaryNewJobs)
            //{
            //    newJobs.Remove(unnecessaryJob);
            //}

            ////removing different jobs for same SubScene
            //var deprecatedJobs = new List<SubSceneJob>();
            //foreach (var job in newJobs)
            //{
            //    deprecatedJobs.AddRange(SubSceneJobsList.Where(item =>
            //        item.JobType != job.JobType &&
            //        item.Region.UniqueId == job.Region.UniqueId &&
            //        item.SubSceneVariant == job.SubSceneVariant &&
            //        item.SubSceneLayer == job.SubSceneLayer
            //    ));
            //}

            //foreach (var deprecatedJob in deprecatedJobs)
            //{
            //    SubSceneJobsList.Remove(deprecatedJob);
            //    deprecatedJob.Region.OnSubSceneJobOver(deprecatedJob, false);
            //}

            ////adding new jobs to queue
            //foreach (var job in newJobs)
            //{
            //    SubSceneJobsList.Add(job);
            //}

            /* 
             * Cleaning the queue.
             */
            SubSceneJobsList.RemoveAll(item => item.CurrentState != eSubSceneJobState.Pending);

            /* 
             * Ordering the queue.
             */
            SubSceneJobsList = SubSceneJobsList.OrderBy(item => item.Priority).ToList();
        }

        /// <summary>
        /// Runtime Coroutine that loads a subScene
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        private IEnumerator LoadSubSceneCR(SubSceneJob job)
        {
            isJobRunning = true;
            job.CurrentState = eSubSceneJobState.Active;
            //Debug.LogFormat("Load Job started: {0} {1} {2} {3}", job.Region.SuperRegion.Type, job.Region.name, job.SubSceneVariant, job.SubSceneLayer);

            string sceneName = WorldUtility.GetSubSceneName(job.Region.UniqueId, job.SubSceneVariant, job.SubSceneLayer, eSuperRegionType.Centre);
            var subSceneRoot = job.Region.GetSubSceneRoot(job.SubSceneVariant, job.SubSceneLayer);

            //editor subScenes are loaded (no streaming)
            if (editorSubScenesLoaded)
            {
                if (subSceneRoot)
                {
                    subSceneRoot.gameObject.SetActive(true);
                    yield return null;

                    //initializing all WorldObjects
                    var worldObjects = subSceneRoot.GetComponentsInChildren<IWorldObject>();
                    for (int i = 0; i < worldObjects.Length; i++)
                    {
                        worldObjects[i].Initialize(GameController);
                    }
                    yield return null;
                }
            }
            //streaming
            else
            {
                if (subSceneRoot)
                {
                    Debug.LogWarningFormat("Load Job for existing subScene started! {0} {1} {2} {3}", eSuperRegionType.Centre, job.Region.name, job.SubSceneVariant, job.SubSceneLayer);
                }
                else if (Application.CanStreamedLevelBeLoaded(sceneName))
                {
                    AsyncOperation async = null;

                    //########
                    float time = 0;
                    float duration;
                    LoadingMeasurement measurement = null;


                    measurement = loadingDebug.Where(i =>
                        i.regionName == job.Region.name &&
                        i.subSceneLayer == job.SubSceneLayer &&
                        i.subSceneVariant == job.SubSceneVariant
                    ).FirstOrDefault();

                    if (measurement == null)
                    {
                        measurement = new LoadingMeasurement();
                        measurement.regionName = job.Region.name;
                        measurement.subSceneLayer = job.SubSceneLayer;
                        measurement.subSceneVariant = job.SubSceneVariant;
                        loadingDebug.Add(measurement);
                    }

                    measurement.loadCount++;
                    time = Time.time;
                    //########

                    //if (CurrentState == eWorldControllerState.Activating)
                    //{
                    //    SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                    //    yield return null;
                    //}
                    //else
                    //{
                    async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    async.allowSceneActivation = false;

                    while (async.progress < 0.9f)
                    {
                        yield return null;
                    }
                    //}

                    //########
                    duration = Time.time - time;
                    measurement.loadingTimeSum += duration;
                    if (duration > measurement.highestLoadingTime)
                    {
                        measurement.highestLoadingTime = duration;
                    }

                    time = Time.time;
                    //########

                    //if (CurrentState != eWorldControllerState.Activating)
                    //{
                    async.allowSceneActivation = true;
                    while (!async.isDone)
                    {
                        yield return null;
                    }
                    //}

                    //########
                    duration = Time.time - time;
                    measurement.initializationTimeSum += duration;
                    if (duration > measurement.highestInitializationTime)
                    {
                        measurement.highestInitializationTime = duration;
                    }

                    time = Time.time;
                    //########

                    //move root to open world scene
                    Scene scene = SceneManager.GetSceneByName(sceneName);
                    var root = scene.GetRootGameObjects()[0].transform;
                    SceneManager.MoveGameObjectToScene(root.gameObject, gameObject.scene);

                    //attach the SubScene to its Region
                    root.SetParent(job.Region.transform, true);

                    //########
                    duration = Time.time - time;
                    measurement.moveTimeSum += duration;
                    //########

                    yield return null;

                    //initializing all WorldObjects
                    var worldObjects = root.GetComponentsInChildren<IWorldObject>();
                    for (int i = 0; i < worldObjects.Length; i++)
                    {
                        worldObjects[i].Initialize(GameController);
                    }
                    yield return null;

                    //unload the SubScene Scene
                    async = SceneManager.UnloadSceneAsync(sceneName);

                    while (!async.isDone)
                    {
                        yield return null;
                    }
                }
            }

            //Debug.Log("Load Job done");
            job.CurrentState = eSubSceneJobState.Successfull;
            isJobRunning = false;
        }

        /// <summary>
        /// Runtime Coroutine that unloads a subScene.
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        private IEnumerator UnloadSubSceneCR(SubSceneJob job)
        {
            isJobRunning = true;
            job.CurrentState = eSubSceneJobState.Active;
            //Debug.LogFormat("Unload Job started: {0} {1} {2} {3}", job.Region.SuperRegion.Type, job.Region.name, job.SubSceneVariant, job.SubSceneLayer);

            var subSceneRoot = job.Region.GetSubSceneRoot(job.SubSceneVariant, job.SubSceneLayer);

            if (subSceneRoot)
            {
                subSceneRoot.gameObject.SetActive(false);
                yield return null;

#if UNITY_EDITOR
                if (!editorSubScenesLoaded)
                {
                    Destroy(subSceneRoot.gameObject);
                }
#else
                Destroy(subSceneRoot.gameObject);
#endif
            }

            yield return null;

            //Debug.Log("Unload Job done");
            job.CurrentState = eSubSceneJobState.Successfull;
            isJobRunning = false;
        }

        //###############################################################
        //###############################################################

        private class LoadingMeasurement
        {
            public string regionName;
            public eSubSceneLayer subSceneLayer;
            public eSubSceneVariant subSceneVariant;

            public int loadCount;
            public float loadingTimeSum;
            public float initializationTimeSum;
            public float moveTimeSum;

            public float highestLoadingTime;
            public float highestInitializationTime;

            public float AverageLoadingTime { get { return loadingTimeSum / loadCount; } }
            public float AverageInitializationTime { get { return initializationTimeSum / loadCount; } }
            public float AverageMoveTime { get { return moveTimeSum / loadCount; } }
        }

        //###############################################################
    }
} //end of namespace
