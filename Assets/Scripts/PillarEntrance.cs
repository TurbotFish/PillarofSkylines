using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class PillarEntrance : MonoBehaviour {

    public bool isOpen;
    public int favoursToSacrify = 3;
    public SceneField pillarLevel;
    
    [SerializeField]
    Animator anim;
    [SerializeField]
    float timeBeforeNewScene = 0.5f;

    bool playerIsHere;
    FavourManager favourManager;

    void Start() {
        favourManager = FavourManager.instance;
    }

    void Update() {
        if (!playerIsHere) return;

        if (Input.GetKeyDown(KeyCode.F)) {
            if (isOpen) {
                anim.SetBool("Door_descent", true);
                Invoke("LoadTheScene", timeBeforeNewScene);
            } else
                favourManager.DisplaySacrificeMenu(this);
        }
    }

    void LoadTheScene() {
        SceneManager.LoadScene(pillarLevel);
    }

    public void OpenDoor() {
        print("Pillar Door Open");
        anim.SetBool("Door_open", true);
        GetComponent<BoxCollider>().enabled = false;
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
    
}
