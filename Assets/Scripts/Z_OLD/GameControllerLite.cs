//using Game.CameraControl;
//using Game.Model;
//using Game.Utilities;
//using Game.World;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//namespace Game.GameControl
//{
//    public class GameControllerLite : MonoBehaviour, IGameController
//    {
//        public const string UI_SCENE_NAME = "UiScene";

//        //###############################################################

//        [SerializeField]
//        private GameObject uiPrefab;

//        //
//        private PlayerModel playerModel;
//        private EchoSystem.EchoManager echoManager;
//        private EclipseManager eclipseManager;

//        //
//        private UI.UiController uiController;
//        private Player.PlayerController playerController;

//        //
//        private bool isOpenWorldLoaded;
//        private WorldController worldController;
//        private DuplicationCameraManager duplicationCameraController;

//        //
//        private bool isPillarLoaded;

//        private bool IsInitialized;

//        //###############################################################

//        public PlayerModel PlayerModel { get { return playerModel; } }
//        public EchoSystem.EchoManager EchoManager { get { return echoManager; } }
//        public EclipseManager EclipseManager { get { return eclipseManager; } }

//        public UI.UiController UiController { get { return uiController; } }
//        public Player.PlayerController PlayerController { get { return playerController; } }
//        public CameraController CameraController { get; private set; }

//        public bool IsOpenWorldLoaded { get { return isOpenWorldLoaded; } }
//        public WorldController WorldController { get { return worldController; } }
//        public DuplicationCameraManager DuplicationCameraManager { get { return duplicationCameraController; } }

//        public bool IsPillarLoaded { get { return isPillarLoaded; } }
//        public PillarId ActivePillarId { get { throw new NotImplementedException(); } }

//        public SpawnPointManager SpawnPointManager { get; private set; }

//        //###############################################################
//        //###############################################################

//        void Start()
//        {
//            StartCoroutine(LoadScenesRoutine());
//        }

//        /// <summary>
//        /// DO NOT USE!
//        /// </summary>
//        public void StartGame()
//        {
//            throw new System.NotImplementedException();
//        }

//        public void ExitGame()
//        {
//#if UNITY_EDITOR
//            UnityEditor.EditorApplication.isPlaying = false;
//#endif
//        }

//        public void SwitchToPillar(PillarId pillar_id)
//        {
//            Debug.LogError("GameControllerLite: SwitchToPillar: cannot do that!");
//        }

//        public void SwitchToOpenWorld()
//        {
//            Debug.LogError("GameControllerLite: SwitchToOpenWorld: cannot do that!");
//        }

//        //###############################################################
//        //###############################################################

//        private IEnumerator LoadScenesRoutine()
//        {
//            //***********************
//            //loading the UI

//            var uiGO = Instantiate(uiPrefab);
//            uiController = uiGO.GetComponentInChildren<UI.UiController>();

//            yield return null;
//            //***********************

//            // creating model
//            playerModel = new PlayerModel();

//            //getting references in game controller
//            echoManager = GetComponentInChildren<EchoSystem.EchoManager>();
//            eclipseManager = GetComponentInChildren<EclipseManager>();

//            //cleaning up, just in case
//            var echoManagers = FindObjectsOfType<EchoSystem.EchoManager>();
//            foreach (var echoManager in echoManagers)
//            {
//                if (echoManager != this.echoManager)
//                {
//                    Destroy(echoManager.gameObject);
//                }
//            }

//            var eclipseManagers = FindObjectsOfType<EclipseManager>();
//            foreach (var eclipseManager in eclipseManagers)
//            {
//                if (eclipseManager != this.eclipseManager)
//                {
//                    Destroy(eclipseManager.gameObject);
//                }
//            }

//            //getting references in local scene
//            playerController = FindObjectOfType<Player.PlayerController>();
//            CameraController = FindObjectOfType<CameraController>();
//            worldController = FindObjectOfType<WorldController>();
//            duplicationCameraController = FindObjectOfType<DuplicationCameraManager>();

//            //initializing the ui
//            uiController.Initialize(this, UI.MenuType.LoadingScreen, new EventManager.OnShowLoadingScreenEventArgs());

//            yield return null;
//            //***********************

//            //initializing game
//            playerController.InitializePlayerController(this);
//            CameraController.InitializeCameraController(this);

//            //pausing game until world has loaded a bit
//            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(true));

//            yield return null;

//            if (worldController != null)
//            {
//                isOpenWorldLoaded = true;
//                isPillarLoaded = false;

//                worldController.Initialize(this);
//                duplicationCameraController.Initialize(this);
//                yield return null;

//                var worldObjects = FindObjectsOfType<MonoBehaviour>();
//                foreach (var obj in worldObjects)
//                {
//                    if (obj is IWorldObject)
//                    {
//                        (obj as IWorldObject).Initialize(this);
//                    }
//                }
//                yield return null;

//                worldController.Activate(PlayerController.CharController.MyTransform.position);
//                duplicationCameraController.Activate();
//                yield return null;

//                while (worldController.CurrentState == WorldControllerState.Activating)
//                {
//                    yield return null;
//                }
//            }
//            else //this is a Pillar
//            {
//                isOpenWorldLoaded = false;
//                isPillarLoaded = true;

//                var worldObjects = FindObjectsOfType<MonoBehaviour>();
//                foreach (var obj in worldObjects)
//                {
//                    if (obj is IWorldObject)
//                    {
//                        (obj as IWorldObject).Initialize(this);
//                    }
//                }
//                yield return null;
//            }

//            echoManager.Initialize(this);
//            EclipseManager.InitializeEclipseManager(this);
//            SpawnPointManager = FindObjectOfType<SpawnPointManager>();

//            yield return null;
//            //***********************

//            //starting game
//            EventManager.SendSceneChangedEvent(this, new EventManager.SceneChangedEventArgs());
//            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(false));
//            EventManager.SendShowMenuEvent(this, new EventManager.OnShowMenuEventArgs(UI.MenuType.HUD));

//            IsInitialized = true;
//        }

//        /// <summary>
//        /// Unity's Update methpod.
//        /// </summary>
//        private void Update()
//        {
//            if (!IsInitialized)
//            {
//                return;
//            }

//            if (Input.GetKeyUp(KeyCode.F2))
//            {
//                Debug.Log("CHEATING: One PillarKey appeared out of nowhere!");
//                foreach (var pillar_mark in Enum.GetValues(typeof(PillarMarkId)).Cast<PillarMarkId>())
//                {
//                    PlayerModel.SetPillarMarkState(pillar_mark, PillarMarkState.active);
//                }
//            }
//            else if (Input.GetKeyUp(KeyCode.F5))
//            {
//                Debug.Log("CHEATING: You were supposed to find the Tombs, not make them useless!");

//                foreach (var ability in PlayerModel.AbilityData.GetAllAbilities())
//                {
//                    PlayerModel.SetAbilityState(ability.Type, AbilityState.active);
//                }
//            }

//            UiController.HandleInput();
//            PlayerController.HandleInput();
//            CameraController.HandleInput();
//        }

//        //###############################################################
//        //###############################################################

//        protected static T SearchForScriptInScene<T>(Scene scene) where T : UnityEngine.Object
//        {
//            T result = null;

//            foreach (var gameObject in scene.GetRootGameObjects())
//            {
//                result = gameObject.GetComponentInChildren<T>();

//                if (result != null)
//                {
//                    break;
//                }
//            }

//            return result;
//        }



//        //###############################################################
//    }
//}