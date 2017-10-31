using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public interface IGameControllerBase
    {
        Player.PlayerModel PlayerModel { get; }
    }
}