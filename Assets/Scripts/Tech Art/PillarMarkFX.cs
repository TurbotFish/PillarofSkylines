using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarMarkFX : MonoBehaviour {

	public Animator eyeAnim;

	public void GetMark()
	{
		eyeAnim.SetBool ("marked", true);
	}
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.H)) {
			GetMark ();
		}	
	}
}
