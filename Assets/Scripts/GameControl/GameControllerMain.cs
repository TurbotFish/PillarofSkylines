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

        TimeController timeController;

        EchoManager echoManager;
        public EchoManager EchoManager { get { return this.echoManager; } }

        Player.PlayerController playerController;
        public Player.PlayerController PlayerController { get { return this.playerController; } }

        //
        OpenWorldSceneInfo openWorldSceneInfo = new OpenWorldSceneInfo();
        public World.ChunkSystem.WorldController WorldController { get { return this.openWorldSceneInfo.WorldController; } }

        UiSceneInfo uiSceneInfo = new UiSceneInfo();
        public Player.UI.UiController UiController { get { return this.uiSceneInfo.UiController; } }

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
            Utilities.EventManager.OnEyeKilledEvent += OnEyeKilledEventHandler;
            Utilities.EventManager.OnEnterPillarEvent += OnEnterPillarEventHandler;
        }

        //###############################################################
        //###############################################################

        #region initialization methods

        IEnumerator LoadScenesRoutine()
        {
            //getting references in main scene
            this.playerModel = GetComponentInChildren<Player.PlayerModel>();
            this.timeController = GetComponentInChildren<TimeController>();

            this.playerController = FindObjectOfType<Player.PlayerController>();

            yield return null;

            //loading ui scene
            SceneManager.LoadScene(UI_SCENE_NAME, LoadSceneMode.Additive);

            yield return null;

            //getting references in ui scene
            this.uiSceneInfo.Scene = SceneManager.GetSceneByName(UI_SCENE_NAME);

            this.uiSceneInfo.UiController = SearchForScriptInScene<Player.UI.UiController>(this.uiSceneInfo.Scene);

            yield return null;

            //loading open world scene
            SceneManager.LoadScene(this.sceneNames.GetOpenWorldSceneName(), LoadSceneMode.Additive);

            yield return null;

            //getting references in open world scene
            this.openWorldSceneInfo.Scene = SceneManager.GetSceneByName(this.sceneNames.GetOpenWorldSceneName());
            CleanScene(this.openWorldSceneInfo.Scene);
            SceneManager.SetActiveScene(this.openWorldSceneInfo.Scene);

            this.openWorldSceneInfo.WorldController = SearchForScriptInScene<World.ChunkSystem.WorldController>(this.openWorldSceneInfo.Scene);
            this.openWorldSceneInfo.SpawnPointManager = SearchForScriptInScene<World.SpawnPointSystem.SpawnPointManager>(this.openWorldSceneInfo.Scene);

            yield return null;

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

                //getting references in pillar scene
                var pillarScene = SceneManager.GetSceneByName(name);
                CleanScene(pillarScene);

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
            }

            //initializing
            this.playerModel.InitializePlayerModel();
            this.playerController.InitializePlayerController(this);

            this.uiSceneInfo.UiController.InitializeUi(this.playerModel);

            this.openWorldSceneInfo.WorldController.InitializeWorldController(this.playerController.transform);


            //starting the game
            Utilities.EventManager.SendOnPlayerSpawnedEvent(this, new Utilities.EventManager.OnPlayerSpawnedEventArgs(this.openWorldSceneInfo.SpawnPointManager.GetInitialSpawnPoint()));
            Utilities.EventManager.SendOnSceneChangedEvent(this, new Utilities.EventManager.OnSceneChangedEventArgs());
            Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(Player.UI.eUiState.Intro));
        }

        #endregion initialization methods

        //###############################################################
        //###############################################################

        void OnEnterPillarEventHandler(object sender, Utilities.EventManager.OnEnterPillarEventArgs args)
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
            Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(Player.UI.eUiState.LoadingScreen));

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
            Utilities.EventManager.SendOnPlayerSpawnedEvent(this, new Utilities.EventManager.OnPlayerSpawnedEventArgs(info.SpawnPointManager.GetInitialSpawnPoint()));
            Utilities.EventManager.SendOnSceneChangedEvent(this, new Utilities.EventManager.OnSceneChangedEventArgs(pillarId));

            yield return new WaitForSeconds(0.1f);

            //
            Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(Player.UI.eUiState.HUD));
            //todo: unpause game
        }

        void OnEyeKilledEventHandler(object sender)
        {
            if (!this.isPillarActive)
            {
                throw new Exception("No Pillar is active!");
            }

            StartCoroutine(ActivateOpenWorldScene());
        }

        IEnumerator ActivateOpenWorldScene()
        {
            //todo: pause game
            Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(Player.UI.eUiState.LoadingScreen));

            yield return null;

            //deactivate pillar scene
            var pillarScene = this.pillarSceneDictionary[this.activePillarId].Scene;
            foreach (var go in pillarScene.GetRootGameObjects())
            {
                go.SetActive(false);
            }
            this.playerModel.SetPillarDestroyed(this.activePillarId);
            this.isPillarActive = false;

            yield return null;

            //activate open world scene
            foreach (var go in this.openWorldSceneInfo.Scene.GetRootGameObjects())
            {
                go.SetActive(true);
            }
            SceneManager.SetActiveScene(this.openWorldSceneInfo.Scene);

            yield return null;

            //
            var playerSpawnedEventArgs = new Utilities.EventManager.OnPlayerSpawnedEventArgs(this.openWorldSceneInfo.SpawnPointManager.GetPillarExitPoint(this.activePillarId));
            Utilities.EventManager.SendOnPlayerSpawnedEvent(this, playerSpawnedEventArgs);

            Utilities.EventManager.SendOnSceneChangedEvent(this, new Utilities.EventManager.OnSceneChangedEventArgs());

            yield return new WaitForSeconds(0.1f);

            //
            Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(Player.UI.eUiState.HUD));
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
            var echoManagers = SearchForScriptsInScene<EchoManager>(scene);
            foreach (var echoManager in echoManagers)
            {
                Destroy(echoManager.gameObject);
            }
        }

        //###############################################################
    }
} //end of namespace