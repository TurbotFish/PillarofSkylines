using Game.GameControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelElements
{
    public class AirParticle : MonoBehaviour
    {
        private IGameControllerBase gameController;
        private string originUniqueId;

        public void Initialize(IGameControllerBase gameController, string originUniqueId)
        {
            this.gameController = gameController;
            this.originUniqueId = originUniqueId;
        }

        private void Update()
        {
            
        }
    }
} // end of namespace