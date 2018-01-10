using UnityEngine;
using System.Collections;
using Game.Player.CharacterController;

[AddComponentMenu("Camera/Third Person Camera")]
[RequireComponent(typeof(Camera))]
public class PoS_Camera : MonoBehaviour {

	#region Properties
	public LayerMask blockingLayer;

	public eCameraState state;
	public enum eCameraState {
        Default = 0,
        Idle = 1,
        Air = 2,
        Slope = 20,
        FollowBehind = 40,
        AroundCorner = 60,
        Overriden = 70,
        Resetting = 80,
        LookAt = 90,
        PlayerControl = 100,
        HomeDoor = 120
	}

    public eResetType resetType;
    public enum eResetType {
        None, ManualGround, ManualAir,
        AxisAlign, InverseAxisAlign, POI,
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
    public float axisAlignDamp = 1f;
    
	public MinMax distanceBasedOnPitch = new MinMax(1, 12);
	public AnimationCurve distanceFromRotation;

	public MinMax fovBasedOnPitch = new MinMax(60, 75);
	public AnimationCurve fovFromRotation;

    [Header("Falling")]
    public float maxJumpHeight = 10;
    public float distanceReductionWhenFalling = 6;
    public float fallingDamp = 2;
    public float recoilOnImpact = 20;

    [Header("Edge of Cliff")]
	public float distanceToCheckGroundForward = 2;
	public float cliffMinDepth = 5;
    /// <summary>
    /// Facteur divisant la distance d'offset sur le bord d'un précipice en fonction de la distance à la caméra.
    /// </summary>
    /// On divise la distance récupérée initialement, par "min" lorsque la caméra est à zéro de distance, par "max" lorsqu'elle est à "maxdistance"
    public MinMax cliffOffsetDivision = new MinMax(100, 20);
    
	[Header("Collision")]
	public float rayRadius = .2f;
	public float dampWhenColliding = .1f;
	public float dampAfterColliding = .6f;
	public float slideDamp = 1f;

	[Header("Panorama Mode")]
	public bool enablePanoramaMode = true;
	public float panoramaAdditionalDistance = 15;
	public float timeToTriggerPanorama = 10;
	public float panoramaDezoomDamp = 1f;
    
	[Header("Eclipse")]
	public bool isEclipse = false;

	new Camera camera;
	Vector3 camPosition, negDistance;
	Vector3 playerVelocity;
	Quaternion camRotation, targetSpace;
	Vector2 input;
	Vector2 rotationSpeed;
	Vector2 offset;
	CharController player;
    CharacControllerRecu controller;
	Transform my;
    Eclipse eclipseFX;

    ePlayerState playerState;

    float deltaTime;

    float yaw, pitch, targetYaw, targetPitch, manualPitch;
	float maxDistance, currentDistance, idealDistance, additionalDistance;

	float lastInput;
    float recoilIntensity;

    bool autoAdjustYaw, autoAdjustPitch;
    bool canAutoReset;

    bool placedByPlayer, onEdgeOfCliff;

    /// <summary>
    /// The dampening value used for the current automatic movement. Set when cameraState changes.
    /// </summary>
    float autoDamp;

    #endregion

    #region MonoBehaviour

    void Start() {
		camera = GetComponent<Camera>();
		Cursor.lockState = CursorLockMode.Locked;

        my = transform;
		player = target.GetComponentInParent<Game.Player.CharacterController.CharController>();
		controller = player.GetComponent<Game.Player.CharacterController.CharacControllerRecu>();
        
		currentDistance = zoomValue = idealDistance = distance;
		maxDistance = canZoom ? zoomDistance.max : distance;

        eclipseFX = GetComponent<Eclipse>();

		manualPitch = defaultPitch;
		state = eCameraState.Default;

        PlaceBehindPlayerNoLerp();
    }

    private void OnEnable() {
        Game.Utilities.EventManager.TeleportPlayerEvent += OnTeleportPlayer;
    }
    private void OnDisable() {
        Game.Utilities.EventManager.TeleportPlayerEvent -= OnTeleportPlayer;
    }

    void OnApplicationFocus(bool hasFocus) {
		if (!GameState.isPaused)
			Cursor.lockState = CursorLockMode.Locked;
	}

	void LateUpdate() {
		deltaTime = Time.deltaTime;
        
		GetInputsAndStates();
		DoRotation();
		EvaluatePosition();
		SmoothMovement();
        RealignPlayer();

        if (enablePanoramaMode)
			DoPanorama();
	}
    #endregion

    #region General Methods

    /// <summary>
    /// Appelé quand le player spawn, change de scène ou qu'il est téléporté par le worldWrapper
    /// </summary>
    /// <param name="sender"> </param>
    /// <param name="args"> Contient la position vers laquelle tp, et un bool pour savoir si on a changé de scène. </param>
    void OnTeleportPlayer(object sender, Game.Utilities.EventManager.TeleportPlayerEventArgs args) { // TODO: cleanup

        if (state == eCameraState.HomeDoor)
            return;

        if (args.IsNewScene) {
            // on reset les paramètres par défaut de la caméra
            currentDistance = distance;
            ResetZoom();
            state = eCameraState.Default;
            resetType = eResetType.None;

            nearPOI = false;
            axisAligned = Vector3.zero;
            enablePanoramaMode = true;
            yaw = SignedAngle(targetSpace * worldForward, args.Rotation * worldForward, target.up);
            pitch = defaultPitch;
            camRotation = my.rotation = targetSpace * Quaternion.Euler(pitch, yaw, 0);

        } else {
            my.position = args.Position - lastFrameOffset;
        }

        negDistance.z = -currentDistance;
        Vector3 targetWithOffset = args.Position + my.right * offset.x + my.up * offset.y;
        camPosition = my.rotation * negDistance + targetWithOffset;
        lastFrameCamPos = camPosition;

        if (args.IsNewScene) {
            my.position = camPosition;
        }
    }
	
    void RealignPlayer() {
        // TODO: During a camera realignment, wait before realigning player
        
        Vector3 characterUp = target.parent.up; // TODO: only change this value when there's a change of gravity?
        target.LookAt(target.position + Vector3.ProjectOnPlane(my.forward, characterUp), characterUp); // Reoriente the character's rotator
    }

    /// <summary>
    /// Hard reset de la caméra, la place immédiatement à sa position par défaut (utilisé quand le player spawn).
    /// </summary>
    void PlaceBehindPlayerNoLerp(float? argYaw = null) {
        currentDistance = distance;
        yaw = argYaw ?? GetYawBehindPlayer();
        pitch = defaultPitch;
        camRotation = my.rotation = targetSpace * Quaternion.Euler(pitch, yaw, 0);
        negDistance.z = -currentDistance;
        Vector3 targetWithOffset = target.position + my.right * offset.x + my.up * offset.y;
        camPosition = my.position = my.rotation * negDistance + targetWithOffset;
    }
	
	/// <summary>
	/// Si un mouvement automatique est activé, l'exécuter. La caméra s'oriente vers targetPitch et targetYaw.
	/// </summary>
    void AutomatedMovement() {
        if (autoAdjustPitch)
            pitch = Mathf.LerpAngle(pitch, targetPitch, deltaTime / autoDamp);
        if (autoAdjustYaw)
            yaw = Mathf.LerpAngle(yaw, targetYaw, deltaTime / autoDamp);

        if (state == eCameraState.Resetting ) {
            if (((autoAdjustYaw && Mathf.Abs(Mathf.DeltaAngle(yaw, targetYaw)) < 1f) || !autoAdjustYaw)
                && ((autoAdjustPitch && Mathf.Abs(Mathf.DeltaAngle(pitch, targetPitch)) < 1f) || !autoAdjustPitch)) {
                StopCurrentReset();
            }
        }
    }

    void StopCurrentReset() {
        state = eCameraState.Default;
        resetType = eResetType.None;
    }

	/// <summary>
	/// Allows the camera to reset automatically.
	/// </summary>
	/// <param name="allow"> Whether or not to allow the automatic reset. </param>
	/// <param name="immediate"> If true, a reset takes place immediately, else, wait for resetTime. </param>
	void AllowAutoReset(bool allow, bool immediate = false) {

		canAutoReset = allow; 

        if (resetType != eResetType.POI) // si je suis dja en train de reset je change pas les options de reset
            lastInput = immediate ? 0 : Time.time;

        if (immediate)
            StopCurrentReset();
	}

	void EvaluatePosition() {

        if (state == eCameraState.HomeDoor) {
            return;
        } else if (state == eCameraState.Overriden) {
            camPosition = overridePos;
            return;
        }

		float distanceFromAngle = Mathf.Lerp(0, 1, distanceFromRotation.Evaluate(pitchRotationLimit.InverseLerp(pitch)));
		idealDistance = 1 + zoomValue * distanceFromAngle + additionalDistance;

		camera.fieldOfView = fovBasedOnPitch.Lerp(fovFromRotation.Evaluate(pitchRotationLimit.InverseLerp(pitch)));

		if (canZoom) Zoom(Input.GetAxis("Mouse ScrollWheel"));

		offset.x = Mathf.Lerp(offsetClose.x, offsetFar.x, currentDistance / maxDistance);
		offset.y = Mathf.Lerp(offsetClose.y, offsetFar.y, currentDistance / maxDistance);

		Vector3 targetPos = target.position;
        
		Vector3 targetWithOffset = targetPos + my.right * offset.x + my.up * offset.y;
		targetWithOffset += GetContextualOffset();

        CheckForCollision(targetPos, targetWithOffset);
        negDistance.z = -currentDistance;

        camPosition = camRotation * negDistance + targetWithOffset;
	}

	Vector3 lastFrameOffset;
    Vector3 velocity = Vector3.zero;
    Vector3 lastFrameCamPos;
    void SmoothMovement() {
        float t = smoothMovement ? deltaTime / smoothDamp : 1;
        // damping value is the inverse of speed, we simply give the camera a speed of 1/smoothDamp
        // so deltaTime * speed = deltaTime * 1/smoothDamp = deltaTime/smoothDamp
        
        my.position = SmoothApproach(my.position, lastFrameCamPos, camPosition, t);
        my.rotation = Quaternion.Lerp(my.rotation, camRotation, t); // TODO: only local space calculation?
        
        lastFrameOffset = target.position - my.position;
        lastFrameCamPos = camPosition;
    }
    
    Vector3 SmoothApproach(Vector3 pastPosition, Vector3 pastTargetPosition, Vector3 targetPosition, float t) {
        Vector3 v = (targetPosition - pastTargetPosition) / t;
        Vector3 f = pastPosition - pastTargetPosition + v;
        return targetPosition - v + f * Mathf.Exp(-t);
    }
    #endregion

    #region Inputs and States

    void GetInputsAndStates() {
		input.x = Input.GetAxis("Mouse X") + Input.GetAxis("RightStick X");
		input.y = Input.GetAxis("Mouse Y") + Input.GetAxis("RightStick Y");

        if (state == eCameraState.HomeDoor) {
            HomeDoorState();
            return; // Dans ce cas, osef de tout le reste
        }

        if (eclipseFX)
            eclipseFX.camSpeed = input;

		playerState = player.CurrentState;
		playerVelocity = player.MovementInfo.velocity;

		targetSpace = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, target.up), Vector3.Cross(Vector3.up, target.up));

        bool isGrounded = (playerState & (ePlayerState.move | ePlayerState.stand | ePlayerState.slide)) != 0;
        float slopeValue = CheckGroundAndReturnSlopeValue();

        // TODO: Il nous faut ptet une fonction SetState() pour pouvoir faire des trucs uniquement lors d'un changement de State
        
        if (isGrounded && additionalDistance != 0)
            additionalDistance = 0;

        if (resetType == eResetType.ManualAir && (playerState & (ePlayerState.move | ePlayerState.stand))!=0)
            StopCurrentReset();

        if (inverseFacingDirection && Input.GetAxis("Vertical") >= 0) {
            startFacingDirection ^= true;
            inverseFacingDirection = false;
        }
        
        if (input.sqrMagnitude != 0) { // Contrôle manuel
			state = eCameraState.PlayerControl;
            resetType = eResetType.None;
			manualPitch = pitch - slopeValue;
			SetTargetRotation(null, null, smoothDamp);

        } else if (state != eCameraState.Resetting) {
            // TODO: un switch sur playerState pour établir l'état de la caméra selon chaque state du joueur ?

            if (state == eCameraState.PlayerControl) { // Si on était en control Manuel avant
                AllowAutoReset(true, false); // On autorise l'auto reset
                state = eCameraState.Default; // On passe en default state
            }

            // Auto reset après quelques secondes sans Input
            if (Time.time > lastInput + timeBeforeAutoReset && canAutoReset) {
                if (overriden)
                    state = eCameraState.Overriden;
                else if (nearPOI) // Points of Interest have priority over axis alignment
                    LookAtTargetPOI(); // TODO: manual priority just in case?
                else if (axisAligned.sqrMagnitude != 0)
                    AlignWithAxis();

            }
            
            if (isGrounded)
                GroundStateCamera(slopeValue);
			else 
				AirStateCamera();
            
            if (playerState == ePlayerState.glide || playerState == ePlayerState.windTunnel) {
                SetTargetRotation(-2 * playerVelocity.y + defaultPitch, GetYawBehindPlayer(), resetDamp);
                state = eCameraState.FollowBehind;
            }
        }
        
		if (Input.GetButton("ResetCamera")) {
			manualPitch = defaultPitch;
			AllowAutoReset(true);

			facingTime = -1;
            state = eCameraState.Resetting;

            // dans les airs, la caméra pointe vers le bas
            if (playerState == ePlayerState.air) { // on n'utilise pas isGrounded ici car cet état est spécifique au fait de tomber
                resetType = eResetType.ManualAir;
                SetTargetRotation(pitchRotationLimit.max, GetYawBehindPlayer(), resetDamp);
            } else { // sinon, elle se met derrière le joueur
                resetType = eResetType.ManualGround;
                SetTargetRotation(defaultPitch + slopeValue, GetYawBehindPlayer(), resetDamp);
            }
        }
    }

    void GroundStateCamera(float slopeValue) {

        if (state == eCameraState.Air) { // Si on était dans les airs avant
            if (!onEdgeOfCliff)
                manualPitch = defaultPitch; // On reset le pitch si on n'est pas au bord d'une falaise
            AllowAutoReset(true, true);
        }
        
        if (state < eCameraState.Overriden) {
            if (playerVelocity != Vector3.zero) {
                state = eCameraState.Default;
                SetTargetRotation(slopeValue + manualPitch, null, resetDamp);

            } else {
                state = eCameraState.Idle;
                SetTargetRotation(slopeValue + manualPitch, null, resetDamp);
                inverseFacingDirection = false;
                startFacingDirection = currentFacingDirection;
            }
        }
    }

	void AirStateCamera() {
        if (state >= eCameraState.Overriden)
            return;

        state = eCameraState.Air;
        AllowAutoReset(true);

        if (playerVelocity.y < 0) { // je suis en train de tomber

			if (additionalDistance > -distanceReductionWhenFalling) // alors je zoom vers le perso
				additionalDistance -= deltaTime / autoResetDamp;

			bool aboveGround = Physics.Raycast(target.position + (playerVelocity.z * Vector3.forward + playerVelocity.x * Vector3.right) / 4, -target.up, maxJumpHeight, controller.collisionMask);
			if (!aboveGround) // je suis au-dessus du vide donc je me penche vers le bas
				SetTargetRotation(pitchRotationLimit.max, null, fallingDamp);

		} else { // je suis en train de sauter
			additionalDistance = 0;

			if (manualPitch <= defaultPitch) // ma caméra a manuellement été pointée vers le haut donc je la replace à son pitch par défaut
				manualPitch = targetPitch = defaultPitch;
		}
	}
	#endregion
	
	#region Rotation

	void DoRotation() {

        if (state == eCameraState.HomeDoor)
            return; // Dans ce cas, osef de tout le reste

        rotationSpeed.x = Mathf.Lerp(minRotationSpeed.x, maxRotationSpeed.x, currentDistance / idealDistance);
		rotationSpeed.y = Mathf.Lerp(minRotationSpeed.y, maxRotationSpeed.y, currentDistance / idealDistance);

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

	void SetTargetRotation(Vector2 rotation, float damp) {
		autoAdjustPitch = true;
		autoAdjustYaw = true;
		targetPitch = rotation.x;
		targetYaw = rotation.y;
		autoDamp = damp;
	}

	void SetTargetRotation(float? newTargetPitch, float? newTargetYaw, float damp) {
		autoAdjustPitch = newTargetPitch != null;
		autoAdjustYaw = newTargetYaw != null;
		targetPitch = newTargetPitch ?? targetPitch;
		targetYaw = newTargetYaw ?? targetYaw;
		autoDamp = damp;
	}

	Vector3 worldForward = new Vector3(0, 0, 1);

	Vector2 GetRotationTowardsPoint(Vector3 point) {
		Vector3 direction = point - camPosition;
		float distance = direction.magnitude;
		direction /= distance;
		return GetRotationTowardsDirection(direction);
	}

	Vector2 GetRotationTowardsDirection(Vector3 direction) {
		Quaternion q = Quaternion.LookRotation(direction);
		return new Vector2(q.eulerAngles.x, q.eulerAngles.y);
	}

	float GetPitchTowardsPoint(Vector3 point) {
		return GetRotationTowardsPoint(point).x;
	}

	float GetYawTowardsPoint(Vector3 point) {
		return GetRotationTowardsPoint(point).y;
	}

	float GetYawBehindPlayer() {
		return SignedAngle(targetSpace * worldForward, target.parent.rotation * worldForward, target.up);
	}

	float SignedAngle(Vector3 v1, Vector3 v2, Vector3 n) {
		return Mathf.Atan2(
			Vector3.Dot(n, Vector3.Cross(v1, v2)),
			Vector3.Dot(v1, v2)) * 57.29578f; // Radian to Degrees constant
	}
	#endregion

	#region Collisions

	bool blockedByAWall;
	float lastHitDistance;
	void CheckForCollision(Vector3 targetPos, Vector3 targetWithOffset) {
		Vector3 rayStart = targetPos;

		negDistance.z = -idealDistance;
        Vector3 rayEnd = camRotation * negDistance + targetWithOffset;

		RaycastHit hit;
		blockedByAWall = Physics.SphereCast(rayStart, rayRadius, rayEnd - rayStart, out hit, idealDistance, blockingLayer);
        //Debug.DrawLine(rayStart, rayEnd, Color.yellow);

        if (blockedByAWall && hit.distance > 0) {// If we hit something, hitDistance cannot be 0, nor higher than idealDistance
			lastHitDistance = Mathf.Min(hit.distance - rayRadius, idealDistance);

			// Les angles on fera ça plus tard
			if (state <= eCameraState.AroundCorner && currentDistance >= hit.distance) { // auto move around corners
				float tempYaw = GetYawBehindPlayer();
				float newYaw = Mathf.LerpAngle(yaw, tempYaw, deltaTime / slideDamp);
				float delta = Mathf.Abs(Mathf.DeltaAngle(newYaw, tempYaw));

				//Debug.DrawLine(hit.point, hit.point + hit.normal, Color.magenta);

				Vector3 centerOfSphereCast = rayStart + (rayEnd - rayStart).normalized * hit.distance;
				Vector3 hitDirection = (hit.point - centerOfSphereCast).normalized;

				if (delta > 1 && delta < 100) {
					SetTargetRotation(null, GetYawBehindPlayer(), slideDamp);
					state = eCameraState.Resetting;
				}
			}
		}
		float fixedDistance = blockedByAWall ? lastHitDistance : idealDistance;

		// If collide, use collisionDamp to quickly get in position and not be blocked by a wall
		// If not colliding, slowly get back to the idealPosition using noCollisionDamp
		currentDistance = Mathf.Lerp(currentDistance, fixedDistance, fixedDistance < currentDistance + .1f ? deltaTime / dampWhenColliding : deltaTime / dampAfterColliding);
	}
    #endregion

    #region Slopes and Cliffs

    /// <summary>
    /// Checks whether we're on a cliff or on a slope and returns the value of that slope
    /// </summary>
    float CheckGroundAndReturnSlopeValue() {
        float limitVertical = 0.7f; // TODO: softcode
        float slopeSameAngleBuffer = 10; // TODO: softcode
        float minSlopeLength = 1; // TODO: softcode

        Vector3 groundNormal = controller.collisions.currentGroundNormal;
        Vector3 targetUp = target.up;
        Vector3 targetPos = target.position;

        if (groundNormal.y < limitVertical)
            groundNormal = targetUp; // Si on est quasi à la verticale, on considère qu'on est contre un mur, on repasse en caméra normale

        // Si on est au sol et qu'il n'y a pas de mur devant
        if ((playerState & (ePlayerState.move | ePlayerState.stand))!=0 && !Physics.Raycast(targetPos, player.transform.forward, 1, controller.collisionMask)) {

            RaycastHit groundInFront;

            if (Physics.Raycast(targetPos + player.transform.forward * distanceToCheckGroundForward,
                            -targetUp, out groundInFront, cliffMinDepth, controller.collisionMask)) {

                Debug.DrawRay(targetPos + player.transform.forward * distanceToCheckGroundForward,
                            -targetUp * cliffMinDepth, Color.red);

                NotOnEdgeOfCliff(); // Y a du sol devant donc on n'est pas au bord d'une falaise

                Vector3 groundInFrontNormal = Quaternion.Inverse(targetSpace) * groundInFront.normal;

                if (groundInFrontNormal.y > 0.999f)
                    groundNormal = targetUp; // Si devant c'est environ du sol plat, on reset slopeValue; pas besoin de calculs en plus

                else {
                    RaycastHit groundFurther;

                    // si la pente devant n'est pas quasi verticale (ie un mur) et a plus de x degrés
                    if (groundInFrontNormal.y > limitVertical) {

                        // on check entre le sol actuel et le sol devant pour voir la taille d'une pente
                        if (Physics.Raycast(targetPos + player.transform.forward * (distanceToCheckGroundForward + minSlopeLength) + targetUp * (1 - groundInFrontNormal.y) * 10,
                                    -targetUp, out groundFurther, cliffMinDepth, controller.collisionMask)) {

                            Vector3 groundFurtherNormal = Quaternion.Inverse(targetSpace) * groundFurther.normal;

                            Debug.DrawRay(targetPos + player.transform.forward * (distanceToCheckGroundForward + minSlopeLength) + targetUp * (1 - groundInFrontNormal.y) * 10,
                                        -targetUp * cliffMinDepth, Color.red);

                            if (Vector3.Angle(groundFurtherNormal, groundInFrontNormal) < slopeSameAngleBuffer) {// Si la pente devant et la pente plus loin sont environ la même
                                groundNormal = groundInFront.normal; // On prend sa slopeValue (world space)
                            } else {
                                groundNormal += groundInFront.normal + groundFurther.normal;
                                groundNormal.x /= 3;
                                groundNormal.y /= 3;
                                groundNormal.z /= 3;
                            }
                        }
                    } // Sinon : ne rien faire, on garde le sol actuel
                }
            } else {// on est au sol, y a pas de mur devant, et y a pas de sol devant non plus, donc on est au bord d'une falaise
                onEdgeOfCliff = true;
                AllowAutoReset(false);
                // TODO: pencher la caméra automatiquement lorsque l'on arrive près d'un bord (mais : seulement si le sol actuel est plat ?)
            }

        } else // soit on n'est pas au sol, soit on est au sol mais y a un mur devant, donc on n'est pas au bord d'une falaise
            NotOnEdgeOfCliff();
        
        return Vector3.Dot(Vector3.ProjectOnPlane(my.forward, target.parent.up), groundNormal) * 60;
        // Ici on recalcule le forward en aplatissant celui de la caméra pour éviter des erreurs quand le perso tourne
    }

    void NotOnEdgeOfCliff() {
        if (onEdgeOfCliff)
            AllowAutoReset(true);
        onEdgeOfCliff = false;
    }
    #endregion

    #region Zoom
    [Header("Zoom")]
	public bool canZoom;
	public float zoomSpeed = 5;
	public MinMax zoomDistance = new MinMax(2, 12);
    Coroutine zoomRoutine;

	float zoomValue;

	void Zoom(float value) {
		if (value != 0) zoomValue = zoomDistance.Clamp(currentDistance - value * zoomSpeed);
	}

    IEnumerator _ZoomAt(float value, float damp) {
        while(Mathf.Abs(zoomValue - value) > 0.01f) {
            zoomValue = Mathf.Lerp(zoomValue, value, deltaTime / damp);
            yield return null;
        }
    }

	/// <summary>
	/// Zoom in a (0,1) value between minimum and maximum distance
	/// </summary>
	public void ZoomAt(float value, float damp) {
        zoomRoutine = StartCoroutine(_ZoomAt(value, damp));
	}

	public void ResetZoom() {
        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);
		zoomValue = distance;
	}
	#endregion

	#region Panorama Mode
	float panoramaTimer = 0;
	bool inPanorama = false;

	void DoPanorama() {
		if (!Input.anyKey && input.sqrMagnitude == 0 && playerVelocity == Vector3.zero)
			panoramaTimer += deltaTime;
		else {
			panoramaTimer = 0;
			if (inPanorama) {
				additionalDistance = 0;
				inPanorama = false;
			}
		}
		if (panoramaTimer >= timeToTriggerPanorama) {
            additionalDistance = Mathf.Lerp(additionalDistance, panoramaAdditionalDistance, deltaTime / panoramaDezoomDamp);
            inPanorama = true;
		}
	}

    #endregion

    #region Home Door

    Vector3 homeDoorPosition, homeDoorForward, homePosition;
    float homeDoorMaxZoom = 7, homeDoorFov = 50;

    public void LookAtHomeDoor(Vector3 doorPosition, Vector3 doorForward, Vector3 homePosition) {
        state = eCameraState.HomeDoor;
        homeDoorPosition = doorPosition;
        homeDoorForward = doorForward;
        this.homePosition = homePosition;
    }

    public void StopLookingAtHomeDoor() {
        state = eCameraState.Default;
    }

    void HomeDoorState() {

        Vector3 playerPos = target.position;
        
        float playerPosValue = -1;

        if ((playerPos - homeDoorPosition).sqrMagnitude < homeDoorMaxZoom * homeDoorMaxZoom) {
            // entrée (-1 à 0)


        } else if ((playerPos - homePosition).sqrMagnitude < homeDoorMaxZoom * homeDoorMaxZoom ) {
            // sortie (0 à 1)


        }

        Vector3 targetPos = homeDoorPosition;
        targetPos.y += 2;


        camPosition = targetPos - homeDoorForward * homeDoorMaxZoom;

        camRotation = Quaternion.LookRotation(homeDoorForward, Vector3.up);

        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, homeDoorFov, 4*deltaTime);

    }

    #endregion

    #region Point of Interest

    bool nearPOI, alwaysLookAt = true;
    Vector3 targetPOI;
    public void SetPointOfInterest(Vector3 point) {
        nearPOI = true;
        targetPOI = point;
		AllowAutoReset(true, true);
		facingTime = -1; // allow immediate alignment
    }

    public void ClearPointOfInterest(Vector3 point) {
        if (targetPOI != point)
            return; // Attempting to clear a point that is not currently set
        nearPOI = false;
        state = eCameraState.Default;
        resetType = eResetType.None;
    }

    bool IcanSee(Vector3 point) {
        //Debug.DrawLine(camPosition, point, Color.magenta, 0.5f);
        return !Physics.Linecast(camPosition, point, blockingLayer);
    }
    
    void LookAtTargetPOI() {
        if (alwaysLookAt || IcanSee(targetPOI) && FacingDirection((targetPOI - target.position).normalized)) {
            state = eCameraState.LookAt;
            resetType = eResetType.POI;
            SetTargetRotation(GetRotationTowardsPoint(targetPOI), autoResetDamp);
        } else
            resetType = eResetType.None;
    }
    #endregion
    
    // TODO: region triggers

    bool startFacingDirection, currentFacingDirection, inverseFacingDirection;
    float maxInverseFacingTime = 0.5f; // TODO: softcode that
    float facingTime = -1;

    bool FacingDirection(Vector3 direction) {
        float dot = Vector3.Dot(target.parent.forward, direction);
        bool temp = currentFacingDirection ? dot > -0.8f : dot > 0.8f;
        if (facingTime == -1) {
            facingTime = 0;
            if (Input.GetAxis("Vertical") <= 0)
                temp ^= true;
            return startFacingDirection = currentFacingDirection = temp;

        } else if (temp != currentFacingDirection && playerVelocity.sqrMagnitude > 0.01f) {
            facingTime += deltaTime;
            if (facingTime >= maxInverseFacingTime) {
                facingTime = 0;
                inverseFacingDirection ^= true;
                return currentFacingDirection = temp;
            }
        } else
            facingTime = 0;
        return currentFacingDirection;
    }
    
    #region Align with Axis

    Vector3 axisAligned = Vector3.zero;
    bool faceBothWays;

    public void SetAxisAlignment(Vector3 direction, bool faceBothWays) {
        this.faceBothWays = faceBothWays;
        axisAligned = direction;
		AllowAutoReset(true, true);
		facingTime = -1; // allow instant alignment
        inverseFacingDirection = false;
    }

    void AlignWithAxis() {
        state = eCameraState.LookAt;
        if (!faceBothWays || FacingDirection(axisAligned))
            SetTargetRotation(GetRotationTowardsDirection(axisAligned), axisAlignDamp);
        else
            SetTargetRotation(GetRotationTowardsDirection(-axisAligned), axisAlignDamp);
        targetPitch += defaultPitch;
    }

    public void RemoveAxisAlignment(Vector3 direction) {
        if (axisAligned != direction)
            return; // Attempting to clear a direction that isn't currently set
        axisAligned = Vector3.zero;
    }
    #endregion
    
    #region Override Position and Rotation

    Vector3 overridePos, overrideRot;
    float overrideDamp;
    bool overriden;

    public void OverrideCamera(Vector3 position, Vector3 rotation, float damp) {
        state = eCameraState.Overriden;
        overriden = true;
        overridePos = position;
        overrideRot = rotation;
        autoDamp = overrideDamp = damp;
    }

    public void StopOverride() {
        overriden = false;
    }

    #endregion
    
	#region Contextual Offset
    Vector3 contextualOffset;
    bool cameraBounce;

    public void SetVerticalOffset(float verticalOffset) {
        return; // TODO: fix that buggy offset thingy
        recoilIntensity = recoilOnImpact * verticalOffset;
        contextualOffset.y = -verticalOffset;
        cameraBounce = true;
    }

    Vector3 GetContextualOffset() {
        Vector3 offset = new Vector3(0, 0, 0);

        // multiplier l'offset contextuel par un modifier qui dépend de la distance actuelle
        // si on est à zéro distance alors pas d'offset

        if (cameraBounce) {
            offset += contextualOffset.y * target.up * recoilIntensity;
            contextualOffset.y = Mathf.Lerp(contextualOffset.y, 0, deltaTime / smoothDamp);
            if (Mathf.Abs(contextualOffset.y - 0) < .01f)
                cameraBounce = false;
            // use impactFromSpeed (animCurve) to attenuate the impact on low speed
            // impactFromSpeed on recoilOnImpact
        }
        if (onEdgeOfCliff) // Appliquer l'offset contextuel sur le bord des falaises
            contextualOffset = Vector3.Lerp(contextualOffset, player.transform.forward * (Mathf.Max(0, pitch) / cliffOffsetDivision.Lerp(currentDistance / maxDistance)), deltaTime / autoResetDamp);
        else
            contextualOffset = Vector3.Lerp(contextualOffset, Vector3.zero, deltaTime / autoResetDamp);
        return offset + contextualOffset;
    }
	#endregion
    
}
