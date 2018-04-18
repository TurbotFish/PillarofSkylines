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

        [SerializeField] GameObject fluidEcho, solidEcho;

        [HideInInspector] public EchoManager echoManager;
        int pickUpLayer;

        public Transform MyTransform { get; private set; }

        Transform echoTransform;

        [Header("Animation")]
        [SerializeField] float speed = 1;
        [SerializeField] float intensity = 0.2f;
        [SerializeField] float height = 1.5f;
        Vector3 fxPosition;

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
            if(other.tag == "Player")
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

        //##################################################################
    }
} //end of namespace