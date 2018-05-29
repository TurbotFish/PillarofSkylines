using Game.GameControl;
using Game.Model;
using Game.World;
using UnityEngine;

namespace Game.LevelElements
{
    public class DestroyWhenPickingFavour : MonoBehaviour, IWorldObject
    {
        //###########################################################

        [SerializeField] private Pickup pickup; // This should not be used at runtime because the object is not guaranteed ot exist.

        [SerializeField, HideInInspector] private string pickupID;

        private GameController gameController;
        private bool isInitialized;

        //###########################################################

        public void Initialize(GameController gameController)
        {
            this.gameController = gameController;

            var persistentData = gameController.PlayerModel.GetPersistentDataObject<PickupPersistentData>(pickupID);

            if (persistentData!=null && persistentData.IsPickedUp) //the favour has already been picked up
            {
                Destroy(gameObject);
            }
            else
            {
                Utilities.EventManager.PickupCollectedEvent += OnFavourPickedUpEventHandler;
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

            var persistentData = gameController.PlayerModel.GetPersistentDataObject<PickupPersistentData>(pickupID);

            if (persistentData != null && persistentData.IsPickedUp) //the favour has been picked up while this was disabled
            {
                Destroy(gameObject);
            }
            else
            {
                Utilities.EventManager.PickupCollectedEvent += OnFavourPickedUpEventHandler;
            }
        }

        private void OnDisable()
        {
            Utilities.EventManager.PickupCollectedEvent -= OnFavourPickedUpEventHandler;
        }

        private void OnValidate()
        {
            if(pickup == null)
            {
                pickupID = "";
            }
            else
            {
                pickupID = pickup.UniqueId;
            }
        }

        //###########################################################

        void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.PickupCollectedEventArgs args)
        {
            if (args.PickupID == pickupID)
            {
                Destroy(gameObject);
            }
        }       

        //###########################################################
    }
} //end of namespace