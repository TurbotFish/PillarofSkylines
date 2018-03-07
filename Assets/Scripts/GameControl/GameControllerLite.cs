using Game.Model;
using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.GameControl
{
    public class GameControllerLite : MonoBehaviour, IGameControllerBase
    {
        public const string UI_SCENE_NAME = "UiScene";

        //###############################################################

        [SerializeField]
        private bool showIntroMenu = false;

        [SerializeField]
        private GameObject uiPrefab;

        //
        private PlayerModel playerModel;
        private EchoSystem.EchoManager echoManager;
        private EclipseManager eclipseManager;

        //
        private UI.UiController uiController;

        //
        private Player.PlayerController playerController;
        private WorldController worldController;
        

        //###############################################################

        public PlayerModel PlayerModel { get { return playerModel; } }
        public EchoSystem.EchoManager EchoManager { get { return echoManager; } }
        public EclipseManager EclipseManager { get { return eclipseManager; } }

        public UI.UiController UiController { get { return uiController; } }

        public Player.PlayerController PlayerController { get { return playerController; } }
        public CameraControl.CameraController CameraController { get; private set; }
        public WorldController WorldController { get { return worldController; } }

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
            playerModel = GetComponentInChildren<PlayerModel>();
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
            worldController = FindObjectOfType<WorldController>();

            //initializing the ui
            uiController.InitializeUi(this, UI.eUiState.LoadingScreen);

            yield return null;
            //***********************

            //initializing game
            playerController.InitializePlayerController(this);
            CameraController.InitializeCameraController(this);

            //teleporting: this puts the camera in the correct position which makes the WorldController preload the right part of the world
            var teleportPlayerEventArgs = new Utilities.EventManager.TeleportPlayerEventArgs(playerController.transform.position, playerController.transform.rotation, true);
            Utilities.EventManager.SendTeleportPlayerEvent(this, teleportPlayerEventArgs);

            //pausing game until world has loaded a bit
            Utilities.EventManager.SendGamePausedEvent(this, new Utilities.EventManager.GamePausedEventArgs(true));

            yield return null;

            if (worldController != null)
            {
                worldController.Initialize(this);
                yield return null;
                yield return null;

                while (worldController.CurrentJobCount > 0)
                {
                    yield return null;
                }
            }
            else //this is a Pillar
            {
                var worldObjects = FindObjectsOfType<MonoBehaviour>();
                foreach(var obj in worldObjects)
                {
                    if (obj is IWorldObject)
                    {
                        (obj as IWorldObject).Initialize(this, false);
                    }
                }
                yield return null;
            }

            echoManager.InitializeEchoManager(this);
            EclipseManager.InitializeEclipseManager(this);

            yield return null;
            //***********************

            //starting game
            Utilities.EventManager.SendSceneChangedEvent(this, new Utilities.EventManager.SceneChangedEventArgs());
            Utilities.EventManager.SendGamePausedEvent(this, new Utilities.EventManager.GamePausedEventArgs(false));

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