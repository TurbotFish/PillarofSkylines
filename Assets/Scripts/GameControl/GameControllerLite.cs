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

        EchoSystem.EchoManager echoManager;
        public EchoSystem.EchoManager EchoManager { get { return this.echoManager; } }

        EclipseManager eclipseManager;
        public EclipseManager EclipseManager { get { return this.eclipseManager; } }


        //
        UI.UiController uiController;
        public UI.UiController UiController { get { return this.uiController; } }


        //
        Player.PlayerController playerController;
        public Player.PlayerController PlayerController { get { return this.playerController; } }

        public CameraControl.CameraController CameraController { get; private set; }

        World.ChunkSystem.WorldController worldController;
        public World.ChunkSystem.WorldController WorldController { get { return this.worldController; } }

        //###############################################################
        //###############################################################

        void Start()
        {
            StartCoroutine(LoadScenesRoutine());
        }

        //###############################################################
        //###############################################################

        IEnumerator LoadScenesRoutine()
        {
            yield return null;
            yield return null;
            //***********************

            //getting references in game controller
            this.playerModel = GetComponentInChildren<Player.PlayerModel>();
            this.echoManager = GetComponentInChildren<EchoSystem.EchoManager>();
            this.eclipseManager = GetComponentInChildren<EclipseManager>();

            //initializing game controller
            this.playerModel.InitializePlayerModel();

            //cleaning up, just in case
            var echoManagers = FindObjectsOfType<EchoSystem.EchoManager>();
            foreach (var echoManager in echoManagers)
            {
                if (echoManager != this.echoManager)
                {
                    Destroy(echoManager.gameObject);
                }
            }

            var eclipseManagers = FindObjectsOfType<EclipseManager>();
            foreach (var eclipseManager in eclipseManagers)
            {
                if (eclipseManager != this.eclipseManager)
                {
                    Destroy(eclipseManager.gameObject);
                }
            }

            //getting references in local scene
            this.playerController = FindObjectOfType<Player.PlayerController>();
            this.CameraController = FindObjectOfType<CameraControl.CameraController>();
            this.worldController = FindObjectOfType<World.ChunkSystem.WorldController>();

            yield return null;
            //***********************

            //loading UI scene
            SceneManager.LoadScene(UI_SCENE_NAME, LoadSceneMode.Additive);

            yield return null;
            //***********************

            //getting references in UI scene
            var uiScene = SceneManager.GetSceneByName(UI_SCENE_NAME);
            this.uiController = SearchForScriptInScene<UI.UiController>(uiScene);

            //initializing ui
            this.uiController.InitializeUi(this);

            yield return null;
            //***********************

            //initializing game
            this.playerController.InitializePlayerController(this);
            this.CameraController.InitializeCameraController(this);
            if (worldController != null)
            {
                worldController.InitializeWorldController(this.playerController.transform);
            }

            yield return null;
            //***********************

            //starting game
            if (this.showIntroMenu)
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(UI.eUiState.Intro));
            }
            else
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(UI.eUiState.HUD));
            }
        }

        //###############################################################
        //###############################################################

        protected static T SearchForScriptInScene<T>(Scene scene) where T : Object
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