﻿using UnityEngine;

public class CharacControllerRecu : MonoBehaviour {



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

	[HideInInspector]
	public bool jumpedOnThisFrame;

	/// <summary>
	/// The max walkable slope angle.
	/// </summary>
	[Tooltip("The max walkable slope angle.")]
	public float maxSlopeAngle = 45f;
	/// <summary>
	/// The slide speed.
	/// </summary>
	public float slideSpeed = 1f;
	/// <summary>
	/// The slide damp.
	/// </summary>
	public float slideDamp = 0.2f;

	Quaternion playerAngle;

	void Start(){
		favourCollider = GetComponentInChildren<CapsuleCollider> ();
		myTransform = transform;
		myPlayer = GetComponent<Player>();
		favourCollider.center = center;
		favourCollider.radius = radius;
		favourCollider.height = height + radius*2;
		capsuleHeightModifier = new Vector3 (0, height, 0);
	}


	public Vector3 Move(Vector3 velocity){
		playerAngle = (Quaternion.AngleAxis(Vector3.Angle (Vector3.up, transform.up), Vector3.Cross(Vector3.up, transform.up)));

		#if UNITY_EDITOR
		Debug.DrawRay (myTransform.position + playerAngle * center, velocity*10, Color.green);
		#endif

		collisionNumber = 0;
		//Recursively check if the movement meets obstacles
		velocity = CollisionDetection (velocity, myTransform.position + playerAngle * center, new RaycastHit());

		//Update collision informations
		CollisionUpdate (velocity);


		/// Check if calculated movement will end up in a wall, if so cancel movement
		if (!Physics.CheckCapsule (myTransform.position + playerAngle * (center - capsuleHeightModifier/2) + velocity, myTransform.position + playerAngle * (center + capsuleHeightModifier/2) + velocity, radius*.9f, collisionMaskNoCloud)) {
			myTransform.Translate (velocity, Space.World);
			//Debug.Log ("controller : " + _velocity/Time.deltaTime);
			return (Quaternion.AngleAxis (Vector3.Angle (transform.up, Vector3.up), Vector3.Cross (Vector3.up, transform.up))) * velocity * 10f;
		} else {
			Debug.LogWarning ("Oh oh, tu vas dans un mur. " + collisions.below);
			return Vector3.zero;
		}
	}


	void CollisionUpdate(Vector3 velocity){
		RaycastHit hit;
		//Send casts to check if there's stuff around the player and sets bools depending on the results
		collisions.below = Physics.SphereCast (myTransform.position + velocity + playerAngle * (center - capsuleHeightModifier/2) + myTransform.up * skinWidth*2, radius, -myTransform.up, out hit, skinWidth*4, collisionMask);
		Debug.DrawRay(myTransform.position + velocity + playerAngle * (center - capsuleHeightModifier/2), -myTransform.up * (skinWidth) * 10f, Color.cyan);
		Debug.Log ("below = " + collisions.below + " start : " + (myTransform.position + velocity + playerAngle * (center - capsuleHeightModifier/2)) + 
			" end : " + ((myTransform.position + velocity + playerAngle * (center - capsuleHeightModifier/2)) - myTransform.up * skinWidth*4));
		if (collisions.below) {
			myPlayer.currentPlayerState = Player.ePlayerState.onGround;
			collisions.currentGroundNormal = hit.normal;
		} else {
			myPlayer.currentPlayerState = Player.ePlayerState.inAir;
		}
		collisions.above = Physics.SphereCast (myTransform.position + playerAngle * center + capsuleHeightModifier, radius * .9f, myTransform.up, out hit, velocity.magnitude + skinWidth, collisionMask);
		if (collisions.above && hit.collider.CompareTag ("cloud")) {
			collisions.above = false;
		}
		collisions.side = Physics.SphereCast (myTransform.position + playerAngle * center, radius, Vector3.ProjectOnPlane(velocity, myTransform.up), out hit, velocity.magnitude + skinWidth, collisionMask);
	}


	//Recursively check if the movement meets obstacles
	Vector3 CollisionDetection(Vector3 velocity, Vector3 position, RaycastHit oldHit){

		Vector3 movementVector = Vector3.zero;
		RaycastHit hit;
		Vector3 veloNorm = velocity.normalized;
		float rayLength = velocity.magnitude;
		Vector3 newOrigin = position;


		//Send a first capsule cast in the direction of the velocity
		if (Physics.CapsuleCast (newOrigin - capsuleHeightModifier/2, newOrigin + capsuleHeightModifier/2, radius, velocity, out hit, rayLength, collisionMask)) {
			collisionNumber++;

			//When an obstacle is met, remember the amount of movement needed to get to the obstacle
			movementVector += veloNorm * (hit.distance - ((skinWidth < hit.distance)? skinWidth : hit.distance));

			//Get the remaining velocity after getting to the obstacle
			Vector3 extraVelocity = (velocity - movementVector);

			//Check if this extra velocity isn't too small
			if (extraVelocity.magnitude > .001f && collisionNumber < 5) {
				Vector3 reflection = new Vector3();
				//if it's the first obstacle met on this frame, project the extra velocity on the plane parallel to this obstacle
				//if it's not the first, project on the line parallel to both the current obstacle and the previous one
				//once the extra velocity has been projected, Call the function once more to check if this new velocity meets obstacle and do everything again
				if (collisionNumber > 1) {
					if (Vector3.Dot (hit.normal, oldHit.normal) < 0) {
						reflection = CollisionDetection (Vector3.Project (extraVelocity, Vector3.Cross (hit.normal, oldHit.normal)), position + movementVector, hit);
					} else {
						reflection = CollisionDetection (Vector3.ProjectOnPlane(extraVelocity, hit.normal), position + movementVector, hit);
					}
				} else {
					reflection = CollisionDetection (Vector3.ProjectOnPlane(extraVelocity, hit.normal), position + movementVector, hit);
				}
				//Add all the reflections calculated together
				movementVector += reflection;
			}
		} else {
			//if no obstacle is met, add the reamining velocity to the movement vector
			movementVector += velocity;
		}

		if (collisionNumber > 4) {
			Debug.LogWarning ("whoa that was a lot of collisions there (" + collisionNumber + ").");
		}
		Debug.Log ("nb coll " + collisionNumber);
		Debug.DrawRay (newOrigin, movementVector, Color.red);

		//return the movement vector calculated
		return movementVector;
	}


	/// <summary>
	/// The struct containing three bools describing collision informations.
	/// </summary>
	public struct CollisionInfo{
		public bool above, below;
		public bool side, onSteepSlope;

		public Vector3 currentGroundNormal;

		public void Reset(){
			above = below = false;
			side = onSteepSlope = false;
			currentGroundNormal = Vector3.zero;
		}

	}

	void OnDrawGizmosSelected()	{
		Quaternion playerAngle = (Quaternion.AngleAxis(Vector3.Angle (Vector3.up, transform.up), Vector3.Cross(Vector3.up, transform.up)));
		Gizmos.color = new Color(1, 0, 1, 0.75F);
		Vector3 upPosition = transform.position + playerAngle * (center + capsuleHeightModifier/2);
		Vector3 downPosition = transform.position + playerAngle * (center - capsuleHeightModifier/2);
		Gizmos.DrawWireSphere(upPosition, radius);
		Gizmos.DrawWireSphere(downPosition, radius);
	}

}
