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

        //
        Player.PlayerController playerController;

        //
        OpenWorldSceneInfo openWorldSceneInfo = new OpenWorldSceneInfo();
        UiSceneInfo uiSceneInfo = new UiSceneInfo();
        Dictionary<World.ePillarId, PillarSceneInfo> pillarSceneDictionary = new Dictionary<World.ePillarId, PillarSceneInfo>();

        bool isPillarActive = false;
        World.ePillarId activePillarId;

        //###############################################################
        //###############################################################

        void Start()
        {
            //
            this.sceneNames = Resources.Load<SceneNamesData>("ScriptableObjects/SceneNamesData");

            //
            this.playerModel = GetComponentInChildren<Player.PlayerModel>();

            this.timeController = GetComponentInChildren<TimeController>();

            //
            this.playerController = FindObjectOfType<Player.PlayerController>();
            this.playerController.InitializePlayerController(this);

            //
            Utilities.EventManager.OnEyeKilledEvent += OnEyeKilledEventHandler;
            SceneManager.sceneLoaded += OnSceneLoadedEventHandler;

            //
            StartCoroutine(LoadScenesRoutine());
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.O))
            {
                if (this.isPillarActive)
                {
                    OnEyeKilledEventHandler(this);
                }
                else
                {
                    OnEnterPillarEventHandler(this, new Utilities.EventManager.OnEnterPillarEventArgs(World.ePillarId.Pillar_01));
                }
            }
        }

        //###############################################################
        //###############################################################

        #region initialization methods

        IEnumerator LoadScenesRoutine()
        {
            yield return null;

            SceneManager.LoadScene(UI_SCENE_NAME, LoadSceneMode.Additive);

            yield return null;

            SceneManager.LoadScene(this.sceneNames.GetOpenWorldSceneName(), LoadSceneMode.Additive);

            yield return null;

            var pillarIdValues = Enum.GetValues(typeof(World.ePillarId)).Cast<World.ePillarId>();

            foreach (var pillarId in pillarIdValues)
            {
                string name = this.sceneNames.GetPillarSceneName(pillarId);

                if (!string.IsNullOrEmpty(name))
                {
                    SceneManager.LoadScene(name, LoadSceneMode.Additive);
                }

                yield return null;
            }

            //
            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;

            Utilities.EventManager.SendOnPlayerSpawnedEvent(this, new Utilities.EventManager.OnPlayerSpawnedEventArgs(this.openWorldSceneInfo.SpawnPointManager.GetInitialSpawnPoint()));
            Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(Player.UI.eUiState.Intro));
        }

        void OnSceneLoadedEventHandler(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == UI_SCENE_NAME)
            {
                this.uiSceneInfo.UiController = SearchForScriptInScene<Player.UI.UiController>(scene);
                this.uiSceneInfo.UiController.InitializeUi(this.playerModel);

                this.uiSceneInfo.Scene = scene;
            }
            else if (scene.name == this.sceneNames.GetOpenWorldSceneName())
            {
                SceneManager.SetActiveScene(scene);

                //cleaning scene
                CleanScene(scene);

                //initializing scene
                this.openWorldSceneInfo.WorldController = SearchForScriptInScene<World.ChunkSystem.WorldController>(scene);
                this.openWorldSceneInfo.WorldController.InitializeWorldController(this.playerController.transform);

                this.openWorldSceneInfo.SpawnPointManager = SearchForScriptInScene<World.SpawnPointSystem.SpawnPointManager>(scene);

                this.openWorldSceneInfo.Scene = scene;
            }
            else
            {
                var pillarIdValues = Enum.GetValues(typeof(World.ePillarId)).Cast<World.ePillarId>();

                foreach (var pillarId in pillarIdValues)
                {
                    string name = this.sceneNames.GetPillarSceneName(pillarId);

                    if (name == scene.name)
                    {
                        //cleaning scene
                        CleanScene(scene);

                        //
                        var pillarInfo = new PillarSceneInfo()
                        {
                            Scene = scene,
                            PillarId = pillarId,
                            SpawnPointManager = SearchForScriptInScene<World.SpawnPointSystem.SpawnPointManager>(scene)
                        };

                        this.pillarSceneDictionary.Add(pillarId, pillarInfo);

                        //deactivating scene
                        foreach (var go in scene.GetRootGameObjects())
                        {
                            go.SetActive(false);
                        }
                    }
                }
            }
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

            yield return new WaitForSeconds(1f);

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

            //todo: pause game
            Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(Player.UI.eUiState.LoadingScreen));

            //deactivate pillar scene
            var pillarScene = this.pillarSceneDictionary[this.activePillarId].Scene;
            foreach (var go in pillarScene.GetRootGameObjects())
            {
                go.SetActive(false);
            }
            this.playerModel.SetPillarDestroyed(this.activePillarId);
            this.isPillarActive = false;

            //activate open world scene
            foreach (var go in this.openWorldSceneInfo.Scene.GetRootGameObjects())
            {
                go.SetActive(true);
            }
            SceneManager.SetActiveScene(this.openWorldSceneInfo.Scene);

            //
            var playerSpawnedEventArgs = new Utilities.EventManager.OnPlayerSpawnedEventArgs(this.openWorldSceneInfo.SpawnPointManager.GetPillarExitPoint(this.activePillarId));
            Utilities.EventManager.SendOnPlayerSpawnedEvent(this, playerSpawnedEventArgs);

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
        }

        //###############################################################
    }
} //end of namespace