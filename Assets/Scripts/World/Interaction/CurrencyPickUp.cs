using System.Collections;
using Game.GameControl;
using Game.Model;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.World.Interaction
{
    public class CurrencyPickUp : MonoBehaviour, IWorldObject
    {
        //##################################################################

        #region variables

        [Header("Pick Up")]
        [FormerlySerializedAs("favourId")]
        [SerializeField]
        string pickUpId = "replace this!";

        [SerializeField]
        private eCurrencyType currencyType;

        bool favourPickedUp = false;
        IGameControllerBase gameController;
        
        BoxCollider myCollider;
        bool isCopy;      

        [HideInInspector]
        public Vector3 FinderTarget;

        // FSM: FaveurManager
        [Header("FaveurManager")]
        [SerializeField] Transform faveur;
        float duration = 1.9f;

        // FSM: Faveur_activation
        float animSpeed = 0.0005f;
        float startDelay = 4;
        float disparitionEnd = 10;
        float disparitionSpeed = .03f;

        [Header("Visuals")]
        [SerializeField]
        Animator animator;
        [SerializeField] Renderer recept;
        [SerializeField] GameObject favSparkUp;

        // FSM: ParticleManager
        float delay2 = 3f;
        float delay3 = 0.2f;

        [SerializeField] ParticleSystem favSparks;
        [SerializeField] GameObject favSparksOpen;
        [SerializeField] GameObject favSparksBurst;

        // Dissolve Tomb
        [Header("Tomb Dissolve"), SerializeField] Renderer tombShell;
        [SerializeField] string dissolveVariableName = "_Dissolve";
        [SerializeField] Material fullyDissolvedMaterial;
        [SerializeField] float timeBeforeDissolve = 2;
        [SerializeField] float dissolveDuration = 2;
        [SerializeField] AnimationCurve dissolveCurve;
        [SerializeField] GameObject[] disableRays;

        #endregion variables

        //##################################################################

        #region properties

        public string PickUpId { get { return pickUpId; } }
        public Transform MyTransform { get; private set; }
        public eCurrencyType CurrencyType { get { return currencyType; } }

        #endregion properties

        //##################################################################

        #region monobehaviour methods

        private void OnDestroy()
        {
        }

        #endregion monobehaviour methods

        //##################################################################

        #region public methods

        void IWorldObject.Initialize(IGameControllerBase gameController, bool isCopy)
        {
            MyTransform = transform;

            FinderTarget = MyTransform.position;

            foreach (Transform child in MyTransform)
            {
                if (child.CompareTag("Favour"))
                {
                    FinderTarget = child.position;
                    break;
                }
            }

            this.gameController = gameController;
            myCollider = GetComponent<BoxCollider>();
            this.isCopy = isCopy;

            if (gameController.PlayerModel.CheckIfPickUpCollected(pickUpId))
            {
                PickUp();

                StartCoroutine(DissolveTomb());
            }
            else
            {
                Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;
            }
        }

        #endregion public methods

        //##################################################################

        #region private methods

        /// <summary>
        /// Handles "FavourPickedUp" events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.FavourPickedUpEventArgs args)
        {
            if (args.FavourId == pickUpId && !favourPickedUp)
            {
                PickUp();

                if (!isCopy)
                {
                    Animate();
                }
                else
                {
                    StartCoroutine(DissolveTomb());
                }
            }
        }

        /// <summary>
        /// Sets the state of the Favour object to picked up.
        /// </summary>
        private void PickUp()
        {
            favourPickedUp = true;

            //check because all colliders are removed in the duplicated worlds
            if (myCollider != null || !myCollider.Equals(null))
            {
                myCollider.enabled = false;
            }
        }

        #endregion private methods

        //##################################################################

        #region animation

        /// <summary>
        /// Starts the pick-up animations and effects.
        /// </summary>
        void Animate()
        {
            // here do playmaker stuff
            animator.SetBool("Fav_activated", true);
            StartCoroutine(FaveurActivation());
            StartCoroutine(ParticleManager());
            StartCoroutine(FavourManager());
            StartCoroutine(DissolveTomb());
        }

        IEnumerator FaveurActivation()
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

        IEnumerator FavourManager()
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
        }

        IEnumerator ParticleManager()
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

        IEnumerator DissolveTomb()
        {
            yield return new WaitForSeconds(timeBeforeDissolve);

            foreach (GameObject go in disableRays)
                go.SetActive(false);

            for (float elapsed = 0; elapsed < dissolveDuration; elapsed += Time.deltaTime)
            {
                tombShell.material.SetFloat(dissolveVariableName, dissolveCurve.Evaluate(elapsed / dissolveDuration));
                yield return null;
            }
            tombShell.sharedMaterial = fullyDissolvedMaterial;
        }

        #endregion animation

        //##################################################################
    }
} //end of namespace