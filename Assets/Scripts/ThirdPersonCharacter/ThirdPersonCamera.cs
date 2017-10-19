using UnityEngine;

[AddComponentMenu("Camera/Third Person Camera")]
[RequireComponent(typeof(Camera))]
public class ThirdPersonCamera : MonoBehaviour {

    public LayerMask blockingLayer;

    [Header("Position")]
    public Transform target;
    public float distance = 10;
    public Vector2 offsetFar = new Vector2(0, 2),
                   offsetClose = new Vector2(2, 0);
    public float defaultPitch = 15;

    [Header("Movement")]
    public Bool3 invertAxis;
    public bool followBehind;
    public Vector2 maxRotationSpeed = new Vector2(15, 15);
    public Vector2 minRotationSpeed = new Vector2(10, 10);
    public Vector2 mouseSpeedLimit = new Vector2(10, 10);
    public MinMax pitchRotationLimit = new MinMax(-20, 80);

    public MinMax distanceBasedOnPitch = new MinMax(1, 12);
    public AnimationCurve distanceFromRotation;

    public MinMax fovBasedOnPitch = new MinMax(60, 75);
    public AnimationCurve fovFromRotation;

    public float rayRadius = .2f;

    public bool smoothMovement = true;
    public float smoothDamp = .1f;
    public float collisionDamp = .1f;
    public float noCollisionDamp = .6f;
    public float resetDamp = .6f;

    [Header("Panorama Mode")]
	public bool enablePanoramaMode = true;
	public float panoramaDistance = 15;
	public float timeToTriggerPanorama = 10;
	public float panoramaDezoomSpeed = 1f;

    [Header("Eclipse")]
    public bool isEclipse = false;

    new Camera camera;
    Vector3 camPosition, negDistance;
    Quaternion camRotation;
    Vector2 input;
    Vector2 rotationSpeed;
    Vector2 offset;
    Transform my;

    float yaw, pitch;
    float maxDistance, currentDistance, idealDistance;
	float deltaTime;
    float targetYaw, targetPitch;
    bool resetting;

    #region MonoBehaviour

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

        input.x = Input.GetAxis("Mouse X") + Input.GetAxis("RightStick X");
        input.y = Input.GetAxis("Mouse Y") + Input.GetAxis("RightStick Y");

        if (input.magnitude != 0)
            resetting = false;

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

        // DEBUG POUR ECLISPE
        if (Input.GetKeyUp(KeyCode.G)) {
            isEclipse ^= true;
            if (isEclipse)
                target.transform.parent.Rotate(0, 0, 90, Space.World);
            else
                target.transform.parent.Rotate(0, 0, -90, Space.World);
        }
        // FIN DEBUG

        target.rotation = Quaternion.Euler(isEclipse ? yaw * Vector3.left + 90 * Vector3.forward : (yaw * Vector3.up) ); // Reoriente the character's rotator
        // change that later

		if (enablePanoramaMode)
			DoPanorama();
    }

    #endregion

    #region Zoom
    [Header("Zoom")]
    public bool canZoom;
    public float zoomSpeed = 5;
    public MinMax zoomDistance = new MinMax(2, 12);

    void Zoom(float value) {
        if (value != 0) idealDistance = zoomDistance.Clamp(currentDistance - value * zoomSpeed);
    }
    #endregion

    #region Panorama Mode
    float panoramaTimer = 0;
    bool inPanorama = false;

    void DoPanorama() {
        if (!Input.anyKey && input.magnitude == 0)
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
    #endregion

    void DoRotation() {
        rotationSpeed.x = Mathf.Lerp(minRotationSpeed.x, maxRotationSpeed.x, currentDistance / maxDistance);
        rotationSpeed.y = Mathf.Lerp(minRotationSpeed.y, maxRotationSpeed.y, currentDistance / maxDistance);

        //print(currentDistance / maxDistance);

        float clampedX = Mathf.Clamp(input.x * (idealDistance / currentDistance), -mouseSpeedLimit.x, mouseSpeedLimit.x); // Avoid going too fast (makes weird lerp)
        if (invertAxis.x) clampedX = -clampedX;
        yaw += clampedX * rotationSpeed.x * deltaTime;

        float clampedY = Mathf.Clamp(input.y * (idealDistance / currentDistance), -mouseSpeedLimit.y, mouseSpeedLimit.y); // Avoid going too fast (makes weird lerp)
        if (invertAxis.y) clampedY = -clampedY;
        pitch -= clampedY * rotationSpeed.y * deltaTime;
        pitch = pitchRotationLimit.Clamp(pitch);

        if (Input.GetButton("ResetCamera")) {
            targetYaw = isEclipse ? -target.parent.eulerAngles.x : target.parent.eulerAngles.y;
            resetting = true;
        }

        if (followBehind) {
            targetYaw = isEclipse ? -target.parent.eulerAngles.x : target.parent.eulerAngles.y;
            float targetPitch = (my.position - target.position).x; // CHANGE WHEN ECLIPSE
            yaw = Mathf.LerpAngle(yaw, targetYaw, deltaTime / resetDamp);
            //pitch = Mathf.LerpAngle(pitch, targetPitch , deltaTime / resetDamp);
        }

        if (resetting) {
            yaw = Mathf.LerpAngle(yaw, targetYaw, deltaTime / resetDamp);
            pitch = Mathf.LerpAngle(pitch, defaultPitch, deltaTime / resetDamp);
            if (Mathf.Abs(yaw - targetYaw) < .1f && Mathf.Abs(pitch - defaultPitch) < .1f/* temp buffer */)
                resetting = false;
        }

        camRotation = Quaternion.Euler(pitch, yaw, 0);
        
        idealDistance = Mathf.Lerp(1, maxDistance, distanceFromRotation.Evaluate(pitchRotationLimit.InverseLerp(pitch))); // prevents Zoom
        //camera.fieldOfView = fovBasedOnPitch.Lerp(fovFromRotation.Evaluate(pitchRotationLimit.InverseLerp(pitch)));

        //Changer la rotation de la caméra pendant l'Éclipse
        if (isEclipse)
            camRotation = Quaternion.AngleAxis(90, Vector3.forward) * camRotation;
    }

    bool blockedByAWall;
    float lastHitDistance;
    void CheckForCollision() {
        Vector3 targetPos = target.position; // Same for the target
        Vector3 startPos = targetPos;

        negDistance.z = -idealDistance;
        Vector3 rayEnd = camRotation * negDistance + (targetPos + my.right * offsetFar.x + my.up * offsetFar.y);
		
		RaycastHit hit;
		blockedByAWall = Physics.SphereCast(startPos, rayRadius, rayEnd - startPos, out hit, idealDistance, blockingLayer);
        Debug.DrawLine(startPos, rayEnd, Color.yellow);
        
		if (blockedByAWall && hit.distance > 0) { // If we hit something, hitDistance cannot be 0, nor higher than idealDistance
			lastHitDistance = Mathf.Min(hit.distance - rayRadius, idealDistance);

            Debug.DrawLine(hit.point - new Vector3(0, .2f, 0), hit.point + new Vector3(0, .2f, 0), Color.red);

            Debug.DrawLine(hit.point - new Vector3(.2f, 0, 0), hit.point + new Vector3(.2f, 0, 0), Color.red);
        }

        float fixedDistance = blockedByAWall ? lastHitDistance : idealDistance;
        
		// If collide, use collisionDamp to quickly get in position and not be blocked by a wall
		// If not colliding, slowly get back to the idealPosition using noCollisionDamp
        currentDistance = Mathf.Lerp(currentDistance, fixedDistance, fixedDistance < currentDistance + .1f /* temp buffer */ ? deltaTime / collisionDamp : deltaTime / noCollisionDamp);
    }

    void SmoothMovement() {
		float t = smoothMovement ? deltaTime / smoothDamp : 1;
        my.position = Vector3.Lerp(my.position, camPosition, t);
        my.rotation = Quaternion.Lerp(my.rotation, camRotation, t);
    }

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
    
}