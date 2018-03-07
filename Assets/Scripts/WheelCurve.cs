using Game.Player.CharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelCurve: MovingPlatform {
    
    public AnimationCurve speedOverTime;
    float timerMax;
    float timer;

	Vector3 centerToPlayer, newPlayerPos, initialGravity;
    Quaternion currentRotation;
    Quaternion startRotation;
    Quaternion lastFrameRotation;

    protected override void Start()
    {
        base.Start();

        startRotation = transform.rotation;
        timerMax = speedOverTime.keys[speedOverTime.length-1].time;
    }

	// Update is called once per frame
	void Update () {
        lastFrameRotation = transform.rotation;
		currentRotation = Quaternion.Euler(0f, 0f, speedOverTime.Evaluate(timerMax - timer));
		transform.rotation = startRotation * currentRotation;


		if (currPlayer != null) {
			currPlayer.transform.RotateAround(transform.position, transform.forward, Quaternion.Angle(lastFrameRotation, transform.rotation));
			currPlayer.ChangeGravityDirection(initialGravity, impactPoint);
			/*
			centerToPlayer = currPlayer.transform.position - transform.position;
			newPlayerPos = currentRotation * centerToPlayer;
			AddVelocityToPlayer(newPlayerPos - centerToPlayer);
			*/
		}

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = timerMax - timer;
        }
        Debug.Log("timer : " + timer);
	}

	override public void AddPlayer(CharController player, Vector3 playerImpactPoint) {
		currPlayer = player;
		initialGravity = -player.MyTransform.up;
		impactPoint = playerImpactPoint;
	}

	void AddVelocityToPlayer(Vector3 velocity) {
		currPlayer.AddExternalVelocity(velocity, false, false);
	}

}
