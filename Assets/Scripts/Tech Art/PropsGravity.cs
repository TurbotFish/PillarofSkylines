using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsGravity : MonoBehaviour {

	public enum gravityFields {EastSurface,WestSurface, None}
	public gravityFields gravityField;
	float gravityForce = 20f; 
	Rigidbody rb;

	public bool useGravity;

	//gravity vectors
	Vector3 gravityWest, gravityEast;

	void Awake ()
	{
		useGravity = false;
		gravityWest = new Vector3 (gravityForce,0,0);
		gravityEast = new Vector3 (-gravityForce,0,0);
		rb = this.GetComponent<Rigidbody> ();
		rb.isKinematic = true;
	}

	void Start ()
	{
		StartCoroutine (StartGravity ());
	}

	void FixedUpdate () {

		if (useGravity) {
			if (gravityField == gravityFields.EastSurface) {
				rb.AddForce (gravityEast);
			} else if (gravityField == gravityFields.WestSurface) {
				rb.AddForce (gravityWest);
			}
		}
	}

	IEnumerator StartGravity()
	{
		yield return new WaitForSecondsRealtime (5);
		rb.isKinematic = false;
		useGravity = true;
	}

}
