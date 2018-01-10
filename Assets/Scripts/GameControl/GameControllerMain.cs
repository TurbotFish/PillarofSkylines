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
        public const string UI_SCENE_NAME = "UiScene";

        //
        SceneNamesData sceneNames;

        //
        Player.PlayerModel playerModel;
        public Player.PlayerModel PlayerModel { get { return this.playerModel; } }

        //TimeController timeController;

        public EchoSystem.EchoManager EchoManager { get; private set; }
        public EclipseManager EclipseManager { get; private set; }


        //
        UiSceneInfo uiSceneInfo = new UiSceneInfo();
        public UI.UiController UiController { get { return this.uiSceneInfo.UiController; } }


        //
        Player.PlayerController playerController;
        public Player.PlayerController PlayerController { get { return this.playerController; } }

        public CameraControl.CameraController CameraController { get; private set; }

        OpenWorldSceneInfo openWorldSceneInfo = new OpenWorldSceneInfo();
        public World.ChunkSystem.WorldController WorldController { get { return this.openWorldSceneInfo.WorldController; } }

        Dictionary<World.ePillarId, PillarSceneInfo> pillarSceneDictionary = new Dictionary<World.ePillarId, PillarSceneInfo>();


        //
        bool isPillarActive = false;
        World.ePillarId activePillarId;

        //###############################################################
        //###############################################################

        void Start()
        {
            //load resources
            this.sceneNames = Resources.Load<SceneNamesData>("ScriptableObjects/SceneNamesData");

            //load everything            
            StartCoroutine(LoadScenesRoutine());

            //register to events
            Utilities.EventManager.LeavePillarEvent += OnLeavePillarEventHandler;
            Utilities.EventManager.EnterPillarEvent += OnEnterPillarEventHandler;
        }

        void Update() //for testing only!
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                Utilities.EventManager.SendLeavePillarEvent(this, new Utilities.EventManager.LeavePillarEventArgs(true));
            }
        }

        //###############################################################
        //###############################################################

        #region initialization methods

        IEnumerator LoadScenesRoutine()
        {
            SceneManager.sceneLoaded += OnSceneLoadedEventHandler;

            //getting references in game controller
            this.playerModel = GetComponentInChildren<Player.PlayerModel>();
            //this.timeController = GetComponentInChildren<TimeController>();
            this.EchoManager = GetComponentInChildren<EchoSystem.EchoManager>();
            this.EclipseManager = GetComponentInChildren<EclipseManager>();

            //initializing game controller
            this.playerModel.InitializePlayerModel();

            //gatting references in main scene
            this.playerController = FindObjectOfType<Player.PlayerController>();
            this.CameraController = FindObjectOfType<CameraControl.CameraController>();

            yield return null;
            //***********************

            //loading ui scene
            SceneManager.LoadScene(UI_SCENE_NAME, LoadSceneMode.Additive);

            yield return null;
            //***********************

            //getting references in ui scene
            this.uiSceneInfo.Scene = SceneManager.GetSceneByName(UI_SCENE_NAME);
            this.uiSceneInfo.UiController = SearchForScriptInScene<UI.UiController>(this.uiSceneInfo.Scene);

            //initializing ui
            this.uiSceneInfo.UiController.InitializeUi(this);

            yield return null;
            //***********************

            //loading open world scene
            SceneManager.LoadScene(this.sceneNames.GetOpenWorldSceneName(), LoadSceneMode.Additive);

            yield return null;
            //***********************

            //getting references in open world scene
            this.openWorldSceneInfo.Scene = SceneManager.GetSceneByName(this.sceneNames.GetOpenWorldSceneName());
            SceneManager.SetActiveScene(this.openWorldSceneInfo.Scene);

            this.openWorldSceneInfo.WorldController = SearchForScriptInScene<World.ChunkSystem.WorldController>(this.openWorldSceneInfo.Scene);
            this.openWorldSceneInfo.SpawnPointManager = SearchForScriptInScene<World.SpawnPointSystem.SpawnPointManager>(this.openWorldSceneInfo.Scene);

            yield return null;
            //***********************

            //pillar scenes
            var pillarIdValues = Enum.GetValues(typeof(World.ePillarId)).Cast<World.ePillarId>();

            foreach (var pillarId in pillarIdValues)
            {
                string name = this.sceneNames.GetPillarSceneName(pillarId);

                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                //loading pillar scene
                SceneManager.LoadScene(name, LoadSceneMode.Additive);

                yield return null;
                //***********************

                //getting references in pillar scene
                var pillarScene = SceneManager.GetSceneByName(name);
                var pillarInfo = new PillarSceneInfo()
                {
                    Scene = pillarScene,
                    PillarId = pillarId,
                    SpawnPointManager = SearchForScriptInScene<World.SpawnPointSystem.SpawnPointManager>(pillarScene)
                };

                this.pillarSceneDictionary.Add(pillarId, pillarInfo);

                //deactivating scene
                foreach (var go in pillarScene.GetRootGameObjects())
                {
                    go.SetActive(false);
                }

                yield return null;
                //***********************
            }

            //initializing game
            this.playerController.InitializePlayerController(this);
            this.CameraController.InitializeCameraController(this);

            this.openWorldSceneInfo.WorldController.InitializeWorldController(this.playerController.transform, CameraController.transform);

            this.EchoManager.InitializeEchoManager(this, openWorldSceneInfo.SpawnPointManager);
            this.EclipseManager.InitializeEclipseManager(this);

            yield return null;
            //***********************

            //starting the game
            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;

            var teleportPlayerEventArgs = new Utilities.EventManager.TeleportPlayerEventArgs(this.openWorldSceneInfo.SpawnPointManager.GetInitialSpawnPoint(), this.openWorldSceneInfo.SpawnPointManager.GetInitialSpawnOrientation(), true);
            Utilities.EventManager.SendTeleportPlayerEvent(this, teleportPlayerEventArgs);

            Utilities.EventManager.SendSceneChangedEvent(this, new Utilities.EventManager.SceneChangedEventArgs());
            Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(UI.eUiState.Intro));
        }

        #endregion initialization methods

        //###############################################################
        //###############################################################

        void OnSceneLoadedEventHandler(Scene scene, LoadSceneMode loadSceneMode)
        {
            CleanScene(scene);
        }

        void OnEnterPillarEventHandler(object sender, Utilities.EventManager.EnterPillarEventArgs args)
        {
            if (this.isPillarActive)
            {
                throw new Exception("A Pillar is already active!");
            }

            StartCoroutine(ActivatePillarScene(args.PillarId));
        }

        IEnumerator ActivatePillarScene(World.ePillarId pillarId)
        {
            //todo: pause game
            Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(UI.eUiState.LoadingScreen));

            yield return null;

            //deactivate open world scene
            foreach (var go in this.openWorldSceneInfo.Scene.GetRootGameObjects())
            {
                go.SetActive(false);
            }

            yield return null;

            //activate pillar scene
            this.isPillarActive = true;
            this.activePillarId = pillarId;
            var info = this.pillarSceneDictionary[this.activePillarId];
            var pillarScene = info.Scene;
            foreach (var go in pillarScene.GetRootGameObjects())
            {
                go.SetActive(true);
            }
            SceneManager.SetActiveScene(pillarScene);

            yield return null;

            //
            var teleportPlayerEventArgs = new Utilities.EventManager.TeleportPlayerEventArgs(info.SpawnPointManager.GetInitialSpawnPoint(), info.SpawnPointManager.GetInitialSpawnOrientation(), true);
            Utilities.EventManager.SendTeleportPlayerEvent(this, teleportPlayerEventArgs);

            Utilities.EventManager.SendSceneChangedEvent(this, new Utilities.EventManager.SceneChangedEventArgs(pillarId));

            yield return new WaitForSeconds(0.1f);

            //
            Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(UI.eUiState.HUD));
            //todo: unpause game
        }

        void OnLeavePillarEventHandler(object sender, Utilities.EventManager.LeavePillarEventArgs args)
        {
            Debug.LogError("OnEyeKilledEventHandler called!");

            if (!this.isPillarActive)
            {
                throw new Exception("No Pillar is active!");
            }

            StartCoroutine(ActivateOpenWorldScene(args.PillarDestroyed));
        }

        IEnumerator ActivateOpenWorldScene(bool pillarDestroyed)
        {
            //todo: pause game
            Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(UI.eUiState.LoadingScreen));

            yield return null;

            //deactivate pillar scene
            var pillarScene = this.pillarSceneDictionary[this.activePillarId].Scene;
            foreach (var go in pillarScene.GetRootGameObjects())
            {
                go.SetActive(false);
            }

            this.isPillarActive = false;

            yield return null;

            //activate open world scene
            foreach (var go in this.openWorldSceneInfo.Scene.GetRootGameObjects())
            {
                go.SetActive(true);
            }
            SceneManager.SetActiveScene(this.openWorldSceneInfo.Scene);

            yield return null;

            //switch pillar to destroyed state
            if (pillarDestroyed)
            {
                this.playerModel.SetPillarDestroyed(this.activePillarId);
            }

            yield return null;

            //
            var teleportPlayerEventArgs = new Utilities.EventManager.TeleportPlayerEventArgs(this.openWorldSceneInfo.SpawnPointManager.GetPillarExitPoint(this.activePillarId), this.openWorldSceneInfo.SpawnPointManager.GetPillarExitOrientation(this.activePillarId), true);
            Utilities.EventManager.SendTeleportPlayerEvent(this, teleportPlayerEventArgs);

            Utilities.EventManager.SendSceneChangedEvent(this, new Utilities.EventManager.SceneChangedEventArgs());

            yield return new WaitForSeconds(0.1f);

            //
            Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(UI.eUiState.HUD));
            //todo: unpause game
        }

        //###############################################################
        //###############################################################

        static T SearchForScriptInScene<T>(Scene scene) where T : class
        {
            T result = null;

            foreach (var gameObject in scene.GetRootGameObjects())
            {
                result = gameObject.GetComponentInChildren<T>();

                if (result != null)
                {
                    break;
                }
            }

            return result;
        }

        static List<T> SearchForScriptsInScene<T>(Scene scene) where T : class
        {
            var result = new List<T>();

            foreach (var gameObject in scene.GetRootGameObjects())
            {
                result.AddRange(gameObject.GetComponentsInChildren<T>());
            }

            return result;
        }

        static void CleanScene(Scene scene)
        {
            var gameControllerLite = SearchForScriptInScene<GameControllerLite>(scene);
            if (gameControllerLite != null)
            {
                Destroy(gameControllerLite.gameObject);
            }

            var scenePlayer = SearchForScriptInScene<Player.PlayerController>(scene);
            if (scenePlayer != null)
            {
                Destroy(scenePlayer.gameObject);
            }

            var sceneCamera = SearchForScriptInScene<PoS_Camera>(scene);
            if (sceneCamera != null)
            {
                Destroy(sceneCamera.gameObject);
            }

            //cleaning up old EchoManagers
            var echoManagers = SearchForScriptsInScene<EchoSystem.EchoManager>(scene);
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