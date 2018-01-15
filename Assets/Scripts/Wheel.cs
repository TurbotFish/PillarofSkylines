using Game.Player.CharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MovingPlatform {

	public float speed;


	Vector3 centerToPlayer, newPlayerPos, initialGravity;
	Quaternion currentRotation;

	// Update is called once per frame
	void Update () {
		currentRotation = Quaternion.Euler(0f, 0f, speed * Time.deltaTime);
		transform.rotation *= currentRotation;


		if (currPlayer != null) {
			currPlayer.transform.RotateAround(transform.position, transform.forward, speed * Time.deltaTime);
			currPlayer.ChangeGravityDirection(initialGravity, impactPoint);
			/*
			centerToPlayer = currPlayer.transform.position - transform.position;
			newPlayerPos = currentRotation * centerToPlayer;
			AddVelocityToPlayer(newPlayerPos - centerToPlayer);
			*/
		}
	}

	override public void AddPlayer(CharController player, Vector3 playerImpactPoint) {
		currPlayer = player;
		initialGravity = -player.MyTransform.up;
		impactPoint = playerImpactPoint;
	}

	void AddVelocityToPlayer(Vector3 velocity) {
		currPlayer.AddExternalVelocity(velocity, false, false);
		print("velocity added : " + velocity * 100);
	}

}
