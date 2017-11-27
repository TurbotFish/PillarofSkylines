//using UnityEngine;

//public class EchoDoor : MonoBehaviour {

//    EclipseManager eclipse;
//    GameObject echo;
//    public bool state;

//    void Start() {
//        eclipse = EclipseManager.instance;
//        echo = GetComponentInChildren<Game.EchoSystem.Echo>(true).gameObject;
//        echo.SetActive(state);
//    }

//    void OnTriggerEnter(Collider col) {
//        if (col.tag == "Player" && !eclipse.isEclipseActive) {
//            state ^= true;
//            if (state)
//                echo.SetActive(true);
//        }
//    }

//}
