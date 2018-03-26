using UnityEngine;

namespace Game.LevelElements
{
    public class TombMarker : MonoBehaviour
    {
        public string favourID;

        [SerializeField] GameObject toDisable;

        Renderer rend;

        private void Start()
        {
            rend = GetComponent<Renderer>();
            Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;
        }

        private void OnDestroy()
        {
            Utilities.EventManager.FavourPickedUpEvent -= OnFavourPickedUpEventHandler;
        }

        void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.FavourPickedUpEventArgs args)
        {
            if (args.FavourId == favourID)
            {
                toDisable.SetActive(false);
            }
        }
    }
} //end of namespace