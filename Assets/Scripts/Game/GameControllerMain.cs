using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameControllerMain : GameControllerBase
    {

        [Header("Scenes")]
        [SerializeField]
        SceneNames sceneNames;

        [Header("")]
        [SerializeField]
        Player.PlayerController playerController;




        Scene openWorldScene;
        World.ChunkSystem.WorldController worldController;

        protected override void Start()
        {
            base.Start();

            playerController.InitializePlayerController(this);
        }

        protected override IEnumerator LoadScenesRoutine()
        {
            yield return null;

            SceneManager.LoadScene(UI_SCENE_NAME, LoadSceneMode.Additive);

            yield return null;

            SceneManager.LoadScene(this.sceneNames.OpenWorldSceneName, LoadSceneMode.Additive);

            yield return null;

            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;
            Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(Player.UI.eUiState.Intro));
        }

        protected override void OnSceneLoadedEventHandler(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == UI_SCENE_NAME)
            {
                this.UiController = SearchForScriptInScene<Player.UI.UiController>(scene);
                this.UiController.InitializeUi(this.playerModel);

                this.uiScene = scene;
            }
            else if (scene.name == this.sceneNames.OpenWorldSceneName)
            {
                SceneManager.SetActiveScene(scene);

                //cleaning scene

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

                //initializing scene

                this.worldController = SearchForScriptInScene<World.ChunkSystem.WorldController>(scene);
                this.worldController.InitializeWorldController(this.playerController.transform);

                this.openWorldScene = scene;
            }
        }


    }
} //end of namespace