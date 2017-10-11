using UnityEngine;

public class CharacControllerRecu : MonoBehaviour {


	float radius;

	float maxCharacSpeed;
	Player player;


	public CollisionInfo collisions;
	public float skinWidth = 0.02f;
	public float maxAngleBeforeFall = 45f;
	public float maxSlopeAngle = 45f;

	public LayerMask collisionMask;

	Vector3 capsuleHeightModifier;

	int collisionNumber;

	void Start(){
		radius = transform.localScale.x / 2f;
		player = GetComponent<Player> ();
		maxCharacSpeed = player.characSpeed;
	}

	public void Move(Vector3 _velocity){
		capsuleHeightModifier = transform.up * .5f;
		collisionNumber = 0;
		collisions.Reset ();
		Debug.DrawRay (transform.position, _velocity*10, Color.green);
		CollisionUpdate (_velocity);
		_velocity = CollisionDetection (_velocity, transform.position, new RaycastHit());

		/// Check if calculated movement will end up in a wall
		/*foreach (Collider coll in Physics.OverlapCapsule (transform.position + _velocity - capsuleHeightModifier, transform.position + _velocity + capsuleHeightModifier, radius)) {
			Debug.Log ("mur");
			Vector3 pointOnCollider = coll.ClosestPoint (transform.position + _velocity);
			
		}*/
		if (!Physics.CheckCapsule (transform.position + _velocity - capsuleHeightModifier, transform.position + _velocity + capsuleHeightModifier, radius, collisionMask)) {
			transform.Translate (_velocity, Space.World);
		}
	}

	void CollisionUpdate(Vector3 velocity){
		RaycastHit hit;
		if (transform.InverseTransformDirection(velocity).y < 0) {
			collisions.below = Physics.SphereCast (transform.position - capsuleHeightModifier, radius, -transform.up, out hit, velocity.magnitude + skinWidth, collisionMask);
		} else {
			collisions.above = Physics.SphereCast (transform.position + capsuleHeightModifier, radius * .9f, transform.up, out hit, velocity.magnitude + skinWidth, collisionMask);
		}
		collisions.side = Physics.SphereCast (transform.position, radius, Vector3.ProjectOnPlane(velocity, transform.up), out hit, velocity.magnitude + skinWidth, collisionMask);
		Debug.DrawRay (transform.position, Vector3.ProjectOnPlane(velocity, transform.up) *10, Color.cyan);
	}

	Vector3 CollisionDetection(Vector3 velocity, Vector3 position, RaycastHit oldHit){
		Vector3 movementVector = Vector3.zero;
		RaycastHit hit;
		Vector3 veloNorm = velocity.normalized;
		float rayLength = velocity.magnitude;
		Vector3 newOrigin = position;

		if (Physics.CapsuleCast (newOrigin - capsuleHeightModifier, newOrigin + capsuleHeightModifier, radius, velocity, out hit, rayLength, collisionMask)) {
			collisionNumber++;
			movementVector += veloNorm * (hit.distance - ((skinWidth < hit.distance)? skinWidth : hit.distance));

			Debug.DrawRay (position, movementVector*5, Color.red);
			Vector3 extraVelocity = (velocity - movementVector);
			if (extraVelocity.magnitude > .001f) {
				Vector3 reflection = new Vector3();
				if (collisionNumber > 1) {
					reflection = CollisionDetection (Vector3.Project(extraVelocity, Vector3.Cross(hit.normal, oldHit.normal)), position + movementVector, hit);
				} else {
					reflection = CollisionDetection (Vector3.ProjectOnPlane(extraVelocity, hit.normal), position + movementVector, hit);
				}
				movementVector += reflection;
			}
		} else {
			movementVector += velocity  - (veloNorm * ((skinWidth < velocity.magnitude)?skinWidth:velocity.magnitude));
			Debug.DrawRay (position, movementVector*5, Color.red);
		}
		//Debug.Log ("nb of collisions : " + collisionNumber);
		return movementVector;
	}




	/*

	void HorizontalCollisions(ref Vector3 _velocity, Transform originTransform){
		RaycastHit hit;
		Vector3 velocityXZ = new Vector3 (_velocity.x, 0, _velocity.z);
		Vector3 XZnorm = velocityXZ.normalized;
		float rayLength = velocityXZ.magnitude + skinWidth;
		Vector3 newOrigin = originTransform.position - (XZnorm * skinWidth);

		if (Physics.SphereCast (newOrigin, radius, velocityXZ, out hit, rayLength)) {
			Vector3 hitVec = XZnorm * (hit.distance - skinWidth);
			float extraVelocity = rayLength - hit.distance;
			//Debug.DrawRay (hit.point, hit.normal * 10f, Color.green);
			if (hit.point.y < originTransform.position.y) {
				Vector3 stepClimbVelocity = ClimbingSteps (hit.normal, XZnorm * extraVelocity, hit.point);
				_velocity = stepClimbVelocity + hitVec;
				//Debug.Log ("IHIHIH");
			} else {

				Vector3 slideVelocity = SlidingAgainstWalls (hit.normal, XZnorm * extraVelocity, originTransform, hitVec, hit.transform.name);
				_velocity.x = slideVelocity.x + hitVec.x;
				_velocity.z = slideVelocity.z + hitVec.z;
			}
		}
	}

	Vector3 SlidingAgainstWalls(Vector3 normal, Vector3 _velocity, Transform originTransform, Vector3 vecToWall, string objectName){
		Vector3 newVelocity = (_velocity + Vector3.Reflect (_velocity, normal)) * .5f;
		Vector3 newOrigin = (originTransform.position + vecToWall - (newVelocity.normalized * skinWidth));

		float newRayLength = newVelocity.magnitude + skinWidth;
		RaycastHit hit;
		if (Physics.SphereCast (newOrigin, radius, newVelocity, out hit, newRayLength)) {

			float radiusReductionCoeff = 1f;
			RaycastHit savedHit = hit;

			if (Vector3.Angle (hit.normal, normal) < 0.2f) {

				RaycastHit[] hits = Physics.SphereCastAll (newOrigin, radius*.99f, newVelocity, newRayLength);
				if (hits.Length > 1) {
					hit = hits [1];
					Debug.Log (hits[1].transform.name);
				}


			} else {
				//Debug.Log ("oups  " + Vector3.Angle (hit.normal, normal));

			}

			float remainingLength = newRayLength - hit.distance;
			//Debug.DrawRay (hit.point, hit.normal * 10f, Color.yellow);

			//Debug.Log ("first " + objectName + "second " + hit.transform.name);
			//newVelocity = radiusReductionCoeff == 1 ? newVelocity.normalized * (hit.distance - skinWidth) : newVelocity.normalized * (hit.distance - (.25f * radius) - skinWidth);
			newVelocity = newVelocity.normalized * (hit.distance - skinWidth);


			//Debug.DrawRay (newOrigin, newVelocity * 10f, Color.black);



			float angle1 = Vector3.Angle (_velocity, -normal);
			float angle2 = Vector3.Angle (-normal, -hit.normal);

			if (angle1 > angle2) {
				Vector3 tempVec = (_velocity.normalized * remainingLength + Vector3.Reflect (_velocity.normalized * remainingLength, hit.normal)) * .5f;
				newVelocity += tempVec;
			}

		}
		return newVelocity;
	}

	Vector3 ClimbingSteps(Vector3 normal, Vector3 _velocity, Vector3 hitPoint){
		Vector3 projectedNormal = Vector3.ProjectOnPlane (normal, transform.right).normalized;
		float stepSpeedCoeff = Mathf.InverseLerp (0, maxCharacSpeed, _velocity.magnitude / Time.deltaTime);
		Vector3 newVelocity = (_velocity + Vector3.Reflect (_velocity, projectedNormal)) * (.85f + stepSpeedCoeff * .15f);
		return newVelocity;
	}

	void DrawDebugSphere(Vector3 position){
		GameObject debugSphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		debugSphere.transform.localScale = Vector3.one * .02f;
		debugSphere.GetComponent<SphereCollider> ().enabled = false;
		debugSphere.transform.position = position;
		Destroy (debugSphere, 1f);
	}

	void VerticalCollisions(ref Vector3 _velocity){
		RaycastHit hit;
		float rayLength = Mathf.Abs(_velocity.y) + skinWidth;

		Vector3 newOrigin = _velocity.y < 0 ? new Vector3 (transform.position.x + _velocity.x, transform.position.y + skinWidth, transform.position.z + _velocity.z)
			: new Vector3 (headTransform.position.x + _velocity.x, headTransform.position.y - skinWidth, headTransform.position.z + _velocity.z);

		Vector3 rayDir = _velocity.y < 0 ? -transform.up : transform.up;




		if (Physics.SphereCast (newOrigin, radius, rayDir, out hit, rayLength)) {



			if (_velocity.y <= 0) {
				collisions.below = true;
			} else {
				collisions.above = true;
			}
			_velocity.y = (hit.distance - skinWidth) * Mathf.Sign (_velocity.y);
		}
	}

*/



	public struct CollisionInfo{
		public bool above, below;
		public bool side;

		public void Reset(){
			above = below = false;
			side = false;
		}

	}

}
