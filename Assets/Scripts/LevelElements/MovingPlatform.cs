using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

	public Game.Player.CharacterController.Character currPlayer;

	public Vector3 impactPoint;

	void Start () {
		transform.tag = "MovingPlatform";
	}

	virtual public void AddPlayer(Game.Player.CharacterController.Character player, Vector3 playerImpactPoint) {
		currPlayer = player;
		impactPoint = playerImpactPoint;
	}

	public void RemovePlayer() {
		currPlayer = null;
	}
}
