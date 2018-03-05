using Game.Model;
using Game.World;

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

        WorldController WorldController { get; }

        //temp
        void StartGame();
    }
}