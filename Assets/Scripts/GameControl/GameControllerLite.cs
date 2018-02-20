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

        [SerializeField]
        private GameObject uiPrefab;

        //
        Player.PlayerModel playerModel;
        public Player.PlayerModel PlayerModel { get { return playerModel; } }

        EchoSystem.EchoManager echoManager;
        public EchoSystem.EchoManager EchoManager { get { return echoManager; } }

        EclipseManager eclipseManager;
        public EclipseManager EclipseManager { get { return eclipseManager; } }


        //
        UI.UiController uiController;
        public UI.UiController UiController { get { return uiController; } }


        //
        Player.PlayerController playerController;
        public Player.PlayerController PlayerController { get { return playerController; } }

        public CameraControl.CameraController CameraController { get; private set; }

        World.ChunkSystem.WorldController worldController;
        public World.ChunkSystem.WorldController WorldController { get { return worldController; } }

        //###############################################################
        //###############################################################

        void Start()
        {
            StartCoroutine(LoadScenesRoutine());
        }

        /// <summary>
        /// DO NOT USE!
        /// </summary>
        public void StartGame()
        {
            throw new System.NotImplementedException();
        }

        //###############################################################
        //###############################################################

        IEnumerator LoadScenesRoutine()
        {
            //***********************
            //loading the UI

            var uiGO = Instantiate(uiPrefab);
            uiController = uiGO.GetComponentInChildren<UI.UiController>();

            yield return null;
            //***********************

            //getting references in game controller
            playerModel = GetComponentInChildren<Player.PlayerModel>();
            echoManager = GetComponentInChildren<EchoSystem.EchoManager>();
            eclipseManager = GetComponentInChildren<EclipseManager>();

            //initializing game controller
            playerModel.InitializePlayerModel();

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
            playerController = FindObjectOfType<Player.PlayerController>();
            CameraController = FindObjectOfType<CameraControl.CameraController>();
            worldController = FindObjectOfType<World.ChunkSystem.WorldController>();

            //initializing the ui
            uiController.InitializeUi(this, UI.eUiState.LoadingScreen);

            yield return null;
            //***********************

            //initializing game
            playerController.InitializePlayerController(this);
            CameraController.InitializeCameraController(this);

            if (worldController != null)
            {
                worldController.InitializeWorldController(this);
            }
            else //this is a Pillar
            {
                var worldObjects = FindObjectsOfType<MonoBehaviour>();
                foreach(var obj in worldObjects)
                {
                    if (obj is World.IWorldObjectInitialization)
                    {
                        (obj as World.IWorldObjectInitialization).Initialize(this, false);
                    }
                }
            }

            echoManager.InitializeEchoManager(this);
            EclipseManager.InitializeEclipseManager(this);

            yield return null;
            //***********************

            //starting game
            Utilities.EventManager.SendSceneChangedEvent(this, new Utilities.EventManager.SceneChangedEventArgs());

            if (showIntroMenu)
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