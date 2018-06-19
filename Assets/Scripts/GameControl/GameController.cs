using Game.CameraControl;
using Game.Cutscene;
using Game.EchoSystem;
using Game.Model;
using Game.Player;
using Game.UI;
using Game.Utilities;
using Game.World;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.GameControl
{
    public class GameController : MonoBehaviour
    {
        //###############################################################

        // -- CONSTANTS

        [Header("General")]
        [SerializeField] private bool PlayIntroCutscene = false;

        [Header("Lite")]
        [SerializeField] private bool IsLiteVersion = false;
        [SerializeField] private GameObject UiPrefab;

        //###############################################################

        // -- ATTRIBUTES

        // permanent refs
        public PlayerModel PlayerModel { get; private set; }
        public CutsceneManager CutsceneManager { get; private set; }
        public EchoManager EchoManager { get; private set; }
        public EclipseManager EclipseManager { get; private set; }

        public PlayerController PlayerController { get; private set; }
        public CameraController CameraController { get; private set; }
        public UiController UiController { get; private set; }

        // world
        public bool IsSwitchingToOpenWorld { get; private set; }
        public bool IsSwitchingToPillar { get; private set; }

        public bool IsOpenWorldLoaded { get; private set; }
        public WorldController WorldController { get; private set; }
        public DuplicationCameraManager DuplicationCameraManager { get; private set; }

        public bool IsPillarLoaded { get; private set; }
        public PillarId ActivePillarId { get; private set; }

        public SpawnPointManager SpawnPointManager { get; private set; }

        // local state
        public GameState CurrentGameState { get; private set; }
        public bool IsGamePaused { get; private set; }

        private bool isInitialized = false;
        private bool isGameStarted = false;
        private bool IsChangingGameState;

        //
        private bool WasPillarDestroyed = false;
        private string PillarDestructionMessageTitle = "";
        private string PillarDestructionMessageDescription = "";
        private Sprite PillarDestructionMessageIcon;

        //###############################################################

        // -- INITIALIZATION

        /// <summary>
        /// Monobehaviour Start
        /// </summary>
        private void Start()
        {
            LocalInitialization();

            if (IsLiteVersion)
            {
                StartCoroutine(LiteInitCoroutine());
            }
            else
            {
                MainInit();
            }
        }

        private void LocalInitialization()
        {
            IsGamePaused = true;

            PlayerModel = new PlayerModel();
            CutsceneManager = new CutsceneManager(this);

            /*
             * getting references
             */
            if (IsLiteVersion)
            {
                var uiGO = Instantiate(UiPrefab);
                UiController = uiGO.GetComponentInChildren<UiController>();
            }
            else
            {
                UiController = FindObjectOfType<UiController>();
            }

            EchoManager = GetComponentInChildren<EchoManager>();
            EclipseManager = GetComponentInChildren<EclipseManager>();

            PlayerController = FindObjectOfType<PlayerController>();
            CameraController = FindObjectOfType<CameraController>();

            /*
             * initializing
             */
            UiController.Initialize(this);

            PlayerController.InitializePlayerController(this);
            CameraController.InitializeCameraController(this);

            EchoManager.Initialize(this);
            EclipseManager.Initialize(this);

            /*
             * Main Menu World Initialization
             */
            foreach (var root_game_object in this.gameObject.scene.GetRootGameObjects())
            {
                var main_scene_world = root_game_object.GetComponent<MainSceneWorld>();

                if(main_scene_world != null)
                {
                    main_scene_world.Initialize(this);
                    break;
                }
            }
        }

        /// <summary>
        /// Initialization for the Lite version of the GameController.
        /// </summary>
        /// <returns></returns>
        private IEnumerator LiteInitCoroutine()
        {
            WorldController = FindObjectOfType<WorldController>();
            DuplicationCameraManager = FindObjectOfType<DuplicationCameraManager>();
            yield return null;

            SwitchGameState(GameState.Loading, MenuType.LoadingScreen);
            yield return null;

            /*
             * Teleporting Player
             */
            Vector3 original_player_pos = PlayerController.Transform.position;
            var first_teleport_player_event_args = new EventManager.TeleportPlayerEventArgs(
                PlayerController.Transform.position,
                new Vector3(10000, 10000, 10000))
            {
                IsNewScene = true
            };
            yield return null;

            /*
             * Initializing scene
             */
            if (WorldController != null)
            {
                IsOpenWorldLoaded = true;
                IsPillarLoaded = false;

                WorldController.Initialize(this);
                DuplicationCameraManager.Initialize(this);
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

                WorldController.Activate(PlayerController.CharController.MyTransform.position);
                DuplicationCameraManager.Activate();
                yield return null;

                while (WorldController.CurrentState == WorldControllerState.Activating)
                {
                    yield return null;
                }
            }
            else
            {


                IsOpenWorldLoaded = false;
                IsPillarLoaded = true;

                var worldObjects = FindObjectsOfType<MonoBehaviour>();
                foreach (var obj in worldObjects)
                {
                    if (obj is IWorldObject)
                    {
                        (obj as IWorldObject).Initialize(this);
                    }
                }
                yield return null;
            }

            SpawnPointManager = FindObjectOfType<SpawnPointManager>();
            yield return null;

            /*
             * Teleporting Player
             */
            var second_teleport_player_event_args = new EventManager.TeleportPlayerEventArgs(PlayerController.Transform.position, original_player_pos)
            {
                IsNewScene = true
            };
            EventManager.SendTeleportPlayerEvent(this, second_teleport_player_event_args);
            yield return null;

            /*
             * finishing loading
             */
            EventManager.SendSceneChangedEvent(this, new EventManager.SceneChangedEventArgs());

            if (WorldController != null && PlayIntroCutscene)
            {
                CutsceneManager.PlayCutscene(CutsceneType.GameIntro, true);
            }
            else
            {
                SwitchGameState(GameState.Play, MenuType.HUD);
            }

            isGameStarted = true;
            isInitialized = true;
        }

        /// <summary>
        /// Initialization for the Main version of the GameController.
        /// </summary>
        private void MainInit()
        {
            SwitchGameState(GameState.MainMenu, MenuType.MainMenu);

            isInitialized = true;
        }

        /// <summary>
        /// MonoBehaviour OnEnable method.
        /// </summary>
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoadedEventHandler;
            EventManager.PillarMarkStateChangedEvent += OnPillarMarkStateChanged;
            EventManager.PillarStateChangedEvent += OnPillarStateChanged;
        }

        /// <summary>
        /// MonoBehaviour OnDisable method.
        /// </summary>
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;
            EventManager.PillarMarkStateChangedEvent -= OnPillarMarkStateChanged;
            EventManager.PillarStateChangedEvent -= OnPillarStateChanged;
        }

        //###############################################################

        // -- INQUIRIES

        public LevelData LevelData { get { return PlayerModel.LevelData; } }

        //###############################################################

        // -- OPERATIONS

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void StartGame()
        {
            if (isGameStarted)
            {
                Debug.LogError("GameControllerMain: StartGame: game already started!");
                return;
            }
            else if (!isInitialized)
            {
                Debug.LogError("GameControllerMain: StartGame: game not initialized!");
                return;
            }

            isGameStarted = true;

            SwitchToOpenWorld();
        }

        /// <summary>
        /// Closes the Application.
        /// </summary>
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pillar_id"></param>
        public void SwitchToPillar(PillarId pillar_id)
        {
            if (IsLiteVersion)
            {
                Debug.LogError("GameControllerMain: SwitchToPillar: Not available in LiteVersion!");
                return;
            }
            else if (IsPillarLoaded)
            {
                Debug.LogError("GameControllerMain: SwitchToPillar: Pillar already loaded!");
                return;
            }
            else
            {
                StartCoroutine(SwitchToPillarCoroutine(pillar_id));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SwitchToOpenWorld()
        {
            if (IsLiteVersion)
            {
                Debug.LogError("GameControllerMain: SwitchToOpenWorld: Not available in LiteVersion!");
                return;
            }
            else if (IsOpenWorldLoaded)
            {
                Debug.LogError("GameControllerMain: SwitchToOpenWorld: Open World already loaded!");
                return;
            }
            else
            {
                StartCoroutine(SwitchToOpenWorldCoroutine());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OpenPauseMenu()
        {
            if (CurrentGameState == GameState.Play)
            {
                SwitchGameState(GameState.Pause, MenuType.PauseMenu);
            }
            else
            {
                Debug.LogError("GameController: OpenPauseMenu: called but game is not in Play state!");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClosePauseMenu()
        {
            if (CurrentGameState == GameState.Pause)
            {
                SwitchGameState(GameState.Play, MenuType.HUD);
            }
            else
            {
                Debug.LogError("GameController: ClosePauseMenu: called but game is not paused!");
            }
        }

        /// <summary>
        /// Sets the state of the game.
        /// </summary>
        /// <param name="game_state"></param>
        public void SwitchGameState(GameState game_state, MenuType menu_type)
        {
            StartCoroutine(SwitchGameStateCoroutine(game_state, menu_type));
        }

        /// <summary>
        /// Unity's Update methpod.
        /// </summary>
        private void Update()
        {
            if (!isInitialized)
            {
                return;
            }

            /*
             * Cheating
             */
            if (Input.GetKeyUp(KeyCode.F2))
            {
                Debug.Log("CHEATING: One PillarKey appeared out of nowhere!");
                foreach (var pillar_mark in Enum.GetValues(typeof(PillarMarkId)).Cast<PillarMarkId>())
                {
                    PlayerModel.SetPillarMarkState(pillar_mark, PillarMarkState.active);
                }
            }
            else if (Input.GetKeyUp(KeyCode.F5))
            {
                Debug.Log("CHEATING: You were supposed to find the Tombs, not make them useless!");

                foreach (var ability in PlayerModel.AbilityData.GetAllAbilities())
                {
                    PlayerModel.SetAbilityState(ability.Type, AbilityState.active);
                }
            }

            /*
             * Handle Input
             */
            UiController.HandleInput();
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator SwitchToOpenWorldCoroutine()
        {
            IsSwitchingToOpenWorld = true;

            Vector3 spawn_position = Vector3.zero;
            Quaternion spawn_rotation;
            bool use_initial_spawn_point;

            /*
             * Pausing game
             */
            EventManager.SendPreSceneChangeEvent(this, new EventManager.PreSceneChangeEventArgs(false));
            SwitchGameState(GameState.Loading, MenuType.LoadingScreen);
            yield return null;

            /*
             * Unloading Pillar scene
             */
            if (IsPillarLoaded)
            {
                StartCoroutine(UnloadPillarSceneCoroutine());

                while (IsPillarLoaded)
                {
                    yield return null;
                }

                use_initial_spawn_point = false;
            }
            else
            {
                use_initial_spawn_point = true;
            }

            /*
             * Teleporting Player
             */
            var first_teleport_player_event_args = new EventManager.TeleportPlayerEventArgs(
                PlayerController.Transform.position,
                new Vector3(10000, 10000, 10000))
            {
                IsNewScene = true
            };

            EventManager.SendTeleportPlayerEvent(this, first_teleport_player_event_args);

            /*
             * Loading Open World scene
             */
            StartCoroutine(LoadOpenWorldSceneCoroutine());

            while (!IsOpenWorldLoaded)
            {
                yield return null;
            }

            /*
             * Activating Open World scene
             */
            if (use_initial_spawn_point)
            {
                spawn_position = SpawnPointManager.GetInitialSpawnPoint();
                spawn_rotation = SpawnPointManager.GetInitialSpawnOrientation();
            }
            else
            {
                PillarVariant pillar_variant = PillarVariant.Intact;
                if (PlayerModel.GetPillarState(ActivePillarId) == PillarState.Destroyed)
                {
                    pillar_variant = PillarVariant.Destroyed;
                }
                spawn_position = SpawnPointManager.GetPillarExitPoint(ActivePillarId, pillar_variant);
                spawn_rotation = SpawnPointManager.GetPillarExitOrientation(ActivePillarId, pillar_variant);
            }

            WorldController.Activate(spawn_position);
            DuplicationCameraManager.Activate();

            while (WorldController.CurrentState == WorldControllerState.Activating)
            {
                yield return null;
            }

            /*
             * Teleporting Player
             */
            var second_teleport_player_event_args = new EventManager.TeleportPlayerEventArgs(PlayerController.Transform.position, spawn_position, spawn_rotation)
            {
                IsNewScene = true
            };
            EventManager.SendTeleportPlayerEvent(this, second_teleport_player_event_args);

            /*
             * Unpausing game
             */
            EventManager.SendSceneChangedEvent(this, new EventManager.SceneChangedEventArgs());
            yield return new WaitForSeconds(0.5f);

            if (use_initial_spawn_point && PlayIntroCutscene)
            {
                CutsceneManager.PlayCutscene(CutsceneType.GameIntro, true);
            }
            else
            {
                SwitchGameState(GameState.Play, MenuType.HUD);

                if (WasPillarDestroyed)
                {
                    UiController.Hud.ShowAnnouncmentMessage(PillarDestructionMessageTitle, PillarDestructionMessageDescription, 6, PillarDestructionMessageIcon);

                    WasPillarDestroyed = false;
                }
            }

            IsSwitchingToOpenWorld = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pillar_id"></param>
        private IEnumerator SwitchToPillarCoroutine(PillarId pillar_id)
        {
            IsSwitchingToPillar = true;
            ActivePillarId = pillar_id;

            /*
             * Pausing game
             */
            EventManager.SendPreSceneChangeEvent(this, new EventManager.PreSceneChangeEventArgs(false));
            SwitchGameState(GameState.Loading, MenuType.LoadingScreen);
            yield return null;

            /*
             * Unloading Open World scene
             */
            if (IsOpenWorldLoaded)
            {
                StartCoroutine(UnloadOpenWorldSceneCoroutine());

                while (IsOpenWorldLoaded)
                {
                    yield return null;
                }
            }

            /*
             * Teleporting Player
             */
            var first_teleport_player_event_args = new EventManager.TeleportPlayerEventArgs(
                PlayerController.Transform.position,
                new Vector3(10000, 10000, 10000))
            {
                IsNewScene = true
            };

            EventManager.SendTeleportPlayerEvent(this, first_teleport_player_event_args);

            /*
             * Loading Pillar scene
             */
            StartCoroutine(LoadPillarSceneCoroutine(pillar_id));

            while (!IsPillarLoaded)
            {
                yield return null;
            }

            /*
             * Teleporting Player
             */
            var second_teleport_player_event_args = new EventManager.TeleportPlayerEventArgs(
                PlayerController.Transform.position,
                SpawnPointManager.GetInitialSpawnPoint(),
                SpawnPointManager.GetInitialSpawnOrientation())
            {
                IsNewScene = true
            };
            EventManager.SendTeleportPlayerEvent(this, second_teleport_player_event_args);

            /*
             * Unpausing game
             */
            EventManager.SendSceneChangedEvent(this, new EventManager.SceneChangedEventArgs(pillar_id));
            yield return new WaitForSeconds(0.5f);

            SwitchGameState(GameState.Play, MenuType.HUD);
            IsSwitchingToPillar = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator LoadOpenWorldSceneCoroutine()
        {
            var async = SceneManager.LoadSceneAsync(LevelData.OpenWorldSceneName, LoadSceneMode.Additive);
            async.allowSceneActivation = false;

            while (!async.isDone)
            {
                if (async.progress >= 0.9f)
                {
                    async.allowSceneActivation = true;
                }

                yield return null;
            }
            yield return null;

            var scene = SceneManager.GetSceneByName(LevelData.OpenWorldSceneName);
            SceneManager.SetActiveScene(scene);

            SpawnPointManager = SearchForScriptInScene<SpawnPointManager>(scene);
            WorldController = SearchForScriptInScene<WorldController>(scene);
            DuplicationCameraManager = SearchForScriptInScene<DuplicationCameraManager>(scene);

            WorldController.Initialize(this);
            DuplicationCameraManager.Initialize(this);

            IsOpenWorldLoaded = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator UnloadOpenWorldSceneCoroutine()
        {
            if (IsOpenWorldLoaded)
            {
                var async = SceneManager.UnloadSceneAsync(LevelData.OpenWorldSceneName);

                while (!async.isDone)
                {
                    yield return null;
                }

                yield return null;
            }

            IsOpenWorldLoaded = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pillar_id"></param>
        private IEnumerator LoadPillarSceneCoroutine(PillarId pillar_id)
        {
            string pillarSceneName = LevelData.GetPillarSceneName(pillar_id);

            var async = SceneManager.LoadSceneAsync(pillarSceneName, LoadSceneMode.Additive);
            async.allowSceneActivation = false;

            while (!async.isDone)
            {
                if (async.progress >= 0.9f)
                {
                    async.allowSceneActivation = true;
                }

                yield return null;
            }
            yield return null;

            var scene = SceneManager.GetSceneByName(pillarSceneName);
            SceneManager.SetActiveScene(scene);

            ActivePillarId = pillar_id;
            SpawnPointManager = SearchForScriptInScene<SpawnPointManager>(scene);

            var worldObjects = SearchForScriptsInScene<IWorldObject>(scene);

            foreach (var worldObject in worldObjects)
            {
                worldObject.Initialize(this);
            }

            IsPillarLoaded = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator UnloadPillarSceneCoroutine()
        {
            string pillarSceneName = LevelData.GetPillarSceneName(ActivePillarId);

            if (IsPillarLoaded && !string.IsNullOrEmpty(pillarSceneName))
            {
                var async = SceneManager.UnloadSceneAsync(pillarSceneName);

                while (!async.isDone)
                {
                    yield return null;
                }

                yield return null;
            }

            IsPillarLoaded = false;
        }

        /// <summary>
        /// Handles the Unity "sceneLoaded" event.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="loadSceneMode"></param>
        private void OnSceneLoadedEventHandler(Scene scene, LoadSceneMode loadSceneMode)
        {
            //Debug.Log("on scene loaded event");
            CleanScene(scene);
        }

        /// <summary>
        /// Handles the PillarMarkStateChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPillarMarkStateChanged(object sender, EventManager.PillarMarkStateChangedEventArgs args)
        {
            foreach (var pillar_id in Enum.GetValues(typeof(PillarId)).Cast<PillarId>())
            {
                var current_pillar_state = PlayerModel.GetPillarState(pillar_id);

                if (current_pillar_state == PillarState.Destroyed)
                {
                    continue;
                }
                else if (PlayerModel.GetActivePillarMarkCount() >= PlayerModel.GetPillarEntryPrice(pillar_id))
                {
                    PlayerModel.SetPillarState(pillar_id, PillarState.Unlocked);
                }
                else
                {
                    PlayerModel.SetPillarState(pillar_id, PillarState.Locked);
                }
            }
        }

        /// <summary>
        /// Handles the PillarStateChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPillarStateChanged(object sender, EventManager.PillarStateChangedEventArgs args)
        {
            if (args.PillarId == ActivePillarId && args.PillarState == PillarState.Destroyed)
            {
                WasPillarDestroyed = true;
                var ability = PlayerModel.AbilityData.GetAbility(PlayerModel.LevelData.GetPillarRewardAbility(ActivePillarId));
                PillarDestructionMessageTitle = "You've been granted the " + ability.Name;
                PillarDestructionMessageDescription = ability.Description;
                PillarDestructionMessageIcon = ability.Icon;
            }
        }

        /// <summary>
        /// Sets the state of the game with a delay of one Frame.
        /// </summary>
        /// <param name="game_state"></param>
        /// <returns></returns>
        private IEnumerator SwitchGameStateCoroutine(GameState game_state, MenuType menu_type)
        {
            while (IsChangingGameState)
            {
                Debug.LogWarning("GameController: SwitchGameStateCoroutine: already running!");
                yield return true;
            }

            IsChangingGameState = true;
            yield return null;

            CurrentGameState = game_state;

            if (CurrentGameState == GameState.Play)
            {
                IsGamePaused = false;
            }
            else
            {
                IsGamePaused = true;
            }

            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(IsGamePaused));
            yield return null;

            UiController.SwitchState(menu_type);
            IsChangingGameState = false;
        }

        /// <summary>
        /// Returns the first instance of T found in the scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <returns></returns>
        private T SearchForScriptInScene<T>(Scene scene) where T : class
        {
            T result = null;

            foreach (var gameObject in scene.GetRootGameObjects())
            {
                result = gameObject.GetComponentInChildren<T>(true);

                if (result != null)
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns all instances of T in the scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <returns></returns>
        private List<T> SearchForScriptsInScene<T>(Scene scene) where T : class
        {
            var result = new List<T>();

            foreach (var gameObject in scene.GetRootGameObjects())
            {
                result.AddRange(gameObject.GetComponentsInChildren<T>(true));
            }

            return result;
        }

        /// <summary>
        /// Cleans up the Level scene.
        /// </summary>
        /// <param name="scene"></param>
        private void CleanScene(Scene scene)
        {
            if (scene == this.gameObject.scene)
            {
                return;
            }

            var gameControllerMainInstances = SearchForScriptsInScene<GameController>(scene);
            foreach (var instance in gameControllerMainInstances)
            {
                Destroy(instance.gameObject);
            }

            //var gameControllerLiteInstances = SearchForScriptsInScene<GameControllerLite>(scene);
            //Debug.LogFormat("Scene {0} contained {1} GameControllerLite!", scene.name, gameControllerLiteInstances.Count());
            //foreach (var instance in gameControllerLiteInstances)
            //{
            //    Destroy(instance.gameObject);
            //}

            var playerControllerInstances = SearchForScriptsInScene<PlayerController>(scene);
            //Debug.LogFormat("Scene {0} contained {1} PlayerController!", scene.name, playerControllerInstances.Count());
            foreach (var instance in playerControllerInstances)
            {
                Destroy(instance.gameObject);
            }

            var cameraInstances = SearchForScriptsInScene<PoS_Camera>(scene);
            //Debug.LogFormat("Scene {0} contained {1} PoS_Camera!", scene.name, cameraInstances.Count());
            foreach (var instance in cameraInstances)
            {
                Destroy(instance.gameObject);
            }

            //cleaning up old EchoManagers
            var echoManagers = SearchForScriptsInScene<EchoManager>(scene);
            foreach (var echoManager in echoManagers)
            {
                Destroy(echoManager.gameObject);
            }

            var eclipseManagers = SearchForScriptsInScene<EclipseManager>(scene);
            foreach (var eclipseManager in eclipseManagers)
            {
                Destroy(eclipseManager.gameObject);
            }
        }

        //###############################################################
    }
} //end of namespace