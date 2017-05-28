using UnityEngine;

public class Eye : MonoBehaviour {

    EclipseManager eclipse;

    private void Start() {
        eclipse = EclipseManager.instance;
    }
    
    void OnTriggerStay(Collider other) {

        if (other.tag == "Player" && eclipse.isEclipseActive) {
            print("Press F to kill the eye");

            if (Input.GetKeyDown(KeyCode.F)) {
                print("C'est gagné !");
                gameObject.SetActive(false);
            }
        }
    }
}
