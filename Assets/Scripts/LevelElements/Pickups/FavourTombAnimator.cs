using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelElements
{
    /// <summary>
    /// Class handling the animation of a Tomb containing a Favour.
    /// </summary>
    public class FavourTombAnimator : TombAnimator
    {
        //##################################################################
        
        // FSM: FaveurManager
        [Header("Favour Manager")]
        [SerializeField] private Transform faveur;
        [SerializeField] private float duration = 1.9f;

        // FSM: Faveur_activation
        [Header("Favour Activation")]
        [SerializeField] private float animSpeed = 0.0005f;
        [SerializeField] private float startDelay = 4;
        [SerializeField] private float disparitionEnd = 10;
        [SerializeField] private float disparitionSpeed = .03f;

        [Header("Visuals")]
        [SerializeField] private Animator animator;
        [SerializeField] private Renderer recept;
        [SerializeField] private GameObject favSparkUp;

        // FSM: ParticleManager
        [Header("Particle Manager")]
        [SerializeField] private float delay2 = 3f;
        [SerializeField] private float delay3 = 0.2f;

        [SerializeField] private ParticleSystem favSparks;
        [SerializeField] private GameObject favSparksOpen;
        [SerializeField] private GameObject favSparksBurst;

        // Dissolve Tomb
        [Header("Tomb Dissolve")]
        [SerializeField] private Renderer tombShell;
        [SerializeField] private string dissolveVariableName = "_Dissolve";
        [SerializeField] private Material fullyDissolvedMaterial;
        [SerializeField] private float timeBeforeDissolve = 2;
        [SerializeField] private float dissolveDuration = 2;
        [SerializeField] private AnimationCurve dissolveCurve;

        [Header("Sound")]
        [SerializeField] private AudioClip getClip;
        [SerializeField, Range(0, 2)] private float volumeGet = 1f;
        [SerializeField] private bool addRandomisationGet = false;
        [SerializeField] private float minDistance = 10f;
        [SerializeField] private float maxDistance = 50f;
        [SerializeField] private float clipDuration = 0f;
        [SerializeField] private AudioClip favourClip;
        [SerializeField, Range(0, 2)] private float volumeFavour = 1f;
        [SerializeField] private bool addRandomisationFavour = false;
        [SerializeField] private AudioClip favourEndClip;
        [SerializeField, Range(0, 2)] private float volumeFavourEnd = 1f;
        [SerializeField] private bool addRandomisationFavourEnd = false;
        [SerializeField] private float minDistanceFavour = 10f;
        [SerializeField] private float maxDistanceFavour = 50f;

        WaitForSeconds _startDelay, _delay2, _delay3, _cent, _beforeDisolve;

        //##################################################################

        public override bool SetTombState(bool isActivated, bool interactWithPlayer, bool doImmediateTransition, TombAnimationFinishedCallback callback = null)
        {
            if(!base.SetTombState(isActivated, interactWithPlayer, doImmediateTransition, callback))
            {
                return false;
            }

            if (isActivated)
            {
                if (interactWithPlayer)
                {
                    _startDelay = new WaitForSeconds(startDelay);
                    _delay2 = new WaitForSeconds(delay2);
                    _delay3 = new WaitForSeconds(delay3);
                    _cent = new WaitForSeconds(0.01f);

                    SoundifierOfTheWorld.PlaySoundAtLocation(getClip, transform, maxDistance, volumeGet, minDistance, clipDuration, addRandomisationGet);
                    animator.SetBool("Fav_activated", true);
                    StartCoroutine(FaveurActivation());
                    StartCoroutine(ParticleManager());
                    StartCoroutine(FavourManager());
                    StartCoroutine(DissolveTomb());
                }
                else
                {
                    StartCoroutine(DissolveTomb());
                }
            }

            return true;
        }

        bool skip = false;

        public void Skip()
        {
            skip = true;
        }

        private IEnumerator FaveurActivation()
        {
            float disparition = 0;
            for(float elapsed = 0; elapsed < startDelay; elapsed += Time.unscaledDeltaTime) {
                if (skip) break;
                yield return null;
            }

            favSparkUp.SetActive(true);
            while (disparition < disparitionEnd)
            {
                disparition += disparitionSpeed;
                recept.material.SetFloat("_Emissive_intensity", disparition);
                animator.speed += animSpeed;
                yield return _cent;
            }
        }

        private IEnumerator FavourManager()
        {
            Transform player = FindObjectOfType<Player.CharacterController.CharController>().transform;
            for (float i = 0; i < startDelay; i += Time.unscaledDeltaTime) {
                if (skip) break;
                yield return null;
            }
            faveur.parent = null;

            SoundifierOfTheWorld.PlaySoundAtLocation(favourClip, faveur, maxDistanceFavour, volumeFavour, minDistanceFavour, 0f, addRandomisationFavour, true, .5f);

            float elapsed = 0;
            while ((faveur.position - (player.position + player.up)).sqrMagnitude > 0.1f && elapsed < 2)
            {
                elapsed += Time.deltaTime;
                faveur.position = Vector3.Lerp(faveur.position, player.position + player.up, elapsed / duration);
                yield return null;
            }

            Destroy(faveur.gameObject);
            SoundifierOfTheWorld.PlaySoundAtLocation(favourEndClip, player, maxDistanceFavour, volumeFavourEnd, minDistanceFavour, 0f, addRandomisationFavourEnd, false, .5f);
            Instantiate(favSparksBurst, faveur.position, Quaternion.identity);

            animationFinishedCallback?.Invoke(); // Informs the Pickup that the Tomb has finished its animation.
        }

        private IEnumerator ParticleManager()
        {
            favSparksOpen.SetActive(true);
            yield return _delay2;
            favSparksBurst.SetActive(true);
            favSparks.gameObject.SetActive(true);
            yield return _delay3;

            var em = favSparks.emission;

            while (em.rate.constant > 0f)
            {
                em.rate = new ParticleSystem.MinMaxCurve(em.rate.constant - 1.4f);
                yield return _cent;
            }
            favSparks.gameObject.SetActive(false);
        }

        private IEnumerator DissolveTomb()
        {
            yield return new WaitForSeconds(timeBeforeDissolve);

            for (float elapsed = 0; elapsed < dissolveDuration; elapsed += Time.deltaTime)
            {
                tombShell.material.SetFloat(dissolveVariableName, dissolveCurve.Evaluate(elapsed / dissolveDuration));
                yield return null;
            }
            tombShell.sharedMaterial = fullyDissolvedMaterial;
        }

        //##################################################################
    }
} // end of namespace