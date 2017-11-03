using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTunnelPart : MonoBehaviour {

	Player currentPlayer;
	public float windStrength;
	public float tunnelAttraction;
	public int idInTunnel;

	// Update is called once per frame
	void Update () {
		if (currentPlayer != null)
		{
			currentPlayer.AddWindVelocity(transform.up*windStrength + Vector3.ProjectOnPlane(transform.position - currentPlayer.transform.position, transform.up)*tunnelAttraction);
		}
	}

	public void AddPlayer(Player player){
		currentPlayer = player;
	}

	public void RemovePlayer(){
		currentPlayer.ExitWindTunnel();
		currentPlayer = null;
	}
}
