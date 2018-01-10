using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PresentationShortcuts : MonoBehaviour {

	private Transform _character;
	private Vector3 _charBasePosition;
	private Vector3 _cameraBaseRotation;

	// Use this for initialization
	void Start () {
		//_character = GameObject.FindObjectOfType<ThirdPersonController> ().transform;
		_charBasePosition = _character.position;
		_cameraBaseRotation = Camera.main.transform.rotation.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.I)) {
			_character.position = _charBasePosition;
		}
		if (Input.GetKeyDown (KeyCode.U)) {
			SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
		}
		if (Input.GetKeyDown (KeyCode.O)) {
			if (Camera.main.targetDisplay != 0) {
				Camera.main.targetDisplay = 0;
				GameObject.FindObjectOfType<PoS_Camera> ().GetComponent<Camera> ().targetDisplay = 1;
				Camera.main.transform.localPosition = Vector3.zero;
				Camera.main.transform.rotation = Quaternion.Euler (_cameraBaseRotation);
			} else {
				Camera.main.targetDisplay = 1;
				GameObject.FindObjectOfType<PoS_Camera> ().GetComponent<Camera> ().targetDisplay = 0;
			}

		}
	}
}
