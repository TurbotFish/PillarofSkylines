using Game.GameControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelElements
{
    public class AirParticle : MonoBehaviour
    {
        private GameController gameController;
        private string originUniqueId;

        public void Initialize(GameController gameController, string originUniqueId)
        {
            this.gameController = gameController;
            this.originUniqueId = originUniqueId;
        }

        private void Update()
        {
            
        }
    }
} // end of namespace