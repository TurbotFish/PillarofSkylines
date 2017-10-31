using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public struct PillarSceneInfo
    {
        public Scene Scene { get; set; }
        public World.ePillarId PillarId { get; set; }
        public World.SpawnPointSystem.SpawnPointManager SpawnPointManager { get; set; }
    }
}