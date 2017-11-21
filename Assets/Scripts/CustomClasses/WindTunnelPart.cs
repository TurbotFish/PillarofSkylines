﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTunnelPart : MonoBehaviour {

    Game.Player.CharacterController.Character currentPlayer;
	public float windStrength;
	public float tunnelAttraction;
	public int idInTunnel;

	// Update is called once per frame
	void Update () {
		if (currentPlayer != null)
		{
//			Debug.Log("wind velocity = " + (transform.up * windStrength + Vector3.ProjectOnPlane(transform.position - currentPlayer.transform.position, transform.up) * tunnelAttraction) + " made from " + Vector3.ProjectOnPlane(transform.position - currentPlayer.transform.position, transform.up));
			currentPlayer.AddWindVelocity(transform.up*windStrength + Vector3.ProjectOnPlane(transform.position - currentPlayer.transform.position, transform.up)*tunnelAttraction);
		}
	}

	public void AddPlayer(Game.Player.CharacterController.Character player){
		currentPlayer = player;
	}

	public void RemovePlayer(){
		currentPlayer.ExitWindTunnel();
		currentPlayer = null;
	}
}
