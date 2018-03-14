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
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.GameControl
{
    public class GameControllerMain : MonoBehaviour, IGameControllerBase
    {
        //###############################################################

        #region member variables

        private SceneNamesData sceneNamesData;

        private PlayerModel playerModel;
        private EchoManager echoManager;
        private EclipseManager eclipseManager;

        private PlayerController playerController;
        private CameraController cameraController;
        private UiController uiController;

        //private OpenWorldSceneInfo openWorldSceneInfo = new OpenWorldSceneInfo();
        //private Dictionary<ePillarId, PillarSceneInfo> pillarSceneDictionary = new Dictionary<ePillarId, PillarSceneInfo>();

        private string currentSceneName;
        private bool isPillarActive = false;
        private ePillarId activePillarId;
        private WorldController worldController;
        private SpawnPointManager currentSpawnPointManager;

        private bool isInitialized;
        private bool isGameStarted = false;

        #endregion member variables

        //###############################################################

        #region propperties

        public PlayerModel PlayerModel { get { return playerModel; } }
        public EchoManager EchoManager { get { return echoManager; } }
        public EclipseManager EclipseManager { get { return eclipseManager; } }

        public PlayerController PlayerController { get { return playerController; } }
        public CameraController CameraController { get { return cameraController; } }
        public UiController UiController { get { return uiController; } }

        public WorldController WorldController { get { return worldController; } }

        #endregion properties

        //###############################################################
        //###############################################################

        #region monobehaviour methods

        private void Start()
        {
            //load resources
            sceneNamesData = Resources.Load<SceneNamesData>("ScriptableObjects/SceneNamesData");

            //getting references in game controller
            playerModel = GetComponentInChildren<PlayerModel>();
            echoManager = GetComponentInChildren<EchoManager>();
            eclipseManager = GetComponentInChildren<EclipseManager>();

            //initializing game controller
            playerModel.InitializePlayerModel();

            //gatting references in main scene
            playerController = FindObjectOfType<PlayerController>();
            cameraController = FindObjectOfType<CameraController>();
            uiController = FindObjectOfType<UiController>();

            //initializing the UI
            uiController.InitializeUi(this, eUiState.MainMenu);

            //register to events
            EventManager.LeavePillarEvent += OnLeavePillarEventHandler;
            EventManager.EnterPillarEvent += OnEnterPillarEventHandler;
        }

        #endregion monobehaviour methods

        //###############################################################
        //###############################################################

        #region initialization methods

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void StartGame()
        {
            Debug.Log("GameControllerMain: StartGame");
            if (isGameStarted)
            {
                Debug.LogError("GameControllerMain: StartGame: game already started!");
                return;
            }

            //initializing game
            playerController.InitializePlayerController(this);
            CameraController.InitializeCameraController(this);

            //loading open world scene
            StartCoroutine(LoadOpenWorldSceneCR(true));
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

        private IEnumerator LoadOpenWorldSceneCR(bool useInitialSpawnPoint)
        {
            AsyncOperation async;
            SceneManager.sceneLoaded += OnSceneLoadedEventHandler;

            //pausing game
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(true));
            EventManager.SendShowMenuEvent(this, new EventManager.OnShowMenuEventArgs(eUiState.LoadingScreen));
            yield return null;

            //unloading pillar scene
            isPillarActive = false;

            if (!string.IsNullOrEmpty(currentSceneName))
            {
                async = SceneManager.UnloadSceneAsync(currentSceneName);

                while (!async.isDone)
                {
                    yield return null;
                }

                currentSceneName = null;
                yield return null;
            }

            //loading open world scene
            currentSceneName = sceneNamesData.GetOpenWorldSceneName();

            async = SceneManager.LoadSceneAsync(currentSceneName, LoadSceneMode.Additive);
            async.allowSceneActivation = false;

            while (!async.isDone)
            {
                if (async.progress >= 0.9f)
                {
                    async.allowSceneActivation = true;
                }
                yield return null;
            }

            Scene scene = SceneManager.GetSceneByName(currentSceneName);
            SceneManager.SetActiveScene(scene);

            worldController = SearchForScriptInScene<WorldController>(scene);
            currentSpawnPointManager = SearchForScriptInScene<SpawnPointManager>(scene);
            yield return null;

            //initializing world controller
            worldController.Initialize(this);

            //teleporting player
            Vector3 position;
            Quaternion rotation;

            if (useInitialSpawnPoint)
            {
                position = currentSpawnPointManager.GetInitialSpawnPoint();
                rotation = currentSpawnPointManager.GetInitialSpawnOrientation();
            }
            else
            {
                ePillarState pillarState = playerModel.CheckIsPillarDestroyed(activePillarId) ? ePillarState.Destroyed : ePillarState.Intact;
                position = currentSpawnPointManager.GetPillarExitPoint(activePillarId, pillarState);
                rotation = currentSpawnPointManager.GetPillarExitOrientation(activePillarId, pillarState);
            }

            var teleportPlayerEventArgs = new EventManager.TeleportPlayerEventArgs(position, rotation, true);
            EventManager.SendTeleportPlayerEvent(this, teleportPlayerEventArgs);
            yield return null;

            //informing everyone!
            EventManager.SendSceneChangedEvent(this, new EventManager.SceneChangedEventArgs());

            while (worldController.CurrentJobCount > 0)
            {
                yield return null;
                yield return null;
            }

            //unpausing game
            EventManager.SendShowMenuEvent(this, new EventManager.OnShowMenuEventArgs(eUiState.HUD));
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(false));

            //
            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;
        }

        private IEnumerator LoadPillarSceneCR(ePillarId pillarId)
        {
            AsyncOperation async;
            SceneManager.sceneLoaded += OnSceneLoadedEventHandler;

            //pausing game
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(true));
            EventManager.SendShowMenuEvent(this, new EventManager.OnShowMenuEventArgs(eUiState.LoadingScreen));
            yield return null;

            //unloading open world scene
            worldController = null;

            if (!string.IsNullOrEmpty(currentSceneName))
            {
                async = SceneManager.UnloadSceneAsync(currentSceneName);

                while (!async.isDone)
                {
                    yield return null;
                }

                currentSceneName = null;
                yield return null;
            }

            //loading pillar scene
            isPillarActive = true;
            activePillarId = pillarId;
            currentSceneName = sceneNamesData.GetPillarSceneName(pillarId);

            async = SceneManager.LoadSceneAsync(currentSceneName, LoadSceneMode.Additive);
            async.allowSceneActivation = false;

            while (!async.isDone)
            {
                if (async.progress >= 0.9f)
                {
                    async.allowSceneActivation = true;
                }

                yield return null;
            }

            Scene scene = SceneManager.GetSceneByName(currentSceneName);
            SceneManager.SetActiveScene(scene);

            currentSpawnPointManager = SearchForScriptInScene<SpawnPointManager>(scene);
            yield return null;

            //initializing world objects in pillar
            var worldObjects = SearchForScriptsInScene<IWorldObject>(scene);

            foreach (var worldObject in worldObjects)
            {
                worldObject.Initialize(this, false);
            }
            yield return null;

            //teleporting player
            var teleportPlayerEventArgs = new EventManager.TeleportPlayerEventArgs(currentSpawnPointManager.GetInitialSpawnPoint(), currentSpawnPointManager.GetInitialSpawnOrientation(), true);
            EventManager.SendTeleportPlayerEvent(this, teleportPlayerEventArgs);

            //informing everyone!
            EventManager.SendSceneChangedEvent(this, new EventManager.SceneChangedEventArgs(pillarId));

            yield return new WaitForSeconds(0.1f);

            //unpausing game
            EventManager.SendShowMenuEvent(this, new EventManager.OnShowMenuEventArgs(eUiState.HUD));
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(false));

            //
            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;
        }

        #endregion scene loading methods

        //###############################################################
        //###############################################################

        #region event methods

        private void OnSceneLoadedEventHandler(Scene scene, LoadSceneMode loadSceneMode)
        {
            Debug.Log("on scene loaded event");
            CleanScene(scene);
        }

        private void OnEnterPillarEventHandler(object sender, Utilities.EventManager.EnterPillarEventArgs args)
        {
            if (isPillarActive)
            {
                throw new Exception("A Pillar is already active!");
            }

            StartCoroutine(LoadPillarSceneCR(args.PillarId));
        }

        private void OnLeavePillarEventHandler(object sender, Utilities.EventManager.LeavePillarEventArgs args)
        {
            if (!isPillarActive)
            {
                Debug.LogError("No Pillar is active!");
                return;
            }

            //switch pillar to destroyed state
            if (args.PillarDestroyed)
            {
                playerModel.DestroyPillar(activePillarId);
            }

            StartCoroutine(LoadOpenWorldSceneCR(false));
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
            Debug.LogFormat("Scene {0} contained {1} GameControllerLite!", scene.name, gameControllerLiteInstances.Count());
            foreach (var instance in gameControllerLiteInstances)
            {
                Destroy(instance.gameObject);
            }

            var playerControllerInstances = SearchForScriptsInScene<PlayerController>(scene);
            Debug.LogFormat("Scene {0} contained {1} PlayerController!", scene.name, playerControllerInstances.Count());
            foreach (var instance in playerControllerInstances)
            {
                Destroy(instance.gameObject);
            }

            var cameraInstances = SearchForScriptsInScene<PoS_Camera>(scene);
            Debug.LogFormat("Scene {0} contained {1} PoS_Camera!", scene.name, cameraInstances.Count());
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