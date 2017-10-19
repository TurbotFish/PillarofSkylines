using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpParticles : MonoBehaviour {

	public float lifeTime;
	public List <Transform> particles;
	public List<float> rotationSpeeds;
	// Use this for initialization
	void Awake () {
		Destroy (gameObject, lifeTime);
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < particles.Count; i++) {
			particles [i].RotateAround (transform.position, Vector3.up, rotationSpeeds [i]);
		}
	}
}
