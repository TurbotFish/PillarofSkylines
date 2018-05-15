using Game.GameControl;
using Game.Utilities;
using Game.World;
using UnityEngine;

namespace Game.LevelElements
{
    /// <summary>
    /// Used to tag the interaction collider that allows the player to leave a pillar and return to the open world.
    /// </summary>
    public class PillarExit : MonoBehaviour, IInteractable, IWorldObject
    {
        private IGameController gameController;

        //########################################################################

        public void Initialize(IGameController gameController)
        {
            this.gameController = gameController;
        }

        //########################################################################

        public Transform Transform { get { return transform; } }

        public bool IsInteractable()
        {
            return !gameController.PlayerModel.hasNeedle;
        }

        //########################################################################

        public void OnPlayerEnter()
        {
        }

        public void OnPlayerExit()
        {
        }

        public void OnHoverBegin()
        {
            var eventArgs = new EventManager.OnShowHudMessageEventArgs(true, "[X]: Exit Pillar");
            EventManager.SendShowHudMessageEvent(this, eventArgs);
        }

        public void OnHoverEnd()
        {
            var eventArgs = new EventManager.OnShowHudMessageEventArgs(false);
            EventManager.SendShowHudMessageEvent(this, eventArgs);
        }

        public void OnInteraction()
        {
            if (!gameController.PlayerModel.hasNeedle)
            {
                gameController.SwitchToOpenWorld();
            }
        }

        //########################################################################
    }
} //end of namespace