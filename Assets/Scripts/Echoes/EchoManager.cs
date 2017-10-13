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

    public List<Echo> nonEchoes; // For echoes that were not created by the player

    [HideInInspector]
    public Transform pool;
    Transform player;
    new EchoCameraEffect camera;

    bool eclipse;

    #region Singleton
    public static EchoManager instance;
    void Awake() {
        if (!instance) {
            instance = this;
        } else if (instance != this)
            Destroy(gameObject);
    }
    #endregion

    void Start() {
		camera = FindObjectOfType<ThirdPersonCamera>().GetComponent<EchoCameraEffect>();
        player = FindObjectOfType<ThirdPersonController>()?.transform ?? FindObjectOfType<Player>().transform; //to fix
        pool = new GameObject().transform;
        pool.name = "Echo Pool";
    }

	void Update () {
        if (!eclipse && !AI) {
            if (Input.GetKeyUp(KeyCode.A)) 
                Drift();
            if (Input.GetKeyUp(KeyCode.E)) 
                CreateEcho();
        } else if (Input.GetKeyUp(KeyCode.A)) {
                StopEclipse();
        }
	}

    public void StopEclipse() {
        EclipseManager.instance.StopEclipse();
    }

    public void Drift() {
        if (echoes.Count > 0) {
			if (!AI) camera.SetFov(70, 0.15f, true);
            Echo targetEcho = echoes[echoes.Count - 1];
            player.position = targetEcho.transform.position; // We should reference Player and move this script in a Manager object
            if (!targetEcho.isActive)
                targetEcho.Break();

			if (demo) {
				GetComponent<Rigidbody>().velocity = demoVelocity;
			}
        }
    }

	public void CreateEcho() {
        Echo newEcho = InstantiateFromPool(echoPrefab, player.position);
        newEcho.playerEcho = true;
        echoes.Add(newEcho);
        if (echoes.Count > maxEchoes)
            echoes[0].Break();
    }

    public void FreezeAll() {
        eclipse = true;
        for (int i = 0; i < echoes.Count; i++)
            echoes[i].Freeze();
        for (int i = 0; i < nonEchoes.Count; i++)
            nonEchoes[i].Freeze();
    }

    public void UnfreezeAll() {
        eclipse = false;
        for (int i = 0; i < echoes.Count; i++)
            echoes[i].Unfreeze();
        for (int i = 0; i < nonEchoes.Count; i++)
            nonEchoes[i].Unfreeze();
    }

    public void BreakAll() {
        for (int i = echoes.Count-1; i >=0; i--)
            echoes[i].Break();
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
        InstantiateFromPool(breakEchoParticles, position);
    }
}
