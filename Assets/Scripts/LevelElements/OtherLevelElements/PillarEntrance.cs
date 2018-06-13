using Game.GameControl;
using Game.Model;
using Game.Utilities;
using Game.World;
using System.Collections;
using UnityEngine;

namespace Game.LevelElements
{
    /// <summary>
    /// Used to tag the interaction collider that allows the player to enter a pillar from the open world.
    /// </summary>
    public class PillarEntrance : PersistentLevelElement<PillarEntryPersistentData>, IInteractable
    {
        //########################################################################

        // -- CONSTANTS

        [SerializeField] private PillarId pillarId;
        [SerializeField] private Animator AnimatorComponent;
        [SerializeField] private float OpenDoorTime = 2f;

        //########################################################################

        // -- ATTRIBUTES

        private bool IsUnlockingDoor;

        //########################################################################

        // -- INITIALIZATION

        public override void Initialize(GameController game_controller)
        {
            if (IsInitialized)
            {
                return;
            }

            base.Initialize(game_controller);
        }

        private void OnEnable()
        {
            if (IsInitialized)
            {
                if(GameController.PlayerModel.GetPillarState(pillarId) == PillarState.Unlocked && !PersistentData.IsPillarUnlocked)
                {
                    OnPillarUnlocked();
                }

                if (PersistentData.IsDoorUnlocked)
                {
                    OnDoorUnlocked();
                }
            }

            EventManager.PillarStateChangedEvent += OnPillarStateChanged;
        }

        private void OnDisable()
        {
            EventManager.PillarStateChangedEvent -= OnPillarStateChanged;
        }

        //########################################################################

        // -- INQUIRIES

        public PillarId PillarId { get { return pillarId; } }

        public Transform Transform { get { return transform; } }

        public bool IsInteractable()
        {
            return PersistentData.IsPillarUnlocked;
        }

        //########################################################################

        // -- OPERATIONS

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
            if (!PersistentData.IsDoorUnlocked)
            {
                OnDoorUnlocked();
            }
            else if(PersistentData.IsDoorUnlocked && !IsUnlockingDoor)
            {
                GameController.SwitchToPillar(pillarId);
            }
        }

        public void OnPlayerEnter()
        {
        }

        public void OnPlayerExit()
        {
        }

        /// <summary>
        /// Creates the PersistentData object.
        /// </summary>
        /// <returns></returns>
        protected override PersistentData CreatePersistentDataObject()
        {
            return new PillarEntryPersistentData(UniqueId);
        }

        /// <summary>
        /// Handles the PillarStateChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPillarStateChanged(object sender, EventManager.PillarStateChangedEventArgs args)
        {
            if (args.PillarId == pillarId)
            {
                if(args.PillarState == PillarState.Unlocked)
                {
                    OnPillarUnlocked();
                }
            }
        }

        private void OnPillarUnlocked()
        {
            PersistentData.IsPillarUnlocked = true;
            if (AnimatorComponent)
                AnimatorComponent.SetBool("BoolA", true);
        }

        private void OnDoorUnlocked()
        {
            PersistentData.IsDoorUnlocked = true;
            if (AnimatorComponent)
                AnimatorComponent.SetBool("BoolB", true);
            StartCoroutine(UnlockDoorCoroutine());
        }

        private IEnumerator UnlockDoorCoroutine()
        {
            IsUnlockingDoor = true;

            yield return new WaitForSeconds(OpenDoorTime);

            IsUnlockingDoor = false;
        }
    }
} //end of namespace