using UnityEngine;

public class Tomb : MonoBehaviour {

    public string mora;
    public Favour favour;

    bool playerIsHere, freedSpirit;

    void OnTriggerEnter(Collider other) {

        if (other.tag == "Player") {
            print("Press F to Pay Respect");
            playerIsHere = true;
        }

    }

    void OnTriggerExit(Collider other) {
        
        if (other.tag == "Player") {
            print("leaved");
            playerIsHere = false;
        }

    }

    void Update() {
        if (playerIsHere) {
            if (!freedSpirit && Input.GetKeyDown(KeyCode.F)) {
                print("thx mate");
                favour.Unlock();
                freedSpirit = true;
            }
        }
    }

    void OnValidate() {
        if (gameObject.activeInHierarchy) {
            if (mora == "")
                Debug.LogWarning("Creature does not have a name");

            name = "Tomb of " + mora;

            if (!favour) {
                favour = FindObjectOfType<FavourManager>().CreateFavour();
            }
            favour.mora = mora;
            favour.name = "Favour of " + mora;
        }
    }

}
