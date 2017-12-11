using System.Collections;
using System.Collections.Generic;
using Game.World.ChunkSystem;
using UnityEngine;

namespace Game.World.Interaction
{
    public class Favour : MonoBehaviour, IWorldObjectInitialization
    {
        //##################################################################

        [SerializeField]
        [HideInInspector]
        int instanceId = -1;
        public int InstanceId { get { return this.instanceId; } }

        [SerializeField]
        [HideInInspector]
        bool instanceIdSet = false;

        public bool FavourPickedUp { get; private set; }
        public Transform MyTransform { get; private set; }
        BoxCollider myCollider;
        
        //##################################################################

        void IWorldObjectInitialization.Initialize(WorldController worldController, bool isCopy)
        {
            if (!instanceIdSet)
            {
                instanceId = GetInstanceID();
                instanceIdSet = true;
            }

            MyTransform = transform;
            myCollider = GetComponent<BoxCollider>();
            
            Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;

            worldController.RegisterFavour(this);
        }

        //##################################################################

        void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.FavourPickedUpEventArgs args) {
            if (args.FavourId == instanceId) {
                if (!FavourPickedUp) {
                    FavourPickedUp = true;
                    
                    if (myCollider != null || !myCollider.Equals(null)) //check because all colliders are removed in the duplicated worlds
                        myCollider.enabled = false;

                    // here do playmaker stuff
                    animator.SetBool("Fav_activated", true);
                    StartCoroutine(FaveurActivation());
                    StartCoroutine(ParticleManager());
                    StartCoroutine(FavSparkUp());

                }
            }
        }

        // FSM: Faveur_activation
        float animSpeed = 0.0005f;
        float startDelay = 6;
        float disparitionEnd = 10;
        float disparitionSpeed = .03f;

        [SerializeField] Animator animator;
        [SerializeField] Renderer recept;
        [SerializeField] GameObject favSparkUp;

        IEnumerator FaveurActivation() {
            float disparition = 0;
            yield return new WaitForSeconds(startDelay);

            while (disparition < disparitionEnd) {
                disparition += disparitionSpeed;
                recept.material.SetFloat("_Emissive_intensity", disparition);
                animator.speed += animSpeed;
                favSparkUp.SetActive(true);
                yield return new WaitForSeconds(0.01f);
            }
        }

        // FSM: ParticleManager
        float delay2 = 2.6f;
        float delay3 = 2.8f;

        [SerializeField] ParticleSystem favSparks;
        [SerializeField] GameObject favSparksOpen;
        [SerializeField] GameObject favSparksBurst;

        IEnumerator ParticleManager() {
            favSparksOpen.SetActive(true);
            yield return new WaitForSeconds(delay2);
            favSparksBurst.SetActive(true);
            favSparks.gameObject.SetActive(true);
            yield return new WaitForSeconds(delay3);

            var em = favSparks.emission;

            while(em.rate.constant > 0f) {
                em.rate = new ParticleSystem.MinMaxCurve(em.rate.constant - 1.4f);
                yield return new WaitForSeconds(0.01f);
            }
            favSparks.gameObject.SetActive(false);

        }

        // FSM: FaveurManager

        [SerializeField] Transform faveur;
        float duration = 1.9f;

        IEnumerator FavSparkUp() {

            Transform player = FindObjectOfType<Player.CharacterController.Character>().transform;
            yield return new WaitForSeconds(6.5f);
            faveur.parent = null;
            
            for(float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime) {
                faveur.position = Vector3.Lerp(faveur.position, player.position, elapsed / duration);
                yield return null;
            }
            Destroy(faveur.gameObject);
            Instantiate(favSparksBurst, faveur.position, Quaternion.identity);
        }


        //##################################################################
    }
} //end of namespace