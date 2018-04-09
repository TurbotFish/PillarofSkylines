using Game.GameControl;
using Game.World;
using UnityEngine;

namespace Game.LevelElements
{
    public class TombMarker : MonoBehaviour, IWorldObject
    {
        //###########################################################

        [SerializeField] public string favourID; //why public?

        [SerializeField] private GameObject toDisable;
        [SerializeField] private GameObject waypointFeedback;

        IGameControllerBase gameController;
        private bool isInitialized;
        private bool favourPickedUp;

        //###########################################################

        public void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            this.gameController = gameController;

            if (gameController.PlayerModel.CheckIfPickUpCollected(favourID)) //the favour has already been picked up
                OnFavourPickedUp();
            else
            {
                Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;
                Utilities.EventManager.SetWaypointEvent += OnSetWaypointEventHandler;
            }

            isInitialized = true;
        }

        //###########################################################

        private void OnEnable()
        {
            if (!isInitialized || favourPickedUp)
                return;
            else if (gameController.PlayerModel.CheckIfPickUpCollected(favourID)) //the favour has been picked up while the marker was disabled
                OnFavourPickedUp();
            else
            {
                Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;
                Utilities.EventManager.SetWaypointEvent += OnSetWaypointEventHandler;
            }
        }

        private void OnDisable()
        {
            Utilities.EventManager.FavourPickedUpEvent -= OnFavourPickedUpEventHandler;
            Utilities.EventManager.SetWaypointEvent -= OnSetWaypointEventHandler;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
                Utilities.EventManager.SendSetWaypointEvent(this, new Utilities.EventManager.SetWaypointEventArgs(favourID, transform.position));
        }

        //###########################################################

        private void OnSetWaypointEventHandler(object sender, Utilities.EventManager.SetWaypointEventArgs args) {
            ActivateWaypoint(args.WaypointID == favourID);
        }

        private void ActivateWaypoint(bool active) {
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

        private void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.FavourPickedUpEventArgs args)
        {
            if (args.FavourId == favourID)
                OnFavourPickedUp();
        }

        private void OnFavourPickedUp()
        {
            toDisable.SetActive(false);
            favourPickedUp = true;
            Utilities.EventManager.FavourPickedUpEvent -= OnFavourPickedUpEventHandler;
        }

        //###########################################################
    }
} //end of namespace