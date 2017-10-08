﻿using UnityEngine;

[AddComponentMenu("Camera/Third Person Camera")]
[RequireComponent(typeof(Camera))]
public class ThirdPersonCamera : MonoBehaviour {
    
    [Header("Position")]
    public Transform target;
    public float distance = 10;
    public Vector2 offsetFar = new Vector2(0, 2),
                   offsetClose = new Vector2(2, 0);

    #region Temporary Offset
        Vector2 _tempOffset;
        bool offsetOverriden;
        public Vector2 temporaryOffset {
            set {
                _tempOffset = value;
                offsetOverriden = true;
            }
        }
    #endregion

    [Header("Movement")]
    public Bool3 invertAxis;
    public Vector2 maxRotationSpeed = new Vector2(15, 15);
    public Vector2 minRotationSpeed = new Vector2(10, 10);
    public Vector2 mouseSpeedLimit = new Vector2(10, 10);
    public MinMax pitchRotationLimit = new MinMax(-20, 80);

    public MinMax distanceBasedOnPitch = new MinMax(1, 12);
    public AnimationCurve distanceFromRotation;

    public MinMax fovBasedOnPitch = new MinMax(60, 75);
    public AnimationCurve fovFromRotation;

    public bool smoothMovement = true;
    public float smoothDamp = .1f; // this name is bad

    #region Zoom
    [Header("Zoom")]
    public bool canZoom;
    public float zoomSpeed = 5;
    public MinMax zoomDistance = new MinMax(2, 12);

    void Zoom(float value) {
        if (value != 0) idealDistance = zoomDistance.Clamp(currentDistance - value * zoomSpeed);
    }
	#endregion

	[Header("Panorama Mode")]
	public bool enablePanoramaMode = true;
	public float panoramaDistance = 15;
	public float timeToTriggerPanorama = 10;
	public float panoramaDezoomSpeed = 1f;

    new Camera camera;
    Vector3 camPosition, negDistance;
    Quaternion camRotation;
    Vector2 rotationSpeed;
    Vector2 offset;
    Transform my;

    float yaw, pitch;
    float maxDistance, currentDistance, idealDistance;
	float deltaTime;

    void Start() {
        camera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;

        my = transform;

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        camPosition = transform.position;

		currentDistance = idealDistance = distance;
        maxDistance = canZoom ? zoomDistance.max : distance;
    }

    void OnApplicationFocus(bool hasFocus) {
        if (!GameState.isPaused)
            Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate() {
        if (!target || GameState.isPaused) return;

		deltaTime = Time.deltaTime;
        DoRotation();

        if (canZoom) Zoom(Input.GetAxis("Mouse ScrollWheel"));
        
        offset.x = Mathf.Lerp(offsetClose.x, offsetFar.x, currentDistance / maxDistance);
        offset.y = Mathf.Lerp(offsetClose.y, offsetFar.y, currentDistance / maxDistance);

        Vector3 targetPosition = target.position;

        if (offsetOverriden) {
            targetPosition.x += _tempOffset.x;
            targetPosition.y += _tempOffset.y;
            _tempOffset = Vector2.Lerp(_tempOffset, Vector2.zero, deltaTime / smoothDamp);
            if (Vector2.Distance(_tempOffset, Vector2.zero) < .01f)
                offsetOverriden = false;
        }

        CheckForCollision();

		negDistance.z = -currentDistance;
		Vector3 targetWithOffset = targetPosition + my.right * offset.x + my.up * offset.y;
		camPosition = camRotation * negDistance + targetWithOffset;
		SmoothMovement();

        target.rotation = Quaternion.Euler(0, yaw, 0); // Reoriente the character's rotator

		if (enablePanoramaMode)
			DoPanorama();
    }

    void DoRotation() {
        rotationSpeed.x = Mathf.Lerp(minRotationSpeed.x, maxRotationSpeed.x, currentDistance / maxDistance);
        rotationSpeed.y = Mathf.Lerp(minRotationSpeed.y, maxRotationSpeed.y, currentDistance / maxDistance);

        float clampedX = Mathf.Clamp(Input.GetAxis("Mouse X") * (idealDistance / currentDistance), -mouseSpeedLimit.x, mouseSpeedLimit.x); // Avoid going too fast (makes weird lerp)
        if (invertAxis.x) clampedX = -clampedX;
        yaw += clampedX * rotationSpeed.x * deltaTime;

        float clampedY = Mathf.Clamp(Input.GetAxis("Mouse Y") * (idealDistance / currentDistance), -mouseSpeedLimit.y, mouseSpeedLimit.y); // Avoid going too fast (makes weird lerp)
        if (invertAxis.y) clampedY = -clampedY;
        pitch -= clampedY * rotationSpeed.y * deltaTime;
        pitch = pitchRotationLimit.Clamp(pitch);
		
		camRotation = Quaternion.Euler(pitch, yaw, 0);


        //idealDistance = Mathf.Lerp(1, maxDistance, distanceFromRotation.Evaluate(pitchRotationLimit.InverseLerp(pitch))); // prevents Zoom
        camera.fieldOfView = fovBasedOnPitch.Lerp(fovFromRotation.Evaluate(pitchRotationLimit.InverseLerp(pitch)));

        //Changer la rotation de la caméra pendant l'Éclipse
        //camRotation = Quaternion.AngleAxis(90, Vector3.forward) * camRotation;
    }

    bool blockedByAWall;
    float sphereRadius = .4f;
    float lastHitDistance;
    void CheckForCollision() {
        negDistance.z = -idealDistance;
        Vector3 targetPos = target.position; // Same for the target
		targetPos.y += offsetClose.y; // Change that during eclipse

        Vector3 rayEnd = camRotation * negDistance + (target.position + my.right * offsetFar.x + my.up * offsetFar.y);

        int layerMask = ~(1 << 10); // ignore Layer #10 : "DontBlockCamera"
		
		RaycastHit hit;
		blockedByAWall = Physics.SphereCast(targetPos, sphereRadius, rayEnd - targetPos, out hit, idealDistance, layerMask);
        Debug.DrawLine(targetPos, rayEnd, Color.yellow);


		if (blockedByAWall && hit.distance > 0) { // If we hit something, hitDistance cannot be 0, nor higher than idealDistance
			lastHitDistance = Mathf.Min(hit.distance, idealDistance);
			//camPosition = hit.point;
		}

        float fixedDistance = blockedByAWall ? lastHitDistance : idealDistance;
        
        currentDistance = Mathf.Lerp(currentDistance, fixedDistance, deltaTime / smoothDamp);
    }

    void SmoothMovement() {
		float t = smoothMovement ? deltaTime / smoothDamp : 1;
        my.position = Vector3.Lerp(my.position, camPosition, t);
        my.rotation = Quaternion.Lerp(my.rotation, camRotation, t);
    }

    public static float ClampAngle(float angle, float min, float max) {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
	
	float panoramaTimer = 0;
	bool inPanorama = false;

	void DoPanorama() {
		if (Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0)
			panoramaTimer += deltaTime;
		else {
			panoramaTimer = 0;
			if (inPanorama) {
				idealDistance = distance;
				inPanorama = false;
			}
		}

		if (panoramaTimer >= timeToTriggerPanorama && idealDistance <= panoramaDistance) {
			idealDistance += deltaTime * panoramaDezoomSpeed;
			inPanorama = true;
		}
	}
}