using UnityEngine;

namespace Game.EchoSystem
{
    [RequireComponent(typeof(Collider))]
    public class Echo : MonoBehaviour
    {
        new BoxCollider collider;
        public bool isActive, isFrozen, playerEcho;

        public float colliderSizeWhenSolid = 2;
        Vector3 defaultColliderSize;

        [SerializeField]
        Material solidMaterial;
        Material defaultMaterial;

        ParticleSystem particles;
        new Renderer renderer;
        EchoManager echoManager;
        int pickUpLayer;
        Transform pool;

        void Start()
        {
            particles = GetComponentInChildren<ParticleSystem>();
            renderer = particles.GetComponent<Renderer>();
            defaultMaterial = renderer.sharedMaterial;

            collider = GetComponent<BoxCollider>();
            collider.isTrigger = true;
            defaultColliderSize = collider.size;

            pickUpLayer = gameObject.layer;

            echoManager = FindObjectOfType<EchoManager>();
            pool = echoManager.pool;

            if (!playerEcho) echoManager.nonEchoes.Add(this);
        }

        void OnTriggerEnter(Collider col)
        {
            if (isActive)
            { // Si un truc rentre dans un écho, il est détruit
                Break();
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.tag == "Player")
            {
                if (isActive)
                    Break();
                else
                    isActive = true;
            }
        }

        public void Break()
        {
            isActive = false;
            echoManager.BreakParticles(transform.position);
            gameObject.SetActive(false);
            if (playerEcho)
            {
                transform.parent = pool;
                echoManager.echoes.Remove(this);
            }
        }

        public void Freeze()
        {
            if (!particles) Start();
            particles.Pause();
            renderer.sharedMaterial = solidMaterial;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            gameObject.layer = 0;
            isFrozen = true;

            collider.size *= colliderSizeWhenSolid;
            collider.isTrigger = false;
        }

        public void Unfreeze()
        {
            if (!particles) Start();
            particles.Play();
            renderer.sharedMaterial = defaultMaterial;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            gameObject.layer = pickUpLayer;
            isFrozen = false;

            collider.size = defaultColliderSize;
            collider.isTrigger = true;
        }
    }
} //end of namespace