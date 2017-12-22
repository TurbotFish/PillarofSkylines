﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTunnelPart : MonoBehaviour {

    Game.Player.CharacterController.CharController currentPlayer;
	public float windStrength;
	public float tunnelAttraction;
	public int idInTunnel;

    public Transform MyTransform { get; private set; }

    void Start()
    {
        MyTransform = transform;
    }

    // Update is called once per frame
    void Update () {
		if (currentPlayer != null)
		{
//			Debug.Log("wind velocity = " + (transform.up * windStrength + Vector3.ProjectOnPlane(transform.position - currentPlayer.transform.position, transform.up) * tunnelAttraction) + " made from " + Vector3.ProjectOnPlane(transform.position - currentPlayer.transform.position, transform.up));
			//currentPlayer.AddWindVelocity(transform.up*windStrength + Vector3.ProjectOnPlane(transform.position - currentPlayer.transform.position, transform.up)*tunnelAttraction);
		}
	}

	public void AddPlayer(Game.Player.CharacterController.CharController player){
		currentPlayer = player;
	}

	public void RemovePlayer(){
		//currentPlayer.ExitWindTunnel();
		currentPlayer = null;
	}
}
