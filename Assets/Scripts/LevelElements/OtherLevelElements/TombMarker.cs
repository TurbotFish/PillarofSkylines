using Game.EchoSystem;
using Game.GameControl;
using Game.Model;
using UnityEngine;

namespace Game.LevelElements
{
    public class TombMarker : PersistentLevelElement<TombMarkerPersistentData>, IInteractable
    {
        //###########################################################

        // -- ATTRIBUTES

        [SerializeField] private Pickup tomb;
        [SerializeField] private GameObject toDisable;
        [SerializeField] private GameObject waypointFeedback;
        [SerializeField] private GameObject activationFeedback;

        [SerializeField, HideInInspector] private string tombID;

        public Transform Transform { get { return transform; } }

        //###########################################################

        // -- INITIALIZATION

        public override void Initialize(GameController game_controller)
        {
            base.Initialize(game_controller);

            if (tombID == "")
            {
                Debug.LogErrorFormat("TombMarker {0} is not linked to a Tomb and has been destroyed!", this.name);
                Destroy(this.gameObject);
            }

            if (this.isActiveAndEnabled)
            {
                OnEnable();
            }
        }

        /// <summary>
        /// Called when the game object is activated.
        /// </summary>
        private void OnEnable()
        {
            if (!IsInitialized)
            {
                return;
            }

            var tomb_persistent_data = GameController.PlayerModel.GetPersistentDataObject<PickupPersistentData>(tombID);

            if (tomb_persistent_data != null && PersistentData.IsTombCollected)    // The tomb has already been "collected".
            {
                OnPickupCollected();
            }
            else    // The tomb has not been "collected" yet.
            {
                Utilities.EventManager.PickupCollectedEvent += OnPickupCollectedEvent;
            }

            ActivateWaypoint(PersistentData.IsWaypoint);
            PersistentData.SetActiveInstance(this);
        }

        /// <summary>
        /// Called when the game object is deactivated.
        /// </summary>
        private void OnDisable()
        {
            if (PersistentData != null)
            {
                PersistentData.SetActiveInstance(null);
            }

            Utilities.EventManager.PickupCollectedEvent -= OnPickupCollectedEvent;
        }

        //###########################################################

        // -- INQUIRIES

        /// <summary>
        /// Used to check whether the player can interact with this object.
        /// </summary>
        public bool IsInteractable()
        {
            return false;
        }

        //###########################################################

        // -- OPERATIONS

        /// <summary>
        /// Called when the player enters the trigger collider.
        /// </summary>
        public void OnPlayerEnter()
        {
            ActivateWaypoint(true);
        }

        /// <summary>
        /// Called when the player leaves the trigger collider.
        /// </summary>
        public void OnPlayerExit()
        {
        }

        /// <summary>
        /// Called when this object becomes the nearest interactable object to the player.
        /// </summary>
        public void OnHoverBegin()
        {
        }

        /// <summary>
        /// Called when this object is not the nearest interactable object to the player anymore.
        /// </summary>
        public void OnHoverEnd()
        {
        }

        /// <summary>
        /// Called when the player interacts with this object.
        /// </summary>
        public void OnInteraction()
        {
        }

        /// <summary>
        /// Used to activate or deactivate the waypoint effect.
        /// </summary>
        /// <param name="active"></param>
        public void ActivateWaypoint(bool active)
        {
            PersistentData.IsWaypoint = active;

            if (active)
            {
                GameController.EchoManager.SetWaypoint(PersistentData);
                activationFeedback.SetActive(true);
            }
            else
            {
                GameController.EchoManager.RemoveWaypoint(UniqueId);
                activationFeedback.SetActive(false);
            }

            if (waypointFeedback != null)
            {
                waypointFeedback.SetActive(active);
            }
            else
            {
                Debug.LogErrorFormat("Tombmarker {0}: ActivateWaypoint: waypointFeedback is null!", this.name);
            }
        }

        /// <summary>
        /// Creates the persitent data object.
        /// </summary>
        /// <returns></returns>
        protected override PersistentData CreatePersistentDataObject()
        {
            return new TombMarkerPersistentData(UniqueId, Transform.position);
        }

        /// <summary>
        /// Called by the editor.
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();

            if (tomb == null)
            {
                tombID = "";
            }
            else
            {
                tombID = tomb.UniqueId;
            }
        }

        /// <summary>
        /// Handles the PickupCollectedEvent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPickupCollectedEvent(object sender, Utilities.EventManager.PickupCollectedEventArgs args)
        {
            if (args.PickupID == tombID)
            {
                OnPickupCollected();
            }
        }

        /// <summary>
        /// Used when the linked Tomb (Pickup) has been "collected".
        /// </summary>
        private void OnPickupCollected()
        {
            toDisable.SetActive(false);
            PersistentData.IsTombCollected = true;
            Utilities.EventManager.PickupCollectedEvent -= OnPickupCollectedEvent;
        }

        //###########################################################
    }
} // end of namespace