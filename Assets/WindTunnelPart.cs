using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTunnelPart : MonoBehaviour {

	Player currentPlayer;
	
	// Update is called once per frame
	void Update () {
		if (currentPlayer != null)
		{
			currentPlayer.AddExternalVelocity(transform.up, true);
		}
	}

	public void AddPlayer(Player player){
		currentPlayer = player;
	}

	public void RemovePlayer(){
		currentPlayer = null;
	}
}
