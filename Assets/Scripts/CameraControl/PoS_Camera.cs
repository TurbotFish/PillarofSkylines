using UnityEngine;
using System.Collections;

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
        Resetting = 80,
        AroundCorner = 85,
        LookAt = 90,
        PlayerControl = 100,
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
	Game.Player.CharacterController.Character player;
    Game.Player.CharacterController.CharacControllerRecu controller;
	Transform my;

    Game.Player.CharacterController.ePlayerState playerState;

	float yaw, pitch, targetYaw, targetPitch, manualPitch;
	float maxDistance, currentDistance, idealDistance, additionalDistance;
	float deltaTime;
	float lastInput;
	float autoDamp;
    float recoilIntensity;
	bool autoAdjustYaw, autoAdjustPitch;
    bool canAutoReset;
	bool placedByPlayer, onEdgeOfCliff;
    
	#endregion

	#region MonoBehaviour

	void Start() {
		camera = GetComponent<Camera>();
		Cursor.lockState = CursorLockMode.Locked;

		my = transform;
		player = target.GetComponentInParent<Game.Player.CharacterController.Character>();
		controller = player.GetComponent<Game.Player.CharacterController.CharacControllerRecu>();
        
		currentDistance = zoomValue = idealDistance = distance;
		maxDistance = canZoom ? zoomDistance.max : distance;

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
		EvaluatePositionFromRotation();
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
    void OnTeleportPlayer(object sender, Game.Utilities.EventManager.TeleportPlayerEventArgs args) {
        Cursor.lockState = CursorLockMode.Locked;
        if (args.IsNewScene) {
            // on reset les paramètres par défaut de la caméra
            currentDistance = distance;
            ResetZoom();
            nearPOI = false;
            axisAligned = Vector3.zero;
            enablePanoramaMode = true;
        }
        my.position = args.Position - lastFrameOffset;
        negDistance.z = -currentDistance;
        Vector3 targetWithOffset = args.Position + my.right * offset.x + my.up * offset.y;
        camPosition = my.rotation * negDistance + targetWithOffset;
    }
	
    void RealignPlayer() {
        // TODO: During a camera realignment, wait before realigning player
        
        Vector3 characterUp = target.parent.up; // TODO: only change this value when there's a change of gravity?
        if (state == eCameraState.LookAt && inverseFacingDirection)
            target.LookAt(target.position + Vector3.ProjectOnPlane(  startFacingDirection != inverseFacingDirection  ? -axisAligned : axisAligned, characterUp), characterUp); // Reoriente the character's rotator
        else
            target.LookAt(target.position + Vector3.ProjectOnPlane(my.forward, characterUp), characterUp); // Reoriente the character's rotator
    }

    /// <summary>
    /// Hard reset de la caméra, la place immédiatement à sa position par défaut (utilisé quand le player spawn).
    /// </summary>
    void PlaceBehindPlayerNoLerp() {
        currentDistance = distance;
        yaw = GetYawBehindPlayer();
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

        if (state == eCameraState.Resetting) {
            if (((autoAdjustYaw && Mathf.Abs(Mathf.DeltaAngle(yaw, targetYaw)) < 1f) || !autoAdjustYaw)
                && ((autoAdjustPitch && Mathf.Abs(Mathf.DeltaAngle(pitch, targetPitch)) < 1f) || !autoAdjustPitch)) {
                state = eCameraState.Default;
                resetType = eResetType.None;
            }
        }
    }

	/// <summary>
	/// Allows the camera to reset automatically.
	/// </summary>
	/// <param name="allow"> Whether or not to allow the automatic reset. </param>
	/// <param name="immediate"> If true, a reset takes place immediately, else, wait for resetTime. </param>
	void AllowAutoReset(bool allow, bool immediate = false) {
		canAutoReset = allow;
		lastInput = immediate ? 0 : Time.time;
	}

	void EvaluatePositionFromRotation() {
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
		targetWithOffset += GetContextualOffset();

		camPosition = camRotation * negDistance + targetWithOffset;
	}

	Vector3 lastFrameOffset;
    void SmoothMovement() {
        float t = smoothMovement ? deltaTime / smoothDamp : 1;
        my.position = Vector3.Lerp(my.position, camPosition, t);
        lastFrameOffset = target.position - my.position;
        my.rotation = Quaternion.Lerp(my.rotation, camRotation, t);
    }
	#endregion
	
	#region Inputs and States

	void GetInputsAndStates() {
		input.x = Input.GetAxis("Mouse X") + Input.GetAxis("RightStick X");
		input.y = Input.GetAxis("Mouse Y") + Input.GetAxis("RightStick Y");

		playerState = player.currentPlayerState;
		playerVelocity = player.velocity;

		targetSpace = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, target.up), Vector3.Cross(Vector3.up, target.up));
        
		float slopeValue = CheckGroundAndReturnSlopeValue();
		// TODO: Il nous faut une fonction SetState() pour pouvoir faire des trucs uniquement lors d'un changement de State
        
        if (resetType == eResetType.ManualAir && playerState == Game.Player.CharacterController.ePlayerState.onGround) {
            state = eCameraState.Default;
            resetType = eResetType.None;
        }

        if (inverseFacingDirection && Input.GetAxis("Vertical") >= 0) {
            startFacingDirection ^= true;
            inverseFacingDirection = false;
        }

        if (input.sqrMagnitude != 0) { // Contrôle manuel
			state = eCameraState.PlayerControl;
			manualPitch = pitch - slopeValue;
			SetTargetRotation(null, null, smoothDamp);
			AllowAutoReset(true, false);

        } else if (state != eCameraState.Resetting) {

			if (playerState == Game.Player.CharacterController.ePlayerState.onGround || playerState == Game.Player.CharacterController.ePlayerState.sliding) {
                
				if (state == eCameraState.Air) { // Si on était dans les airs avant
                    if (!onEdgeOfCliff)
    					manualPitch = defaultPitch; // On reset le pitch si on n'est pas au bord d'une falaise
					additionalDistance = 0;    // On reset le zoom
					AllowAutoReset(true, true);
				}
                
                if (Time.time > lastInput + timeBeforeAutoReset && canAutoReset) { // Si ça fait genre 5 secondes qu'on n'a pas touché à la caméra on passe en mode automatique
                    
					if (nearPOI) // Points of Interest have priority over axis alignment
						LookAtTargetPOI(); // TODO: manual priority just in case?
					else if (axisAligned.sqrMagnitude != 0)
						AlignWithAxis();

					#if false // l'autoreset est enlevé pour l'instant parce que gênant, le faire que sous certaines conditions

                    if (state != eCameraState.LookAt && canAutoReset) { // Si y a pas de PoI on se replace tout seul derrière le joueur
                        state = eCameraState.Resetting;
                        
                        SetTargetRotation(defaultPitch + slopeValue, GetYawBehindPlayer(), autoResetDamp);
                        manualPitch = defaultPitch;
                        canAutoReset = false; // On n'autoReset qu'une fois
                    }
					#endif
				}

				if (state != eCameraState.LookAt) {
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

			} else { // Le joueur est dans les airs
				state = eCameraState.Air;
				AllowAutoReset(true);
				AirStateCamera();
			}
		}

		if (playerState == Game.Player.CharacterController.ePlayerState.gliding || playerState == Game.Player.CharacterController.ePlayerState.inWindTunnel) {
			SetTargetRotation(-2 * playerVelocity.y + defaultPitch, GetYawBehindPlayer(), resetDamp);
			state = eCameraState.FollowBehind;
		}

		if (Input.GetButton("ResetCamera")) {
			manualPitch = defaultPitch;
			AllowAutoReset(true);

			facingTime = -1;
            state = eCameraState.Resetting;

            if (playerState == Game.Player.CharacterController.ePlayerState.inAir) { // dans les airs, la caméra pointe vers le bas
                resetType = eResetType.ManualAir;
                SetTargetRotation(pitchRotationLimit.max, GetYawBehindPlayer(), resetDamp);
            } else { // au sol, la caméra se met derrière le joueur
                resetType = eResetType.ManualGround;
                SetTargetRotation(defaultPitch + slopeValue, GetYawBehindPlayer(), resetDamp);
            }
        }
    }

	void AirStateCamera() {
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
	void CheckForCollision(Vector3 targetPos) {
		Vector3 startPos = targetPos;

		negDistance.z = -idealDistance;
		Vector3 rayEnd = camRotation * negDistance + (targetPos + my.right * offsetFar.x + my.up * offsetFar.y);

		RaycastHit hit;
		blockedByAWall = Physics.SphereCast(startPos, rayRadius, rayEnd - startPos, out hit, idealDistance, blockingLayer);
		//Debug.DrawLine(startPos, rayEnd, Color.yellow);

		if (blockedByAWall && hit.distance > 0) {// If we hit something, hitDistance cannot be 0, nor higher than idealDistance
			lastHitDistance = Mathf.Min(hit.distance - rayRadius, idealDistance);

			// Les angles on fera ça plus tard
			if (state <= eCameraState.AroundCorner && currentDistance >= hit.distance) { // auto move around corners
				float tempYaw = GetYawBehindPlayer();
				float newYaw = Mathf.LerpAngle(yaw, tempYaw, deltaTime / slideDamp);
				float delta = Mathf.Abs(Mathf.DeltaAngle(newYaw, tempYaw));

				//Debug.DrawLine(hit.point, hit.point + hit.normal, Color.magenta);

				Vector3 centerOfSphereCast = startPos + (rayEnd - startPos).normalized * hit.distance;
				Vector3 hitDirection = (hit.point - centerOfSphereCast).normalized;

				if (delta > 1 && delta < 100) {
					SetTargetRotation(null, GetYawBehindPlayer(), slideDamp);
					state = eCameraState.Resetting;

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

    #region Point of Interest

    bool nearPOI;
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
    }

    bool IcanSee(Vector3 point) {
        //Debug.DrawLine(camPosition, point, Color.magenta, 0.5f);
        return !Physics.Linecast(camPosition, point, blockingLayer);
    }
    
    void LookAtTargetPOI() {
        if (IcanSee(targetPOI) && FacingDirection((targetPOI - target.position).normalized)) {
            state = eCameraState.LookAt;
            SetTargetRotation(GetRotationTowardsPoint(targetPOI), autoResetDamp);
        }
    }
    #endregion
    
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

    public void SetAxisAlignment(Vector3 direction) {
        axisAligned = direction;
		AllowAutoReset(true, true);
		facingTime = -1; // allow instant alignment
        inverseFacingDirection = false;
    }

    void AlignWithAxis() {
        state = eCameraState.LookAt;
        if (FacingDirection(axisAligned))
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

	#region Slopes and Cliffs

    /// <summary>
    /// Checks whether we're on a cliff or on a slope and returns the value of that slope
    /// </summary>
    float CheckGroundAndReturnSlopeValue() {
        float limitVertical = 0.7f; // TODO: softcode
        float slopeSameAngleBuffer = 10; // TODO: softcode
        float minSlopeLength = 1; // TODO: softcode

        Vector3 groundNormal = controller.collisions.currentGroundNormal;
		
        // Si on est au sol et qu'il n'y a pas de mur devant
        if (playerState == Game.Player.CharacterController.ePlayerState.onGround && !Physics.Raycast(target.position, player.transform.forward, 1, controller.collisionMask)) {

            RaycastHit groundInFront;

            if (Physics.Raycast(target.position + player.transform.forward * distanceToCheckGroundForward,
                            -target.up, out groundInFront, cliffMinDepth, controller.collisionMask)) {

                Debug.DrawRay(target.position + player.transform.forward * distanceToCheckGroundForward,
                            -target.up * cliffMinDepth, Color.red);

                NotOnEdgeOfCliff(); // Y a du sol devant donc on n'est pas au bord d'une falaise

                print("GroundNormal dans targetSpace: " + (targetSpace * groundInFront.normal));

                if ((targetSpace * groundInFront.normal).y > 0.999f)
                    groundNormal = target.up; // Si devant c'est environ du sol plat, on reset slopeValue; pas besoin de calculs en plus

                else {
                    RaycastHit groundFurther;
                    // on check entre le sol actuel et le sol devant pour voir la taille d'une pente
                    if (Physics.Raycast(target.position + player.transform.forward * (distanceToCheckGroundForward + minSlopeLength),
                                -target.up, out groundFurther, cliffMinDepth, controller.collisionMask)) {

                        Debug.DrawRay(target.position + player.transform.forward * (distanceToCheckGroundForward + minSlopeLength),
                                    -target.up * cliffMinDepth, Color.red);

                        print("groundNormal: " + groundNormal + " | groundInFront: " + groundInFront.normal + " | groundEndOfSlope: " + groundFurther.normal);

                        if (Vector3.Angle(groundFurther.normal, groundInFront.normal) < slopeSameAngleBuffer // Si la pente devant et la pente plus loin sont environ la même
                            && Vector3.Angle(groundNormal, groundInFront.normal) > slopeSameAngleBuffer // et la pente devant a plus de X degrés de différence avec celle actuelle
                            && groundInFront.normal.y > limitVertical) // et qu'elle n'est pas quasi verticale (un mur)
                            groundNormal = groundInFront.normal; // On prend sa slopeValue
                        else
                            groundNormal = target.up; // Sinon on dit que c'est plat ?
                            // on devrait plutôt faire une moyenne

                    } // Sinon : ne rien faire, on garde le sol actuel
                }

            } else {// on est au sol, y a pas de mur devant, et y a pas de sol devant non plus, donc on est au bord d'une falaise
                onEdgeOfCliff = true;
				AllowAutoReset(false);

				groundNormal.y -= 0.1f;
                groundNormal.Normalize();
				// TODO: pencher la caméra automatiquement lorsque l'on arrive près d'un bord
				// mais : seulement si le sol actuel est plat ?
            }

        } else // soit on n'est pas au sol, soit on est au sol mais y a un mur devant, donc on n'est pas au bord d'une falaise
            NotOnEdgeOfCliff();

        if (groundNormal.y < limitVertical)
            groundNormal = target.up; // Si on est quasi à la verticale, on considère qu'on est contre un mur, on repasse en caméra normale

        return Vector3.Dot(Vector3.ProjectOnPlane(my.forward, target.parent.up), groundNormal) * 60;
        // Ici on recalcule le forward en aplatissant celui de la caméra pour éviter des erreurs quand le perso tourne
    }

    void NotOnEdgeOfCliff() {
        if (onEdgeOfCliff) {
			AllowAutoReset(true);
        }
        onEdgeOfCliff = false;
    }
	#endregion

	#region Contextual Offset
    Vector3 contextualOffset;
    bool cameraBounce;

    public void SetVerticalOffset(float verticalOffset) {
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
