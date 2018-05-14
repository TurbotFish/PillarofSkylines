using Game.GameControl;
using Game.Model;
using Game.World;
using UnityEngine;

namespace Game.LevelElements
{
    public class TombMarker : MonoBehaviour, IWorldObject
    {
        //###########################################################

        //[SerializeField] public string favourID; //why public?
        [SerializeField] private Pickup tomb;

        [SerializeField] private GameObject toDisable;
        [SerializeField] private GameObject waypointFeedback;

        [SerializeField, HideInInspector] private string tombID;

        private IGameControllerBase gameController;
        private PickupPersistentData persistentData;
        private bool isInitialized;
        private bool isFavourPickedUp;

        //###########################################################

        public void Initialize(IGameControllerBase gameController)
        {
            this.gameController = gameController;

            persistentData = gameController.PlayerModel.GetPersistentDataObject<PickupPersistentData>(tombID);
            if (persistentData != null && persistentData.IsPickedUp) //the favour has already been picked up
                OnPickupCollected();
            else
            {
                Utilities.EventManager.PickupCollectedEvent += OnPickupCollectedEvent;
                Utilities.EventManager.SetWaypointEvent += OnSetWaypointEventHandler;
            }

            isInitialized = true;
        }

        //###########################################################

        private void OnEnable()
        {
            if (!isInitialized || isFavourPickedUp)
            {
                return;
            }

            if (persistentData == null)
            {
                persistentData = gameController.PlayerModel.GetPersistentDataObject<PickupPersistentData>(tombID);
            }

            if (persistentData.IsPickedUp) //the favour has been picked up while the marker was disabled
            {
                OnPickupCollected();
            }
            else
            {
                Utilities.EventManager.PickupCollectedEvent += OnPickupCollectedEvent;
                Utilities.EventManager.SetWaypointEvent += OnSetWaypointEventHandler;
            }
        }

        private void OnDisable()
        {
            Utilities.EventManager.PickupCollectedEvent -= OnPickupCollectedEvent;
            Utilities.EventManager.SetWaypointEvent -= OnSetWaypointEventHandler;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                Utilities.EventManager.SendSetWaypointEvent(this, new Utilities.EventManager.SetWaypointEventArgs(tombID, transform.position));
            }
        }

        private void OnValidate()
        {
            if (tomb == null)
            {
                tombID = "";
            }
            else
            {
                tombID = tomb.UniqueId;
            }
        }

        //###########################################################

        private void OnSetWaypointEventHandler(object sender, Utilities.EventManager.SetWaypointEventArgs args)
        {
            ActivateWaypoint(args.WaypointID == tombID);
        }

        private void ActivateWaypoint(bool active)
        {
            if (waypointFeedback != null)
            {
                waypointFeedback.SetActive(active);
            }
            else
            {
                Debug.LogErrorFormat("Tombmarker {0}: ActivateWaypoint: waypointFeedback is null!", this.name);
            }
        }

        //###########################################################

        private void OnPickupCollectedEvent(object sender, Utilities.EventManager.PickupCollectedEventArgs args)
        {
            if (args.PickupID == tombID)
            {
                OnPickupCollected();
            }
        }

        private void OnPickupCollected()
        {
            toDisable.SetActive(false);
            isFavourPickedUp = true;
            Utilities.EventManager.PickupCollectedEvent -= OnPickupCollectedEvent;
        }

        //###########################################################
    }
} // end of namespace