using UnityEngine;

public class EchoDoor : MonoBehaviour {

    EclipseManager eclipse;
    GameObject echo;
    bool state;

    void Start() {
        eclipse = EclipseManager.instance;
        echo = GetComponentInChildren<Echo>(true).gameObject;
        state = echo.activeSelf;
    }

    void OnTriggerEnter(Collider col) {
        if (col.tag == "Player" && !eclipse.isEclipseActive) {
            state ^= true;
            if (state)
                echo.SetActive(true);
        }
    }

}
