using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobbler : MonoBehaviour {

	public float speed = 1;
	public float amplitude = 0.2f;

	
	// Update is called once per frame
	void Update () {
		transform.localPosition += new Vector3 (Mathf.Sin (Time.time * speed) * amplitude, 0, 0);
	}
}
