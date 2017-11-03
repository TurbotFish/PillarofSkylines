using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.GameControl
{
    public class GameControllerLite : MonoBehaviour, IGameControllerBase
    {
        public const string UI_SCENE_NAME = "UiScene";

        [SerializeField]
        bool showIntroMenu = false;

        //
        Player.PlayerModel playerModel;
        public Player.PlayerModel PlayerModel { get { return this.playerModel; } }

        EchoManager echoManager;
        public EchoManager EchoManager { get { return this.echoManager; } }

        Player.PlayerController playerController;
        public Player.PlayerController PlayerController { get { return this.playerController; } }

        World.ChunkSystem.WorldController worldController;
        public World.ChunkSystem.WorldController WorldController { get { return this.worldController; } }

        Player.UI.UiController uiController;
        public Player.UI.UiController UiController { get { return this.uiController; } }

        //###############################################################
        //###############################################################

        void Start()
        {
            StartCoroutine(LoadScenesRoutine());

            //cleaning up, just in case
            var echoManagers = FindObjectsOfType<EchoManager>();
            foreach(var echoManager in echoManagers)
            {
                if(echoManager != this.echoManager)
                {
                    Destroy(echoManager.gameObject);
                }
            }
        }

        //###############################################################
        //###############################################################

        IEnumerator LoadScenesRoutine()
        {
            //getting references in local scene
            this.playerModel = GetComponentInChildren<Player.PlayerModel>();
            this.echoManager = GetComponentInChildren<EchoManager>();

            var player = SearchForScriptInScene<Player.PlayerController>(SceneManager.GetActiveScene());
            var worldController = SearchForScriptInScene<World.ChunkSystem.WorldController>(SceneManager.GetActiveScene());

            yield return null;

            //loading UI scene
            SceneManager.LoadScene(UI_SCENE_NAME, LoadSceneMode.Additive);

            yield return null;

            //getting references in UI scene
            var uiScene = SceneManager.GetSceneByName(UI_SCENE_NAME);

            this.uiController = SearchForScriptInScene<Player.UI.UiController>(uiScene);

            yield return null;

            //initializing
            this.playerModel.InitializePlayerModel();

            player.InitializePlayerController(this);

            if (worldController != null)
            {
                worldController.InitializeWorldController(player.transform);
            }

            this.uiController.InitializeUi(this.playerModel);

            //starting the game
            if (this.showIntroMenu)
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(Player.UI.eUiState.Intro));
            }
            else
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(Player.UI.eUiState.HUD));
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