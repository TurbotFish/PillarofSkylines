using Game.GameControl;
using Game.World;
using UnityEngine;

namespace Game.LevelElements
{
    public class DestroyWhenPickingFavour : MonoBehaviour, IWorldObject
    {
        //###########################################################

        [SerializeField] public string favourID; //why public?

        private IGameControllerBase gameController;
        private bool isInitialized;

        //###########################################################

        public void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            this.gameController = gameController;

            if (gameController.PlayerModel.CheckIfPickUpCollected(favourID)) //the favour has already been picked up
            {
                Destroy(gameObject);
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
            if (!isInitialized)
            {
                return;
            }
            else if (gameController.PlayerModel.CheckIfPickUpCollected(favourID)) //the favour has been picked up while this was disabled
            {
                Destroy(gameObject);
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

        void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.FavourPickedUpEventArgs args)
        {
            if (args.FavourId == favourID)
            {
                Destroy(gameObject);
            }
        }       

        //###########################################################
    }
} //end of namespace