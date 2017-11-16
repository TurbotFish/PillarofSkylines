﻿using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera/Third Person Camera")]
[RequireComponent(typeof(Camera))]
public class PoS_Camera : MonoBehaviour {

	#region Properties
	public LayerMask blockingLayer;

	public eCameraState state;
	public enum eCameraState {
		Default, Idle, PlayerControl,
		Resetting, AroundCorner, FollowBehind,
		Slope, Air, LookAt
	}

	[Header("Position")]
	public Transform target;
	public float distance = 10;
	public Vector2 offsetFar = new Vector2(0, 2),
	offsetClose = new Vector2(2, 0);
	public float defaultPitch = 15;
    public Vector2 obliqueness;

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
    
    [Header("Points of Interest")]
    public LayerMask layerPoI;
    public float maxDistanceToPoI = 300;

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
    float targetYaw, targetPitch;
    float manualPitch, manualYaw;
    float recoilIntensity;
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

        SetObliqueness();
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

		SmoothMovement();
        
        Vector3 characterUp = target.parent.up;
        target.LookAt(target.position + Vector3.ProjectOnPlane(my.forward, characterUp), characterUp); // Reoriente the character's rotator

        if (enablePanoramaMode)
			DoPanorama();
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
		if (!Input.anyKey && input.magnitude == 0 && playerVelocity == Vector3.zero)
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
        lastInput = 0; // allows the autoReset to take place immediately
    }

    public void ClearPointOfInterest() {
        nearPOI = false;
    }

    bool IcanSee(Vector3 point) {
        //Debug.DrawLine(camPosition, point, Color.magenta, 0.5f);
        return !Physics.Linecast(camPosition, point, blockingLayer);
    }

    void LookAtTargetPOI() {
        if (IcanSee(targetPOI)) {
            state = eCameraState.LookAt;
            SetTargetRotation(GetRotationTowardsPoint(targetPOI), autoResetDamp);
        }
    }

    /// <summary>
    /// Old, systemic version of Points of Interest
    /// </summary>
    void LookForPointsofInterest() {
        Collider[] points = Physics.OverlapSphere(camPosition, maxDistanceToPoI, layerPoI, QueryTriggerInteraction.Collide);

        if (points.Length == 0) {
            state = eCameraState.Default;
            return;
        }

        Collider targetPoI = null;
        float currentPoIScore = 300f;

        for (int i = 0, j = 0; i < points.Length; i++) {
            // S'il n'y a pas de mur entre moi et le PoI, je peux le voir, donc je peux potentiellement me tourner vers lui
            if (IcanSee(points[i].transform.position)) {

                // Si le PoI est plus petit que les autres, il devient ma cible
                // peut être faire un truc avec la distance aussi
                if (targetPoI == null || (points[i].bounds.extents.x + Vector3.Distance(points[i].transform.position, camPosition)) < currentPoIScore) {
                    currentPoIScore = points[i].bounds.extents.x + Vector3.Distance(points[i].transform.position, camPosition);
                    targetPoI = points[i];
                }
                //print("salut je suis " + points[i] + " tu peux me regarder si tu veux");
            }
        }
        if (targetPoI == null) return;

        // si le joueur va dans la direction opposée au PoI, on ne veut peut être pas se tourner vers lui ?
        // same pour la caméra ? si le PoI est dans mon dos je veux ptet pas y aller ?

        targetPOI = targetPoI.transform.position;
        LookAtTargetPOI();
    }

    #endregion
    
    #region Slopes and Cliffs

    /// <summary>
    /// Checks whether we're on a cliff or on a slope and returns the value of that slope
    /// </summary>
    float CheckGroundAndReturnSlopeValue() {
        Vector3 groundNormal = controller.collisions.currentGroundNormal;

        // Si on est au sol et qu'il n'y a pas de mur devant
        if (playerState == ePlayerState.onGround && !Physics.Raycast(target.position, player.transform.forward, 1, controller.collisionMask)) {

            RaycastHit groundInFront;

            if (Physics.Raycast(target.position + player.transform.forward * distanceToCheckGroundForward,
                            -target.up, out groundInFront, cliffMinDepth, controller.collisionMask)) {

                NotOnEdgeOfCliff(); // Y a du sol devant donc on n'est pas au bord d'une falaise

                if ((targetSpace * groundInFront.normal).y > 0.99f)
                    groundNormal = target.up; // Si devant c'est environ du sol plat, on reset slopeValue

                else if (Vector3.Angle(groundNormal, groundInFront.normal) > 10)
                    groundNormal = groundInFront.normal; // Si la pente devant a plus de X degrés de différence, on prend sa slopeValue

            } else {// on est au sol, y a pas de mur devant, et y a pas de sol devant non plus, donc on est au bord d'une falaise
                onEdgeOfCliff = true;
                canAutoReset = false;
                return 10;
            }

        } else // soit on n'est pas au sol, soit on est au sol mais y a un mur devant, donc on n'est pas au bord d'une falaise
            NotOnEdgeOfCliff();

        return Vector3.Dot(Vector3.ProjectOnPlane(my.forward, target.parent.up), groundNormal) * 60;
        // Ici on recalcule le forward en aplatissant celui de la caméra pour éviter des erreurs quand le perso tourne
    }

    void NotOnEdgeOfCliff() {
        if (onEdgeOfCliff) {
            lastInput = Time.time;
            canAutoReset = true;
        }
        onEdgeOfCliff = false;
    }
    #endregion
    
    /// <summary>
    /// Appelé quand le player spawn, change de scène ou qu'il est téléporté par le worldWrapper
    /// </summary>
    /// <param name="sender"> </param>
    /// <param name="args"> Contient la position vers laquelle tp, et un bool pour savoir si on a changé de scène. </param>
    void OnTeleportPlayer(object sender, Game.Utilities.EventManager.OnTeleportPlayerEventArgs args) {
        if (args.IsNewScene) {
            PlaceBehindPlayerNoLerp();
            // on reset les paramètres par défaut de la caméra
            ResetZoom();
            ClearPointOfInterest();
            enablePanoramaMode = true;
        }
        else {
            print("yo je me tp là: " + lastFrameOffset);
            my.position = args.Position - lastFrameOffset;
            negDistance.z = -currentDistance;
            Vector3 targetWithOffset = args.Position + my.right * offset.x + my.up * offset.y;
            camPosition = my.rotation * negDistance + targetWithOffset;
        }
    }

    void SetObliqueness() {
        Matrix4x4 mat = camera.projectionMatrix;
        mat[0, 2] = obliqueness.x;
        mat[1, 2] = obliqueness.y;
        camera.projectionMatrix = mat;
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

    // pas trop satisfait par l'organisation de cette méthode
    // au début elle servait juste à prendre les inputs mais elle gère aussi les états vu que c'est interconnecté du coup elle est devenue un peu grosse
    void GetInputsAndStates() {
		input.x = Input.GetAxis("Mouse X") + Input.GetAxis("RightStick X");
		input.y = Input.GetAxis("Mouse Y") + Input.GetAxis("RightStick Y");

		playerState = player.currentPlayerState;
		playerVelocity = player.velocity;
        
		targetSpace = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, target.up), Vector3.Cross(Vector3.up, target.up));
        
        // Get the value of the slope we're walking on (or we're about to walk on) also check if we're on a cliff
        float slopeValue = CheckGroundAndReturnSlopeValue();

        // Il nous faut une fonction SetState() pour pouvoir faire des trucs uniquement lors d'un changement de State

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

            if (playerState == ePlayerState.onGround) {
                
                if (state == eCameraState.Air) { // Si on était dans les airs avant
                    manualPitch = defaultPitch; // On reset le pitch
                    additionalDistance = 0; // On reset le zoom
                }

                if (Time.time > lastInput + timeBeforeAutoReset) { // Si ça fait genre 5 secondes qu'on n'a pas touché à la caméra on passe en mode automatique

                    if (nearPOI)
                        LookAtTargetPOI();
                    //LookForPointsofInterest(); // Ancienne version des POI (Systémique)

                    if (state != eCameraState.LookAt && canAutoReset) { // Si y a pas de PoI on se replace tout seul derrière le joueur
                        state = eCameraState.Resetting;
                        SetTargetRotation(defaultPitch - slopeValue, GetYawBehindPlayer(), autoResetDamp);
                        manualPitch = defaultPitch - slopeValue;
                        canAutoReset = false; // On n'autoReset qu'une fois
                    }

                } else {
                    if (playerVelocity != Vector3.zero) {
                        state = eCameraState.Default;
                        SetTargetRotation(slopeValue + manualPitch, null, resetDamp);

                    } else {
                        state = eCameraState.Idle;
                        SetTargetRotation(slopeValue + manualPitch, null, resetDamp);
                    }
                }

			} else {
				state = eCameraState.Air;
                canAutoReset = true;
                
                if (playerVelocity.y < 0) { // je suis en train de tomber

                    if (additionalDistance > -distanceReductionWhenFalling) // Je zoom vers le perso
                        additionalDistance -= deltaTime / autoResetDamp;

                    bool aboveGround = Physics.Raycast(target.position + (playerVelocity.z * Vector3.forward + playerVelocity.x * Vector3.right) / 4, -target.up, maxJumpHeight, controller.collisionMask);
                    if (!aboveGround) {
                        SetTargetRotation(pitchRotationLimit.max, null, fallingDamp);
                    }
                } else { // je suis en train de sauter
                    additionalDistance = 0;
                    if (targetPitch != pitchRotationLimit.max) { // peut être qu'on devrait ajuster le targetPitch selon la distance du rayon (genre si on est haut mais ça va on se penche mais pas trop)
                        autoAdjustPitch = false;
                        autoAdjustYaw = false;
                    }
                }

			}
		}

		if (playerState == ePlayerState.gliding || playerState == ePlayerState.inWindTunnel) {
            SetTargetRotation(-2 * playerVelocity.y + defaultPitch, GetYawBehindPlayer(), resetDamp);
			state = eCameraState.FollowBehind;
		}

		if (Input.GetButton("ResetCamera")) {
            manualPitch = defaultPitch;
            manualYaw = targetYaw;
            lastInput = Time.time;
            canAutoReset = true;
            state = eCameraState.Resetting;

            if (playerState == ePlayerState.inAir) {
                SetTargetRotation(pitchRotationLimit.max, GetYawBehindPlayer(), resetDamp);
            } else {
                SetTargetRotation(defaultPitch, GetYawBehindPlayer(), resetDamp);
            }
		}
	}

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
		//Debug.DrawLine(startPos, rayEnd, Color.yellow);

		if (blockedByAWall && hit.distance > 0) {// If we hit something, hitDistance cannot be 0, nor higher than idealDistance
			lastHitDistance = Mathf.Min(hit.distance - rayRadius, idealDistance);

            // Les angles on fera ça plus tard
			if (state != eCameraState.PlayerControl && currentDistance >= hit.distance) { // auto move around corners
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

    Vector3 lastFrameOffset;
	void SmoothMovement() {
		float t = smoothMovement ? deltaTime / smoothDamp : 1;
		my.position = Vector3.Lerp(my.position, camPosition, t);
        lastFrameOffset = target.position - my.position;
		my.rotation = Quaternion.Lerp(my.rotation, camRotation, t);
	}

    #region ValueMethods
    Vector3 worldForward = new Vector3(0, 0, 1),
            worldRight = new Vector3(1, 0, 0);
    
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

    Vector2 GetRotationTowardsPoint(Vector3 point) {
        Vector3 direction = point - camPosition;
        float distance = direction.magnitude;
        direction /= distance;
        Quaternion newq = Quaternion.LookRotation(direction, Vector3.up);

        return new Vector2(SignedAngle(targetSpace * worldRight, newq * worldRight, target.right), SignedAngle(targetSpace * worldForward, newq * worldForward, target.up));
    }

    float GetPitchTowardsPoint(Vector3 point) {
        Vector3 direction = point - camPosition;
        float distance = direction.magnitude;
        direction /= distance;
        Quaternion newq = Quaternion.LookRotation(direction, Vector3.up);
        
        return SignedAngle(targetSpace * worldRight, newq * worldRight, target.right);
    }

    float GetYawTowardsPoint(Vector3 point) {
        Vector3 direction = point - camPosition;
        float distance = direction.magnitude;
        direction /= distance;
        Quaternion newq = Quaternion.LookRotation(direction, Vector3.up);

        return SignedAngle(targetSpace * worldForward, newq * worldForward, target.up);
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