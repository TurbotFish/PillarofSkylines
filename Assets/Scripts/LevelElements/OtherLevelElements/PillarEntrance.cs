using Game.GameControl;
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

        [SerializeField] private ePillarId pillarId;

        private IGameController gameController;
        private bool isInitialized;
        private bool isPillarDestroyed;

        //########################################################################

        #region initialization

        public void Initialize(IGameController gameController)
        {
            if (isInitialized)
            {
                return;
            }

            this.gameController = gameController;

            isPillarDestroyed = gameController.PlayerModel.CheckIsPillarDestroyed(pillarId);
            isInitialized = true;
        }

        private void OnEnable()
        {
            EventManager.PillarDestroyedEvent += OnPillarDestroyedEvent;

            if (isInitialized)
            {
                if(!isPillarDestroyed && gameController.PlayerModel.CheckIsPillarDestroyed(pillarId))
                {
                    isPillarDestroyed = true;
                }
            }
        }

        private void OnDisable()
        {
            EventManager.PillarDestroyedEvent -= OnPillarDestroyedEvent;
        }

        #endregion initialization

        //########################################################################

        #region inquiries

        public ePillarId PillarId { get { return pillarId; } }

        public Vector3 Position { get { return transform.position; } }

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
            if (gameController.PlayerModel.CheckIsPillarUnlocked(pillarId))
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

        private void OnPillarDestroyedEvent(object sender, EventManager.PillarDestroyedEventArgs args)
        {
            if (args.PillarId == pillarId)
            {
                isPillarDestroyed = true;
            }
        }

        #endregion operations

        //########################################################################
    }
} //end of namespace