//using Game.CameraControl;
//using Game.Model;
//using Game.World;

//namespace Game.GameControl
//{
//    public interface IGameController
//    {
//        PlayerModel PlayerModel { get; }
//        EchoSystem.EchoManager EchoManager { get; }
//        EclipseManager EclipseManager { get; }

//        Player.PlayerController PlayerController { get; }
//        CameraController CameraController { get; }        
//        UI.UiController UiController { get; }

//        /// <summary>
//        /// Is an open world scene currently loaded?
//        /// </summary>
//        bool IsOpenWorldLoaded { get; }
//        /// <summary>
//        /// The controller of the open world scene. Is null when the open world is not loaded!
//        /// </summary>
//        WorldController WorldController { get; }
//        /// <summary>
//        /// The controller for the cameras that duplicate the world.
//        /// </summary>
//        DuplicationCameraManager DuplicationCameraManager { get; }
//        /// <summary>
//        /// Is a pillar scene currently loaded?
//        /// </summary>
//        bool IsPillarLoaded { get; }
//        /// <summary>
//        /// The id of the pillar scene.
//        /// </summary>
//        PillarId ActivePillarId { get; }
//        /// <summary>
//        /// The spawn point manager of the currently loaded scene. Is null if no scene is loaded!
//        /// </summary>
//        SpawnPointManager SpawnPointManager { get; }

//        //###########################################################

//        void StartGame();

//        void ExitGame();

//        void SwitchToPillar(PillarId pillar_id);

//        void SwitchToOpenWorld();
//    }
//}