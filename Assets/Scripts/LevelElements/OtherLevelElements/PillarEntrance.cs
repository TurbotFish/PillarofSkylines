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

        private GameController GameController;
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

            this.GameController = gameController;

            isPillarDestroyed = gameController.PlayerModel.GetPillarState(pillarId) == PillarState.Destroyed;
            isInitialized = true;
        }

        private void OnEnable()
        {
            if (isInitialized)
            {
                if (!isPillarDestroyed && GameController.PlayerModel.GetPillarState(pillarId) == PillarState.Destroyed)
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
            GameController.UiController.Hud.ShowHelpMessage("[X]: Enter Pillar", "PillarEntrance");
        }

        public void OnHoverEnd()
        {
            GameController.UiController.Hud.HideHelpMessage("PillarEntrance");
        }

        public void OnInteraction()
        {
            var pillar_state = GameController.PlayerModel.GetPillarState(pillarId);

            if(pillar_state == PillarState.Unlocked)
            {
                GameController.SwitchToPillar(pillarId);
            }
            else if(pillar_state == PillarState.Locked)
            {
                int current_pillar_marks = GameController.PlayerModel.GetActivePillarMarkCount();
                int needed_pillar_marks = GameController.PlayerModel.GetPillarEntryPrice(pillarId);
                string title = "The Pillar is locked!";
                string description = "You need " + needed_pillar_marks + " Pillar Marks but only have " + current_pillar_marks + ".";

                GameController.UiController.Hud.ShowAnnouncmentMessage(title, description);
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