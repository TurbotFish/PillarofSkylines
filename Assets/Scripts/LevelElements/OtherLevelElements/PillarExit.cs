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
        private GameController gameController;

        //########################################################################

        public void Initialize(GameController gameController)
        {
            this.gameController = gameController;
        }

        //########################################################################

        public Transform Transform { get { return transform; } }

        public bool IsInteractable()
        {
            return !gameController.PlayerModel.PlayerHasNeedle;
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
            gameController.UiController.Hud.ShowHelpMessage("[X]: Exit Pillar", "PillarExit");
        }

        public void OnHoverEnd()
        {
            gameController.UiController.Hud.HideHelpMessage("PillarExit");
        }

        public void OnInteraction()
        {
            if (!gameController.PlayerModel.PlayerHasNeedle)
            {
                gameController.SwitchToOpenWorld();
            }
        }

        //########################################################################
    }
} //end of namespace