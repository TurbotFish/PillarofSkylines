using Game.GameControl;
using Game.Model;
using Game.World;
using UnityEngine;

namespace Game.LevelElements
{
    public class EnableWhenPickingFavour : MonoBehaviour, IWorldObject
    {
        //###########################################################

        #region member variables

        [SerializeField] private Pickup pickup; // This should not be used at runtime because the object is not guaranteed ot exist.

        [SerializeField] private bool disableAtStart = true;
        [SerializeField] public GameObject[] objectsToEnable = new GameObject[0]; //why public?

        [SerializeField, HideInInspector] private string pickupID;

        private GameController gameController;
        private bool isInitialized;
        private bool favourPickedUp;

        #endregion member variables

        //###########################################################

        #region initialization methods

        public void Initialize(GameController gameController)
        {
            this.gameController = gameController;

            var persistentData = gameController.PlayerModel.GetPersistentDataObject<PickupPersistentData>(pickupID);

            if (persistentData != null && persistentData.IsPickedUp) //the favour has already been picked up
            {
                ActivateAllObjects();
                favourPickedUp = true;
            }
            else
            {
                if (disableAtStart)
                {
                    DeactivateAllObjects();
                }

                Utilities.EventManager.PickupCollectedEvent += OnPickpCollectedEventHandler;
            }

            isInitialized = true;
        }

        #endregion initialization methods

        //###########################################################

        #region monobehaviour methods

        private void OnEnable()
        {
            if (!isInitialized || favourPickedUp)
            {
                return;
            }

            var persistentData = gameController.PlayerModel.GetPersistentDataObject<PickupPersistentData>(pickupID);

            if (persistentData != null && persistentData.IsPickedUp) //the favour has been picked up while this was disabled
            {
                ActivateAllObjects();
                favourPickedUp = true;
            }
            else
            {
                Utilities.EventManager.PickupCollectedEvent += OnPickpCollectedEventHandler;
            }
        }

        private void OnDisable()
        {
            Utilities.EventManager.PickupCollectedEvent -= OnPickpCollectedEventHandler;
        }

        private void OnValidate()
        {
            if (pickup == null)
            {
                pickupID = "";
            }
            else
            {
                pickupID = pickup.UniqueId;
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        #region private methods

        private void OnPickpCollectedEventHandler(object sender, Utilities.EventManager.PickupCollectedEventArgs args)
        {
            if (args.PickupID == pickupID)
            {
                ActivateAllObjects();
                favourPickedUp = true;
                Utilities.EventManager.PickupCollectedEvent -= OnPickpCollectedEventHandler;
            }
        }

        private void ActivateAllObjects()
        {
            for (int i = 0; i < objectsToEnable.Length; i++)
            {
                objectsToEnable[i].SetActive(true);
            }
        }

        private void DeactivateAllObjects()
        {
            for (int i = 0; i < objectsToEnable.Length; i++)
            {
                objectsToEnable[i].SetActive(false);
            }
        }

        #endregion private methods

        //###########################################################
    }
} //end of namespace