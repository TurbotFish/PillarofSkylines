﻿using UnityEngine;

[AddComponentMenu("Camera/Third Person Camera")]
[RequireComponent(typeof(Camera))]
public class PoS_Camera : MonoBehaviour {

	#region Properties
	public LayerMask blockingLayer;

	public eCameraState state;
	public enum eCameraState {
		Default, Idle, PlayerControl,
		Resetting, AroundCorner, FollowBehind,
		Slope, Air
	}

	[Header("Position")]
	public Transform target;
	public float distance = 10;
	public Vector2 offsetFar = new Vector2(0, 2),
	offsetClose = new Vector2(2, 0);
	public float defaultPitch = 15;

	[Header("Control")]
	public Bool3 invertAxis;
	public Vector2 minRotationSpeed = new Vector2(10, 10);
	public Vector2 maxRotationSpeed = new Vector2(100, 100);
	public Vector2 mouseSpeedLimit = new Vector2(5, 5);
	public MinMax pitchRotationLimit = new MinMax(-70, 80);

	public bool smoothMovement = true;
	public float smoothDamp = .1f;
	public float resetDamp = .3f;

	[Header("Behaviour")]
	public bool followBehind;
	public float timeBeforeAutoReset = 5;
	public float autoResetDamp = 1;

	public float recoilOnImpact = 20;

	public MinMax distanceBasedOnPitch = new MinMax(1, 12);
	public AnimationCurve distanceFromRotation;

	public MinMax fovBasedOnPitch = new MinMax(60, 75);
	public AnimationCurve fovFromRotation;

	public float cliffEdgeMaxWidth = 2;
	public float cliffMinDepth = 5;

	[Header("Collision")]
	public float rayRadius = .2f;
	public float dampWhenColliding = .1f;
	public float dampAfterColliding = .6f;
	public float slideDamp = 1f;

	[Header("Panorama Mode")]
	public bool enablePanoramaMode = true;
	public float panoramaDistance = 15;
	public float timeToTriggerPanorama = 10;
	public float panoramaDezoomSpeed = 1f;

	[Header("Eclipse")]
	public bool isEclipse = false;

	new Camera camera;
	Vector3 camPosition, negDistance;
	Vector3 playerVelocity;
	Quaternion camRotation, targetSpace;
	Vector2 input;
	Vector2 rotationSpeed;
	Vector2 offset;
	Player player;
	CharacControllerRecu controller;
	Transform my;

	ePlayerState playerState;

	float yaw, pitch;
	float maxDistance, currentDistance, idealDistance;
	float additionalDistance;
	float deltaTime;
	float lastInput;
	float autoDamp;
	float manualPitch, manualYaw;
    float recoilIntensity;
	float slopeValue;
	bool resetting, autoAdjustYaw, autoAdjustPitch;
    bool canAutoReset;
	bool placedByPlayer, onEdgeOfCliff;

	#endregion

	#region MonoBehaviour

	void Start() {
		camera = GetComponent<Camera>();
		Cursor.lockState = CursorLockMode.Locked;

		my = transform;
		player = target.GetComponentInParent<Player>();
		controller = player.GetComponent<CharacControllerRecu>();
        
		currentDistance = zoomValue = idealDistance = distance;
		maxDistance = canZoom ? zoomDistance.max : distance;

		manualPitch = defaultPitch;
		state = eCameraState.Default;

        PlaceBehindPlayerNoLerp();
    }

    private void OnEnable() {
        Game.Utilities.EventManager.OnPlayerSpawnedEvent += OnPlayerSpawn;
    }
    private void OnDisable() {
        Game.Utilities.EventManager.OnPlayerSpawnedEvent -= OnPlayerSpawn;
    }

    void OnApplicationFocus(bool hasFocus) {
		if (!GameState.isPaused)
			Cursor.lockState = CursorLockMode.Locked;
	}

	void LateUpdate() {
		deltaTime = Time.deltaTime;
		GetInputsAndStates();
		DoRotation();

		float distanceFromAngle = Mathf.Lerp(0, 1, distanceFromRotation.Evaluate(pitchRotationLimit.InverseLerp(pitch)));
		idealDistance = 1 + zoomValue * distanceFromAngle + additionalDistance;

		camera.fieldOfView = fovBasedOnPitch.Lerp(fovFromRotation.Evaluate(pitchRotationLimit.InverseLerp(pitch)));

		if (canZoom) Zoom(Input.GetAxis("Mouse ScrollWheel"));

		offset.x = Mathf.Lerp(offsetClose.x, offsetFar.x, currentDistance / maxDistance);
		offset.y = Mathf.Lerp(offsetClose.y, offsetFar.y, currentDistance / maxDistance);

		Vector3 targetPos = target.position;

		CheckForCollision(targetPos);

		negDistance.z = -currentDistance;
		Vector3 targetWithOffset = targetPos + my.right * offset.x + my.up * offset.y;

		{ // Contextual Offset
			if (cameraBounce) {
				targetWithOffset += contextualOffset.y * target.up * recoilIntensity;
				contextualOffset.y = Mathf.Lerp(contextualOffset.y, 0, deltaTime / smoothDamp);
				if (Mathf.Abs(contextualOffset.y - 0) < .01f)
					cameraBounce = false;
                // use impactFromSpeed (animCurve) to attenuate the impact on low speed
                // impactFromSpeed on recoilOnImpact
            }
            if (onEdgeOfCliff)
				contextualOffset = Vector3.Lerp(contextualOffset, player.transform.forward * (Mathf.Max(0, pitch) / 20), deltaTime / autoResetDamp);
			else
				contextualOffset = Vector3.Lerp(contextualOffset, Vector3.zero, deltaTime / autoResetDamp);
			targetWithOffset += contextualOffset;
		}

		camPosition = camRotation * negDistance + targetWithOffset;

		SmoothMovement();

		// Reoriente the character's rotator
		Vector3 characterUp = target.parent.up;
		target.LookAt(targetPos + Vector3.ProjectOnPlane(my.forward, characterUp), characterUp);

		if (enablePanoramaMode)
			DoPanorama();
	}

	#endregion

	#region Zoom
	[Header("Zoom")]
	public bool canZoom;
	public float zoomSpeed = 5;
	public MinMax zoomDistance = new MinMax(2, 12);

	float zoomValue;

	void Zoom(float value) {
		if (value != 0) zoomValue = zoomDistance.Clamp(currentDistance - value * zoomSpeed);
	}

	/// <summary>
	/// Zoom in a (0,1) value between minimum and maximum distance
	/// </summary>
	public void ZoomAt(float value) {
		zoomValue = value;
	}

	public void ResetZoom() {
		zoomValue = distance;
	}
	#endregion

	#region Panorama Mode
	float panoramaTimer = 0;
	bool inPanorama = false;

	void DoPanorama() {
		if (!Input.anyKey && input.magnitude == 0 && playerVelocity == Vector3.zero)
			panoramaTimer += deltaTime;
		else {
			panoramaTimer = 0;
			if (inPanorama) {
				additionalDistance = 0;
				inPanorama = false;
			}
		}
		if (panoramaTimer >= timeToTriggerPanorama && currentDistance <= panoramaDistance) {
			additionalDistance += deltaTime * panoramaDezoomSpeed;
			inPanorama = true;
		}
	}
	#endregion

    void OnPlayerSpawn(object sender, Game.Utilities.EventManager.OnPlayerSpawnedEventArgs args) {
        PlaceBehindPlayerNoLerp();
    }

    void PlaceBehindPlayerNoLerp() {
        currentDistance = distance;
        yaw = GetYawBehindPlayer();
        pitch = defaultPitch;
        camRotation = my.rotation = targetSpace * Quaternion.Euler(pitch, yaw, 0);
        negDistance.z = -currentDistance;
        Vector3 targetWithOffset = target.position + my.right * offset.x + my.up * offset.y;
        camPosition = my.position = my.rotation * negDistance + targetWithOffset;
    }

    void GetInputsAndStates() {
		input.x = Input.GetAxis("Mouse X") + Input.GetAxis("RightStick X");
		input.y = Input.GetAxis("Mouse Y") + Input.GetAxis("RightStick Y");

		playerState = player.currentPlayerState;
		playerVelocity = player.velocity;
        
		targetSpace = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, target.up), Vector3.Cross(Vector3.up, target.up));

		Vector3 groundNormal = controller.collisions.currentGroundNormal;
        slopeValue = Vector3.Dot(target.forward, groundNormal) * 60;

		if (input.magnitude != 0) {
			state = eCameraState.PlayerControl;
			resetting = false;
			manualPitch = pitch - slopeValue;
			manualYaw = yaw;
			autoAdjustPitch = false;
			autoAdjustYaw = false;
			autoDamp = smoothDamp;
			lastInput = Time.time;
            canAutoReset = true;

		} else if (state != eCameraState.Resetting) {
            if (canAutoReset && Time.time > lastInput + timeBeforeAutoReset && !onEdgeOfCliff) { // Si ça fait genre 5 secondes qu'on n'a pas touché à la caméra
                state = eCameraState.Resetting;
                SetTargetRotation(defaultPitch - slopeValue, GetYawBehindPlayer(), autoResetDamp);
                manualPitch = defaultPitch - slopeValue;
                canAutoReset = false;

            } else if (playerState == ePlayerState.onGround) {

				// POUR LES BORDS
				onEdgeOfCliff = !Physics.Raycast(target.position, player.transform.forward, 1, blockingLayer) 
					         && !Physics.Raycast(target.position + player.transform.forward * cliffEdgeMaxWidth, 
                                -target.up, cliffMinDepth, blockingLayer);

				if (playerVelocity != Vector3.zero) {
					state = eCameraState.Default;
                    SetTargetRotation(slopeValue + manualPitch, null, resetDamp);

				} else {
					state = eCameraState.Idle;
                    SetTargetRotation(slopeValue + manualPitch, null, resetDamp);
                }
			} else {
				state = eCameraState.Air;
				autoAdjustPitch = false;
				autoAdjustYaw = false;
			}
		}

		if (playerState == ePlayerState.gliding) {
            SetTargetRotation(-2 * playerVelocity.y + defaultPitch, GetYawBehindPlayer(), resetDamp);
			state = eCameraState.FollowBehind;
		}

		if (Input.GetButton("ResetCamera")) {
			manualPitch = defaultPitch;
			manualYaw = targetYaw;
            lastInput = Time.time;
            canAutoReset = true;
            
            SetTargetRotation(defaultPitch, GetYawBehindPlayer(), resetDamp);
			state = eCameraState.Resetting;
		}
	}

	void DoRotation() {
		rotationSpeed.x = Mathf.Lerp(minRotationSpeed.x, maxRotationSpeed.x, currentDistance / maxDistance);
		rotationSpeed.y = Mathf.Lerp(minRotationSpeed.y, maxRotationSpeed.y, currentDistance / maxDistance);

		float clampedX = Mathf.Clamp(input.x * (idealDistance / currentDistance), -mouseSpeedLimit.x, mouseSpeedLimit.x); // Avoid going too fast (makes weird lerp)
		if (invertAxis.x) clampedX = -clampedX;
		yaw += clampedX * rotationSpeed.x * deltaTime;

		float clampedY = Mathf.Clamp(input.y * (idealDistance / currentDistance), -mouseSpeedLimit.y, mouseSpeedLimit.y); // Avoid going too fast (makes weird lerp)
		if (invertAxis.y) clampedY = -clampedY;
		pitch -= clampedY * rotationSpeed.y * deltaTime;
		pitch = pitchRotationLimit.Clamp(pitch);

		if (state != eCameraState.PlayerControl)
			AutomatedMovement();

		camRotation = targetSpace * Quaternion.Euler(pitch, yaw, 0);
	}

    float targetYaw, targetPitch;
    void SetTargetRotation(float? newTargetPitch, float? newTargetYaw, float damp) {
        autoAdjustPitch = newTargetPitch != null;
        autoAdjustYaw = newTargetYaw != null;
        targetPitch = newTargetPitch ?? targetPitch;
        targetYaw = newTargetYaw ?? targetYaw;
        autoDamp = damp;
    }

	void AutomatedMovement() { // place camera behind
        if (autoAdjustPitch)
            pitch = Mathf.LerpAngle(pitch, targetPitch, deltaTime / autoDamp);
        if (autoAdjustYaw)
			yaw = Mathf.LerpAngle(yaw, targetYaw, deltaTime / autoDamp);

		if (state == eCameraState.Resetting) {
			if ( ((autoAdjustYaw   && Mathf.Abs(Mathf.DeltaAngle(yaw,   targetYaw))   < 1f) || !autoAdjustYaw)
				&& ((autoAdjustPitch && Mathf.Abs(Mathf.DeltaAngle(pitch, targetPitch)) < 1f) || !autoAdjustPitch) ) {
				state = eCameraState.Default;
			}
		}
	}

	bool blockedByAWall;
	float lastHitDistance;
	void CheckForCollision(Vector3 targetPos) {
		Vector3 startPos = targetPos;

		negDistance.z = -idealDistance;
		Vector3 rayEnd = camRotation * negDistance + (targetPos + my.right * offsetFar.x + my.up * offsetFar.y);

		RaycastHit hit;
		blockedByAWall = Physics.SphereCast(startPos, rayRadius, rayEnd - startPos, out hit, idealDistance, blockingLayer);
		Debug.DrawLine(startPos, rayEnd, Color.yellow);

		if (blockedByAWall && hit.distance > 0) {// If we hit something, hitDistance cannot be 0, nor higher than idealDistance
			lastHitDistance = Mathf.Min(hit.distance - rayRadius, idealDistance);

			if (state != eCameraState.PlayerControl && currentDistance >= hit.distance) { // auto move around corners
				float targetYaw = GetYawBehindPlayer();
				float newYaw = Mathf.LerpAngle(yaw, targetYaw, deltaTime / slideDamp);
				float delta = Mathf.Abs(Mathf.DeltaAngle(newYaw, targetYaw));

				Debug.DrawLine(hit.point, hit.point + hit.normal, Color.magenta);

				Vector3 centerOfSphereCast = startPos + (rayEnd - startPos).normalized * hit.distance;
				Vector3 hitDirection = (hit.point - centerOfSphereCast).normalized;

				if (delta > 1 && delta < 100) {
					targetYaw = GetYawBehindPlayer();
					state = eCameraState.AroundCorner;
					autoDamp = slideDamp;

					//yaw = newYaw;
					//camRotation = targetSpace * Quaternion.Euler(pitch, yaw, 0);
					//CheckForCollision(targetPos);
					//print("recursion at frame: " + Time.frameCount);
				}
			}

		}
		float fixedDistance = blockedByAWall ? lastHitDistance : idealDistance;

		// If collide, use collisionDamp to quickly get in position and not be blocked by a wall
		// If not colliding, slowly get back to the idealPosition using noCollisionDamp
		currentDistance = Mathf.Lerp(currentDistance, fixedDistance, fixedDistance < currentDistance + .1f ? deltaTime / dampWhenColliding : deltaTime / dampAfterColliding);
	}

	void SmoothMovement() {
		float t = smoothMovement ? deltaTime / smoothDamp : 1;
		my.position = Vector3.Lerp(my.position, camPosition, t);
		my.rotation = Quaternion.Lerp(my.rotation, camRotation, t);
	}

	float GetYawBehindPlayer() {
		return SignedAngle(targetSpace * Vector3.forward, target.parent.rotation * Vector3.forward, target.up);
	}
	public float SignedAngle(Vector3 v1, Vector3 v2, Vector3 n) {
		return Mathf.Atan2(
			Vector3.Dot(n, Vector3.Cross(v1, v2)),
			Vector3.Dot(v1, v2)) * 57.29578f; // Radian to Degrees constant
	}

	#region Temporary Offset
	Vector3 contextualOffset;
	bool cameraBounce;

	public void SetVerticalOffset(float verticalOffset) {
        recoilIntensity = recoilOnImpact * verticalOffset;
        contextualOffset.y = -verticalOffset;
		cameraBounce = true;
	}
	#endregion

}