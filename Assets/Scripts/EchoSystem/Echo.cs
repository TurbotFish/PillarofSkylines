using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

        public Transform MyTransform { get { if (myTransform == null) { myTransform = transform; } return myTransform; } }

        private Transform myTransform;

        [Header("Particles")]
        public List<ParticleSystem> pss = new List<ParticleSystem>();

        //##################################################################

        // -- ATTRIBUTES

        private List<ParticleSystem> FluidEchoParticleSystemList;
        private List<ParticleSystem> SolidEchoParticleSystmeList;

        //##################################################################

        // -- INITIALIZATION

        void Start()
        {
            collider = GetComponent<BoxCollider>();
            collider.isTrigger = true;
            fluidEcho.SetActive(true);
            solidEcho.SetActive(false);
            defaultColliderSize = collider.size;

            echoTransform = fluidEcho.transform;

            pickUpLayer = gameObject.layer;

            FluidEchoParticleSystemList = fluidEcho.GetComponentsInChildren<ParticleSystem>().ToList();
            SolidEchoParticleSystmeList = solidEcho.GetComponentsInChildren<ParticleSystem>().ToList();
        }

        //##################################################################

        // -- OPERATIONS

        public void Freeze()
        {
            isFrozen = true;

            foreach (var particle_system in FluidEchoParticleSystemList)
            {
                particle_system.Pause();
            }

            fluidEcho.SetActive(false);

            solidEcho.SetActive(true);

            foreach (var particle_system in SolidEchoParticleSystmeList)
            {
                particle_system.Play();
            }

            collider.size *= colliderSizeWhenSolid;
            collider.isTrigger = false;
            gameObject.layer = 0;

            MyTransform.rotation = Quaternion.identity;
        }

        public void Unfreeze()
        {
            isFrozen = false;

            foreach (var particle_system in SolidEchoParticleSystmeList)
            {
                particle_system.Pause();
            }

            solidEcho.SetActive(false);

            fluidEcho.SetActive(true);

            foreach (var particle_system in FluidEchoParticleSystemList)
            {
                particle_system.Play();
            }

            collider.size = defaultColliderSize;
            collider.isTrigger = true;
            gameObject.layer = pickUpLayer;
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
            foreach (ParticleSystem ps in pss)
            {
                ps.Play();
            }
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

        //##################################################################
    }
} //end of namespace