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
	#endregion cast variables


	public CollisionInfo collisions;

	Transform myTransform;

	/// <summary>
	/// The number of collisions detected on this frame.
	/// </summary>
	int collisionNumber;

	/// <summary>
	/// The max walkable slope angle.
	/// </summary>
	[Tooltip("The max walkable slope angle.")]
	public float maxSlopeAngle = 45f;

	void Start(){
		myTransform = transform;
	}


	public Vector3 Move(Vector3 _velocity){

		//Debug.Log("movement : " + _velocity/Time.deltaTime);
		Quaternion playerAngle = (Quaternion.AngleAxis(Vector3.Angle (Vector3.up, transform.up), Vector3.Cross(Vector3.up, transform.up)));

		//Set the vector between the points of the capsule on this frame.
		capsuleHeightModifier = myTransform.up * height/2;

		//resets collisions
		collisionNumber = 0;
		collisions.Reset ();

		#if UNITY_EDITOR
		Debug.DrawRay (myTransform.position + playerAngle * center, _velocity*10, Color.green);
		#endif


		//Update collision informations
		CollisionUpdate (_velocity);

		//Reorient velocity along the slope, accelerate down slopes and decelerate up.
		if (collisions.below) {
			_velocity = Vector3.ProjectOnPlane (_velocity, collisions.currentGroundNormal);
			_velocity *= Vector3.Angle (myTransform.forward, Vector3.ProjectOnPlane(myTransform.forward, collisions.currentGroundNormal)) * (Vector3.Dot(myTransform.forward, collisions.currentGroundNormal) > 0 ? 1 : -1)/90 +1;
		}

		//Recursively check if the movement meets obstacles
		_velocity = CollisionDetection (_velocity, myTransform.position + playerAngle * center, new RaycastHit());

		/// Check if calculated movement will end up in a wall, if so cancel movement
		if (!Physics.CheckCapsule (myTransform.position + playerAngle * center + _velocity - capsuleHeightModifier, myTransform.position + playerAngle * center + _velocity + capsuleHeightModifier, radius, collisionMask)) {
			myTransform.Translate (_velocity, Space.World);
			//Debug.Log ("controller : " + _velocity/Time.deltaTime);
			return (Quaternion.AngleAxis (Vector3.Angle (transform.up, Vector3.up), Vector3.Cross (Vector3.up, transform.up))) * _velocity / Time.deltaTime;
		} else {
			return Vector3.zero;
		}
	}


	void CollisionUpdate(Vector3 velocity){
		//Debug.Log ("magnitude : " + velocity.magnitude + " w/ skinwidth : " + (velocity.magnitude + skinWidth));
		RaycastHit hit;
		Quaternion playerAngle = (Quaternion.AngleAxis(Vector3.Angle (Vector3.up, transform.up), Vector3.Cross(Vector3.up, transform.up)));
		//Send casts to check if there's stuff around the player and sets bools depending on the results
		if (myTransform.InverseTransformDirection(velocity).y < 0) {
			collisions.below = Physics.SphereCast (myTransform.position + playerAngle * center - capsuleHeightModifier, radius, -myTransform.up, out hit, skinWidth, collisionMask);
			Debug.DrawRay(myTransform.position + playerAngle * center - capsuleHeightModifier, -myTransform.up * (skinWidth), Color.magenta);
			if (collisions.below) {
				collisions.onSteepSlope = Vector3.Angle (myTransform.up, hit.normal) > maxSlopeAngle;
				collisions.currentGroundNormal = hit.normal;
			}

		} else {
			collisions.above = Physics.SphereCast (myTransform.position + playerAngle * center + capsuleHeightModifier, radius * .9f, myTransform.up, out hit, velocity.magnitude + skinWidth, collisionMask);
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
		if (Physics.CapsuleCast (newOrigin - capsuleHeightModifier, newOrigin + capsuleHeightModifier, radius, velocity, out hit, rayLength, collisionMask)) {
			collisionNumber++;


			//When an obstacle is met, remember the amount of movement needed to get to the obstacle
			movementVector += veloNorm * (hit.distance - ((skinWidth < hit.distance)? skinWidth : hit.distance));

			//Get the remaining velocity after getting to the obstacle
			Vector3 extraVelocity = (velocity - movementVector);

			//Check if this extra velocity isn't too small
			if (extraVelocity.magnitude > .001f) {
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
		Vector3 upPosition = transform.position + playerAngle * (center + (new Vector3 (0, height / 2, 0)));
		Vector3 downPosition = transform.position + playerAngle * (center - (new Vector3 (0, height / 2, 0)));
		Gizmos.DrawWireSphere(upPosition, radius);
		Gizmos.DrawWireSphere(downPosition, radius);
	}

}
