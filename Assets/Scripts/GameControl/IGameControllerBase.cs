using Game.Model;

namespace Game.GameControl
{
    public interface IGameControllerBase
    {
        PlayerModel PlayerModel { get; }
        EchoSystem.EchoManager EchoManager { get; }
        EclipseManager EclipseManager { get; }

        Player.PlayerController PlayerController { get; }
        CameraControl.CameraController CameraController { get; }        
        UI.UiController UiController { get; }

        World.ChunkSystem.WorldController WorldController { get; }

        //temp
        void StartGame();
    }
}