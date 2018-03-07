using Game.GameControl;
using Game.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.World
{
    public class WorldController : MonoBehaviour, IWorldEventHandler
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

        [SerializeField]
        [HideInInspector]
        private Vector3 worldSize;

        [SerializeField]
        [HideInInspector]
        private float renderDistanceFar;

        [SerializeField]
        [HideInInspector]
        private float renderDistanceInactive;

        [SerializeField]
        [HideInInspector]
        private float preTeleportOffset;

        [SerializeField]
        [HideInInspector]
        private float secondaryPositionDistanceModifier;

        private IGameControllerBase gameController;
        private Transform myTransform;

        private List<SuperRegion> superRegionsList = new List<SuperRegion>();
        private List<SubSceneJob> subSceneJobsList = new List<SubSceneJob>();

        private bool isInitialized = false;
        private bool isJobRunning = false;

        private int currentSuperRegionIndex;

#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private bool drawBounds;

        [SerializeField]
        [HideInInspector]
        private bool drawRegionBounds;
#endif

        [SerializeField]
        [HideInInspector]
        private bool editorSubScenesLoaded;

        #endregion member variables 

        //========================================================================================

        #region properties

        public IGameControllerBase GameController { get { return gameController; } }

        public Vector3 WorldSize { get { return worldSize; } }

        public float RenderDistanceFar { get { return renderDistanceFar; } }

        public float RenderDistanceInactive { get { return renderDistanceInactive; } }

        public bool EditorSubScenesLoaded { get { return editorSubScenesLoaded; } }

        public float SecondaryPositionDistanceModifier { get { return secondaryPositionDistanceModifier; } }

        public int CurrentJobCount { get { return subSceneJobsList.Count; } }

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
            myTransform = transform;

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

            var cameraTransform = gameController.CameraController.transform;
            var playerPosition = myTransform.position;
            var teleportPositions = new List<Vector3>();
            var halfSize = worldSize * 0.5f;

            //***********************************************
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
                telePos.y = -halfSize.z;
                teleportPositions.Add(telePos);
            }
            else if (playerPosition.z < -halfSize.z + preTeleportOffset)
            {
                var telePos = playerPosition;
                telePos.y = halfSize.z;
                teleportPositions.Add(telePos);
            }

            //***********************************************
            //updating one super region, getting a list of new jobs
            var newJobs = superRegionsList[currentSuperRegionIndex].UpdateSuperRegion(cameraTransform, playerPosition, teleportPositions);

            currentSuperRegionIndex++;
            if (currentSuperRegionIndex == superRegionsList.Count)
            {
                currentSuperRegionIndex = 0;
            }

            //***********************************************
            //cleaning and updating the list of SubSceneJobs
            var deprecatedJobs = new List<SubSceneJob>();
            foreach (var job in newJobs)
            {
                deprecatedJobs.AddRange(subSceneJobsList.Where(item =>
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

            foreach (var job in newJobs)
            {
                subSceneJobsList.Add(job);
            }

            subSceneJobsList.OrderBy(item => item.Priority);

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
            float part = renderDistanceFar * 0.5f;
            if (part < 1)
            {
                part = 1;
            }
            else if (part - (int)part > 0)
            {
                part = (int)part + 1;
            }

            if (renderDistanceInactive < renderDistanceFar + part)
            {
                renderDistanceInactive = renderDistanceFar + part;
            }

            if (preTeleportOffset < 1)
            {
                preTeleportOffset = 1;
            }

            if (secondaryPositionDistanceModifier < 0)
            {
                secondaryPositionDistanceModifier = 0;
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

        #endregion monobehaviour methods

        //========================================================================================

        #region private methods

        /// <summary>
        /// Runtime Coroutine that loads a subScene
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        private IEnumerator LoadSubSceneCR(SubSceneJob job)
        {
            isJobRunning = true;
            //Debug.LogFormat("Load Job started: {0} {1} {2} {3}", job.Region.SuperRegion.Type, job.Region.name, job.SubSceneMode, job.SubSceneLayer);

            string sceneName = WorldUtility.GetSubSceneName(job.Region.UniqueId, job.SubSceneVariant, job.SubSceneLayer);
            var subSceneRoot = job.Region.GetSubSceneRoot(job.SubSceneLayer);

            //editor subScenes are loaded (no streaming)
            if (editorSubScenesLoaded)
            {
                if (subSceneRoot)
                {
                    subSceneRoot.gameObject.SetActive(true);
                }
            }
            //streaming
            else
            {
                if (subSceneRoot)
                {
                    Debug.LogWarningFormat("Load Job for existing subScene started! {0} {1} {2} {3}", job.Region.SuperRegion.Type, job.Region.name, job.SubSceneVariant, job.SubSceneLayer);
                }
                else if (!Application.CanStreamedLevelBeLoaded(sceneName))
                {
                    //Debug.LogWarningFormat("scene {0} cannot be streamed", sceneName);
                    //var root = new GameObject("empty").transform;
                    //root.SetParent(job.Region.transform);
                }
                else
                {
                    AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                    while (!async.isDone)
                    {
                        yield return null;
                    }

                    //move root to open world scene
                    Scene scene = SceneManager.GetSceneByName(sceneName);
                    var root = scene.GetRootGameObjects()[0].transform;
                    SceneManager.MoveGameObjectToScene(root.gameObject, gameObject.scene);

                    //remove "do not repeat" objects
                    if (job.Region.SuperRegion.Type != eSuperRegionType.Centre)
                    {
                        List<DoNotRepeatTag> doNotRepeatTags = root.GetComponentsInChildren<DoNotRepeatTag>(true).ToList();
                        while (doNotRepeatTags.Count > 0)
                        {
                            var tag = doNotRepeatTags[0];
                            doNotRepeatTags.RemoveAt(0);
                            DestroyImmediate(tag.gameObject);
                        }
                    }
                    yield return null;

                    //initializing all WorldObjects
                    int initCount = 0;
                    var worldObjects = GetComponentsInChildren<IWorldObject>();
                    for (int i = 0; i < worldObjects.Length; i++)
                    {
                        worldObjects[i].Initialize(gameController, job.Region.SuperRegion.Type != eSuperRegionType.Centre);
                        if (++initCount % 30 == 0)
                        {
                            yield return null;
                        }
                    }
                    yield return null;

                    //attach the SubScene to its Region
                    root.SetParent(job.Region.transform, false);

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
            //Debug.LogFormat("Unload Job started: {0} {1} {2}", job.Region.SuperRegion.Type, job.Region.name, job.SceneType);

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

#if UNITY_EDITOR
        /// <summary>
        /// Editor methods for loading all the SubScenes.
        /// </summary>
        void IWorldEventHandler.ImportSubScenes()
        {
            if (editorSubScenesLoaded)
            {
                return;
            }

            var regions = new List<RegionBase>();

            for (int i = 0; i < transform.childCount; i++)
            {
                var region = transform.GetChild(i).GetComponent<RegionBase>();

                if (region)
                {
                    regions.Add(region);
                }
            }

            foreach (var region in regions)
            {
                foreach (var subSceneLayer in Enum.GetValues(typeof(eSubSceneLayer)).Cast<eSubSceneLayer>())
                {
                    foreach (var subSceneMode in region.AvailableSubSceneVariants)
                    {
                        if (region.GetSubSceneRoot(subSceneLayer, subSceneMode) != null)
                        {
                            Debug.LogErrorFormat("The \"{0}\" of Region \"{1}\" is already loaded!", WorldUtility.GetSubSceneRootName(subSceneMode, subSceneLayer), region.name);
                            continue;
                        }

                        //paths
                        string subScenePath = WorldUtility.GetSubScenePath(gameObject.scene.path, region.UniqueId, subSceneMode, subSceneLayer);
                        string subScenePathFull = WorldUtility.GetFullPath(subScenePath);

                        Scene subScene = new Scene();

                        if (System.IO.File.Exists(subScenePathFull))
                        {
                            subScene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(subScenePath, UnityEditor.SceneManagement.OpenSceneMode.Additive);
                        }

                        //move subScene content to open world scene
                        if (subScene.IsValid())
                        {
                            var rootGO = subScene.GetRootGameObjects()[0];
                            UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(rootGO, this.gameObject.scene);

                            var root = rootGO.transform;
                            root.SetParent(region.transform, false);

                            if (!root.gameObject.activeSelf)
                            {
                                root.gameObject.SetActive(true);
                            }
                        }

                        //end: close subScene
                        UnityEditor.SceneManagement.EditorSceneManager.CloseScene(subScene, true);
                    }
                }
            }

            editorSubScenesLoaded = true;

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif

#if UNITY_EDITOR
        /// <summary>
        /// Editor methods for creating and unloading all the SubScenes.
        /// </summary>
        void IWorldEventHandler.ExportSubScenes()
        {
            if (!editorSubScenesLoaded)
            {
                return;
            }

            //clear subScene folder
            ((IWorldEventHandler)this).ClearSubSceneFolder();

            //create folder, in case it does not exist
            string scenePath = gameObject.scene.path;
            string folderPath = scenePath.Remove(scenePath.LastIndexOf('.'));
            string parentFolderPath = folderPath.Remove(folderPath.LastIndexOf('/'));
            if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
            {
                UnityEditor.AssetDatabase.CreateFolder(parentFolderPath, gameObject.scene.name);
            }

            //finding all the regions
            var regions = new List<RegionBase>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var region = transform.GetChild(i).GetComponent<RegionBase>();

                if (region != null)
                {
                    regions.Add(region);
                }
            }

            var buildSettingsScenes = UnityEditor.EditorBuildSettings.scenes.ToList();

            foreach (var region in regions)
            {
                foreach (var subSceneLayer in Enum.GetValues(typeof(eSubSceneLayer)).Cast<eSubSceneLayer>())
                {
                    foreach (var subSceneMode in region.AvailableSubSceneVariants)
                    {
                        var root = region.GetSubSceneRoot(subSceneLayer, subSceneMode);

                        if (!root || root.childCount == 0) //if root is null or empty there is no need to create a subScene
                        {
                            continue;
                        }

                        //paths
                        string subScenePath = WorldUtility.GetSubScenePath(gameObject.scene.path, region.UniqueId, subSceneMode, subSceneLayer);

                        //moving rott to subScene
                        root.SetParent(null, false);
                        var subScene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Additive);
                        UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(root.gameObject, subScene);

                        //saving and closing the sub scene
                        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(subScene, subScenePath);
                        UnityEditor.SceneManagement.EditorSceneManager.CloseScene(subScene, true);

                        //add subScene to buildsettings
                        buildSettingsScenes.Add(new UnityEditor.EditorBuildSettingsScene(subScenePath, true));
                    }
                }
            }

            //adding open world scene to build settings
            if (!buildSettingsScenes.Exists(item => item.path == gameObject.scene.path))
            {
                buildSettingsScenes.Add(new UnityEditor.EditorBuildSettingsScene(gameObject.scene.path, true));
            }
            UnityEditor.EditorBuildSettings.scenes = buildSettingsScenes.ToArray();

            editorSubScenesLoaded = false;

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif

#if UNITY_EDITOR
        /// <summary>
        /// Deletes the SubScene folder and its content and removes the subScenes from the build settings, but only if the subScenes have been loaded first.
        /// </summary>
        void IWorldEventHandler.ClearSubSceneFolder()
        {
            if (!editorSubScenesLoaded)
            {
                return;
            }

            string worldScenePath = gameObject.scene.path;
            string worldSceneFolderPath = worldScenePath.Remove(worldScenePath.LastIndexOf('.'));

            //cleaning build settings
            var scenes = UnityEditor.EditorBuildSettings.scenes.ToList();
            var scenesToRemove = new List<UnityEditor.EditorBuildSettingsScene>();
            string pathPart = worldSceneFolderPath + "/SubScene_"; //string.Concat(worldSceneFolderPath, "/SubScene_");

            foreach (var sceneEntry in scenes)
            {
                if (sceneEntry.path.Contains(pathPart) || string.IsNullOrEmpty(sceneEntry.path))
                {
                    scenesToRemove.Add(sceneEntry);
                }
            }

            foreach (var sceneEntry in scenesToRemove)
            {
                scenes.Remove(sceneEntry);
            }

            UnityEditor.EditorBuildSettings.scenes = scenes.ToArray();

            //deleting subScene folder (of the current scene only)
            UnityEditor.FileUtil.DeleteFileOrDirectory(worldSceneFolderPath);
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
        #endregion editor methods

        //========================================================================================
    }
} //end of namespace