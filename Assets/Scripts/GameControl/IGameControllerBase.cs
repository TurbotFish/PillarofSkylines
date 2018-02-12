using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.GameControl
{
    public interface IGameControllerBase
    {
        Player.PlayerModel PlayerModel { get; }
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