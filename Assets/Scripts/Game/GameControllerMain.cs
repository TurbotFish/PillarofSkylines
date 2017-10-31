using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameControllerMain : MonoBehaviour, IGameControllerBase
    {
        public const string UI_SCENE_NAME = "UiScene";

        //
        SceneNamesData sceneNames;

        //
        Player.PlayerModel playerModel;
        public Player.PlayerModel PlayerModel { get { return this.playerModel; } }

        //
        Player.PlayerController playerController;

        //
        OpenWorldSceneInfo openWorldSceneInfo = new OpenWorldSceneInfo();
        UiSceneInfo uiSceneInfo = new UiSceneInfo();
        Dictionary<World.ePillarId, PillarSceneInfo> pillarSceneDictionary = new Dictionary<World.ePillarId, PillarSceneInfo>();

        //###############################################################
        //###############################################################

        void Start()
        {
            //
            this.sceneNames = Resources.Load<SceneNamesData>("ScriptableObjects/SceneNamesData");

            //
            this.playerModel = GetComponentInChildren<Player.PlayerModel>();

            //
            this.playerController = FindObjectOfType<Player.PlayerController>();
            this.playerController.InitializePlayerController(this);

            //
            SceneManager.sceneLoaded += OnSceneLoadedEventHandler;

            StartCoroutine(LoadScenesRoutine());
        }

        //###############################################################
        //###############################################################

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

            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;
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
                        foreach(var go in scene.GetRootGameObjects())
                        {
                            go.SetActive(false);
                        }
                    }
                }
            }
        }

        //###############################################################
        //###############################################################

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