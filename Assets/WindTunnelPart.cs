using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTunnelPart : MonoBehaviour {

	Player currentPlayer;
	public float windStrength;
	public float tunnelAttraction;

	// Update is called once per frame
	void Update () {
		if (currentPlayer != null)
		{
			currentPlayer.AddExternalVelocity(transform.up*windStrength + Vector3.ProjectOnPlane(transform.position - currentPlayer.transform.position, transform.up)*tunnelAttraction, true);
		}
	}

	public void AddPlayer(Player player){
		currentPlayer = player;
	}

	public void RemovePlayer(){
		currentPlayer = null;
	}
}
