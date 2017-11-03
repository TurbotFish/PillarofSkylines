using UnityEngine;

[AddComponentMenu("Camera/Third Person Camera")]
[RequireComponent(typeof(Camera))]
public class PoS_Camera : MonoBehaviour {

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
    
    public bool smoothMovement = true;
    public float smoothDamp = .1f;
    public float resetDamp = .6f;

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

    [HideInInspector]
    CameraState cameraState;
    enum CameraState { }

    ResetMode resetMode;
    enum ResetMode {
        Manual, Angle, FollowBehind
    }

    new Camera camera;
    Vector3 camPosition, negDistance;
    Quaternion camRotation, targetSpace;
    Vector2 input;
    Vector2 rotationSpeed;
    Vector2 offset;
    Transform my, characterModel;

    float yaw, pitch;
    float maxDistance, currentDistance, idealDistance;
    float additionalDistance;
	float deltaTime;
    float autoDamp;
    bool resetting;
    bool placedByPlayer;
    
    #region MonoBehaviour

    void Start() {
        camera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;

        my = transform;
        characterModel = target.parent.GetComponentInChildren<Animator>().transform;

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        camPosition = transform.position;

        currentDistance = zoomValue = idealDistance = distance;
        maxDistance = canZoom ? zoomDistance.max : distance;
    }

    void OnApplicationFocus(bool hasFocus) {
        if (!GameState.isPaused)
            Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate() {
        if (!target || GameState.isPaused) return;

        deltaTime = Time.deltaTime;
        GetInputs();
        DoRotation();
        
        float distanceFromAngle = Mathf.Lerp(0, 1, distanceFromRotation.Evaluate(pitchRotationLimit.InverseLerp(pitch)));
        idealDistance = 1 + zoomValue * distanceFromAngle + additionalDistance;

        camera.fieldOfView = fovBasedOnPitch.Lerp(fovFromRotation.Evaluate(pitchRotationLimit.InverseLerp(pitch)));

        if (canZoom) Zoom(Input.GetAxis("Mouse ScrollWheel"));
        
        offset.x = Mathf.Lerp(offsetClose.x, offsetFar.x, currentDistance / maxDistance);
        offset.y = Mathf.Lerp(offsetClose.y, offsetFar.y, currentDistance / maxDistance);

        Vector3 targetPos = target.position;

        if (offsetOverriden) { // temporary camera offset
            //targetPos.x += _tempOffset.x;
            targetPos.y += _tempOffset.y;
            _tempOffset = Vector2.Lerp(_tempOffset, Vector2.zero, deltaTime / smoothDamp);
            if (Vector2.Distance(_tempOffset, Vector2.zero) < .01f)
                offsetOverriden = false;
        }

        CheckForCollision(targetPos);

		negDistance.z = -currentDistance;
		Vector3 targetWithOffset = targetPos + my.right * offset.x + my.up * offset.y;
		camPosition = camRotation * negDistance + targetWithOffset;

		SmoothMovement();

        #if UNITY_EDITOR
            DoDebug(); // TO DELETE
        #endif

        targetSpace = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, target.up), Vector3.Cross(Vector3.up, target.up));
        
        // Reoriente the character's rotator
        Vector3 characterUp = target.parent.up;
        target.LookAt(targetPos + Vector3.ProjectOnPlane(my.forward, characterUp), characterUp);
        
        if (enablePanoramaMode)
			DoPanorama();
    }

    #endregion

    void DoDebug() {
        // DEBUG POUR ECLISPE
        if (Input.GetKeyUp(KeyCode.G)) {
            isEclipse ^= true;
            if (isEclipse)
                target.parent.Rotate(0, 0, 90, Space.World);
            else
                target.parent.Rotate(0, 0, -90, Space.World);
        }
        // DEBUG POUR FOLLOWBEHIND
        if (Input.GetButtonUp("Back")) {
            followBehind ^= true;
        }
    }

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
        if (!Input.anyKey && input.magnitude == 0 && Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
            panoramaTimer += deltaTime;
        else {
            panoramaTimer = 0;
            if (inPanorama) {
                additionalDistance = 0;
                inPanorama = false;
            }
        }

        //print(panoramaTimer + " & " + timeToTriggerPanorama + " | " + idealDistance + " < " + panoramaDistance);
        if (panoramaTimer >= timeToTriggerPanorama && currentDistance <= panoramaDistance) {
            additionalDistance += deltaTime * panoramaDezoomSpeed;
            inPanorama = true;
        }
    }
    #endregion

    void GetInputs() {
        input.x = Input.GetAxis("Mouse X") + Input.GetAxis("RightStick X");
        input.y = Input.GetAxis("Mouse Y") + Input.GetAxis("RightStick Y");
        
        if (input.magnitude != 0)
            placedByPlayer = true;

        if (input.magnitude == 0 && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
            placedByPlayer = false;
        
        if (input.magnitude != 0 // if we have manual control, stop automated movement
            || (resetMode == ResetMode.Manual && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)))
            resetting = false;
        
        if (Input.GetButton("ResetCamera"))
            GoBehindPlayer(ResetMode.Manual, resetDamp);
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

        if (followBehind || resetting)
            Resetting();

        camRotation = targetSpace * Quaternion.Euler(pitch, yaw, 0);
    }

    void GoBehindPlayer(ResetMode mode, float damp) {
        resetting = true;
        resetMode = mode;
        autoDamp = damp;
    }

    void Resetting() { // place camera behind
        float targetYaw = GetYawBehindPlayer();
        float targetPitch = characterModel.localEulerAngles.x;

        yaw = Mathf.LerpAngle(yaw, targetYaw, deltaTime / autoDamp);
        pitch = Mathf.LerpAngle(pitch, targetPitch + defaultPitch, deltaTime / autoDamp);

        if (resetMode != ResetMode.FollowBehind) {
            if (Mathf.Abs(Mathf.DeltaAngle(yaw, targetYaw)) < 1f && Mathf.Abs(Mathf.DeltaAngle(pitch, defaultPitch)) < 1f)
                resetting = false; // si on n'est pas trop loin on arrête de reset
        }
    }

    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n) {
        return Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * 57.29578f; // Radian to Degrees constant
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

            if (input.magnitude == 0 && !placedByPlayer && currentDistance >= hit.distance) { // auto move around corners
                float targetYaw = GetYawBehindPlayer();
                float newYaw = Mathf.LerpAngle(yaw, targetYaw, deltaTime / slideDamp);
                float delta = Mathf.Abs(Mathf.DeltaAngle(newYaw, targetYaw));

                Debug.DrawLine(hit.point, hit.point + hit.normal, Color.magenta);

                Vector3 centerOfSphereCast = startPos + (rayEnd - startPos).normalized * hit.distance;
                Vector3 hitDirection = (hit.point - centerOfSphereCast).normalized;

                print("la direction: " + hitDirection);

                if (delta > 1 && delta < 100) {

                    GoBehindPlayer(ResetMode.Angle, slideDamp);
                    
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
        return AngleSigned(targetSpace * Vector3.forward, target.parent.rotation * Vector3.forward, target.up);
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