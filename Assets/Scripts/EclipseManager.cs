using UnityEngine;
using System.Collections;

public class EclipseManager : MonoBehaviour {

    [TestButton("Start Eclipse", "StartEclipse", isActiveInEditor = false)]
    [TestButton("Stop Eclipse", "StopEclipse", isActiveInEditor = false)]
    public bool isEclipseActive;

    public float rotationDuration = 1;

    [SerializeField]
    Transform pillar;

    EchoManager echoes;
    Transform player;
    Needle needle;

    #region Singleton
    public static EclipseManager instance;
    void Awake() {
        if (!instance) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this)
            Destroy(gameObject);
    }
    #endregion

    void Start () {
        echoes = EchoManager.instance;
        needle = FindObjectOfType<Needle>();
        needle.transform.parent = pillar;
        player = FindObjectOfType<ThirdPersonController>().transform;
	}
	
    public void StartEclipse() {
        echoes.FreezeAll();
        PlaceAllObjectsInPillar();
        isEclipseActive = true;
        RotatePillar(player.position, Vector3.forward, -90);
    }

    void PlaceAllObjectsInPillar() {
        GameObject[] allRootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject go in allRootObjects) {
            if (go.GetComponent<Camera>()) continue; //not the camera
            go.transform.parent = pillar;
        }
    }

    public void StopEclipse() {
        echoes.UnfreezeAll();
        isEclipseActive = false;
        needle.gameObject.SetActive(true);
        RotatePillar(player.position, Vector3.forward, 90);
    }

    void RotatePillar(Vector3 pivot, Vector3 axis, float angle) {
        StartCoroutine(_RotatePillar(pivot, axis, angle));
    }

    IEnumerator _RotatePillar(Vector3 pivot, Vector3 axis, float angle) {
        
        for (float elapsed = 0; elapsed < rotationDuration; elapsed += Time.deltaTime) {
            float t = elapsed / rotationDuration;
            
            pillar.RotateAround(pivot, axis, angle * rotationDuration * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
    }

}
