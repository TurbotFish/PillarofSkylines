using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.GameControl
{
    public struct UiSceneInfo
    {
        public Scene Scene { get; set; }
        public Player.UI.UiController UiController { get; set; }
    }
} //end of namespace