using Game.GameControl;
using Game.World;
using UnityEngine;

namespace Game.LevelElements
{
    public class EnableWhenPickingFavour : MonoBehaviour, IWorldObject
    {
        //###########################################################

        #region member variables

        [SerializeField] public string favourID; //why public?
        [SerializeField] private bool disableAtStart = true;
        [SerializeField] public GameObject[] objectsToEnable = new GameObject[0]; //why public?

        private IGameControllerBase gameController;
        private bool isInitialized;
        private bool favourPickedUp;

        #endregion member variables

        //###########################################################

        #region public methods

        public void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            this.gameController = gameController;

            if (gameController.PlayerModel.CheckIfPickUpCollected(favourID)) //the favour has already been picked up
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

                Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;
            }

            isInitialized = true;
        }

        #endregion public methods

        //###########################################################

        #region monobehaviour methods

        private void OnEnable()
        {
            if (!isInitialized || favourPickedUp)
            {
                return;
            }
            else if (gameController.PlayerModel.CheckIfPickUpCollected(favourID)) //the favour has been picked up while this was disabled
            {
                ActivateAllObjects();
                favourPickedUp = true;
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

        #endregion monobehaviour methods

        //###########################################################

        #region private methods

        private void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.FavourPickedUpEventArgs args)
        {
            if (args.FavourId == favourID)
            {
                ActivateAllObjects();
                favourPickedUp = true;
                Utilities.EventManager.FavourPickedUpEvent -= OnFavourPickedUpEventHandler;               
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