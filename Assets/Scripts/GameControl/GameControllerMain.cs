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

        // -- ATTRIBUTES     

        public PlayerModel PlayerModel { get; private set; }
        public EchoManager EchoManager { get; private set; }
        public EclipseManager EclipseManager { get; private set; }

        public PlayerController PlayerController { get; private set; }
        public CameraController CameraController { get; private set; }
        public UiController UiController { get; private set; }

        public bool IsOpenWorldLoaded { get; private set; }
        public WorldController WorldController { get; private set; }
        public DuplicationCameraController DuplicationCameraController { get; private set; }

        public bool IsPillarLoaded { get; private set; }
        public ePillarId ActivePillarId { get; private set; }

        public SpawnPointManager SpawnPointManager { get; private set; }

        private SceneNamesData sceneNamesData;
        private bool isInitialized = false;
        private bool isGameStarted = false;


        //###############################################################
        //###############################################################

        // -- INITIALIZATION

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

        private IEnumerator LoadOpenWorldSceneCR()
        {
            CameraController.PoS_Camera.CameraComponent.enabled = false;

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
            DuplicationCameraController = SearchForScriptInScene<DuplicationCameraController>(scene);
            yield return null;

            WorldController.Initialize(this);
            DuplicationCameraController.Initialize(this);

            yield return null;

            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;
            isInitialized = true;

            EventManager.SendShowMenuEvent(this, new EventManager.OnShowMenuEventArgs(eUiState.MainMenu));
        }

        //###############################################################
        //###############################################################

        // -- OPERATIONS

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

        /// <summary>
        /// Closes the Application.
        /// </summary>
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// Coroutine for switching to the open world scene.
        /// </summary>
        /// <param name="useInitialSpawnPoint"></param>
        /// <returns></returns>
        private IEnumerator ActivateOpenWorldCR(bool useInitialSpawnPoint)
        {
            CameraController.PoS_Camera.CameraComponent.enabled = false;

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
            DuplicationCameraController = SearchForScriptInScene<DuplicationCameraController>(scene);
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
            DuplicationCameraController.Activate();

            while (WorldController.CurrentState == eWorldControllerState.Activating)
            {
                yield return null;
            }

            //*****************************************
            //informing everyone!
            EventManager.SendSceneChangedEvent(this, new EventManager.SceneChangedEventArgs());

            //*****************************************
            //unpausing game
            CameraController.PoS_Camera.CameraComponent.enabled = true;
            yield return new WaitForSeconds(0.5f);

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
            CameraController.PoS_Camera.CameraComponent.enabled = false;

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
            DuplicationCameraController.Deactivate();

            while (WorldController.CurrentState == eWorldControllerState.Deactivating)
            {
                yield return null;
            }

            IsOpenWorldLoaded = false;
            WorldController = null;
            DuplicationCameraController = null;
            SpawnPointManager = null;

            foreach (var obj in scene.GetRootGameObjects())
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
                worldObject.Initialize(this);
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
            CameraController.PoS_Camera.CameraComponent.enabled = true;
            yield return new WaitForSeconds(0.5f);

            EventManager.SendShowMenuEvent(this, new EventManager.OnShowMenuEventArgs(eUiState.HUD));
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(false));

            //*****************************************
            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;
        }

        /// <summary>
        /// Handles the Unity "sceneLoaded" event.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="loadSceneMode"></param>
        private void OnSceneLoadedEventHandler(Scene scene, LoadSceneMode loadSceneMode)
        {
            //Debug.Log("on scene loaded event");
            CleanScene(scene);
        }

        /// <summary>
        /// Handles the "EnterPillar" event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnEnterPillarEventHandler(object sender, EventManager.EnterPillarEventArgs args)
        {
            if (IsPillarLoaded)
            {
                throw new Exception("A Pillar is already active!");
            }

            StartCoroutine(LoadPillarSceneCR(args.PillarId));
        }

        /// <summary>
        /// Handles the "LeavePillar" event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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

        /// <summary>
        /// Returns the first instance of T found in the scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <returns></returns>
        private T SearchForScriptInScene<T>(Scene scene) where T : class
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

        /// <summary>
        /// Returns all instances of T in the scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <returns></returns>
        private List<T> SearchForScriptsInScene<T>(Scene scene) where T : class
        {
            var result = new List<T>();

            foreach (var gameObject in scene.GetRootGameObjects())
            {
                result.AddRange(gameObject.GetComponentsInChildren<T>(true));
            }

            return result;
        }

        /// <summary>
        /// Cleans up the Level scene.
        /// </summary>
        /// <param name="scene"></param>
        private void CleanScene(Scene scene)
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

        //###############################################################
    }
} //end of namespace