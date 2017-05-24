using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotation : MonoBehaviour {

	public float rotationSpeed = 0.2f;
	public Vector3 rotationVector = new Vector3(0,1,0);

	// Update is called once per frame
	void Update () {
		transform.Rotate(rotationVector*rotationSpeed);
	}
}
