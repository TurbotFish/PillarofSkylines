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
        //###############################################################

        // -- CONSTANTS

        [SerializeField, HideInInspector] private Vector3 worldSize;

        [SerializeField, HideInInspector] private float renderDistanceNear;
        [SerializeField, HideInInspector] private float renderDistanceMedium;
        [SerializeField, HideInInspector] private float renderDistanceFar;

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

        // -- ATTRIBUTES     

        public GameController GameController { get; private set; }
        public WorldControllerState CurrentState { get; private set; }

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
        public void Initialize(GameController gameController)
        {
            GameController = gameController;
            RegionList.Clear();

            CurrentState = WorldControllerState.Deactivated;

            var initialRegions = GetComponentsInChildren<RegionBase>().ToList();

            foreach (var region in initialRegions)
            {
                region.Initialize(this);
                RegionList.Add(region);
            }

            isInitialized = true;
        }

        /// <summary>
        /// Activates the WorldController.
        /// </summary>
        public void Activate(Vector3 spawn_position)
        {
            if (!isInitialized)
            {
                return;
            }
            else if (CurrentState != WorldControllerState.Deactivated)
            {
                return;
            }

            CurrentState = WorldControllerState.Activating;
            CurrentRegionIndex = 0;

            if (EditorSubScenesLoaded)
            {
                var worldObjects = FindObjectsOfType<MonoBehaviour>();
                foreach (var obj in worldObjects)
                {
                    if (obj is IWorldObject)
                    {
                        (obj as IWorldObject).Initialize(GameController);
                    }
                }
            }

            foreach (var region in RegionList)
            {
                var teleport_positions = ComputeTeleportPositions(spawn_position);

                region.UpdateRegion(spawn_position, teleport_positions);
            }

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
            else if (CurrentState != WorldControllerState.Activated)
            {
                return;
            }

            CurrentState = WorldControllerState.Deactivating;

            foreach (var region in RegionList)
            {
                region.UnloadAll();
            }

            StartCoroutine(DeactivationCR());
        }

        //###############################################################

        // -- INQUIRIES

        public Vector3 WorldSize { get { return worldSize; } }

        public float RenderDistanceNear { get { return renderDistanceNear; } }
        public float RenderDistanceMedium { get { return renderDistanceMedium; } }
        public float RenderDistanceFar { get { return renderDistanceFar; } }

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


        //###############################################################

        // -- OPERATIONS

        /// <summary>
        /// 
        /// </summary>
        /// <param name="job"></param>
        public void AddSubSceneJob(SubSceneJob job)
        {
            if (!SubSceneJobsList.Contains(job))
            {
                SubSceneJobsList.Add(job);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="job"></param>
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

            /*
             * updating world
             */
            if (CurrentState == WorldControllerState.Activated)
            {
                var player_position = GameController.PlayerController.CharController.MyTransform.position;
                var teleport_positions = ComputeTeleportPositions(player_position);
                teleport_positions.AddRange(GameController.EchoManager.GetEchoPositions());

                RegionList[CurrentRegionIndex].UpdateRegion(player_position, teleport_positions);

                CurrentRegionIndex++;
                if (CurrentRegionIndex == RegionList.Count)
                {
                    CurrentRegionIndex = 0;
                }
            }

            /*
             * executing jobs
             */
            if (!isJobRunning && SubSceneJobsList.Count > 0)
            {
                UpdateSubSceneJobQueue();

                var newJob = SubSceneJobsList[0];
                SubSceneJobsList.RemoveAt(0);

                switch (newJob.JobType)
                {
                    case SubSceneJobType.Load:
                        StartCoroutine(LoadSubSceneCR(newJob));
                        break;
                    case SubSceneJobType.Unload:
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

            invisibilityAngle = Mathf.Clamp(invisibilityAngle, 0, 360);

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

            CurrentState = WorldControllerState.Activated;
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

            CurrentState = WorldControllerState.Deactivated;
        }

        /// <summary>
        /// Creates a list of the positions the player would teleport to if he reached the border of the world.
        /// </summary>
        /// <param name="player_position"></param>
        /// <returns></returns>
        private List<Vector3> ComputeTeleportPositions(Vector3 player_position)
        {
            var teleport_positions = new List<Vector3>();

            teleport_positions.Add(player_position + new Vector3(0, worldSize.y, 0));
            teleport_positions.Add(player_position + new Vector3(0, worldSize.y, worldSize.z));
            teleport_positions.Add(player_position + new Vector3(0, 0, worldSize.z));
            teleport_positions.Add(player_position + new Vector3(0, -worldSize.y, worldSize.z));
            teleport_positions.Add(player_position + new Vector3(0, -worldSize.y, 0));
            teleport_positions.Add(player_position + new Vector3(0, -worldSize.y, -worldSize.z));
            teleport_positions.Add(player_position + new Vector3(0, 0, -worldSize.z));
            teleport_positions.Add(player_position + new Vector3(0, worldSize.y, -worldSize.z));

            return teleport_positions;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateSubSceneJobQueue()
        {
            /* 
             * Cleaning the queue.
             */
            SubSceneJobsList.RemoveAll(item => item.CurrentState != SubSceneJobState.Pending);

            /* 
             * Ordering the queue.
             */
            SubSceneJobsList = SubSceneJobsList.OrderBy(item => item.GetPriority()).ToList();
        }

        /// <summary>
        /// Runtime Coroutine that loads a subScene
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        private IEnumerator LoadSubSceneCR(SubSceneJob job)
        {
            isJobRunning = true;
            job.CurrentState = SubSceneJobState.Active;
            //Debug.LogFormat("Load Job started: {0} {1} {2} {3}", job.Region.SuperRegion.Type, job.Region.name, job.SubSceneVariant, job.SubSceneLayer);

            string sceneName = WorldUtility.GetSubSceneName(job.Region.UniqueId, job.SubSceneVariant, job.SubSceneLayer);
            var subSceneRoot = job.Region.GetSubSceneRoot(job.SubSceneVariant, job.SubSceneLayer);

            float sub_scene_loading_start_time = Time.time;

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
                    Debug.LogWarningFormat("Load Job for existing subScene started! {0} {1} {2}", job.Region.name, job.SubSceneVariant, job.SubSceneLayer);
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
                    Transform root = null;
                    if (scene.GetRootGameObjects().Length > 0)
                    {
                        root = scene.GetRootGameObjects()[0].transform;
                        SceneManager.MoveGameObjectToScene(root.gameObject, gameObject.scene);
                        root.SetParent(job.Region.transform, true);    // Attach the SubScene to its Region.
                    }
                    else
                    {
                        Debug.LogErrorFormat("Region {0}: Subscene {1} {2} is empty!", job.Region.name, job.SubSceneVariant, job.SubSceneLayer);
                    }

                    //########
                    duration = Time.time - time;
                    measurement.moveTimeSum += duration;
                    //########

                    yield return null;

                    //initializing all WorldObjects
                    if (root != null)
                    {
                        var worldObjects = root.GetComponentsInChildren<IWorldObject>();
                        for (int i = 0; i < worldObjects.Length; i++)
                        {
                            worldObjects[i].Initialize(GameController);
                        }
                        yield return null;
                    }

                    //unload the SubScene Scene
                    async = SceneManager.UnloadSceneAsync(sceneName);

                    while (!async.isDone)
                    {
                        yield return null;
                    }
                }
            }

            if (Time.time - sub_scene_loading_start_time > 0)
            {
                //Debug.LogFormat("SubScene {0} {1} {2} loaded! duration={3}", job.Region.name, job.SubSceneVariant, job.SubSceneLayer, (Time.time - sub_scene_loading_start_time));
            }

            job.CurrentState = SubSceneJobState.Successfull;
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
            job.CurrentState = SubSceneJobState.Active;
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
            job.CurrentState = SubSceneJobState.Successfull;
            isJobRunning = false;
        }

        //###############################################################
        //###############################################################

        private class LoadingMeasurement
        {
            public string regionName;
            public SubSceneLayer subSceneLayer;
            public SubSceneVariant subSceneVariant;

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
