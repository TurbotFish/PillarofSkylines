using UnityEngine;

namespace Game.LevelElements
{
    public class DestroyWhenPickingFavour : MonoBehaviour
    {
        public string favourID;

        private void Awake()
        {
            Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;
        }

        void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.FavourPickedUpEventArgs args)
        {
            if (args.FavourId == favourID)
            {
                Destroy(gameObject);
            }
        }
    }
} //end of namespace