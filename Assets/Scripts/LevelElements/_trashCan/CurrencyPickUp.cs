//using System;
//using System.Collections;
//using Game.GameControl;
//using Game.Model;
//using Game.World;
//using UnityEngine;
//using UnityEngine.Serialization;

//namespace Game.LevelElements
//{
//    [Obsolete]
//    public class CurrencyPickUp : MonoBehaviour, IWorldObject
//    {
//        //##################################################################

//        #region variables

//        [Header("Pick Up")]

//        [FormerlySerializedAs("favourId")]
//        [SerializeField] private string pickUpId = "replace this!";
//        [SerializeField] private eCurrencyType currencyType;

//        [SerializeField, HideInInspector] public Vector3 FinderTarget; //what is this for?

//        // FSM: FaveurManager
//        [Header("FaveurManager")]
//        [SerializeField] private Transform faveur;
//        private float duration = 1.9f;

//        // FSM: Faveur_activation
//        private float animSpeed = 0.0005f;
//        private float startDelay = 4;
//        private float disparitionEnd = 10;
//        private float disparitionSpeed = .03f;

//        [Header("Visuals")]
//        [SerializeField] private Animator animator;
//        [SerializeField] private Renderer recept;
//        [SerializeField] private GameObject favSparkUp;

//        // FSM: ParticleManager
//        private float delay2 = 3f;
//        private float delay3 = 0.2f;

//        [SerializeField] private ParticleSystem favSparks;
//        [SerializeField] private GameObject favSparksOpen;
//        [SerializeField] private GameObject favSparksBurst;

//        // Dissolve Tomb
//        [Header("Tomb Dissolve")]
//        [SerializeField] private Renderer tombShell;
//        [SerializeField] private string dissolveVariableName = "_Dissolve";
//        [SerializeField] private Material fullyDissolvedMaterial;
//        [SerializeField] private float timeBeforeDissolve = 2;
//        [SerializeField] private float dissolveDuration = 2;
//        [SerializeField] private AnimationCurve dissolveCurve;

//        //

//        private IGameControllerBase gameController;
//        private BoxCollider myCollider;
//        private bool isInitialized;
//        private bool isCopy;
//        private bool favourPickedUp;

//        #endregion variables

//        //##################################################################

//        #region properties

//        public string PickUpId { get { return pickUpId; } }
//        public Transform MyTransform { get; private set; }
//        public eCurrencyType CurrencyType { get { return currencyType; } }

//        #endregion properties      

//        //##################################################################

//        #region public methods

//        void IWorldObject.Initialize(IGameControllerBase gameController, bool isCopy)
//        {
//            MyTransform = transform;
//            FinderTarget = MyTransform.position;

//            foreach (Transform child in MyTransform)
//            {
//                if (child.CompareTag("Favour"))
//                {
//                    FinderTarget = child.position;
//                    break;
//                }
//            }

//            this.gameController = gameController;
//            myCollider = GetComponent<BoxCollider>();
//            this.isCopy = isCopy;

//            if (gameController.PlayerModel.CheckIfPickUpCollected(pickUpId))
//            {
//                OnFavourPickedUp();

//                StartCoroutine(DissolveTomb());
//            }
//            else
//            {
//                Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;
//            }

//            isInitialized = true;
//        }

//        #endregion public methods

//        //##################################################################

//        #region monobehaviour methods

//        private void OnEnable()
//        {
//            if (!isInitialized || favourPickedUp)
//            {
//                return;
//            }
//            else if (gameController.PlayerModel.CheckIfPickUpCollected(pickUpId))
//            {
//                OnFavourPickedUp();

//                StartCoroutine(DissolveTomb());
//            }
//            else
//            {
//                Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;
//            }
//        }

//        private void OnDisable()
//        {
//            Utilities.EventManager.FavourPickedUpEvent -= OnFavourPickedUpEventHandler;
//        }

//        #endregion monobehaviour methods

//        //##################################################################

//        #region private methods

//        /// <summary>
//        /// Handles "FavourPickedUp" events.
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="args"></param>
//        private void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.FavourPickedUpEventArgs args)
//        {
//            if (args.FavourId == pickUpId)
//            {
//                OnFavourPickedUp();

//                if (!isCopy)
//                {
//                    Animate();
//                }
//                else
//                {
//                    StartCoroutine(DissolveTomb());
//                }
//            }
//        }

//        /// <summary>
//        /// Sets the state of the Favour object to picked up.
//        /// </summary>
//        private void OnFavourPickedUp()
//        {
//            favourPickedUp = true;

//            //check because all colliders are removed in the duplicated worlds
//            if (myCollider != null)
//            {
//                myCollider.enabled = false;
//            }
//        }

//        void OnFavourReceived()
//        {
//            string message = currencyType == eCurrencyType.Favour ? "You have been granted a Favour" : "The Eyes have marked you";
//            string desc = currencyType == eCurrencyType.Favour ? "Press Start to open the ability menu" : "Destroy the Pillars to free the world";

//            Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(true, message, UI.eMessageType.Announcement, desc, 4));
//        }

//        #endregion private methods

//        //##################################################################

//        #region animation

//        /// <summary>
//        /// Starts the pick-up animations and effects.
//        /// </summary>
//        void Animate()
//        {
//            // here do playmaker stuff
//            animator.SetBool("Fav_activated", true);
//            StartCoroutine(FaveurActivation());
//            StartCoroutine(ParticleManager());
//            StartCoroutine(FavourManager());
//            StartCoroutine(DissolveTomb());
//        }
        
//        IEnumerator FaveurActivation()
//        {
//            float disparition = 0;
//            yield return new WaitForSeconds(startDelay);

//            favSparkUp.SetActive(true);
//            while (disparition < disparitionEnd)
//            {
//                disparition += disparitionSpeed;
//                recept.material.SetFloat("_Emissive_intensity", disparition);
//                animator.speed += animSpeed;
//                yield return new WaitForSeconds(0.01f);
//            }
//        }

//        IEnumerator FavourManager()
//        {
//            Transform player = FindObjectOfType<Player.CharacterController.CharController>().transform;
//            yield return new WaitForSeconds(startDelay);
//            faveur.parent = null;

//            float elapsed = 0;
//            while ((faveur.position - (player.position + player.up)).sqrMagnitude > 0.1f)
//            {
//                elapsed += Time.deltaTime;
//                faveur.position = Vector3.Lerp(faveur.position, player.position + player.up, elapsed / duration);
//                yield return null;
//            }

//            Destroy(faveur.gameObject);
//            Instantiate(favSparksBurst, faveur.position, Quaternion.identity);
//            OnFavourReceived();
//        }

//        IEnumerator ParticleManager()
//        {
//            favSparksOpen.SetActive(true);
//            yield return new WaitForSeconds(delay2);
//            favSparksBurst.SetActive(true);
//            favSparks.gameObject.SetActive(true);
//            yield return new WaitForSeconds(delay3);

//            var em = favSparks.emission;

//            while (em.rate.constant > 0f)
//            {
//                em.rate = new ParticleSystem.MinMaxCurve(em.rate.constant - 1.4f);
//                yield return new WaitForSeconds(0.01f);
//            }
//            favSparks.gameObject.SetActive(false);
//        }

//        IEnumerator DissolveTomb()
//        {
//            yield return new WaitForSeconds(timeBeforeDissolve);

//            for (float elapsed = 0; elapsed < dissolveDuration; elapsed += Time.deltaTime)
//            {
//                tombShell.material.SetFloat(dissolveVariableName, dissolveCurve.Evaluate(elapsed / dissolveDuration));
//                yield return null;
//            }
//            tombShell.sharedMaterial = fullyDissolvedMaterial;
//        }

//        #endregion animation

//        //##################################################################
//    }
//} //end of namespace