using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Echo : MonoBehaviour {

    new BoxCollider collider;
    public bool isActive, isFrozen;

    public float colliderSizeWhenSolid = 2;
    float defaultColliderSize;

    [SerializeField]
    Material solidMaterial;
    Material defaultMaterial;

    ParticleSystem particles;
    new Renderer renderer;
    EchoManager echoManager;
    Transform pool;

	void Start() {
        particles = GetComponentInChildren<ParticleSystem>();
        renderer = particles.GetComponent<Renderer>();
        defaultMaterial = renderer.sharedMaterial;

        collider = GetComponent<BoxCollider>();
        collider.isTrigger = true;
        defaultColliderSize = collider.size.x;

        echoManager = FindObjectOfType<EchoManager>();

        pool = echoManager.pool;
	}

    void OnTriggerEnter(Collider col) {
        if (isActive && col.tag == "Player") {
            Break();
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.tag == "Player") {
            isActive = true;
        }
    }

    public void Break() {
        echoManager.BreakParticles(transform.position);
        gameObject.SetActive(false);
        transform.parent = pool;
        echoManager.echoes.Remove(this);
    }

    public void Freeze() {
        if (!particles) Start();
        particles.Pause();
        renderer.sharedMaterial = solidMaterial;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        isFrozen = true;

        collider.size = Vector3.one * colliderSizeWhenSolid;
        collider.isTrigger = false;
    }

    public void Unfreeze() {
        if (!particles) Start();
        particles.Play();
        renderer.sharedMaterial = defaultMaterial;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        isFrozen = false;

        collider.size = Vector3.one * defaultColliderSize;
        collider.isTrigger = true;
    }
}
