using Game.Player.CharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

	public CharController currPlayer;

	[HideInInspector]
	public Vector3 impactPoint;

	void Start () {
		transform.tag = "MovingPlatform";
	}

	virtual public void AddPlayer(CharController player, Vector3 playerImpactPoint) {
		currPlayer = player;
		impactPoint = playerImpactPoint;
	}

	public void RemovePlayer() {
		currPlayer = null;
	}
}
