using Game.GameControl;
using Game.Model;
using Game.World;
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

        [SerializeField, HideInInspector] private string tombID;       

        public Vector3 Position { get { return transform.position; } }

        private PickupPersistentData TombPersistentData;

        //###########################################################

        // -- INITIALIZATION

        public override void Initialize(IGameController game_controller)
        {
            base.Initialize(game_controller);

            if(tombID == "")
            {
                Debug.LogErrorFormat("TombMarker {0} is not linked to a Tomb and has been destroyed!", this.name);
                Destroy(this.gameObject);
            }

            if (this.isActiveAndEnabled)
            {
                OnEnable();
            }
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
            Utilities.EventManager.SendSetWaypointEvent(this, new Utilities.EventManager.SetWaypointEventArgs(tombID, transform.position));
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
        /// Creates the persitent data object.
        /// </summary>
        /// <returns></returns>
        protected override PersistentData CreatePersistentDataObject()
        {
            return new TombMarkerPersistentData(UniqueId);
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
        /// Called when the game object is activated.
        /// </summary>
        private void OnEnable()
        {
            if (!IsInitialized)
            {
                return;
            }

            if (TombPersistentData == null)
            {
                TombPersistentData = GameController.PlayerModel.GetPersistentDataObject<PickupPersistentData>(tombID);
            }

            if (TombPersistentData != null && TombPersistentData.IsPickedUp) // The tomb has already been "collected".
            {
                OnPickupCollected();
            }
            else // The tomb has not been "collected" yet.
            {
                Utilities.EventManager.PickupCollectedEvent += OnPickupCollectedEvent;
            }

            ActivateWaypoint(PersistentData.IsWaypoint);
            Utilities.EventManager.SetWaypointEvent += OnSetWaypointEventHandler;
        }

        /// <summary>
        /// Called when the game object is deactivated.
        /// </summary>
        private void OnDisable()
        {
            Utilities.EventManager.PickupCollectedEvent -= OnPickupCollectedEvent;
            Utilities.EventManager.SetWaypointEvent -= OnSetWaypointEventHandler;
        }

        /// <summary>
        /// Handles the SetWaypointEvent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnSetWaypointEventHandler(object sender, Utilities.EventManager.SetWaypointEventArgs args)
        {
            ActivateWaypoint(args.WaypointID == tombID);
        }

        /// <summary>
        /// Used to activate or deactivate the waypoint effect.
        /// </summary>
        /// <param name="active"></param>
        private void ActivateWaypoint(bool active)
        {
            PersistentData.IsWaypoint = active;

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
        /// 
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