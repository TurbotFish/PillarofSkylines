using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameControl
{
    public class MainSceneWorld : MonoBehaviour
    {
        public void Initialize(GameController game_controller)
        {
            foreach(var world_object in this.transform.GetComponentsInChildren<IWorldObject>())
            {
                world_object.Initialize(game_controller);
            }
        }
    }
}