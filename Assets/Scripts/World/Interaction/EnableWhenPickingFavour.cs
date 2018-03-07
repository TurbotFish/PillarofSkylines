using UnityEngine;

namespace Game.World.Interaction
{
    public class EnableWhenPickingFavour : MonoBehaviour
    {
        public string favourID;
        public GameObject[] objectsToEnable;

        private void Start()
        {
            Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;
            for (int i = 0; i < objectsToEnable.Length; i++)
                objectsToEnable[i].SetActive(false);
        }

        void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.FavourPickedUpEventArgs args)
        {
            if (args.FavourId == favourID)
            {
                for (int i = 0; i < objectsToEnable.Length; i++)
                    objectsToEnable[i].SetActive(true);
            }
        }
    }
}