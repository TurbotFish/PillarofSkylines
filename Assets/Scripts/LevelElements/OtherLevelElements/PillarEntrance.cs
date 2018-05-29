using Game.GameControl;
using Game.Model;
using Game.Utilities;
using Game.World;
using UnityEngine;

namespace Game.LevelElements
{
    /// <summary>
    /// Used to tag the interaction collider that allows the player to enter a pillar from the open world.
    /// </summary>
    public class PillarEntrance : MonoBehaviour, IInteractable, IWorldObject
    {
        //########################################################################

        [SerializeField] private PillarId pillarId;

        private GameController gameController;
        private bool isInitialized;
        private bool isPillarDestroyed;

        //########################################################################

        #region initialization

        public void Initialize(GameController gameController)
        {
            if (isInitialized)
            {
                return;
            }

            this.gameController = gameController;

            isPillarDestroyed = gameController.PlayerModel.GetPillarState(pillarId) == PillarState.Destroyed;
            isInitialized = true;
        }

        private void OnEnable()
        {
            if (isInitialized)
            {
                if (!isPillarDestroyed && gameController.PlayerModel.GetPillarState(pillarId) == PillarState.Destroyed)
                {
                    isPillarDestroyed = true;
                }

                EventManager.PillarStateChangedEvent += OnPillarStateChanged;
            }
        }

        private void OnDisable()
        {
            EventManager.PillarStateChangedEvent -= OnPillarStateChanged;
        }

        #endregion initialization

        //########################################################################

        #region inquiries

        public PillarId PillarId { get { return pillarId; } }

        public Transform Transform { get { return transform; } }

        public bool IsInteractable()
        {
            return !isPillarDestroyed;
        }

        #endregion inquiries

        //########################################################################

        #region operations

        public void OnHoverBegin()
        {
            var eventArgs = new EventManager.OnShowHudMessageEventArgs(true, "[X]: Enter Pillar");
            EventManager.SendShowHudMessageEvent(this, eventArgs);
        }

        public void OnHoverEnd()
        {
            var eventArgs = new EventManager.OnShowHudMessageEventArgs(false);
            EventManager.SendShowHudMessageEvent(this, eventArgs);
        }

        public void OnInteraction()
        {
            if (gameController.PlayerModel.GetPillarState(pillarId) == PillarState.Unlocked)
            {
                gameController.SwitchToPillar(pillarId);
            }
            else
            {
                EventManager.SendShowMenuEvent(this, new EventManager.OnShowPillarEntranceMenuEventArgs(pillarId));
            }
        }

        public void OnPlayerEnter()
        {
        }

        public void OnPlayerExit()
        {
        }

        private void OnPillarStateChanged(object sender, EventManager.PillarStateChangedEventArgs args)
        {
            if (args.PillarId == pillarId)
            {
                isPillarDestroyed = (args.PillarState == PillarState.Destroyed);
            }
        }

        #endregion operations

        //########################################################################
    }
} //end of namespace