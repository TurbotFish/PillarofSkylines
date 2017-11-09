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

        Player.PlayerController PlayerController { get; }
        World.ChunkSystem.WorldController WorldController { get; }
        Player.UI.UiController UiController { get; }
    }
}