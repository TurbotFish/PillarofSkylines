using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.AbilitySystem
{
    public class TombFinderController : MonoBehaviour
    {
        PlayerModel model;
        World.ChunkSystem.WorldController worldController;

        Transform myTransform;
        ParticleSystem myParticleSystem;

        bool isParticleSystemActive = false;
        bool isInOpenWorld = false;
        bool isFavourInWorld = false;

        //################################################################

        public void InitializeTombFinderController(GameControl.IGameControllerBase gameController)
        {
            this.model = gameController.PlayerModel;
            this.worldController = gameController.WorldController;

            if(this.model == null || this.worldController == null)
            {
                Debug.LogError("TombFinderController could not be initialized correctly!");
                this.gameObject.SetActive(false);
                return;
            }

            this.myTransform = this.transform;
            this.myParticleSystem = GetComponent<ParticleSystem>();

            Utilities.EventManager.OnSceneChangedEvent += OnSceneChangedEventHandler;
            Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;
        }

        //################################################################
        //################################################################

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
                this.isInOpenWorld = false;
            }
            else 
            {
                this.isInOpenWorld = true;
            }
        }

        void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.FavourPickedUpEventArgs args)
        {
            if(this.worldController.FindNearestFavour(this.myTransform.position) == null)
            {
                this.isFavourInWorld = false;
            }
        }

        //################################################################
        //################################################################

        void OrientToNearestFavour()
        {

        }

        //################################################################
    }
} //end of namespace