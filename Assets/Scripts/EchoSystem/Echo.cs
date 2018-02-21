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

        ParticleSystem myParticleSystem;
        Renderer myRenderer;

        [HideInInspector] public EchoManager echoManager;
        int pickUpLayer;

        public Transform MyTransform { get; private set; }
        Transform fxTransform;

        [Header("Animation")]
        [SerializeField] float speed = 1;
        [SerializeField] float intensity = 0.2f;
        [SerializeField] float height = 1.5f;
        Vector3 fxPosition;

        //##################################################################

        void Start() {
            MyTransform = transform;

            myParticleSystem = GetComponentInChildren<ParticleSystem>();
            myRenderer = myParticleSystem.GetComponent<Renderer>();
            defaultMaterial = myRenderer.sharedMaterial;

            fxTransform = myParticleSystem.transform;

            collider = GetComponent<BoxCollider>();
            collider.isTrigger = true;
            defaultColliderSize = collider.size;

            pickUpLayer = gameObject.layer;
        }

        private void Update()
        {
            fxPosition.y = Mathf.Sin(Time.time * speed) * intensity + height;
            fxTransform.localPosition = fxPosition;
        }

        //##################################################################

        #region collision stuff

        void OnTriggerEnter(Collider col) {
            if (!col.isTrigger)
                Break(col.tag == "Player"); // Si un truc rentre dans un écho, il est détruit
        }

        #endregion collision stuff

        //##################################################################

        public void Break(bool byPlayer = false) {
            if (isActive) {
                isActive = false;
                if (byPlayer)
                    Utilities.EventManager.SendEchoDestroyedEvent(this);
                echoManager.Break(this);
            }
        }

        public void Freeze()
        {
            if (!myParticleSystem)
                Start();

            myParticleSystem.Pause();

            myRenderer.sharedMaterial = solidMaterial;
            myRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

            gameObject.layer = 0;

            isFrozen = true;

            collider.size *= colliderSizeWhenSolid;
            collider.isTrigger = false;

            MyTransform.rotation = Quaternion.identity;
        }

        public void Unfreeze()
        {
            if (!myParticleSystem)
                Start();

            myParticleSystem.Play();

            myRenderer.sharedMaterial = defaultMaterial;
            myRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            gameObject.layer = pickUpLayer;

            isFrozen = false;

            collider.size = defaultColliderSize;
            collider.isTrigger = true;
        }

        //##################################################################
    }
} //end of namespace