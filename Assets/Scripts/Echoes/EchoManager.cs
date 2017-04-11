using System.Collections.Generic;
using UnityEngine;

public class EchoManager : MonoBehaviour {

	public bool AI;
	public bool demo;
	public Vector3 demoVelocity;

    [SerializeField]
    Echo echoPrefab;
    [SerializeField]
    BreakEchoParticles breakEchoParticles;

    [TestButton("Freeze All Echoes", "FreezeAll", isActiveInEditor = false)]
    [TestButton("Unfreeze All Echoes", "UnfreezeAll", isActiveInEditor = false)]
    public List<Echo> echoes;
    public int maxEchoes = 3;

    [HideInInspector]
    public Transform pool;

    new EchoCameraEffect camera;

    bool eclipse;

    #region Singleton
    public static EchoManager instance;
    void Awake() {
        if (!instance) {
            instance = this;
			if (!demo)
           		DontDestroyOnLoad(gameObject);
        } else if (instance != this)
            Destroy(gameObject);
    }
    #endregion

    void Start() {
		camera = FindObjectOfType<ThirdPersonCamera>().GetComponent<EchoCameraEffect>();
        pool = new GameObject().transform;
        pool.name = "Echo Pool";
    }

	void Update () {
        if (!eclipse && !AI) {
            if (Input.GetKeyUp(KeyCode.A)) 
                Drift();
            if (Input.GetKeyUp(KeyCode.E)) 
                CreateEcho();
        }
	}

    public void Drift() {
        if (echoes.Count > 0) {
			if (!AI) camera.SetFov(70, 0.15f, true);
            Echo targetEcho = echoes[echoes.Count - 1];
            transform.position = targetEcho.transform.position; // We should reference Player and move this script in a Manager object
            if (!targetEcho.isActive)
                targetEcho.Break();

			if (demo)
			{
				GetComponent<Rigidbody>().velocity = demoVelocity;
			}
        }
    }

	public void CreateEcho() {
        Echo newEcho = InstantiateFromPool(echoPrefab, transform.position);
        echoes.Add(newEcho);
		Debug.Log ("echo");
        if (echoes.Count > maxEchoes)
            echoes[0].Break();
    }

    public void FreezeAll() {
        eclipse = true;
        for (int i = 0; i < echoes.Count; i++)
            echoes[i].Freeze();
    }

    public void UnfreezeAll() {
        eclipse = false;
        for (int i = 0; i < echoes.Count; i++)
            echoes[i].Unfreeze();
    }

    public void SetEchoesParent(Transform parent) {
        for (int i = 0; i < echoes.Count; i++)
            echoes[i].transform.parent = parent;
    }

    T InstantiateFromPool<T>(T prefab, Vector3 position) where T : MonoBehaviour {

        T poolObject = pool.GetComponentInChildren<T>();

        if (poolObject != null) {
            poolObject.transform.parent = null;
            poolObject.transform.position = position;
            poolObject.gameObject.SetActive(true);

        } else
            poolObject = Instantiate(prefab, position, Quaternion.identity);
        return poolObject;
    }

    public void BreakParticles(Vector3 position) {
        BreakEchoParticles breakParticles = InstantiateFromPool(breakEchoParticles, position);
    }
}
