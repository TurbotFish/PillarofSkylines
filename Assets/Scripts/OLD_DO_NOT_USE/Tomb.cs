//using UnityEngine;
//using TMPro;

//public class Tomb : MonoBehaviour {

//    public string mora;
//    public Favour favour;
//	public Animator anim;
//	public GameObject F;

//    bool playerIsHere, freedSpirit;

//    void OnTriggerEnter(Collider other) {

//        if (other.tag == "Player") {
//            if (!freedSpirit) {
//                print("Press F to Pay Respect");
//				F.SetActive (true);
//				F.GetComponent<TextMeshProUGUI> ().SetText("[F] : Take Favour");
//				anim.SetBool ("Tomb_approach", true);
//            }
//            playerIsHere = true;
//        }

//    }

//    void OnTriggerExit(Collider other) {
        
//        if (other.tag == "Player") {
//            playerIsHere = false;
//			F.SetActive (false);

//			anim.SetBool ("Tomb_approach", false);
//        }

//    }

//    void Update() {
//        if (!playerIsHere) return;
        
//        if (!freedSpirit && Input.GetKeyDown(KeyCode.F)) {
//            print("thx mate");
//			anim.SetBool ("Get_favor",true);
//            favour.Unlock();
//            freedSpirit = true;
//			F.SetActive (false);

//        }
//    }

//    void OnValidate() {
//        if (gameObject.activeInHierarchy) {
//            if (mora == "")
//                Debug.LogWarning("Creature does not have a name");

//            name = "Tomb of " + mora;

//            if (!favour) {
//                favour = FindObjectOfType<FavourManager>().CreateFavour();
//            }
//            favour.mora = mora;
//            favour.name = "Favour of " + mora;
//        }
//    }

//}
