using UnityEngine;

public class EchoDoor : MonoBehaviour {

    GameObject echo;
    bool state;

    void Start() {
        echo = GetComponentInChildren<Echo>(true).gameObject;
        state = echo.activeSelf;
    }

    void OnTriggerEnter(Collider col) {
        if (col.tag == "Player") {
            state ^= true;
            if (state)
                echo.SetActive(true);
        }
    }

}
