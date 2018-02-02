using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsCollisions : MonoBehaviour {

	public Game.Player.CharacterController.CharController charController;
	public Vector3 playerVelocity;
	public float multiplier = 100f;
	public float minForce = 30;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		playerVelocity = charController.MovementInfo.velocity;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.GetComponent<PropsGravity> () != null) {
			col.GetComponent<Rigidbody> ().isKinematic = false;
			col.GetComponent<Rigidbody> ().AddForce (playerVelocity.normalized*minForce + playerVelocity*multiplier);
		}
	}
}
