using UnityEngine;
using TMPro;

public class Needle : MonoBehaviour {

    [SerializeField]
    GameObject fakeNeedle;
    EclipseManager eclipseManager;

	public Animator anim;
	public GameObject needle;
	public GameObject F;

    void Start() {
        eclipseManager = EclipseManager.instance;
    }

    private void OnEnable() {
        fakeNeedle.SetActive(false);
    }

    void OnTriggerStay(Collider col) {

        if (col.tag == "Player" && eclipseManager.isEclipseActive == false) {
			F.SetActive (true);
			F.GetComponent<TextMeshProUGUI> ().SetText("[F] : Take Needle");

			anim.SetBool ("Needle_approach", true);

            if (Input.GetKeyDown(KeyCode.F)) {
                eclipseManager.StartEclipse();
                fakeNeedle.SetActive(true);
				needle.SetActive (false);
				gameObject.SetActive(false);
				F.SetActive (false);


            }
        }
    }

	void OnTriggerExit(Collider col)
	{
		if (col.tag == "Player" && eclipseManager.isEclipseActive == false) {
			anim.SetBool ("Needle_approach", false);
			F.SetActive (false);

		}
	}

}