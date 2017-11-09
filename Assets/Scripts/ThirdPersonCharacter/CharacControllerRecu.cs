﻿using UnityEngine;

public class CharacControllerRecu : MonoBehaviour
{



	#region collider properties

	/// <summary>
	/// The center of the capsule used as the player collider.
	/// </summary>
	[Tooltip("The center of the capsule used as the player collider.")]
	public Vector3 center = new Vector3(0f, 0f, 0f);
	/// <summary>
	/// The radius of the capsule used as the player collider.
	/// </summary>
	[Tooltip("The radius of the capsule used as the player collider.")]
	public float radius = .5f;
	/// <summary>
	/// The height difference between the position and the center of the upper sphere of the capsule.
	/// </summary>
	[Tooltip("The height of the capsule.")]
	public float height = 1f;

	#endregion collider properties

	#region cast variables

	/// <summary>
	/// The height difference between the two points of the capsule.
	/// </summary>
	Vector3 capsuleHeightModifier;
	/// <summary>
	/// The safety margin used when casting for obstacles.
	/// </summary>
	[Tooltip("The safety margin used when casting for obstacles.")]
	public float skinWidth = 0.02f;
	/// <summary>
	/// The layer mask used to detect obstacles.
	/// </summary>
	[Tooltip("The layer mask used to detect obstacles.")]
	public LayerMask collisionMask;
	/// <summary>
	/// The layer mask used to detect obstacles (doesn't detect clouds).
	/// </summary>
	[Tooltip("The layer mask used to detect obstacles (doesn't detect clouds).")]
	public LayerMask collisionMaskNoCloud;

	#endregion cast variables


	public CollisionInfo collisions;
	public CapsuleCollider favourCollider;
	Transform myTransform;
	Player myPlayer;

	/// <summary>
	/// The number of collisions detected on this frame.
	/// </summary>
	int collisionNumber;

	/// <summary>
	/// The cloud the player is currently on (null if not on a cloud).
	/// </summary>
	Cloud currentCloud;

	bool climbingStep;
	Vector3 stepOffset;

	Quaternion playerAngle;
	RaycastHit hit;
	RaycastHit hit2;
	Vector3 wallDir;

	void Start() {
		favourCollider = GetComponentInChildren<CapsuleCollider>();
		myTransform = transform;
		myPlayer = GetComponent<Player>();
		favourCollider.center = center;
		favourCollider.radius = radius;
		favourCollider.height = height + radius * 2;
		capsuleHeightModifier = new Vector3(0, height, 0);
	}


	public Vector3 Move(Vector3 velocity) {
//		print("---------------------------------new movement----------------------------------");
		playerAngle = (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, transform.up), Vector3.Cross(Vector3.up, transform.up)));
		collisions.initialVelocityOnThisFrame = velocity;

		#if UNITY_EDITOR
		Debug.DrawRay(myTransform.position + playerAngle * center, velocity / Time.deltaTime, Color.green);
		#endif

		collisionNumber = 0;
		//Recursively check if the movement meets obstacles
		velocity = CollisionDetection(velocity, myTransform.position + playerAngle * center, new RaycastHit());
		if (climbingStep) {
//			print("moved player up");
			transform.position += stepOffset;
			stepOffset = Vector3.zero;
		}

		Vector3 finalVelocity = Vector3.zero;
		/// Check if calculated movement will end up in a wall, if so try to adjust movement
		if (!Physics.CheckCapsule(myTransform.position + playerAngle * (center - capsuleHeightModifier / 2) + velocity, myTransform.position + playerAngle * (center + capsuleHeightModifier / 2) + velocity, radius, collisionMaskNoCloud)) {
			myTransform.Translate(velocity, Space.World);
			finalVelocity = (Quaternion.AngleAxis(Vector3.Angle(transform.up, Vector3.up), Vector3.Cross(transform.up, Vector3.up))) * velocity / Time.deltaTime;
		} else {
			wallDir = Vector3.zero;
			Debug.LogWarning("Oh oh, tu vas dans un mur. " + collisions.below);
			if (collisions.below) {
				wallDir = collisions.currentGroundNormal;
			} else if (collisions.side) {
				wallDir = collisions.currentWallNormal;
			} else if (Physics.Raycast(transform.position + playerAngle * center, collisions.initialVelocityOnThisFrame, out hit, collisions.initialVelocityOnThisFrame.magnitude + radius + height / 2 + skinWidth, collisionMask)) {
				wallDir = hit.normal;
			}
			if (Physics.Raycast(transform.position + playerAngle * center + collisions.initialVelocityOnThisFrame, -wallDir, out hit, radius + height / 2 + skinWidth, collisionMask)) {
				Vector3 destination = (hit.point + (hit.normal * (Mathf.Abs(Vector3.Dot(transform.up, hit.normal)) * height / 2 + radius + skinWidth))) - myTransform.up * (height / 2 + radius); 
				if (!Physics.CheckCapsule(myTransform.position + playerAngle * (center - capsuleHeightModifier / 2) + (destination - myTransform.position)
					, myTransform.position + playerAngle * (center + capsuleHeightModifier / 2) + (destination - myTransform.position), radius, collisionMaskNoCloud)) {
					myTransform.Translate(destination - myTransform.position, Space.World);
					finalVelocity = (Quaternion.AngleAxis(Vector3.Angle(transform.up, Vector3.up), Vector3.Cross(transform.up, Vector3.up))) * (velocity) / Time.deltaTime;
				}
			} else {
				finalVelocity = Vector3.zero;
			}
		}

		//Update collision informations
		CollisionUpdate(velocity);

		return finalVelocity;
	}


	void CollisionUpdate(Vector3 velocity) {
		//Send casts to check if there's stuff around the player and set bools depending on the results

		collisions.below = Physics.SphereCast(myTransform.position + playerAngle * (center - capsuleHeightModifier / 2) + myTransform.up * skinWidth * 2, radius, -myTransform.up, out hit, skinWidth * 4, collisionMask) || climbingStep;
		if (collisions.below && !Physics.SphereCast(myTransform.position + playerAngle * (center - capsuleHeightModifier / 2) + myTransform.up * skinWidth * 2, radius, -myTransform.up, out hit, skinWidth * 4, collisionMask)) {
			if (Physics.SphereCast(myTransform.position + myTransform.up * radius, radius, -myTransform.up, out hit2, collisions.stepHeight, collisionMask)) {
				transform.position += -myTransform.up * hit2.distance;
//				print("adjusted position on step");
			}
		}
		if (collisions.below && !climbingStep) {
			collisions.currentGroundNormal = hit.normal;
			if (currentCloud == null && hit.collider.CompareTag("cloud")) {
				currentCloud = hit.collider.GetComponent<Cloud>();
				currentCloud.AddPlayer(myPlayer);
			}
		} else {
			if (currentCloud != null) {
				currentCloud.RemovePlayer();
				currentCloud = null;
			}
		}
		collisions.above = Physics.SphereCast(myTransform.position + playerAngle * (center + capsuleHeightModifier / 2) - myTransform.up * skinWidth * 2, radius, myTransform.up, out hit, skinWidth * 4, collisionMask);
		if (collisions.above && hit.collider.CompareTag("cloud")) {
			collisions.above = false;
		}
		collisions.side = Physics.CapsuleCast(myTransform.position + playerAngle * (center - capsuleHeightModifier / 2), myTransform.position + playerAngle * (center + capsuleHeightModifier / 2), radius
			, Vector3.ProjectOnPlane(collisions.initialVelocityOnThisFrame, (collisions.below ? collisions.currentGroundNormal : myTransform.up)), out hit, skinWidth * 2, collisionMask);
		if (collisions.side) {
			collisions.currentWallNormal = hit.normal;
		}
	}


	//Recursively check if the movement meets obstacles
	Vector3 CollisionDetection(Vector3 velocity, Vector3 position, RaycastHit oldHit) {

		Vector3 movementVector = Vector3.zero;
		RaycastHit hit;
		Vector3 veloNorm = velocity.normalized;
		float rayLength = velocity.magnitude;
		Vector3 newOrigin = position;

//		print("pass " + (collisionNumber + 1));

		//Send a first capsule cast in the direction of the velocity
		if (Physics.CapsuleCast(newOrigin - (playerAngle * capsuleHeightModifier / 2), newOrigin + (playerAngle * capsuleHeightModifier / 2), radius, velocity, out hit, rayLength
			, ((veloNorm.y > 0 || myPlayer.currentPlayerState == ePlayerState.gliding) ? collisionMaskNoCloud : collisionMask))) {
			collisionNumber++;

			/*
			if ((hit.normal != collisions.currentGroundNormal || climbedStep) && collisionNumber < 2) {
				print("slt");
				collisions.stepHeight = 0f;
				Debug.DrawRay(myTransform.position + myTransform.up * (height + radius * 2) + Vector3.ProjectOnPlane(hit.point - myTransform.position, collisions.currentGroundNormal).normalized * (radius + skinWidth), -myTransform.up * (height + radius * 2), Color.red);
				if (Physics.Raycast(myTransform.position + myTransform.up * (height + radius * 2) + Vector3.ProjectOnPlane(hit.point - myTransform.position, collisions.currentGroundNormal).normalized * (radius + skinWidth), -myTransform.up, out hit2, height + radius * 2, collisionMask)) {
					collisions.stepHeight = (height + radius * 2) - hit2.distance;
					Debug.Log(hit2.collider.name + ", height " + collisions.stepHeight + ", distance : " + hit2.distance);
					if (collisions.stepHeight < 1f && collisions.stepHeight > .1f) {
						if (Physics.SphereCast(myTransform.position + Vector3.ProjectOnPlane(velocity, collisions.currentGroundNormal) + myTransform.up * collisions.stepHeight, radius, -myTransform.up, out hit2, collisions.stepHeight, collisionMask)) {

//							myTransform.position += Vector3.ProjectOnPlane(velocity, collisions.currentGroundNormal) + myTransform.up * (collisions.stepHeight + skinWidth - hit2.distance);
							climbedStep = true;
							return Vector3.ProjectOnPlane(velocity, collisions.currentGroundNormal) + myTransform.up * (collisions.stepHeight + skinWidth - hit2.distance);
							//myTransform.position += myTransform.up * collisions.stepHeight;
						}
					}
					else {
						climbedStep = false;
					}
				}
			}*/

			//When an obstacle is met, remember the amount of movement needed to get to the obstacle
			movementVector += veloNorm * (hit.distance - ((skinWidth < hit.distance) ? skinWidth : hit.distance));

			//Get the remaining velocity after getting to the obstacle
			Vector3 extraVelocity = (velocity - movementVector);

			//Detect the obstacle met from above to check if it's a step
//			Debug.DrawRay(myTransform.position + myTransform.up * (height + radius * 2) + Vector3.ProjectOnPlane(hit.point - myTransform.position, myTransform.up).normalized * (radius + skinWidth), -myTransform.up * (height + radius * 2), Color.red);
			if (myPlayer.currentPlayerState == ePlayerState.onGround && Physics.Raycast(myTransform.position + movementVector + myTransform.up * (height + radius * 2) + Vector3.ProjectOnPlane(hit.point - myTransform.position, myTransform.up).normalized * (radius + skinWidth), -myTransform.up, out hit2, height + radius * 2, collisionMask)) {
				collisions.stepHeight = (height + radius * 2) - hit2.distance;
				// Once checked if it's a step, check if it's not too high, and if it's not a slope
//				print("new ground angle : " + Vector3.Angle(hit2.normal, myTransform.up) + ", step height : " + collisions.stepHeight + ", dot product : " + Vector3.Dot(hit.normal, hit2.normal));
				if (Vector3.Angle(hit2.normal, myTransform.up) < myPlayer.maxSlopeAngle && collisions.stepHeight < myPlayer.maxStepHeight && Vector3.Dot(hit.normal, hit2.normal) < .95f) {
					if (Physics.SphereCast(myTransform.position + movementVector + extraVelocity + myTransform.up * collisions.stepHeight, radius, -myTransform.up, out hit2, collisions.stepHeight, collisionMask)) {
						stepOffset += myTransform.up * (collisions.stepHeight - hit2.distance);
//						print("step added via sphere : " + stepOffset);
					} else {
						stepOffset = myTransform.up * collisions.stepHeight;
//						print("step not added via sphere : " + stepOffset);
					}
					climbingStep = true;
//					print("added step offset");
				} else {
//					print("stopped climbing 1");
					climbingStep = false;
				}
			} else {
//				print("stopped climbing 2");
				climbingStep = false;
			}

			//Check if this extra velocity isn't too small
			if (extraVelocity.magnitude > skinWidth && collisionNumber < (climbingStep ? 4 : 5)) {
				Vector3 reflection = new Vector3();
				//if it's the first obstacle met on this frame, project the extra velocity on the plane parallel to this obstacle
				//if it's not the first, project on the line parallel to both the current obstacle and the previous one
				//if the player is on the ground, count the ground as a first collision
				//once the extra velocity has been projected, Call the function once more to check if this new velocity meets obstacle and do everything again
				if (climbingStep) {
					// If the controller detected the player is going up a step, send a new detection from above the step
					reflection = CollisionDetection(extraVelocity, position + movementVector * (1 + skinWidth) + stepOffset, hit);
				} else {
					if (collisionNumber > 1) {
						if (Vector3.Dot(hit.normal, oldHit.normal) < 0) {
							reflection = CollisionDetection(Vector3.Project(extraVelocity, Vector3.Cross(hit.normal, oldHit.normal)), position + movementVector, hit);
						} else {
							reflection = CollisionDetection(Vector3.ProjectOnPlane(extraVelocity, hit.normal), position + movementVector, hit);
						}
					} else {
						if (collisions.below && Vector3.Dot(hit.normal, collisions.currentGroundNormal) < 0) {
							reflection = CollisionDetection(Vector3.Project(extraVelocity, Vector3.Cross(hit.normal, collisions.currentGroundNormal)), position + movementVector, hit);
						} else {
							reflection = CollisionDetection(Vector3.ProjectOnPlane(extraVelocity, hit.normal), position + movementVector, hit);
						}
					}
				}
				//Add all the reflections calculated together
				movementVector += reflection;
			}
		} else {
			if (collisionNumber == 0) {
//				print("stopped climbing 3");
				climbingStep = false;
			}

			//if no obstacle is met, add the reamining velocity to the movement vector
			movementVector += velocity;
		}

		if (collisionNumber > 4) {
			Debug.LogWarning("whoa that was a lot of collisions there (" + collisionNumber + ").");
		}
		Debug.DrawRay(newOrigin, movementVector, Color.red);


		//return the movement vector calculated
		return movementVector;
	}


	/// <summary>
	/// The struct containing three bools describing collision informations.
	/// </summary>
	public struct CollisionInfo
	{
		public bool above, below;
		public bool side, onSteepSlope;

		public float stepHeight;

		public Vector3 initialVelocityOnThisFrame;

		public Vector3 currentGroundNormal;
		public Vector3 currentWallNormal;

		public void Reset() {
			above = below = false;
			side = onSteepSlope = false;
			currentGroundNormal = Vector3.zero;
			currentWallNormal = Vector3.zero;
		}

	}

	void OnDrawGizmosSelected() {
		Quaternion playerAngle = (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, transform.up), Vector3.Cross(Vector3.up, transform.up)));
		Gizmos.color = new Color(1, 0, 1, 0.75F);
		Vector3 upPosition = transform.position + playerAngle * (center + capsuleHeightModifier / 2);
		Vector3 downPosition = transform.position + playerAngle * (center - capsuleHeightModifier / 2);
		Gizmos.DrawWireSphere(upPosition, radius);
		Gizmos.DrawWireSphere(downPosition, radius);
	}

}
