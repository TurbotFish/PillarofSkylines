using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerController : MonoBehaviour {

	public Animator cameraTrailerAnimator;
	public Animation test;
	public List<GameObject> characterInvisible;
	public bool isCharacterVisible = true;

	public Camera cameraTrailer;
	public Camera posCamera;
	public bool cameraTrailerActive = false;

	//public MinMax cameraSpeedLimit;
	//public float changeRate;

	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			ToggleCharacter (isCharacterVisible);
			isCharacterVisible = !isCharacterVisible;
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			ToggleCamera (cameraTrailerActive);
			cameraTrailerActive = !cameraTrailerActive;
		}


	}


	public void ToggleCharacter(bool state)
	{
		if (state) {
			foreach (GameObject go in characterInvisible) {
				go.SetActive (false);
			}
		} else {
			foreach (GameObject go in characterInvisible) {
				go.SetActive (true);
			}
		}
	}

	public void ToggleCamera(bool state)
	{
		if (state) {
			posCamera.targetDisplay = 1;
			cameraTrailer.targetDisplay = 0;

		}
		else {
			cameraTrailer.targetDisplay = 1;
			posCamera.targetDisplay = 0;

		}
	}
}
