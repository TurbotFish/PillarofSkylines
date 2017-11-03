using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.GameControl
{
    public struct OpenWorldSceneInfo
    {
        public Scene Scene { get; set; }
        public World.ChunkSystem.WorldController WorldController { get; set; }
        public World.SpawnPointSystem.SpawnPointManager SpawnPointManager { get; set; }
    }
}