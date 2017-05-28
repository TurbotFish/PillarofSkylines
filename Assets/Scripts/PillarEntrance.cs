using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class PillarEntrance : MonoBehaviour {

    public bool isOpen;
    public int favoursToSacrify = 3;
    public SceneField pillarLevel;

    [SerializeField]
    Transform actualDoor;
    [SerializeField]
    float timeToOpen = 1;

    bool playerIsHere;
    FavourManager favourManager;

    void Start() {
        favourManager = FavourManager.instance;
    }

    void Update() {
        if (!playerIsHere) return;

        if (Input.GetKeyDown(KeyCode.F)) {
            if (isOpen) {
                SceneManager.LoadScene(pillarLevel);
            } else
                favourManager.DisplaySacrificeMenu(this);
        }
    }

    public void OpenDoor() {
        print("Pillar Door Open");
        StartCoroutine(_OpenDoor());
        isOpen = true;
    }

    void OnTriggerEnter(Collider other) {

        if (other.tag == "Player") {
            if (isOpen)
                print("press F to Enter");
            else
                print("Press F to Open");
            playerIsHere = true;
        }
    }

    void OnTriggerExit(Collider other) {

        if (other.tag == "Player") {
            playerIsHere = false;
        }
    }

    IEnumerator _OpenDoor() {

        Vector3 startPos = actualDoor.localPosition;

        for (float elapsed = 0; elapsed < timeToOpen; elapsed+=Time.deltaTime) {
            float t = elapsed / timeToOpen;
            actualDoor.localPosition = Vector3.Lerp(startPos, Vector3.zero, t);
            yield return null;
        }
        actualDoor.localPosition = Vector3.zero;
    }
}
