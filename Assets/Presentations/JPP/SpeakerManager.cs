using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpeakerManager : MonoBehaviour {

	//public string alois, antoine, boris, david, gwendal, nicolas, patrick;

	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			DOTween.Restart ("alois_appear");
		}
		if (Input.GetKeyDown (KeyCode.Z)) {
			DOTween.Restart ("antoine_appear");
		}
		if (Input.GetKeyDown (KeyCode.B)) {
			DOTween.Restart ("boris_appear");
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			DOTween.Restart ("david_appear");
		}
		if (Input.GetKeyDown (KeyCode.G)) {
			DOTween.Restart ("gwendal_appear");
		}
		if (Input.GetKeyDown (KeyCode.N)) {
			DOTween.Restart ("nicolas_appear");
		}
		if (Input.GetKeyDown (KeyCode.P)) {
			DOTween.Restart ("patrick_appear");
		}

	}
}
