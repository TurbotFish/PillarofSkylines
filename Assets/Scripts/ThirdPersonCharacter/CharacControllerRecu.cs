using UnityEngine;

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

	//To integrate
	public float maxAngleBeforeFall = 45f;
	public float maxSlopeAngle = 45f;

	void Start(){
		myTransform = transform;
	}


	public void Move(Vector3 _velocity){
		//Set the vector between the points of the capsule on this frame.
		capsuleHeightModifier = myTransform.up * height/2;

		//resets collisions
		collisionNumber = 0;
		collisions.Reset ();

		#if UNITY_EDITOR
		Debug.DrawRay (myTransform.position + center, _velocity*10, Color.green);
		#endif

		//Update collision informations
		CollisionUpdate (_velocity);

		//Recursively check if the movement meets obstacles
		_velocity = CollisionDetection (_velocity, myTransform.position + center, new RaycastHit());

		/// Check if calculated movement will end up in a wall, if so cancel movement
		if (!Physics.CheckCapsule (myTransform.position + center + _velocity - capsuleHeightModifier, myTransform.position + center + _velocity + capsuleHeightModifier, radius, collisionMask)) {
			myTransform.Translate (_velocity, Space.World);
		}
	}


	void CollisionUpdate(Vector3 velocity){
		RaycastHit hit;
		//Send casts to check if there's stuff around the player and sets bools depending on the results
		if (myTransform.InverseTransformDirection(velocity).y < 0) {
			collisions.below = Physics.SphereCast (myTransform.position + center - capsuleHeightModifier, radius, -myTransform.up, out hit, velocity.magnitude + skinWidth, collisionMask);
		} else {
			collisions.above = Physics.SphereCast (myTransform.position + center + capsuleHeightModifier, radius * .9f, myTransform.up, out hit, velocity.magnitude + skinWidth, collisionMask);
		}
		collisions.side = Physics.SphereCast (myTransform.position + center, radius, Vector3.ProjectOnPlane(velocity, myTransform.up), out hit, velocity.magnitude + skinWidth, collisionMask);
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
					reflection = CollisionDetection (Vector3.Project(extraVelocity, Vector3.Cross(hit.normal, oldHit.normal)), position + movementVector, hit);
				} else {
					reflection = CollisionDetection (Vector3.ProjectOnPlane(extraVelocity, hit.normal), position + movementVector, hit);
				}
				//Add all the reflections calculated together
				movementVector += reflection;
			}
		} else {
			//if no obstacle is met, add the reamining velocity to the movement vector
			movementVector += velocity - (veloNorm * ((skinWidth < velocity.magnitude)?skinWidth:velocity.magnitude));
		}
		//return the movement vector calculated
		return movementVector;
	}


	/// <summary>
	/// The struct containing three bools describing collision informations.
	/// </summary>
	public struct CollisionInfo{
		public bool above, below;
		public bool side;

		public void Reset(){
			above = below = false;
			side = false;
		}

	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1, 0, 1, 0.75F);
		Vector3 upPosition = transform.position + center + new Vector3 (0, height / 2, 0);
		Vector3 downPosition = transform.position + center - new Vector3 (0, height / 2, 0);
		Gizmos.DrawWireSphere(upPosition, radius);
		Gizmos.DrawWireSphere(downPosition, radius);
	}

}
