using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(BoxCollider))]
public class PillarEntrance : MonoBehaviour {

    public bool isOpen;
    public int favoursToSacrify = 3;
    public SceneField pillarLevel;
	public GameObject F;

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
				F.SetActive (false);
                Invoke("LoadTheScene", timeBeforeNewScene);
            } else
                favourManager.DisplaySacrificeMenu(this);
        }
    }

    void LoadTheScene() {
        SceneManager.LoadScene(pillarLevel);
    }

    public void OpenDoor() {
		F.GetComponent<TextMeshProUGUI> ().SetText("[F] : Enter");
        print("Pillar Door Open");
        anim.SetBool("Door_open", true);
        GetComponent<BoxCollider>().enabled = false;
        isOpen = true;
    }

    void OnTriggerEnter(Collider other) {

        if (other.tag == "Player") {
			if (isOpen) {
				F.SetActive (true);
				F.GetComponent<TextMeshProUGUI> ().SetText("[F] : Enter");
				print ("press F to Enter");

			} else {
				F.SetActive (true);
				F.GetComponent<TextMeshProUGUI> ().SetText("[F] : Open the Gate");

				print("Press F to Open");

			}
            playerIsHere = true;
        }
    }

    void OnTriggerExit(Collider other) {

        if (other.tag == "Player") {
            playerIsHere = false;
			F.SetActive (false);
        }
    }
    
}
