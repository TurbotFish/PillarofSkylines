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
		List<ParticleSystem> myParticleSystems = new List<ParticleSystem>();

        bool isParticleSystemActive = false;
        bool isInOpenWorld = false;
        bool isFavourInWorld = false;
        bool isInitialized = false;

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
			this.myParticleSystems.Add(GetComponent<ParticleSystem>());
			this.myParticleSystems.Add (GetComponentInChildren<ParticleSystem> ());

			foreach (ParticleSystem ps in this.myParticleSystems) {
				ps.Stop ();
			}
            Utilities.EventManager.OnSceneChangedEvent += OnSceneChangedEventHandler;
            Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;

            this.isInitialized = true;
        }

        //################################################################
        //################################################################

        // Update is called once per frame
        void Update()
        {
            if (!this.isInitialized)
            {
                return;
            }

            bool abilityActive = this.model.CheckAbilityActive(eAbilityType.TombFinder);
            //Debug.LogErrorFormat("TombFinderController: abilityActive={0}, isInOpenWorld={1}, isFavourInWorld={2}", abilityActive, isInOpenWorld, isFavourInWorld);

            if(!this.isParticleSystemActive && (this.isInOpenWorld && this.isFavourInWorld && abilityActive))
            {
				foreach (ParticleSystem ps in this.myParticleSystems) {
					ps.Play ();
				}
                this.isParticleSystemActive = true;
            }
            else if(this.isParticleSystemActive && (!this.isInOpenWorld || !this.isFavourInWorld || !abilityActive))
            {
				foreach (ParticleSystem ps in this.myParticleSystems) {
					ps.Stop ();
				}
                this.isParticleSystemActive = false;
            }

            if (this.isParticleSystemActive)
            {
                OrientToNearestFavour();
            }
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

                if (this.worldController.FindNearestFavour(this.myTransform.position) == null)
                {
                    this.isFavourInWorld = false;
                }
                else
                {
                    this.isFavourInWorld = true;
                }
            }
        }

        void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.FavourPickedUpEventArgs args)
        {
            if(this.worldController.FindNearestFavour(this.myTransform.position) == null)
            {
                this.isFavourInWorld = false;
            }
            else
            {
                this.isFavourInWorld = true;
            }
        }

        //################################################################
        //################################################################

        void OrientToNearestFavour()
        {
            var favour = this.worldController.FindNearestFavour(this.myTransform.position);

            if(favour == null)
            {
                return;
            }

            var relativePos = favour.MyTransform.position - this.myTransform.position;
            Quaternion newRotation = Quaternion.LookRotation(relativePos);
            this.myTransform.rotation = newRotation;
        }

        //################################################################
    }
} //end of namespace