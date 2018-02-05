﻿using System;
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

        //###############################################################

        private SceneNamesData sceneNames;

        private Player.PlayerModel playerModel;
        private EchoSystem.EchoManager echoManager;
        private EclipseManager eclipseManager;

        private OpenWorldSceneInfo openWorldSceneInfo = new OpenWorldSceneInfo();

        private Player.PlayerController playerController;
        private CameraControl.CameraController cameraController;
        private UI.UiController uiController;

        private Dictionary<World.ePillarId, PillarSceneInfo> pillarSceneDictionary = new Dictionary<World.ePillarId, PillarSceneInfo>();

        private bool isPillarActive = false;
        private World.ePillarId activePillarId;
        private bool isGameStarted = false;

        //###############################################################

        public Player.PlayerModel PlayerModel { get { return playerModel; } }
        public EchoSystem.EchoManager EchoManager { get { return echoManager; } }
        public EclipseManager EclipseManager { get { return eclipseManager; } }

        public World.ChunkSystem.WorldController WorldController { get { return openWorldSceneInfo.WorldController; } }

        public Player.PlayerController PlayerController { get { return playerController; } }
        public CameraControl.CameraController CameraController { get { return cameraController; } }
        public UI.UiController UiController { get { return uiController; } }

        //###############################################################
        //###############################################################

        void Start()
        {
            //load resources
            sceneNames = Resources.Load<SceneNamesData>("ScriptableObjects/SceneNamesData");

            //getting references in game controller
            playerModel = GetComponentInChildren<Player.PlayerModel>();
            echoManager = GetComponentInChildren<EchoSystem.EchoManager>();
            eclipseManager = GetComponentInChildren<EclipseManager>();

            //initializing game controller
            playerModel.InitializePlayerModel();

            //gatting references in main scene
            playerController = FindObjectOfType<Player.PlayerController>();
            cameraController = FindObjectOfType<CameraControl.CameraController>();
            uiController = FindObjectOfType<UI.UiController>();

            //initializing the UI
            uiController.InitializeUi(this, UI.eUiState.MainMenu);

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

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void StartGame()
        {
            if (isGameStarted)
            {
                return;
            }

            StartCoroutine(StartGameCR());
        }

        IEnumerator StartGameCR()
        {
            //switch to loading screen
            Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(UI.eUiState.LoadingScreen));

            //register to sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoadedEventHandler;

            //loading open world scene
            SceneManager.LoadScene(sceneNames.GetOpenWorldSceneName(), LoadSceneMode.Additive);

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

            openWorldSceneInfo.WorldController.InitializeWorldController(this);

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
                this.playerModel.DestroyPillar(this.activePillarId);
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