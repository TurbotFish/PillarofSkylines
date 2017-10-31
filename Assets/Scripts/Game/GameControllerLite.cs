using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameControllerLite : MonoBehaviour, IGameControllerBase
    {
        public const string UI_SCENE_NAME = "UiScene";

        [SerializeField]
        bool showIntroMenu = false;

        //
        Player.PlayerModel playerModel;
        public Player.PlayerModel PlayerModel { get { return this.playerModel; } }

        //###############################################################
        //###############################################################

        void Start()
        {
            //
            this.playerModel = GetComponentInChildren<Player.PlayerModel>();

            //
            SceneManager.sceneLoaded += OnSceneLoadedEventHandler;
            StartCoroutine(LoadScenesRoutine());

            //
            var player = SearchForScriptInScene<Player.PlayerController>(SceneManager.GetActiveScene());
            player.InitializePlayerController(this);

            var worldController = SearchForScriptInScene<World.ChunkSystem.WorldController>(SceneManager.GetActiveScene());
            if (worldController != null)
            {
                worldController.InitializeWorldController(player.transform);
            }
        }

        //###############################################################
        //###############################################################

        IEnumerator LoadScenesRoutine()
        {
            yield return null;

            SceneManager.LoadScene(UI_SCENE_NAME, LoadSceneMode.Additive);

            yield return null;

            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;

            if (this.showIntroMenu)
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(Player.UI.eUiState.Intro));
            }
            else
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(Player.UI.eUiState.HUD));
            }
        }

        void OnSceneLoadedEventHandler(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == UI_SCENE_NAME)
            {
                var uiController = SearchForScriptInScene<Player.UI.UiController>(scene);
                uiController.InitializeUi(this.playerModel);
            }
        }

        //###############################################################
        //###############################################################

        protected static T SearchForScriptInScene<T>(Scene scene) where T : class
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

        //###############################################################
    }
}