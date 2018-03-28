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

        IGameControllerBase gameController;
        private bool isInitialized;
        private bool favourPickedUp;

        //###########################################################

        public void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            this.gameController = gameController;

            if (gameController.PlayerModel.CheckIfPickUpCollected(favourID)) //the favour has already been picked up
            {
                OnFavourPickedUp();
            }
            else
            {
                Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;
            }

            isInitialized = true;
        }

        //###########################################################

        private void OnEnable()
        {
            if (!isInitialized || favourPickedUp)
            {
                return;
            }
            else if (gameController.PlayerModel.CheckIfPickUpCollected(favourID)) //the favour has been picked up while the marker was disabled
            {
                OnFavourPickedUp();
            }
            else
            {
                Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;
            }
        }

        private void OnDisable()
        {
            Utilities.EventManager.FavourPickedUpEvent -= OnFavourPickedUpEventHandler;
        }

        //###########################################################

        private void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.FavourPickedUpEventArgs args)
        {
            if (args.FavourId == favourID)
            {
                OnFavourPickedUp();
            }
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