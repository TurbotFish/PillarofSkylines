//using UnityEngine;
//using TMPro;
//public class Eye : MonoBehaviour {

//    EclipseManager eclipse;
//	public GameObject F;

//    private void Start() {
//        eclipse = EclipseManager.instance;
//    }
    
//    void OnTriggerStay(Collider other) {

//        if (other.tag == "Player" && eclipse.isEclipseActive) {
//            print("Press F to kill the eye");
//			F.SetActive (true);
//			F.GetComponent<TextMeshProUGUI> ().SetText("[F] : Plant Needle");
//            if (Input.GetKeyDown(KeyCode.F)) {
//                print("C'est gagné !");
//                gameObject.SetActive(false);
//            }
//        }
//    }

//	void OnTriggerExit(Collider other)
//	{
//		if (other.tag == "Player" && eclipse.isEclipseActive) {
//			F.SetActive (false);
//		}
//	}
//}
