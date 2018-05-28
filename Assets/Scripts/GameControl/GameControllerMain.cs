using Game.CameraControl;
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
    public class GameControllerMain : MonoBehaviour, IGameController
    {
        //###############################################################

        // -- ATTRIBUTES     

        public PlayerModel PlayerModel { get; private set; }
        public EchoManager EchoManager { get; private set; }
        public EclipseManager EclipseManager { get; private set; }

        public PlayerController PlayerController { get; private set; }
        public CameraController CameraController { get; private set; }
        public UiController UiController { get; private set; }

        public bool IsOpenWorldLoaded { get; private set; }
        public WorldController WorldController { get; private set; }
        public DuplicationCameraManager DuplicationCameraManager { get; private set; }

        public bool IsPillarLoaded { get; private set; }
        public PillarId ActivePillarId { get; private set; }

        public SpawnPointManager SpawnPointManager { get; private set; }

        private bool isInitialized = false;
        private bool isGameStarted = false;

        private bool WasPillarDestroyed = false;
        private string PillarDestructionMessageTitle = "";
        private string PillarDestructionMessageDescription = "";

        //###############################################################

        // -- INITIALIZATION

        private void Start()
        {
            // creating model
            PlayerModel = new PlayerModel();

            //getting references in game controller
            EchoManager = GetComponentInChildren<EchoManager>();
            EclipseManager = GetComponentInChildren<EclipseManager>();

            //gatting references in main scene
            PlayerController = FindObjectOfType<PlayerController>();
            CameraController = FindObjectOfType<CameraController>();
            UiController = FindObjectOfType<UiController>();

            //initializing
            UiController.Initialize(this, MenuType.MainMenu, new EventManager.OnShowMenuEventArgs(MenuType.MainMenu));

            PlayerController.InitializePlayerController(this);
            CameraController.InitializeCameraController(this);

            EchoManager.Initialize(this);
            EclipseManager.InitializeEclipseManager(this);

            //
            SceneManager.sceneLoaded += OnSceneLoadedEventHandler;
            EventManager.PillarMarkStateChangedEvent += OnPillarMarkStateChanged;
            EventManager.PillarStateChangedEvent += OnPillarStateChanged;

            //load open world scene
            //StartCoroutine(LoadOpenWorldSceneCR());

            isInitialized = true;
        }

        private void OnDestroy()
        {
            EventManager.PillarMarkStateChangedEvent -= OnPillarMarkStateChanged;
        }

        private IEnumerator LoadOpenWorldSceneCR()
        {
            AsyncOperation async;
            SceneManager.sceneLoaded += OnSceneLoadedEventHandler;

            string sceneName = LevelData.OpenWorldSceneName;

            async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            async.allowSceneActivation = false;

            while (!async.isDone)
            {
                if (async.progress >= 0.9f)
                {
                    async.allowSceneActivation = true;
                }
                yield return null;
            }

            Scene scene = SceneManager.GetSceneByName(sceneName);

            WorldController = SearchForScriptInScene<WorldController>(scene);
            DuplicationCameraManager = SearchForScriptInScene<DuplicationCameraManager>(scene);
            yield return null;

            WorldController.Initialize(this);
            DuplicationCameraManager.Initialize(this);
            yield return null;

            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;
            isInitialized = true;

            EventManager.SendShowMenuEvent(this, new EventManager.OnShowMenuEventArgs(MenuType.MainMenu));
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
            //Debug.Log("GameControllerMain: StartGame");
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

            //activate open world
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
            if (IsPillarLoaded)
            {
                Debug.LogError("A Pillar is already active!");
                return;
            }

            StartCoroutine(SwitchToPillarCoroutine(pillar_id));
        }

        /// <summary>
        /// 
        /// </summary>
        public void SwitchToOpenWorld()
        {
            if (IsOpenWorldLoaded)
            {
                Debug.LogError("GameControllerMain: SwitchToOpenWorld: Open World already loaded!");
                return;
            }

            StartCoroutine(SwitchToOpenWorldCoroutine());
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
             * Input handling
             */
            UiController.HandleInput();
        }

        /// <summary>
        /// Coroutine for switching to the open world scene.
        /// </summary>
        /// <param name="useInitialSpawnPoint"></param>
        /// <returns></returns>
        private IEnumerator __SwitchToOpenWorldCoroutine(bool useInitialSpawnPoint)
        {
            bool show_ability_message = false;
            string message = "";
            string description = "";

            /*
             * initializing
             */
            AsyncOperation async;
            SceneManager.sceneLoaded += OnSceneLoadedEventHandler;

            if (IsPillarLoaded && PlayerModel.GetPillarState(ActivePillarId) == PillarState.Destroyed)
            {
                show_ability_message = true;
                var ability = PlayerModel.AbilityData.GetAbility(PlayerModel.LevelData.GetPillarRewardAbility(ActivePillarId));
                message = "You've been granted the " + ability.Name + " Ability";
                description = ability.Description;
            }

            /*
             * pausing game
             */
            EventManager.SendPreSceneChangeEvent(this, new EventManager.PreSceneChangeEventArgs(true));
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(true));
            EventManager.SendShowMenuEvent(this, new EventManager.OnShowLoadingScreenEventArgs());
            yield return null;

            /*
             * unloading pillar scene
             */
            string pillarSceneName = LevelData.GetPillarSceneName(ActivePillarId);

            if (IsPillarLoaded && !string.IsNullOrEmpty(pillarSceneName))
            {
                async = SceneManager.UnloadSceneAsync(pillarSceneName);

                while (!async.isDone)
                {
                    yield return null;
                }

                yield return null;
            }

            IsPillarLoaded = false;
            SpawnPointManager = null;

            /*
             * "loading" open world scene
             */
            string worldSceneName = LevelData.OpenWorldSceneName;
            Scene scene = SceneManager.GetSceneByName(worldSceneName);

            foreach (var obj in scene.GetRootGameObjects())
            {
                obj.SetActive(true);
            }

            SceneManager.SetActiveScene(scene);
            IsOpenWorldLoaded = true;

            WorldController = SearchForScriptInScene<WorldController>(scene);
            DuplicationCameraManager = SearchForScriptInScene<DuplicationCameraManager>(scene);
            SpawnPointManager = SearchForScriptInScene<SpawnPointManager>(scene);

            yield return null;

            /*
             * preparing player spawn
             */
            Vector3 spawn_position;
            Quaternion spawn_rotation;

            if (useInitialSpawnPoint)
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

            /*
             * activating world
             */
            WorldController.Activate(spawn_position);
            DuplicationCameraManager.Activate();

            while (WorldController.CurrentState == WorldControllerState.Activating)
            {
                yield return null;
            }

            /*
             * teleporting player to spawn
             */
            var teleportPlayerEventArgs = new EventManager.TeleportPlayerEventArgs(spawn_position, spawn_rotation, true);
            EventManager.SendTeleportPlayerEvent(this, teleportPlayerEventArgs);
            yield return null;

            /*
             * unpausing game
             */
            EventManager.SendSceneChangedEvent(this, new EventManager.SceneChangedEventArgs());
            yield return new WaitForSeconds(0.5f);

            EventManager.SendShowMenuEvent(this, new EventManager.OnShowMenuEventArgs(MenuType.HUD));
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(false));

            /*
             * show ability gained message
             */
            if (show_ability_message)
            {
                var HUDMessageEventArgs = new EventManager.OnShowHudMessageEventArgs(true, message, eMessageType.Announcement, description, 6);
                EventManager.SendShowHudMessageEvent(this, HUDMessageEventArgs);
            }

            /*
             * cleaning up
             */
            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;
        }

        /// <summary>
        /// Coroutine for switching to a pillar scene.
        /// </summary>
        /// <param name="pillarId"></param>
        /// <returns></returns>
        private IEnumerator __SwitchToPillarCoroutine(PillarId pillarId)
        {
            AsyncOperation async;
            SceneManager.sceneLoaded += OnSceneLoadedEventHandler;

            //*****************************************
            //pausing game
            EventManager.SendPreSceneChangeEvent(this, new EventManager.PreSceneChangeEventArgs(false));
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(true));
            EventManager.SendShowMenuEvent(this, new EventManager.OnShowLoadingScreenEventArgs(pillarId));
            yield return null;

            //*****************************************
            //deactivating open world scene
            string worldSceneName = LevelData.OpenWorldSceneName;
            Scene scene = SceneManager.GetSceneByName(worldSceneName);

            WorldController.Deactivate();
            DuplicationCameraManager.Deactivate();

            while (WorldController.CurrentState == WorldControllerState.Deactivating)
            {
                yield return null;
            }

            IsOpenWorldLoaded = false;
            WorldController = null;
            DuplicationCameraManager = null;
            SpawnPointManager = null;

            foreach (var obj in scene.GetRootGameObjects())
            {
                obj.SetActive(false);
            }

            //*****************************************
            //loading pillar scene
            string pillarSceneName = LevelData.GetPillarSceneName(pillarId);

            async = SceneManager.LoadSceneAsync(pillarSceneName, LoadSceneMode.Additive);
            async.allowSceneActivation = false;

            while (!async.isDone)
            {
                if (async.progress >= 0.9f)
                {
                    async.allowSceneActivation = true;
                }

                yield return null;
            }

            scene = SceneManager.GetSceneByName(pillarSceneName);
            SceneManager.SetActiveScene(scene);

            IsPillarLoaded = true;
            ActivePillarId = pillarId;
            SpawnPointManager = SearchForScriptInScene<SpawnPointManager>(scene);
            yield return null;

            //*****************************************
            //initializing world objects in pillar
            var worldObjects = SearchForScriptsInScene<IWorldObject>(scene);

            foreach (var worldObject in worldObjects)
            {
                worldObject.Initialize(this);
            }
            yield return null;

            //*****************************************
            //teleporting player
            var teleportPlayerEventArgs = new EventManager.TeleportPlayerEventArgs(SpawnPointManager.GetInitialSpawnPoint(), SpawnPointManager.GetInitialSpawnOrientation(), true);
            EventManager.SendTeleportPlayerEvent(this, teleportPlayerEventArgs);

            //*****************************************
            //informing everyone!
            EventManager.SendSceneChangedEvent(this, new EventManager.SceneChangedEventArgs(pillarId));
            yield return new WaitForSeconds(0.5f);

            EventManager.SendShowMenuEvent(this, new EventManager.OnShowMenuEventArgs(MenuType.HUD));
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(false));

            //*****************************************
            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator SwitchToOpenWorldCoroutine()
        {
            Vector3 spawn_position = Vector3.zero;
            Quaternion spawn_rotation;
            bool use_initial_spawn_point;

            /*
             * Pausing game
             */
            EventManager.SendPreSceneChangeEvent(this, new EventManager.PreSceneChangeEventArgs(false));
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(true));
            EventManager.SendShowMenuEvent(this, new EventManager.OnShowLoadingScreenEventArgs());
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
            var teleportPlayerEventArgs = new EventManager.TeleportPlayerEventArgs(spawn_position, spawn_rotation, true);
            EventManager.SendTeleportPlayerEvent(this, teleportPlayerEventArgs);

            /*
             * Unpausing game
             */
            EventManager.SendSceneChangedEvent(this, new EventManager.SceneChangedEventArgs());
            yield return new WaitForSeconds(0.5f);
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(false));

            UiController.SwitchState(MenuType.HUD, null);

            /*
             * show ability gained message
             */
            if (WasPillarDestroyed)
            {
                var HUDMessageEventArgs = new EventManager.OnShowHudMessageEventArgs(true, PillarDestructionMessageTitle, eMessageType.Announcement, PillarDestructionMessageDescription, 6);
                EventManager.SendShowHudMessageEvent(this, HUDMessageEventArgs);

                WasPillarDestroyed = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pillar_id"></param>
        private IEnumerator SwitchToPillarCoroutine(PillarId pillar_id)
        {
            // TODO: init?

            /*
             * Pausing game
             */
            EventManager.SendPreSceneChangeEvent(this, new EventManager.PreSceneChangeEventArgs(false));
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(true));
            EventManager.SendShowMenuEvent(this, new EventManager.OnShowLoadingScreenEventArgs(pillar_id));
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
            var teleportPlayerEventArgs = new EventManager.TeleportPlayerEventArgs(SpawnPointManager.GetInitialSpawnPoint(), SpawnPointManager.GetInitialSpawnOrientation(), true);
            EventManager.SendTeleportPlayerEvent(this, teleportPlayerEventArgs);

            /*
             * Unpausing game
             */
            EventManager.SendSceneChangedEvent(this, new EventManager.SceneChangedEventArgs(pillar_id));
            yield return new WaitForSeconds(0.5f);
            EventManager.SendGamePausedEvent(this, new EventManager.GamePausedEventArgs(false));

            UiController.SwitchState(MenuType.HUD, null);
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
                PillarDestructionMessageTitle = "You've been granted the " + ability.Name + " Ability";
                PillarDestructionMessageDescription = ability.Description;
            }
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
            var gameControllerLiteInstances = SearchForScriptsInScene<GameControllerLite>(scene);
            //Debug.LogFormat("Scene {0} contained {1} GameControllerLite!", scene.name, gameControllerLiteInstances.Count());
            foreach (var instance in gameControllerLiteInstances)
            {
                Destroy(instance.gameObject);
            }

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