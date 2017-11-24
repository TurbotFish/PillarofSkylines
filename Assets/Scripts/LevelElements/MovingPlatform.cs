using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

	public Player currPlayer;

	void Start () {
		transform.tag = "MovingPlatform";
	}

	public void AddPlayer(Player player) {
		currPlayer = player;
	}

	public void RemovePlayer() {
		currPlayer = null;
	}
}
