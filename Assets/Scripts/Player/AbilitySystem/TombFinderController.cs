using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.AbilitySystem
{
    public class TombFinderController : MonoBehaviour
    {
        PlayerModel model;

        bool isOpenWorldActive = false;

        //################################################################

        public void InitializeTombFinderController(GameControl.IGameControllerBase gameController)
        {
            this.model = gameController.PlayerModel;

            Utilities.EventManager.OnSceneChangedEvent += OnSceneChangedEventHandler;
            Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;
        }

        //################################################################
        //################################################################

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        //################################################################
        //################################################################

        void OnSceneChangedEventHandler(object sender, Utilities.EventManager.OnSceneChangedEventArgs args)
        {
            if (args.HasChangedToPillar)
            {
                this.isOpenWorldActive = false;
            }
            else
            {
                this.isOpenWorldActive = true;
            }
        }

        void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.FavourPickedUpEventArgs args)
        {
        }

        //################################################################
    }
} //end of namespace