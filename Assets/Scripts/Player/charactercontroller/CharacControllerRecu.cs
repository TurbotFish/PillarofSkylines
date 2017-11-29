﻿using UnityEngine;

namespace Game.Player.CharacterController
{
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
		Character myPlayer;

		/// <summary>
		/// The number of collisions detected on this frame.
		/// </summary>
		int collisionNumber;
		Collider[] wallsOverPlayer;

		/// <summary>
		/// The cloud the player is currently on (null if not on a cloud).
		/// </summary>
		MovingPlatform currentPF;

		bool climbingStep;
		Vector3 stepOffset;

		Quaternion playerAngle;
		RaycastHit hit;
		RaycastHit hit2;
		Vector3 wallDir;
		RaycastHit sideHit;

		void Start() {
			favourCollider = GetComponentInChildren<CapsuleCollider>();
			myTransform = transform;
			myPlayer = GetComponent<Character>();
			favourCollider.center = center;
			favourCollider.radius = radius;
			favourCollider.height = height + radius * 2;
			capsuleHeightModifier = new Vector3(0, height, 0);
		}


		public Vector3 Move(Vector3 velocity) {
//			print("---------------------------------new movement----------------------------------");
			playerAngle = (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, transform.up), Vector3.Cross(Vector3.up, transform.up)));
			collisions.initialVelocityOnThisFrame = velocity;

#if UNITY_EDITOR
			Debug.DrawRay(myTransform.position + playerAngle * center, velocity / Time.deltaTime, Color.green);
#endif

			collisionNumber = 0;
			//Recursively check if the movement meets obstacles
			velocity = CollisionDetection(velocity, myTransform.position + playerAngle * center, new RaycastHit());
			if (climbingStep) {
				transform.position += stepOffset;
				stepOffset = Vector3.zero;
			}

			Vector3 finalVelocity = Vector3.zero;
			/// Check if calculated movement will end up in a wall, if so try to adjust movement
			wallsOverPlayer = Physics.OverlapCapsule(myTransform.position + playerAngle * (center - capsuleHeightModifier / 2) + velocity, myTransform.position + playerAngle * (center + capsuleHeightModifier / 2) + velocity, radius, collisionMaskNoCloud);
			if (wallsOverPlayer.Length == 0) {
				finalVelocity = ConfirmMovement(velocity);
			} else {
				Debug.LogWarning("The player's inside " + wallsOverPlayer.Length + " wall(s) : " + wallsOverPlayer[0].name);
				finalVelocity = AdjustPlayerPosition(velocity);
			}

			//Update collision informations
			CollisionUpdate(velocity);

			return finalVelocity;
		}

		Vector3 ConfirmMovement(Vector3 velocity) {
			myTransform.Translate(velocity, Space.World);
			return (Quaternion.AngleAxis(Vector3.Angle(transform.up, Vector3.up), Vector3.Cross(transform.up, Vector3.up))) * velocity / Time.deltaTime;
		}

		Vector3 AdjustPlayerPosition(Vector3 velocity) {

			Vector3 OutOfWallDirection = Vector3.zero;
			foreach (Collider wall in wallsOverPlayer) {
//                print("collider : " + wall.name);
				bool foundSmth = false;
				Vector3 position = myTransform.position + velocity + playerAngle * center;
				do {
					foundSmth = Physics.Linecast(position, wall.transform.position, out hit, collisionMask);
					if (foundSmth)
						position = hit.point + (wall.transform.position - hit.point) * .01f;
				} while (foundSmth && !wall.gameObject.Equals(hit.transform.gameObject));

				if (foundSmth) {
					OutOfWallDirection += hit.normal;
				} else {
					OutOfWallDirection += transform.up;
				}
//				print("added : " + hit.normal + " from : " + hit.transform.name);
			}

			Physics.Raycast(myTransform.position + velocity + playerAngle * center, -OutOfWallDirection, out hit, radius + height / 2, collisionMask);
			OutOfWallDirection = OutOfWallDirection.normalized * (radius + height / 2);

//            print("moved towards : " + OutOfWallDirection);
			if (Physics.CapsuleCast(myTransform.position + playerAngle * (center - capsuleHeightModifier / 2) + OutOfWallDirection, myTransform.position + playerAngle * (center + capsuleHeightModifier / 2) + OutOfWallDirection, radius
                , -OutOfWallDirection, out hit, radius + height, collisionMask)) {
				myTransform.Translate(OutOfWallDirection * (1 - hit.distance), Space.World);
				return ConfirmMovement(velocity);
			} else {
				myTransform.Translate(OutOfWallDirection, Space.World);
				return ConfirmMovement(velocity);
			}
		}


		void CollisionUpdate(Vector3 velocity) {

			if (currentPF != null) {
				currentPF.RemovePlayer();
				currentPF = null;
			}
			// EN TEST POUR BIEN RESTER AU SOL, à voir ce que ça vaut
			if (myPlayer.currentPlayerState == ePlayerState.onGround) {
				if (Physics.SphereCast(myTransform.position + myTransform.up * (radius + skinWidth), radius, -myTransform.up, out hit2, myPlayer.maxStepHeight, collisionMask)) {
					transform.position += -myTransform.up * (hit2.distance - skinWidth);
//									print("adjusted position on ground by : " + hit2.distance);
				}
			}

			//Send casts to check if there's stuff around the player and set bools depending on the results
			collisions.below = Physics.SphereCast(myTransform.position + playerAngle * (center - capsuleHeightModifier / 2) + myTransform.up * skinWidth * 2, radius, -myTransform.up, out hit, skinWidth * 4, collisionMask) || climbingStep;
			// POUR BIEN SE COLLER AU SOL EN MONTANT UNE MARCHE
//			if (collisions.below && !Physics.SphereCast(myTransform.position + playerAngle * (center - capsuleHeightModifier / 2) + myTransform.up * skinWidth * 2, radius, -myTransform.up, out hit, skinWidth * 4, collisionMask)) {
//				if (Physics.SphereCast(myTransform.position + myTransform.up * radius, radius, -myTransform.up, out hit2, collisions.stepHeight, collisionMask)) {
//					transform.position += -myTransform.up * hit2.distance;
//					print("adjusted position on step by : " + hit2.distance);
//				}
//			}
			if (collisions.below && !climbingStep) {
				collisions.currentGroundNormal = hit.normal;
				if (currentPF == null && hit.collider.CompareTag("MovingPlatform")) {
					currentPF = hit.collider.GetComponentInParent<MovingPlatform>();
					currentPF.AddPlayer(myPlayer, hit.point);
				}
			}
			collisions.above = Physics.SphereCast(myTransform.position + playerAngle * (center + capsuleHeightModifier / 2) - myTransform.up * skinWidth * 2, radius, myTransform.up, out hit, skinWidth * 4, collisionMask);
			if (collisions.above) {
				if (hit.collider.CompareTag("cloud")) {
					collisions.above = false;
				}
				if (currentPF == null && hit.collider.CompareTag("MovingPlatform")) {
					currentPF = hit.collider.GetComponentInParent<MovingPlatform>();
					currentPF.AddPlayer(myPlayer, hit.point);
				}
			}

			//***********************************************************
			//side collision

			if (collisions.side) { //here we check if the player is still "touching" the wall he previously collided with
				RaycastHit searchHit;

				bool colliding = Physics.CapsuleCast(
					                             myTransform.position + playerAngle * (center - capsuleHeightModifier / 2),
					                             myTransform.position + playerAngle * (center + capsuleHeightModifier / 2),
					                             radius,
					                             -sideHit.normal,
					                             out searchHit,
					                             skinWidth * 2,
					                             collisionMask
				                             );

				if (colliding && searchHit.transform.Equals(sideHit.transform)) {
					sideHit = searchHit;
					collisions.currentWallNormal = sideHit.normal;
				} else {
					collisions.side = false;
				}
			}

			if (!collisions.side) { //if the player is not touching the previous wall anymore, check for a new collision
				collisions.side = Physics.CapsuleCast(
					myTransform.position + playerAngle * (center - capsuleHeightModifier / 2),
					myTransform.position + playerAngle * (center + capsuleHeightModifier / 2),
					radius,
					Vector3.ProjectOnPlane(
						collisions.initialVelocityOnThisFrame,
						(collisions.below ? collisions.currentGroundNormal : myTransform.up)
					),
					out sideHit,
					skinWidth * 2,
					collisionMask
				);

				if (collisions.side) {
					//Debug.LogErrorFormat("hitName = {0}; hitNormal={1}", sideHit.collider.name, sideHit.normal);

					collisions.currentWallNormal = sideHit.normal;
				}
			}

			if (collisions.side) { //register with moving platforms
				if (currentPF == null && sideHit.collider.CompareTag("MovingPlatform")) {
					currentPF = sideHit.collider.GetComponentInParent<MovingPlatform>();
					currentPF.AddPlayer(myPlayer, sideHit.point);
				}
			}

			//***********************************************************
		}
		//end of CollisionUpdate


		//Recursively check if the movement meets obstacles
		Vector3 CollisionDetection(Vector3 velocity, Vector3 position, RaycastHit oldHit) {

			Vector3 movementVector = Vector3.zero;
			RaycastHit hit;
			Vector3 veloNorm = velocity.normalized;
			float rayLength = velocity.magnitude;
			Vector3 newOrigin = position;

			//print("velocity : " + velocity*100 + "initial velocity : " + collisions.initialVelocityOnThisFrame*100);

			//Send a first capsule cast in the direction of the velocity
			if (Physics.CapsuleCast(newOrigin - (playerAngle * capsuleHeightModifier / 2), newOrigin + (playerAngle * capsuleHeightModifier / 2), radius, velocity, out hit, rayLength
                , ((veloNorm.y > 0 || myPlayer.currentPlayerState == ePlayerState.gliding) ? collisionMaskNoCloud : collisionMask))) {
				collisionNumber++;

//				print("met smth ! name : " + hit.collider.name + " at : " + hit.point + " distance : " + hit.distance);

				//When an obstacle is met, remember the amount of movement needed to get to the obstacle
				movementVector += veloNorm * (hit.distance - skinWidth);

				//Get the remaining velocity after getting to the obstacle
				Vector3 extraVelocity = (velocity - movementVector);


				#region step detection
				//Detect the obstacle met from above to check if it's a step
//				Debug.DrawRay(myTransform.position + movementVector + myTransform.up * (height + radius * 2) + Vector3.ProjectOnPlane(hit.point - myTransform.position, myTransform.up).normalized * (radius + skinWidth), -myTransform.up * (height + radius * 2), Color.red);
//				Debug.DrawRay(myTransform.position + movementVector + myTransform.up * (height + radius * 2), Vector3.ProjectOnPlane(hit.point - myTransform.position, myTransform.up).normalized * (radius + skinWidth), Color.red);
				if ((myPlayer.currentPlayerState == ePlayerState.onGround || climbingStep)
				                && Physics.Raycast(myTransform.position + movementVector + myTransform.up * (height + radius * 2) + Vector3.ProjectOnPlane(hit.point - myTransform.position, myTransform.up).normalized * (radius + skinWidth), -myTransform.up, out hit2, height + radius * 2, collisionMask)
				                && !Physics.Raycast(myTransform.position + movementVector + myTransform.up * (height + radius * 2), Vector3.ProjectOnPlane(hit.point - myTransform.position, myTransform.up), (radius + skinWidth), collisionMask)) {
					collisions.stepHeight = (height + radius * 2) - hit2.distance;
					// Once checked if it's a step, check if it's not too high, and if it's not a slope
//					print("detected collision at " + hit2.point + ", new ground angle : " + Vector3.Angle(hit2.normal, myTransform.up) + ", step height : " + collisions.stepHeight + ", dot product : " + Vector3.Dot(hit.normal, hit2.normal));
					if (Vector3.Angle(hit2.normal, myTransform.up) < myPlayer.maxSlopeAngle && collisions.stepHeight < myPlayer.maxStepHeight && Vector3.Dot(hit.normal, hit2.normal) < .95f && Vector3.Dot(hit.normal, hit2.normal) >= -.01f) {
						stepOffset = myTransform.up * collisions.stepHeight;
//						print("step added : " + stepOffset.y);
						climbingStep = true;
					} else {
//						print("stopped climbing 1");
						climbingStep = false;
					}
				} else {
//					print("stopped climbing 2");
					climbingStep = false;
				}
				#endregion step detection


				//Stop the process if the script detected a lot of collisions
				if (collisionNumber < (climbingStep ? 4 : 5)) {
					Vector3 reflection = new Vector3();
					//if it's the first obstacle met on this frame, project the extra velocity on the plane parallel to this obstacle
					//if it's not the first, project on the line parallel to both the current obstacle and the previous one
					//if the player is on the ground, count the ground as a first collision
					//once the extra velocity has been projected, Call the function once more to check if this new velocity meets obstacle and do everything again
					if (climbingStep) {
						// If the controller detected the player is going up a step, send a new detection from above the step
						reflection = CollisionDetection(extraVelocity, position + movementVector + stepOffset, hit);
					} else {
						if (collisionNumber > 1) {
							if (Vector3.Dot(hit.normal, oldHit.normal) < 0) {
//								print("recu 1");
								reflection = CollisionDetection(Vector3.Project(extraVelocity, Vector3.Cross(hit.normal, oldHit.normal)), position + movementVector, hit);
							} else {
//								print("recu 2");
								reflection = CollisionDetection(Vector3.ProjectOnPlane(extraVelocity, hit.normal), position + movementVector, hit);
							}
						} else {
							if (collisions.below && Vector3.Dot(hit.normal, collisions.currentGroundNormal) < 0) {
//								print("recu 3");
								reflection = CollisionDetection(Vector3.Project(extraVelocity, Vector3.Cross(hit.normal, collisions.currentGroundNormal)), position + movementVector, hit);
							} else {
//								print("recu 4");
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
//			print("coll nmber : " + collisionNumber);
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
}
 //end of namespace