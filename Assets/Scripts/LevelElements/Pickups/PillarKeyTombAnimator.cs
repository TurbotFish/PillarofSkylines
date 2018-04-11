using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelElements
{
    /// <summary>
    /// Class handling the animation of a Tomb containing a PillarKey.
    /// </summary>
    public class PillarKeyTombAnimator : TombAnimator
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

        //##################################################################

        public override bool SetTombState(bool isActivated, bool interactWithPlayer, bool doImmediateTransition, TombAnimationFinishedCallback callback = null)
        {
            if (!base.SetTombState(isActivated, interactWithPlayer, doImmediateTransition, callback))
            {
                return false;
            }

            //if (isActivated)
            //{
            //    if (interactWithPlayer)
            //    {
            //        animator.SetBool("Fav_activated", true);
            //        StartCoroutine(FaveurActivation());
            //        StartCoroutine(ParticleManager());
            //        StartCoroutine(FavourManager());
            //        StartCoroutine(DissolveTomb());
            //    }
            //    else
            //    {
            //        StartCoroutine(DissolveTomb());
            //    }
            //}
            StartCoroutine(Test());
            
            return true;
        }

        private IEnumerator Test()
        {
            yield return new WaitForSeconds(0.5f);
            animationFinishedCallback?.Invoke(); // Informs the Pickup that the Tomb has finished its animation.
        }

        private IEnumerator FaveurActivation()
        {
            float disparition = 0;
            yield return new WaitForSeconds(startDelay);

            favSparkUp.SetActive(true);
            while (disparition < disparitionEnd)
            {
                disparition += disparitionSpeed;
                recept.material.SetFloat("_Emissive_intensity", disparition);
                animator.speed += animSpeed;
                yield return new WaitForSeconds(0.01f);
            }
        }

        private IEnumerator FavourManager()
        {
            Transform player = FindObjectOfType<Player.CharacterController.CharController>().transform;
            yield return new WaitForSeconds(startDelay);
            faveur.parent = null;

            float elapsed = 0;
            while ((faveur.position - (player.position + player.up)).sqrMagnitude > 0.1f)
            {
                elapsed += Time.deltaTime;
                faveur.position = Vector3.Lerp(faveur.position, player.position + player.up, elapsed / duration);
                yield return null;
            }

            Destroy(faveur.gameObject);
            Instantiate(favSparksBurst, faveur.position, Quaternion.identity);

            animationFinishedCallback?.Invoke(); // Informs the Pickup that the Tomb has finished its animation.
        }

        private IEnumerator ParticleManager()
        {
            favSparksOpen.SetActive(true);
            yield return new WaitForSeconds(delay2);
            favSparksBurst.SetActive(true);
            favSparks.gameObject.SetActive(true);
            yield return new WaitForSeconds(delay3);

            var em = favSparks.emission;

            while (em.rate.constant > 0f)
            {
                em.rate = new ParticleSystem.MinMaxCurve(em.rate.constant - 1.4f);
                yield return new WaitForSeconds(0.01f);
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