using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PillarEntrance : MonoBehaviour {

    public bool isOpen;
    public int favoursToSacrify = 3;
    public SceneField pillarLevel;

    bool playerIsHere;
    FavourManager favourManager;

    void Start() {
        favourManager = FavourManager.instance;
    }

    void Update() {
        if (!playerIsHere) return;

        if (Input.GetKeyDown(KeyCode.F)) {
            favourManager.DisplaySacrificeMenu(this);
        }
    }

    public void OpenDoor() {
        print("Pillar Door Open");
        isOpen = true;
    }

    void OnTriggerEnter(Collider other) {

        if (other.tag == "Player") {
            if (!isOpen) {
                print("Press F to Open");
            }
            playerIsHere = true;
        }
    }

    void OnTriggerExit(Collider other) {

        if (other.tag == "Player") {
            playerIsHere = false;
        }
    }
}
