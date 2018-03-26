﻿using UnityEngine;

namespace Game.LevelElements
{
    public class DestroyWhenPickingFavour : MonoBehaviour
    {
        public string favourID;

        private void OnEnable()
        {
            Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;
        }

        private void OnDisable()
        {
            Utilities.EventManager.FavourPickedUpEvent -= OnFavourPickedUpEventHandler;
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