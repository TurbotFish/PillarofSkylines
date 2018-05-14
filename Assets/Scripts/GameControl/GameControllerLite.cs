using Game.CameraControl;
using Game.Model;
using Game.Utilities;
using Game.World;
using System;
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
        private Player.PlayerController playerController;

        //
        private bool isOpenWorldLoaded;
        private WorldController worldController;
        private DuplicationCameraManager duplicationCameraController;

        //
        private bool isPillarLoaded;

        //###############################################################

        public PlayerModel PlayerModel { get { return playerModel; } }
        public EchoSystem.EchoManager EchoManager { get { return echoManager; } }
        public EclipseManager EclipseManager { get { return eclipseManager; } }

        public UI.UiController UiController { get { return uiController; } }
        public Player.PlayerController PlayerController { get { return playerController; } }
        public CameraController CameraController { get; private set; }

        public bool IsOpenWorldLoaded { get { return isOpenWorldLoaded; } }
        public WorldController WorldController { get { return worldController; } }
        public DuplicationCameraManager DuplicationCameraController { get { return duplicationCameraController; } }

        public bool IsPillarLoaded { get { return isPillarLoaded; } }
        public ePillarId ActivePillarId { get { throw new NotImplementedException(); } }

        public SpawnPointManager SpawnPointManager { get; private set; }

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

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
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
            CameraController = FindObjectOfType<CameraController>();
            worldController = FindObjectOfType<WorldController>();
            duplicationCameraController = FindObjectOfType<DuplicationCameraManager>();

            //initializing the ui
            uiController.InitializeUi(this, UI.eUiState.LoadingScreen, new EventManager.OnShowLoadingScreenEventArgs());

            yield return null;
            //***********************

            //initializing game
            playerController.InitializePlayerController(this);
            CameraController.InitializeCameraController(this);

            //pausing game until world has loaded a bit
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(true));

            yield return null;

            if (worldController != null)
            {
                isOpenWorldLoaded = true;
                isPillarLoaded = false;

                worldController.Initialize(this);
                duplicationCameraController.Initialize(this);
                yield return null;

                var worldObjects = FindObjectsOfType<MonoBehaviour>();
                foreach (var obj in worldObjects)
                {
                    if (obj is IWorldObject)
                    {
                        (obj as IWorldObject).Initialize(this);
                    }
                }
                yield return null;

                worldController.Activate(PlayerController.CharController.MyTransform.position);
                duplicationCameraController.Activate();
                yield return null;

                while (worldController.CurrentState == eWorldControllerState.Activating)
                {
                    yield return null;
                }
            }
            else //this is a Pillar
            {
                isOpenWorldLoaded = false;
                isPillarLoaded = true;

                var worldObjects = FindObjectsOfType<MonoBehaviour>();
                foreach(var obj in worldObjects)
                {
                    if (obj is IWorldObject)
                    {
                        (obj as IWorldObject).Initialize(this);
                    }
                }
                yield return null;
            }

            echoManager.Initialize(this);
            EclipseManager.InitializeEclipseManager(this);
            SpawnPointManager = FindObjectOfType<SpawnPointManager>();

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

        protected static T SearchForScriptInScene<T>(Scene scene) where T : UnityEngine.Object
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