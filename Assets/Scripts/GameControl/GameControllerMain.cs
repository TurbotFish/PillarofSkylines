using Game.CameraControl;
using Game.EchoSystem;
using Game.Model;
using Game.Player;
using Game.UI;
using Game.Utilities;
using Game.World;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.GameControl
{
    public class GameControllerMain : MonoBehaviour, IGameControllerBase
    {
        //###############################################################

        #region member variables

        private SceneNamesData sceneNamesData;

        //private string currentSceneName;

        private bool isInitialized = false;
        private bool isGameStarted = false;

        #endregion member variables

        //###############################################################

        #region propperties

        public PlayerModel PlayerModel { get; private set; }
        public EchoManager EchoManager { get; private set; }
        public EclipseManager EclipseManager { get; private set; }

        public PlayerController PlayerController { get; private set; }
        public CameraController CameraController { get; private set; }
        public UiController UiController { get; private set; }

        public bool IsOpenWorldLoaded { get; private set; }
        public WorldController WorldController { get; private set; }

        public bool IsPillarLoaded { get; private set; }
        public ePillarId ActivePillarId { get; private set; }

        public SpawnPointManager SpawnPointManager { get; private set; }

        #endregion properties

        //###############################################################
        //###############################################################

        #region monobehaviour methods

        private void Start()
        {
            //load resources
            sceneNamesData = Resources.Load<SceneNamesData>("ScriptableObjects/SceneNamesData");

            //getting references in game controller
            PlayerModel = GetComponentInChildren<PlayerModel>();
            EchoManager = GetComponentInChildren<EchoManager>();
            EclipseManager = GetComponentInChildren<EclipseManager>();

            //gatting references in main scene
            PlayerController = FindObjectOfType<PlayerController>();
            CameraController = FindObjectOfType<CameraController>();
            UiController = FindObjectOfType<UiController>();

            //initializing
            PlayerModel.InitializePlayerModel();
            UiController.InitializeUi(this, eUiState.LoadingScreen, new EventManager.OnShowLoadingScreenEventArgs());

            PlayerController.InitializePlayerController(this);
            CameraController.InitializeCameraController(this);

            EchoManager.Initialize(this);
            EclipseManager.InitializeEclipseManager(this);

            //register to events
            EventManager.LeavePillarEvent += OnLeavePillarEventHandler;
            EventManager.EnterPillarEvent += OnEnterPillarEventHandler;

            //load open world scene
            StartCoroutine(LoadOpenWorldSceneCR());
        }

        #endregion monobehaviour methods

        //###############################################################
        //###############################################################

        #region initialization methods

        private IEnumerator LoadOpenWorldSceneCR()
        {
            AsyncOperation async;
            SceneManager.sceneLoaded += OnSceneLoadedEventHandler;

            string sceneName = sceneNamesData.GetOpenWorldSceneName();

            async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            async.allowSceneActivation = false;

            while (!async.isDone)
            {
                if (async.progress >= 0.9f)
                {
                    async.allowSceneActivation = true;
                }
                yield return null;
            }

            Scene scene = SceneManager.GetSceneByName(sceneName);

            WorldController = SearchForScriptInScene<WorldController>(scene);
            yield return null;

            WorldController.Initialize(this);

            yield return null;

            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;
            isInitialized = true;

            EventManager.SendShowMenuEvent(this, new EventManager.OnShowMenuEventArgs(eUiState.MainMenu));
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void StartGame()
        {
            //Debug.Log("GameControllerMain: StartGame");
            if (isGameStarted)
            {
                Debug.LogError("GameControllerMain: StartGame: game already started!");
                return;
            }
            else if (!isInitialized)
            {
                Debug.LogError("GameControllerMain: StartGame: game not initialized!");
                return;
            }

            isGameStarted = true;

            //activate open world
            StartCoroutine(ActivateOpenWorldCR(true));
        }

        //IEnumerator StartGameCR()
        //{
        //    //switch to loading screen
        //    Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(UI.eUiState.LoadingScreen));

        //    //register to sceneLoaded event
        //    SceneManager.sceneLoaded += OnSceneLoadedEventHandler;

        //    //loading open world scene
        //    SceneManager.LoadScene(sceneNamesData.GetOpenWorldSceneName(), LoadSceneMode.Additive);

        //    yield return null;
        //    //***********************

        //    //getting references in open world scene
        //    this.openWorldSceneInfo.Scene = SceneManager.GetSceneByName(this.sceneNamesData.GetOpenWorldSceneName());
        //    SceneManager.SetActiveScene(this.openWorldSceneInfo.Scene);

        //    this.openWorldSceneInfo.WorldController = SearchForScriptInScene<WorldController>(this.openWorldSceneInfo.Scene);
        //    this.openWorldSceneInfo.SpawnPointManager = SearchForScriptInScene<SpawnPointManager>(this.openWorldSceneInfo.Scene);

        //    yield return null;
        //    //***********************

        //    //pillar scenes
        //    var pillarIdValues = Enum.GetValues(typeof(World.ePillarId)).Cast<World.ePillarId>();

        //    foreach (var pillarId in pillarIdValues)
        //    {
        //        string name = this.sceneNamesData.GetPillarSceneName(pillarId);

        //        if (string.IsNullOrEmpty(name))
        //        {
        //            continue;
        //        }

        //        //loading pillar scene
        //        SceneManager.LoadScene(name, LoadSceneMode.Additive);

        //        yield return null;
        //        //***********************

        //        //getting references in pillar scene
        //        var pillarScene = SceneManager.GetSceneByName(name);
        //        var pillarInfo = new PillarSceneInfo()
        //        {
        //            Scene = pillarScene,
        //            PillarId = pillarId,
        //            SpawnPointManager = SearchForScriptInScene<SpawnPointManager>(pillarScene)
        //        };

        //        this.pillarSceneDictionary.Add(pillarId, pillarInfo);

        //        //initializing world objects in pillar
        //        var worldObjects = SearchForScriptsInScene<World.IWorldObject>(pillarScene);

        //        foreach (var worldObject in worldObjects)
        //        {
        //            worldObject.Initialize(this, false);
        //        }

        //        //deactivating scene
        //        foreach (var go in pillarScene.GetRootGameObjects())
        //        {
        //            go.SetActive(false);
        //        }

        //        yield return null;
        //        //***********************
        //    }

        //    //initializing game
        //    this.playerController.InitializePlayerController(this);
        //    this.CameraController.InitializeCameraController(this);

        //    //pausing game until world has loaded a bit
        //    Utilities.EventManager.SendGamePausedEvent(this, new Utilities.EventManager.GamePausedEventArgs(true));

        //    openWorldSceneInfo.WorldController.Initialize(this);

        //    yield return null;
        //    while (openWorldSceneInfo.WorldController.CurrentJobCount > 0)
        //    {
        //        yield return null;
        //    }

        //    this.EchoManager.InitializeEchoManager(this, openWorldSceneInfo.SpawnPointManager);
        //    this.EclipseManager.InitializeEclipseManager(this);

        //    yield return null;
        //    //***********************

        //    //starting the game
        //    SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;

        //    var teleportPlayerEventArgs = new Utilities.EventManager.TeleportPlayerEventArgs(this.openWorldSceneInfo.SpawnPointManager.GetInitialSpawnPoint(), this.openWorldSceneInfo.SpawnPointManager.GetInitialSpawnOrientation(), true);
        //    Utilities.EventManager.SendTeleportPlayerEvent(this, teleportPlayerEventArgs);

        //    Utilities.EventManager.SendSceneChangedEvent(this, new Utilities.EventManager.SceneChangedEventArgs());
        //    Utilities.EventManager.SendGamePausedEvent(this, new Utilities.EventManager.GamePausedEventArgs(false));
        //    Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(UI.eUiState.Intro));
        //}

        #endregion initialization methods

        //###############################################################
        //###############################################################

        #region scene loading methods

        //private IEnumerator ActivatePillarScene(ePillarId pillarId)
        //{
        //    //todo: pause game
        //    EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(eUiState.LoadingScreen));

        //    yield return null;

        //    //deactivate open world scene
        //    foreach (var go in openWorldSceneInfo.Scene.GetRootGameObjects())
        //    {
        //        go.SetActive(false);
        //    }

        //    yield return null;

        //    //activate pillar scene
        //    isPillarActive = true;
        //    activePillarId = pillarId;

        //    var info = pillarSceneDictionary[activePillarId];
        //    var pillarScene = info.Scene;

        //    foreach (var go in pillarScene.GetRootGameObjects())
        //    {
        //        go.SetActive(true);
        //    }
        //    SceneManager.SetActiveScene(pillarScene);

        //    yield return null;

        //    //
        //    var teleportPlayerEventArgs = new Utilities.EventManager.TeleportPlayerEventArgs(info.SpawnPointManager.GetInitialSpawnPoint(), info.SpawnPointManager.GetInitialSpawnOrientation(), true);
        //    Utilities.EventManager.SendTeleportPlayerEvent(this, teleportPlayerEventArgs);

        //    Utilities.EventManager.SendSceneChangedEvent(this, new Utilities.EventManager.SceneChangedEventArgs(pillarId));

        //    yield return new WaitForSeconds(0.1f);

        //    //
        //    Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(UI.eUiState.HUD));
        //    //todo: unpause game
        //}        

        //private IEnumerator ActivateOpenWorldScene(bool pillarDestroyed)
        //{
        //    //todo: pause game
        //    Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(UI.eUiState.LoadingScreen));

        //    yield return null;

        //    //deactivate pillar scene
        //    var pillarScene = pillarSceneDictionary[activePillarId].Scene;
        //    foreach (var go in pillarScene.GetRootGameObjects())
        //    {
        //        go.SetActive(false);
        //    }

        //    isPillarActive = false;

        //    yield return null;

        //    //activate open world scene
        //    foreach (var go in openWorldSceneInfo.Scene.GetRootGameObjects())
        //    {
        //        go.SetActive(true);
        //    }
        //    SceneManager.SetActiveScene(openWorldSceneInfo.Scene);

        //    yield return null;

        //    //switch pillar to destroyed state
        //    if (pillarDestroyed)
        //    {
        //        playerModel.DestroyPillar(activePillarId);
        //    }

        //    yield return null;

        //    //
        //    World.ePillarState pillarState = pillarDestroyed ? World.ePillarState.Destroyed : World.ePillarState.Intact;
        //    Vector3 position = openWorldSceneInfo.SpawnPointManager.GetPillarExitPoint(activePillarId, pillarState);
        //    Quaternion rotation = openWorldSceneInfo.SpawnPointManager.GetPillarExitOrientation(activePillarId, pillarState);

        //    var teleportPlayerEventArgs = new Utilities.EventManager.TeleportPlayerEventArgs(position, rotation, true);
        //    Utilities.EventManager.SendTeleportPlayerEvent(this, teleportPlayerEventArgs);

        //    Utilities.EventManager.SendSceneChangedEvent(this, new Utilities.EventManager.SceneChangedEventArgs());

        //    yield return new WaitForSeconds(0.1f);

        //    //
        //    Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(UI.eUiState.HUD));
        //    //todo: unpause game
        //}

        /// <summary>
        /// Coroutine for switching to the open world scene.
        /// </summary>
        /// <param name="useInitialSpawnPoint"></param>
        /// <returns></returns>
        private IEnumerator ActivateOpenWorldCR(bool useInitialSpawnPoint)
        {
            AsyncOperation async;
            SceneManager.sceneLoaded += OnSceneLoadedEventHandler;

            //*****************************************
            //pausing game
            EventManager.SendPreSceneChangeEvent(this, new EventManager.PreSceneChangeEventArgs(true));
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(true));
            EventManager.SendShowMenuEvent(this, new EventManager.OnShowLoadingScreenEventArgs());
            yield return null;

            //*****************************************
            //unloading pillar scene   
            string pillarSceneName = sceneNamesData.GetPillarSceneName(ActivePillarId);

            if (IsPillarLoaded && !string.IsNullOrEmpty(pillarSceneName))
            {
                async = SceneManager.UnloadSceneAsync(pillarSceneName);

                while (!async.isDone)
                {
                    yield return null;
                }

                yield return null;
            }

            IsPillarLoaded = false;
            SpawnPointManager = null;

            //*****************************************
            //"activating" open world scene
            string worldSceneName = sceneNamesData.GetOpenWorldSceneName();
            Scene scene = SceneManager.GetSceneByName(worldSceneName);            

            foreach (var obj in scene.GetRootGameObjects())
            {
                obj.SetActive(true);
            }

            SceneManager.SetActiveScene(scene);
            IsOpenWorldLoaded = true;

            WorldController = SearchForScriptInScene<WorldController>(scene);
            SpawnPointManager = SearchForScriptInScene<SpawnPointManager>(scene);            

            yield return null;

            //*****************************************
            //teleporting player
            Vector3 position;
            Quaternion rotation;

            if (useInitialSpawnPoint)
            {
                position = SpawnPointManager.GetInitialSpawnPoint();
                rotation = SpawnPointManager.GetInitialSpawnOrientation();
            }
            else
            {
                ePillarState pillarState = PlayerModel.CheckIsPillarDestroyed(ActivePillarId) ? ePillarState.Destroyed : ePillarState.Intact;
                position = SpawnPointManager.GetPillarExitPoint(ActivePillarId, pillarState);
                rotation = SpawnPointManager.GetPillarExitOrientation(ActivePillarId, pillarState);
            }

            var teleportPlayerEventArgs = new EventManager.TeleportPlayerEventArgs(position, rotation, true);
            EventManager.SendTeleportPlayerEvent(this, teleportPlayerEventArgs);
            yield return null;

            //*****************************************
            //activating world controller
            WorldController.Activate();

            while (!WorldController.ActivationDone)
            {
                yield return null;
            }

            //*****************************************
            //informing everyone!
            EventManager.SendSceneChangedEvent(this, new EventManager.SceneChangedEventArgs());           

            //*****************************************
            //unpausing game
            EventManager.SendShowMenuEvent(this, new EventManager.OnShowMenuEventArgs(eUiState.HUD));
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(false));

            //*****************************************
            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;
        }

        /// <summary>
        /// Coroutine for switching to a pillar scene.
        /// </summary>
        /// <param name="pillarId"></param>
        /// <returns></returns>
        private IEnumerator LoadPillarSceneCR(ePillarId pillarId)
        {
            AsyncOperation async;
            SceneManager.sceneLoaded += OnSceneLoadedEventHandler;

            //*****************************************
            //pausing game
            EventManager.SendPreSceneChangeEvent(this, new EventManager.PreSceneChangeEventArgs(false));
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(true));
            EventManager.SendShowMenuEvent(this, new EventManager.OnShowLoadingScreenEventArgs(pillarId));
            yield return null;

            //*****************************************
            //deactivating open world scene
            string worldSceneName = sceneNamesData.GetOpenWorldSceneName();
            Scene scene = SceneManager.GetSceneByName(worldSceneName);

            WorldController.Deactivate();

            while (!WorldController.DeactivationDone)
            {
                yield return null;
            }

            IsOpenWorldLoaded = false;
            WorldController = null;
            SpawnPointManager = null;

            foreach(var obj in scene.GetRootGameObjects())
            {
                obj.SetActive(false);
            }

            //*****************************************
            //loading pillar scene
            string pillarSceneName = sceneNamesData.GetPillarSceneName(pillarId);

            async = SceneManager.LoadSceneAsync(pillarSceneName, LoadSceneMode.Additive);
            async.allowSceneActivation = false;

            while (!async.isDone)
            {
                if (async.progress >= 0.9f)
                {
                    async.allowSceneActivation = true;
                }

                yield return null;
            }

            scene = SceneManager.GetSceneByName(pillarSceneName);
            SceneManager.SetActiveScene(scene);

            IsPillarLoaded = true;
            ActivePillarId = pillarId;
            SpawnPointManager = SearchForScriptInScene<SpawnPointManager>(scene);
            yield return null;

            //*****************************************
            //initializing world objects in pillar
            var worldObjects = SearchForScriptsInScene<IWorldObject>(scene);

            foreach (var worldObject in worldObjects)
            {
                worldObject.Initialize(this, false);
            }
            yield return null;

            //*****************************************
            //teleporting player
            var teleportPlayerEventArgs = new EventManager.TeleportPlayerEventArgs(SpawnPointManager.GetInitialSpawnPoint(), SpawnPointManager.GetInitialSpawnOrientation(), true);
            EventManager.SendTeleportPlayerEvent(this, teleportPlayerEventArgs);

            //*****************************************
            //informing everyone!
            EventManager.SendSceneChangedEvent(this, new EventManager.SceneChangedEventArgs(pillarId));
            yield return new WaitForSeconds(0.1f);

            //*****************************************
            //unpausing game
            EventManager.SendShowMenuEvent(this, new EventManager.OnShowMenuEventArgs(eUiState.HUD));
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(false));

            //*****************************************
            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;
        }

        #endregion scene loading methods

        //###############################################################
        //###############################################################

        #region event methods

        private void OnSceneLoadedEventHandler(Scene scene, LoadSceneMode loadSceneMode)
        {
            //Debug.Log("on scene loaded event");
            CleanScene(scene);
        }

        private void OnEnterPillarEventHandler(object sender, EventManager.EnterPillarEventArgs args)
        {
            if (IsPillarLoaded)
            {
                throw new Exception("A Pillar is already active!");
            }

            StartCoroutine(LoadPillarSceneCR(args.PillarId));
        }

        private void OnLeavePillarEventHandler(object sender, EventManager.LeavePillarEventArgs args)
        {
            if (!IsPillarLoaded)
            {
                Debug.LogError("No Pillar is active!");
                return;
            }

            //switch pillar to destroyed state
            if (args.PillarDestroyed)
            {
                PlayerModel.DestroyPillar(ActivePillarId);
            }

            StartCoroutine(ActivateOpenWorldCR(false));
        }

        #endregion event methods

        //###############################################################
        //###############################################################

        #region static methods

        private static T SearchForScriptInScene<T>(Scene scene) where T : class
        {
            T result = null;

            foreach (var gameObject in scene.GetRootGameObjects())
            {
                result = gameObject.GetComponentInChildren<T>(true);

                if (result != null)
                {
                    break;
                }
            }

            return result;
        }

        private static List<T> SearchForScriptsInScene<T>(Scene scene) where T : class
        {
            var result = new List<T>();

            foreach (var gameObject in scene.GetRootGameObjects())
            {
                result.AddRange(gameObject.GetComponentsInChildren<T>(true));
            }

            return result;
        }

        private static void CleanScene(Scene scene)
        {
            var gameControllerLiteInstances = SearchForScriptsInScene<GameControllerLite>(scene);
            //Debug.LogFormat("Scene {0} contained {1} GameControllerLite!", scene.name, gameControllerLiteInstances.Count());
            foreach (var instance in gameControllerLiteInstances)
            {
                Destroy(instance.gameObject);
            }

            var playerControllerInstances = SearchForScriptsInScene<PlayerController>(scene);
            //Debug.LogFormat("Scene {0} contained {1} PlayerController!", scene.name, playerControllerInstances.Count());
            foreach (var instance in playerControllerInstances)
            {
                Destroy(instance.gameObject);
            }

            var cameraInstances = SearchForScriptsInScene<PoS_Camera>(scene);
            //Debug.LogFormat("Scene {0} contained {1} PoS_Camera!", scene.name, cameraInstances.Count());
            foreach (var instance in cameraInstances)
            {
                Destroy(instance.gameObject);
            }

            //cleaning up old EchoManagers
            var echoManagers = SearchForScriptsInScene<EchoManager>(scene);
            foreach (var echoManager in echoManagers)
            {
                Destroy(echoManager.gameObject);
            }

            var eclipseManagers = SearchForScriptsInScene<EclipseManager>(scene);
            foreach (var eclipseManager in eclipseManagers)
            {
                Destroy(eclipseManager.gameObject);
            }
        }

        #endregion static methods

        //###############################################################
    }
} //end of namespace