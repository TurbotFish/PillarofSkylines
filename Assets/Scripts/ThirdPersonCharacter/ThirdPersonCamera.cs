using UnityEngine;

[AddComponentMenu("Camera/Third Person Camera")]
[RequireComponent(typeof(Camera))]
public class ThirdPersonCamera : MonoBehaviour {
    
    [Header("Position")]
    public Transform target;
    public float distance = 12;
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
        if (value != 0) idealDistance = zoomDistance.Clamp(distance - value * zoomSpeed);
    }
    #endregion

    new Camera camera;
    Vector3 camPosition, negDistance;
    Quaternion camRotation;
    Vector2 rotationSpeed;
    Vector2 offset;
    Transform my;

    float yaw, pitch;
    float maxDistance, idealDistance;
    
    void Start() {
        camera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;

        my = transform;

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        camPosition = transform.position;

        //defaultDistance = idealDistance = distance;
        maxDistance = canZoom ? zoomDistance.max : idealDistance;
    }

    void OnApplicationFocus(bool hasFocus) {
        if (!GameState.isPaused)
            Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate() {
        if (!target || GameState.isPaused) return;
        
        DoRotation();

        if (canZoom) Zoom(Input.GetAxis("Mouse ScrollWheel"));
        
        offset.x = Mathf.Lerp(offsetClose.x, offsetFar.x, distance / maxDistance);
        offset.y = Mathf.Lerp(offsetClose.y, offsetFar.y, distance / maxDistance);

        Vector3 targetPosition = target.position;

        if (offsetOverriden) {
            targetPosition.x += _tempOffset.x;
            targetPosition.y += _tempOffset.y;
            _tempOffset = Vector2.Lerp(_tempOffset, Vector2.zero, Time.deltaTime / smoothDamp);
            if (Vector2.Distance(_tempOffset, Vector2.zero) < .01f)
                offsetOverriden = false;
        }

        CheckForCollision();

        negDistance.z = -distance;
        Vector3 targetWithOffset = targetPosition + my.right * offset.x + my.up * offset.y;
        camPosition = camRotation * negDistance + targetWithOffset;

        //my.LookAt(targetWithOffset);

        SmoothMovement();

        target.rotation = Quaternion.Euler(0, yaw, 0); // Reoriente the character's rotator
    }

    void DoRotation() {
        rotationSpeed.x = Mathf.Lerp(minRotationSpeed.x, maxRotationSpeed.x, distance / maxDistance);
        rotationSpeed.y = Mathf.Lerp(minRotationSpeed.y, maxRotationSpeed.y, distance / maxDistance);

        float clampedX = Mathf.Clamp(Input.GetAxis("Mouse X") * (idealDistance / distance), -mouseSpeedLimit.x, mouseSpeedLimit.x); // Avoid going too fast (makes weird lerp)
        if (invertAxis.x) clampedX = -clampedX;
        yaw += clampedX * rotationSpeed.x * Time.deltaTime;

        float clampedY = Mathf.Clamp(Input.GetAxis("Mouse Y") * (idealDistance / distance), -mouseSpeedLimit.y, mouseSpeedLimit.y); // Avoid going too fast (makes weird lerp)
        if (invertAxis.y) clampedY = -clampedY;
        pitch -= clampedY * rotationSpeed.y * Time.deltaTime;
        pitch = pitchRotationLimit.Clamp(pitch);

        camRotation = Quaternion.Euler(pitch, yaw, 0);

        idealDistance = distanceBasedOnPitch.Lerp(distanceFromRotation.Evaluate(pitchRotationLimit.InverseLerp(pitch)));
        camera.fieldOfView = fovBasedOnPitch.Lerp(fovFromRotation.Evaluate(pitchRotationLimit.InverseLerp(pitch)));

        //Changer la rotation de la caméra pendant l'Éclipse
        //camRotation = Quaternion.AngleAxis(90, Vector3.forward) * camRotation;
    }

    bool blockedByAWall;
    float sphereRadius = 1f;
    float lastHitDistance;
    void CheckForCollision() {
        negDistance.z = -idealDistance;
        //Vector3 idealPosition = camRotation * negDistance + target.position; // Virtually ideal position that does not take offset into account to avoid infinite back and forth
        Vector3 targetPos = target.position; // Same for the target
        targetPos.y += offsetClose.y;

        int layerMask = ~(1 << 10); // ignore Layer #10 : "DontBlockCamera"
        
        RaycastHit hit;
        blockedByAWall = Physics.SphereCast(targetPos, sphereRadius, my.position - targetPos, out hit, distance, layerMask);
        Debug.DrawLine(targetPos, my.position, Color.yellow);

        if (blockedByAWall && hit.distance > 0) // If we hit something, hitDistance cannot be 0, nor higher than idealDistance
            lastHitDistance = Mathf.Min(hit.distance, idealDistance);

        float fixedDistance = blockedByAWall ? lastHitDistance : idealDistance;
        
        distance = Mathf.Lerp(distance, fixedDistance, Time.deltaTime / smoothDamp);
    }

    void SmoothMovement() {
        float t = smoothMovement ? Time.deltaTime / smoothDamp : 1;
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
}