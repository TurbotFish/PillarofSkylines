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
        private bool IsPlayerInside;

        //########################################################################

        // -- INITIALIZATION

        public override void Initialize(GameController game_controller)
        {
            if (IsInitialized)
            {
                return;
            }

            base.Initialize(game_controller);

            OnEntranceEnabled();
        }

        private void OnEnable()
        {
            if (IsInitialized)
            {
                OnEntranceEnabled();
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
            if (!PersistentData.IsPillarUnlocked)
            {
                return;
            }

            IsPlayerInside = true;

            if (!PersistentData.IsDoorUnlocked)
            {
                GameController.UiController.Hud.ShowHelpMessage("[X]: Open Pillar", "PillarEntrance");
            }
            else if (PersistentData.IsDoorUnlocked && !IsUnlockingDoor)
            {
                GameController.UiController.Hud.ShowHelpMessage("[X]: Enter Pillar", "PillarEntrance");
            }
        }

        public void OnHoverEnd()
        {
            if (!PersistentData.IsPillarUnlocked)
            {
                return;
            }

            IsPlayerInside = false;
            GameController.UiController.Hud.HideHelpMessage("PillarEntrance");
        }

        public void OnInteraction()
        {
            if (!PersistentData.IsPillarUnlocked)
            {
                return;
            }

            if (!PersistentData.IsDoorUnlocked)
            {
                OnDoorUnlocked();
            }
            else if (PersistentData.IsDoorUnlocked && !IsUnlockingDoor)
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
                if (args.PillarState == PillarState.Unlocked)
                {
                    OnPillarUnlocked();
                }
            }
        }

        /// <summary>
        /// Called when the pillar is unlocked.
        /// </summary>
        private void OnPillarUnlocked()
        {
            PersistentData.IsPillarUnlocked = true;

            if (AnimatorComponent)
            {
                AnimatorComponent.SetBool("BoolA", true);
            }
        }

        /// <summary>
        /// Called when the door is unlocked.
        /// </summary>
        private void OnDoorUnlocked()
        {
            PersistentData.IsDoorUnlocked = true;

            if (AnimatorComponent)
            {
                AnimatorComponent.SetBool("BoolB", true);
            }

            StartCoroutine(UnlockDoorCoroutine());
        }

        /// <summary>
        /// Coroutine to handle the opening of the door.
        /// </summary>
        /// <returns></returns>
        private IEnumerator UnlockDoorCoroutine()
        {
            IsUnlockingDoor = true;

            GameController.UiController.Hud.HideHelpMessage("PillarEntrance");

            yield return new WaitForSeconds(OpenDoorTime);

            if (IsPlayerInside)
            {
                OnHoverBegin();
            }

            IsUnlockingDoor = false;
        }

        /// <summary>
        /// called when the entrance is initialized for the first time or when the gameobject is reenabled.
        /// </summary>
        private void OnEntranceEnabled()
        {
            if (GameController.PlayerModel.GetPillarState(pillarId) == PillarState.Unlocked || PersistentData.IsPillarUnlocked)
            {
                //Debug.LogErrorFormat("PillarEntrance {0}: OnEntranceEnabled: pillar unlocked!", this.name);
                OnPillarUnlocked();
            }

            if (PersistentData.IsDoorUnlocked)
            {
                //Debug.LogErrorFormat("PillarEntrance {0}: OnEntranceEnabled: door unlocked!", this.name);
                OnDoorUnlocked();
            }
        }
    }
} //end of namespace