using UnityEngine;
using System.Collections.Generic;

namespace Game.EchoSystem
{
    [RequireComponent(typeof(Collider))]
    public class Echo : MonoBehaviour
    {
        [SerializeField] public bool isActive;
        [SerializeField] private bool isFrozen;
        [SerializeField] public bool playerEcho; // TODO: init
        [SerializeField] private float colliderSizeWhenSolid = 2;

        [SerializeField] private GameObject fluidEcho;
        [SerializeField] private GameObject solidEcho;

        [Header("Animation")]
        [SerializeField] private float speed = 1;
        [SerializeField] private float intensity = 0.2f;
        [SerializeField] private float height = 1.5f;

        [HideInInspector] public EchoManager echoManager; // TODO: init

        private new BoxCollider collider;       
        private Vector3 defaultColliderSize;
        private int pickUpLayer;
        private Transform echoTransform;      
        private Vector3 fxPosition;

        public Transform MyTransform { get; private set; }

		[Header("Particles")]
		public List<ParticleSystem> pss = new List<ParticleSystem> ();

        //##################################################################

        void Start()
        {
            MyTransform = transform;

            collider = GetComponent<BoxCollider>();
            collider.isTrigger = true;
            fluidEcho.SetActive(true);
            solidEcho.SetActive(false);
            defaultColliderSize = collider.size;

            echoTransform = fluidEcho.transform;

            pickUpLayer = gameObject.layer;
        }

        //##################################################################

        //##################################################################

        public void Freeze()
        {
            fluidEcho.SetActive(false);
            solidEcho.SetActive(true);

            gameObject.layer = 0;

            isFrozen = true;

            collider.size *= colliderSizeWhenSolid;
            collider.isTrigger = false;

            MyTransform.rotation = Quaternion.identity;
        }

        public void Unfreeze()
        {
            fluidEcho.SetActive(true);
            solidEcho.SetActive(false);
            gameObject.layer = pickUpLayer;

            isFrozen = false;

            collider.size = defaultColliderSize;
            collider.isTrigger = true;
        }

        private void Update()
        {
            if (!isFrozen)
            {
                fxPosition.y = Mathf.Sin(Time.time * speed) * intensity + height;
                echoTransform.localPosition = fxPosition;
            }
        }

        private void OnTriggerEnter(Collider col)
        {
            if (!col.isTrigger)
            {
                Break(col.tag == "Player"); // Si un truc rentre dans un écho, il est détruit
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                isActive = true;
            }
        }

        public void Break(bool byPlayer = false)
        {
            if (isActive)
            {
                isActive = false;
                if (byPlayer)
                {
                    Utilities.EventManager.SendEchoDestroyedEvent(this);
                }
                echoManager.Break(this);
            }
        }

		public void StartParticles()
		{
			foreach (ParticleSystem ps in pss) {
				ps.Play ();
			}
		}
        //##################################################################
    }
} //end of namespace