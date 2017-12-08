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

        //##################################################################

        void Start() {
            MyTransform = transform;

            myParticleSystem = GetComponentInChildren<ParticleSystem>();
            myRenderer = myParticleSystem.GetComponent<Renderer>();
            defaultMaterial = myRenderer.sharedMaterial;

            collider = GetComponent<BoxCollider>();
            collider.isTrigger = true;
            defaultColliderSize = collider.size;

            pickUpLayer = gameObject.layer;
        }

        //##################################################################

        #region collision stuff

        //void OnTriggerEnter(Collider col)
        //{
        //    if (isActive)
        //    { // Si un truc rentre dans un écho, il est détruit
        //        Break();
        //    }
        //}

        //void OnTriggerExit(Collider col)
        //{
        //    if (col.tag == "Player")
        //    {
        //        if (isActive)
        //            Break();
        //        else
        //            isActive = true;
        //    }
        //}

        #endregion collision stuff

        //##################################################################

        public void Break() {
            if (isActive)
                echoManager.Break(this);
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