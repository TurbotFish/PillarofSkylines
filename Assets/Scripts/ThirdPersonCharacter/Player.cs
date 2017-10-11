using UnityEngine;

[RequireComponent(typeof(CharacControllerRecu))]
public class Player : MonoBehaviour {

	public float characSpeed = 5f;

	public float turnSmoothTime = .2f;
	Vector3 turnSmoothVelocity;

	public float speedSmoothTimeGround = .2f;
	public float speedSmoothTimeAir = .14f;
	float speedSmoothVelocity;
	float currentSpeed;

	CharacControllerRecu controller;

	[Header("Jump parameters")]
	public float minJumpHeight = 3f;
	public float maxJumpHeight = 6f;
	public float timeToJumpApex = .4f;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;

	Vector3 velocity;
	bool suddenStop;

	void Start(){
		controller = GetComponent<CharacControllerRecu> ();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
	}


	void Update(){

		float angle = Vector3.Angle (Vector3.up, transform.up);

		Vector3 input = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		Vector3 inputDir = (Quaternion.AngleAxis(angle, Vector3.Cross(Vector3.up, transform.up))) * input.normalized;
		//Debug.Log ("input : " + inputDir);

		//Debug.Log (Camera.main.transform.forward);

		if (input != Vector3.zero) {

			float targetRotation = Mathf.Atan2 (input.x, input.z) * Mathf.Rad2Deg;

			//hard coded turnSmoothTime with value of 0.11
			//prevent ugly rotation when turning back
			//hard coded speedSmoothTime with value of 0.2
			turnSmoothTime = Vector3.Dot (transform.forward, inputDir) < -.75f ? 0.01f : .11f;
		
			if (Vector3.Dot (transform.forward, inputDir) < -.75f) {
				if (!suddenStop) {
					currentSpeed = -currentSpeed;
					speedSmoothTimeGround = .7f;
					suddenStop = true;
				} else {
					speedSmoothTimeGround = Mathf.Clamp (speedSmoothTimeGround - Time.deltaTime, .2f, .7f);
				}
			} else {
				suddenStop = false;
				speedSmoothTimeGround = .2f;
			}

			Vector3 direction = Vector3.SmoothDamp (transform.forward, inputDir, ref turnSmoothVelocity, turnSmoothTime);

			transform.rotation = Quaternion.LookRotation (direction, transform.up);



			//transform.LookAt (transform.position + inputDir, transform.up);

			// c'est soit ça ^ soit ça v  mais un peu les deux.
			/*
			Vector3 rot = transform.InverseTransformVector (transform.eulerAngles);
			rot = Vector3.up * Mathf.SmoothDampAngle (rot.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
			transform.eulerAngles = transform.TransformVector (rot);

			Vector3 forwCamera = Vector3.ProjectOnPlane (Camera.main.transform.forward, transform.up);
			*/
			//transform.RotateAround (transform.up, Mathf.SmoothDampAngle (targetRotation, Vector3.Angle(forwCamera, transform.forward), ref turnSmoothVelocity, turnSmoothTime));

			//transform.RotateAround (transform.up, Vector3.Angle(forwCamera, transform.forward) - targetRotation);
			//Debug.DrawRay(transform.position, transform.up * 10f, Color.red);
			//transform.RotateAround (transform.up, targetRotation);

			/*
			Vector3 forwCamera = Vector3.ProjectOnPlane (Camera.main.transform.forward, transform.up);

			transform.eulerAngles = transform.up * Mathf.SmoothDampAngle (Vector3.Angle(forwCamera, transform.forward), targetRotation, ref turnSmoothVelocity, turnSmoothTime);
			*/


			// vieux v
			/*
			if (!eclipse) {
				transform.eulerAngles = transform.up * Mathf.SmoothDampAngle (transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
			} else {
				transform.eulerAngles = transform.up * Mathf.SmoothDampAngle (transform.eulerAngles.z, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
			}*/
		} else {
			suddenStop = false;
			speedSmoothTimeGround = .2f;
		}


		Vector3 forwCamera = Vector3.ProjectOnPlane (Camera.main.transform.forward, transform.up);
		float targetSpeed = characSpeed * Mathf.Clamp01(input.magnitude);
		currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref speedSmoothVelocity, (controller.collisions.below ? speedSmoothTimeGround : speedSmoothTimeAir));


		velocity.z = Vector3.ProjectOnPlane(transform.forward, Camera.main.transform.right).magnitude * currentSpeed * (Vector3.Dot(transform.forward, forwCamera) > 0 ? 1 : -1);
		//velocity.y = transform.forward.y * currentSpeed;
		velocity.x = Vector3.ProjectOnPlane(transform.forward, forwCamera).magnitude * currentSpeed * (Vector3.Dot(transform.forward, Camera.main.transform.right) > 0 ? 1 : -1);

		if (controller.collisions.below || controller.collisions.above) {
			velocity.y = 0;
		}

		#region Jump controls
		if (Input.GetButtonDown ("Jump") && controller.collisions.below) {
			velocity.y = maxJumpVelocity;
		}

		if (Input.GetButtonUp ("Jump")) {
			if (velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}
		}
		#endregion


		velocity.y += gravity * Time.deltaTime;


		controller.Move ((Quaternion.AngleAxis(angle, Vector3.Cross(Vector3.up, transform.up))) * velocity * Time.deltaTime);
	}

}
