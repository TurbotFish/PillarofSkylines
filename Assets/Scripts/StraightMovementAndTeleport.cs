using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightMovementAndTeleport : MovingPlatform {

	public float waitTime;
	public float initialWaitTime;
	public float timeMoving;
	public Vector3 movement;

	float currWaitTime;
	Vector3 initialPosition;
	float movementProgression;

	bool waiting = false, moving = false;

	protected override void Start() {
		waiting = true;
		currWaitTime = waitTime + initialWaitTime;
		initialPosition = transform.position;
        base.Start();
    }
    
	void Update () {
		if (waiting) {
			currWaitTime -= Time.deltaTime;
			if (currWaitTime < 0) {
				waiting = false;
				moving = true;
				currWaitTime = waitTime;
			}
		}
		if (moving) {
			transform.position += movement * 1 / timeMoving * Time.deltaTime;
            
            if (currPlayer != null)
				currPlayer.ImmediateMovement(movement * 1 / timeMoving * Time.deltaTime, true, false);
			movementProgression += 1 / timeMoving * Time.deltaTime;

			if (movementProgression >= 1f) {
				moving = false;
				waiting = true;
				movementProgression = 0f;
				transform.position = initialPosition;
			}
		}
	}
    
}
