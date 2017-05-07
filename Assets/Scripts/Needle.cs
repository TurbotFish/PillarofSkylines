using UnityEngine;

public class Needle : MonoBehaviour {

    [SerializeField]
    GameObject fakeNeedle;
    EclipseManager eclipseManager;

    void Start() {
        eclipseManager = EclipseManager.instance;
        fakeNeedle.SetActive(false);
    }
    
    void OnTriggerStay(Collider col) {

        if (col.tag == "Player" && eclipseManager.isEclipseActive == false) {
            print("Press F to take noodle");

            if (Input.GetKeyDown(KeyCode.F)) {
                eclipseManager.StartEclipse();
                fakeNeedle.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }

}