using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameControllerLite : GameControllerBase
    {
        protected override void Start()
        {
            base.Start();

            var player = SearchForScriptInScene<Player.PlayerController>(SceneManager.GetActiveScene());
            player.InitializePlayerController(this);

            var worldController = SearchForScriptInScene<World.ChunkSystem.WorldController>(SceneManager.GetActiveScene());
            worldController.InitializeWorldController(player.transform);
        }

        protected override IEnumerator LoadScenesRoutine()
        {
            yield return null;

            SceneManager.LoadScene(UI_SCENE_NAME, LoadSceneMode.Additive);

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
        }
    }
}