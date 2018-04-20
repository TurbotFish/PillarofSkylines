﻿using Game.GameControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.World
{
    public class WorldController : MonoBehaviour/*, IWorldEventHandler*/
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

        //========================================================================================

        #region member variables

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

        [SerializeField, HideInInspector] private bool unloadInvisibleRegions = true; //should the regions behind the player be unloaded?
        [SerializeField, HideInInspector] private float invisibilityAngle = 90; //if the angle between camera forward and a region is higher than this, the region is invisible.

        private IGameControllerBase gameController;

        private List<SuperRegion> superRegionsList = new List<SuperRegion>();
        private List<SubSceneJob> subSceneJobsList = new List<SubSceneJob>();

        private bool isInitialized = false;
        private bool isJobRunning = false;
        private int currentSuperRegionIndex;

        [SerializeField, HideInInspector] private int debugResultCount = 10;
        private List<LoadingMeasurement> loadingDebug = new List<LoadingMeasurement>();

        #endregion member variables 

        //========================================================================================

        #region properties

        public IGameControllerBase GameController { get { return gameController; } }

        public Vector3 WorldSize { get { return worldSize; } }

        public float RenderDistanceNear { get { return renderDistanceNear; } }

        public float RenderDistanceMedium { get { return renderDistanceMedium; } }

        public float RenderDistanceFar { get { return renderDistanceFar; } }

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

        public float SecondaryPositionDistanceModifier { get { return secondaryPositionDistanceModifier; } }

        public int CurrentJobCount { get { return subSceneJobsList.Count; } }

        public bool ShowRegionMode { get { return showRegionMode; } }

        public Color ModeNearColor { get { return modeNearColor; } }

        public Color ModeMediumColor { get { return modeMediumColor; } }

        public Color ModeFarColor { get { return modeFarColor; } }

        public bool UnloadInvisibleRegions { get { return unloadInvisibleRegions; } }

        public float InvisibilityAngle { get { return invisibilityAngle; } }

        public eWorldControllerState CurrentState { get; private set; }

        #endregion properties

        //========================================================================================

        #region public methods

        /// <summary>
        /// Initializes the GameController.
        /// </summary>
        /// <param name="gameController"></param>
        public void Initialize(IGameControllerBase gameController)
        {
            this.gameController = gameController;

            CurrentState = eWorldControllerState.Deactivated;

            //find all the initial regions
            var initialRegions = GetComponentsInChildren<RegionBase>().ToList();

            //if the subScenes are loaded they are used instead of the saved files and the world is not duplicated
            if (editorSubScenesLoaded)
            {
                var superRegion = new GameObject(string.Concat("SuperRegion_", eSuperRegionType.Centre.ToString()), typeof(SuperRegion)).GetComponent<SuperRegion>();
                superRegion.transform.SetParent(transform);
                superRegion.transform.Translate(Vector3.Scale(worldSize, SUPERREGION_OFFSETS[eSuperRegionType.Centre]));

                foreach (var region in initialRegions)
                {
                    region.transform.SetParent(superRegion.transform, true);

                    //deactivating all subScenes
                    foreach (var child in region.GetAllSubSceneRoots())
                    {
                        child.gameObject.SetActive(false);
                    }
                }

                superRegion.Initialize(eSuperRegionType.Centre, this, initialRegions);
                superRegionsList.Add(superRegion);
            }
            //else if the subScenes are not loaded the world is duplicated and the initial (empty) regions are destroyed
            //create SuperRegions and clone the regions once for every SuperRegions
            else
            {
                //"cleaning" the initial regions, just in case
                foreach (var initialRegion in initialRegions)
                {
                    initialRegion.Clear();
                }

                //creating all the superRegions and duplicating the initial regions into them
                foreach (var superRegionType in Enum.GetValues(typeof(eSuperRegionType)).Cast<eSuperRegionType>())
                {
                    var superRegion = new GameObject(string.Concat("SuperRegion_", superRegionType.ToString()), typeof(SuperRegion)).GetComponent<SuperRegion>();
                    superRegion.transform.SetParent(transform);
                    superRegion.transform.Translate(Vector3.Scale(worldSize, SUPERREGION_OFFSETS[superRegionType]));


                    var clonedRegions = new List<RegionBase>();
                    foreach (var initialRegion in initialRegions)
                    {
                        if (superRegionType != eSuperRegionType.Centre && initialRegion.DoNotDuplicate)
                        {
                            continue;
                        }

                        var regionClone = Instantiate(initialRegion.gameObject, superRegion.transform, false);
                        regionClone.name = initialRegion.name;

                        clonedRegions.Add(regionClone.GetComponent<RegionBase>());
                    }

                    superRegion.Initialize(superRegionType, this, clonedRegions);
                    superRegionsList.Add(superRegion);
                }

                //destroy the initial regions
                foreach (var initialRegion in initialRegions)
                {
                    Destroy(initialRegion.gameObject);
                }
            }

            isInitialized = true;
        }

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
            currentSuperRegionIndex = 0;

            subSceneJobsList.Clear();
            foreach (var superRegion in superRegionsList)
            {
                subSceneJobsList.AddRange(CreateNewSubSceneJobs(superRegion));
            }
            subSceneJobsList = subSceneJobsList.OrderBy(item => item.Priority).ToList();

            StartCoroutine(ActivationCR());
        }

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

            subSceneJobsList.Clear();
            foreach (var superRegion in superRegionsList)
            {
                subSceneJobsList.AddRange(superRegion.UnloadAll());
            }

            StartCoroutine(DeactivationCR());
        }

        #endregion public regions

        //========================================================================================

        #region monobehaviour methods

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
                var newJobs = CreateNewSubSceneJobs(superRegionsList[currentSuperRegionIndex]);

                currentSuperRegionIndex++;
                if (currentSuperRegionIndex == superRegionsList.Count)
                {
                    currentSuperRegionIndex = 0;
                }

                //removing jobs that are already in the queue
                var unnecessaryNewJobs = new List<SubSceneJob>();
                foreach (var job in newJobs)
                {
                    var existingJob = subSceneJobsList.Where(i =>
                        i.JobType == job.JobType &&
                        i.Region.SuperRegion.Type == job.Region.SuperRegion.Type &&
                        i.Region.UniqueId == job.Region.UniqueId &&
                        i.SubSceneLayer == job.SubSceneLayer &&
                        i.SubSceneVariant == job.SubSceneVariant
                    ).FirstOrDefault();

                    if (existingJob != null)
                    {
                        unnecessaryNewJobs.Add(job);
                    }
                }

                foreach (var unnecessaryJob in unnecessaryNewJobs)
                {
                    newJobs.Remove(unnecessaryJob);
                }

                //removing different jobs for same SubScene
                var deprecatedJobs = new List<SubSceneJob>();
                foreach (var job in newJobs)
                {
                    deprecatedJobs.AddRange(subSceneJobsList.Where(item =>
                        item.JobType != job.JobType &&
                        item.Region.SuperRegion.Type == job.Region.SuperRegion.Type &&
                        item.Region.UniqueId == job.Region.UniqueId &&
                        item.SubSceneVariant == job.SubSceneVariant &&
                        item.SubSceneLayer == job.SubSceneLayer
                    ));
                }

                foreach (var deprecatedJob in deprecatedJobs)
                {
                    subSceneJobsList.Remove(deprecatedJob);
                    deprecatedJob.Callback(deprecatedJob, false);
                }

                //adding new jobs to queue
                foreach (var job in newJobs)
                {
                    subSceneJobsList.Add(job);
                }

                //ordering queue
                subSceneJobsList = subSceneJobsList.OrderBy(item => item.Priority).ToList();
            }

            //***********************************************
            //executing jobs
            if (!isJobRunning && subSceneJobsList.Count > 0)
            {
                var newJob = subSceneJobsList[0];
                subSceneJobsList.RemoveAt(0);

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
        } //end of Update()

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

        #endregion monobehaviour methods

        //========================================================================================

        #region activation coroutines

        private IEnumerator ActivationCR()
        {
            yield return null;

            while (subSceneJobsList.Count > 0 && subSceneJobsList[0].Priority <= renderDistanceMedium + (renderDistanceFar - renderDistanceMedium))
            {
                yield return null;
            }

            CurrentState = eWorldControllerState.Activated;
        }

        private IEnumerator DeactivationCR()
        {
            yield return null;

            while (isJobRunning && subSceneJobsList.Count > 0)
            {
                yield return null;
            }

            CurrentState = eWorldControllerState.Deactivated;
        }

        #endregion activation coroutines

        //========================================================================================

        #region update helper methods

        private List<SubSceneJob> CreateNewSubSceneJobs(SuperRegion superRegion)
        {
            var cameraTransform = gameController.CameraController.transform;
            var playerPosition = gameController.PlayerController.CharController.MyTransform.position;
            var teleportPositions = new List<Vector3>();
            var halfSize = worldSize * 0.5f;

            //identifying teleport positions
            if (playerPosition.y > halfSize.y - preTeleportOffset)
            {
                var telePos = playerPosition;
                telePos.y = -halfSize.y;
                teleportPositions.Add(telePos);
            }
            else if (playerPosition.y < -halfSize.y + preTeleportOffset)
            {
                var telePos = playerPosition;
                telePos.y = halfSize.y;
                teleportPositions.Add(telePos);
            }

            if (playerPosition.z > halfSize.z - preTeleportOffset)
            {
                var telePos = playerPosition;
                telePos.z = -halfSize.z;
                teleportPositions.Add(telePos);
            }
            else if (playerPosition.z < -halfSize.z + preTeleportOffset)
            {
                var telePos = playerPosition;
                telePos.z = halfSize.z;
                teleportPositions.Add(telePos);
            }

            return superRegion.UpdateSuperRegion(cameraTransform, playerPosition, teleportPositions);
        }

        #endregion update helper methods

        //========================================================================================

        #region job handling coroutines

        /// <summary>
        /// Runtime Coroutine that loads a subScene
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        private IEnumerator LoadSubSceneCR(SubSceneJob job)
        {
            isJobRunning = true;
            //Debug.LogFormat("Load Job started: {0} {1} {2} {3}", job.Region.SuperRegion.Type, job.Region.name, job.SubSceneVariant, job.SubSceneLayer);

            string sceneName = WorldUtility.GetSubSceneName(job.Region.UniqueId, job.SubSceneVariant, job.SubSceneLayer, job.Region.SuperRegion.Type);
            var subSceneRoot = job.Region.GetSubSceneRoot(job.SubSceneLayer);

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
                        worldObjects[i].Initialize(gameController, job.Region.SuperRegion.Type != eSuperRegionType.Centre);
                    }
                    yield return null;
                }
            }
            //streaming
            else
            {
                if (subSceneRoot)
                {
                    Debug.LogWarningFormat("Load Job for existing subScene started! {0} {1} {2} {3}", job.Region.SuperRegion.Type, job.Region.name, job.SubSceneVariant, job.SubSceneLayer);
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
                        worldObjects[i].Initialize(gameController, job.Region.SuperRegion.Type != eSuperRegionType.Centre);
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

            job.Callback(job, true);

            //Debug.Log("Load Job done");
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
            //Debug.LogFormat("Unload Job started: {0} {1} {2} {3}", job.Region.SuperRegion.Type, job.Region.name, job.SubSceneVariant, job.SubSceneLayer);

            var subSceneRoot = job.Region.GetSubSceneRoot(job.SubSceneLayer, job.SubSceneVariant);

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

            job.Callback(job, true);

            //Debug.Log("Unload Job done");
            isJobRunning = false;
        }

        #endregion job handling coroutines

        //========================================================================================
        //========================================================================================

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
    }
} //end of namespace