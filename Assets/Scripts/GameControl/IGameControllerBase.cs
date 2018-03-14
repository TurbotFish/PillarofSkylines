﻿using Game.Model;
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

        /// <summary>
        /// Is an open world scene currently loaded?
        /// </summary>
        bool IsOpenWorldLoaded { get; }
        /// <summary>
        /// The controller of the open world scene. Is null when the open world is not loaded!
        /// </summary>
        WorldController WorldController { get; }

        /// <summary>
        /// Is a pillar scene currently loaded?
        /// </summary>
        bool IsPillarLoaded { get; }
        /// <summary>
        /// The id of the pillar scene.
        /// </summary>
        ePillarId ActivePillarId { get; }

        /// <summary>
        /// The spawn point manager of the currently loaded scene. Is null if no scene is loaded!
        /// </summary>
        SpawnPointManager SpawnPointManager { get; }

        //temp
        void StartGame();
    }
}