using Game.GameControl;
using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelElements
{
    public class NoRunZone : MonoBehaviour, IInteractable, IWorldObject
    {
        //########################################################################

        private static List<NoRunZone> activeNoRunZones = new List<NoRunZone>();

        private GameController gameController;

        //########################################################################

        #region initialization

        public void Initialize(GameController gameController)
        {
            this.gameController = gameController;
        }

        private void OnDestroy()
        {
            OnPlayerExit();
        }

        #endregion initialization

        //########################################################################

        #region inquiries

        public Transform Transform { get { return transform; } }

        public bool IsInteractable()
        {
            return false;
        }

        #endregion inquiries

        //########################################################################

        #region operations

        public void OnPlayerEnter()
        {
            if (!activeNoRunZones.Contains(this))
            {
                activeNoRunZones.Add(this);

                gameController.PlayerController.CharController.isInsideNoRunZone = true;
            }
        }

        public void OnPlayerExit()
        {
            if (activeNoRunZones.Contains(this))
            {
                activeNoRunZones.Remove(this);

                if(activeNoRunZones.Count == 0)
                {
                    gameController.PlayerController.CharController.isInsideNoRunZone = false;
                }
            }
        }

        public void OnHoverBegin()
        {
        }

        public void OnHoverEnd()
        {
        }

        public void OnInteraction()
        {
        }

        #endregion operations

        //########################################################################
    }
} // end of namespace