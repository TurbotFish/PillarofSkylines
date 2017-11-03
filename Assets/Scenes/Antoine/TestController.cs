using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour {

	public Transform test;
	public float rotSpeed;
	public float angleMax;


	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = test.position;
		if (Quaternion.Angle (transform.rotation, test.rotation) > angleMax) {
			transform.rotation = Quaternion.Lerp (transform.rotation, test.rotation, rotSpeed * Time.deltaTime);
		}

	}
}
