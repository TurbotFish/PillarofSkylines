using UnityEngine;
using System.IO;
using System.Collections;
using Game.Player.CharacterController;
using Game.GameControl;
using UnityEngine.PostProcessing;

[AddComponentMenu("Camera/Third Person Camera")]
[RequireComponent(typeof(Camera))]
public class PoS_Camera : MonoBehaviour
{

    #region Properties
    public LayerMask blockingLayer;

    public eCameraState state;
    public enum eCameraState
    {
        Default = 0,
        Idle = 1,
        Air = 2,
        Slope = 20,
        FollowBehind = 40,
        AroundCorner = 50,
        WallRun = 60,
        Overriden = 70,
        Resetting = 80,
        LookAt = 90,
        PlayerControl = 100,
        HomeDoor = 120
    }

    public eResetType resetType;
    public enum eResetType
    {
        None, ManualGround, ManualAir,
        AxisAlign, InverseAxisAlign, POI,
        WallRun,
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
    public float recoilOnImpact = 1;
    public float recoilFOVImpact = 20;

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

    [Header("FOV")]
    public float fovDamp = 4;
    public float resetCameraFovSupplement = -10;

    [Header("Dash")]
    public float dashFovSupplement = 15;
    public float dashDistance = -1;
    public float dashDamp = 0.1f;
    public float dashDotLimit = 0.2f;

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
    CameraControlTrigger currentTrigger;

    ePlayerState playerState;

    float deltaTime;
    float startFov, targetFov;

    float yaw, pitch, targetYaw, targetPitch, manualPitch;
    float maxDistance, currentDistance, idealDistance, additionalDistance;

    float lastInput;
    float recoilIntensity;

    bool gamePaused = true;

    bool autoAdjustYaw, autoAdjustPitch;
    bool canAutoReset;

    bool placedByPlayer, onEdgeOfCliff;

    /// <summary>
    /// The dampening value used for the current automatic movement. Set when cameraState changes.
    /// </summary>
    float autoDamp;

    public Camera CameraComponent { get { return camera; } }

    Vector3 characterUp;

    private GameController gameController;
    private bool isInitialized;

    #endregion

    #region MonoBehaviour

    public void Initialize(GameController gameController)
    {
        this.gameController = gameController;

        camera = GetComponent<Camera>();

        isInitialized = true;
    }

    void Start()
    {
        camera = GetComponent<Camera>();
#if !UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Locked;
#endif

        my = transform;
        eclipseFX = GetComponent<Eclipse>();
        player = target.GetComponentInParent<CharController>();
        controller = player.GetComponent<CharacControllerRecu>();
        postProcess = GetComponent<PostProcessingBehaviour>();

        currentDistance = zoomValue = idealDistance = distance;
        maxDistance = canZoom ? zoomDistance.max : distance;

        startFov = camera.fieldOfView;

        manualPitch = defaultPitch;
        state = eCameraState.Default;
        
        trueOffsetFar = offsetFar;

        photoPath = Application.dataPath + "/Photos/";

        ResetGravity();
        PlaceBehindPlayerNoLerp();
    }

    private void OnEnable()
    {
        Game.Utilities.EventManager.TeleportPlayerEvent += OnTeleportPlayer;
        Game.Utilities.EventManager.GamePausedEvent += OnGamePaused;
        Game.Utilities.EventManager.PreSceneChangeEvent += OnPreSceneChangeEvent;
        Game.Utilities.EventManager.SceneChangedEvent += OnSceneChangedEvent;
    }

    private void OnDisable()
    {
        Game.Utilities.EventManager.TeleportPlayerEvent -= OnTeleportPlayer;
        Game.Utilities.EventManager.GamePausedEvent -= OnGamePaused;
        Game.Utilities.EventManager.PreSceneChangeEvent -= OnPreSceneChangeEvent;
        Game.Utilities.EventManager.SceneChangedEvent -= OnSceneChangedEvent;
    }

    void OnApplicationFocus(bool hasFocus)
    {
#if !UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Locked;
#endif
    }

    void Update()
    {
        if ((gamePaused && !photoMode) || !isInitialized)
        {
            return;
        }

        deltaTime = Time.deltaTime;

        DebugCameraMovement();

        GetInputsAndStates();
        DoRotation();
        EvaluatePosition();
        SmoothMovement();
        RealignPlayer();

        if (enablePanoramaMode)
        {
            DoPanorama();
        }

        if (gameController.DuplicationCameraManager != null)
        {
            gameController.DuplicationCameraManager.UpdateDuplicationCameras();
        }
    }
    #endregion

    #region General Methods
    
    /// <summary>
    /// Appelé quand le player spawn, change de scène ou qu'il est téléporté par le worldWrapper
    /// </summary>
    /// <param name="sender"> </param>
    /// <param name="args"> Contient la position vers laquelle tp, et un bool pour savoir si on a changé de scène. </param>
    void OnTeleportPlayer(object sender, Game.Utilities.EventManager.TeleportPlayerEventArgs args)
    {
        if (args.IsNewScene)
        {
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
        }
        else
        {
            //my.position = args.Position - lastFrameOffset;
            my.position += (args.Position - args.FromPosition);
        }

        negDistance.z = -currentDistance;
        Vector3 targetWithOffset = args.Position + my.right * offset.x + my.up * offset.y;
        camPosition = my.rotation * negDistance + targetWithOffset;
        lastFrameCamPos = camPosition;

        //PlaceBehindPlayerNoLerp();

        if (args.IsNewScene)
        {
            my.position = camPosition;
        }

        if (gameController.DuplicationCameraManager)
            gameController.DuplicationCameraManager.UpdateDuplicationCameras();
    }

    void OnGamePaused(object sender, Game.Utilities.EventManager.GamePausedEventArgs args)
    {
        gamePaused = args.PauseActive;
    }

    private void OnSceneChangedEvent(object sender, Game.Utilities.EventManager.SceneChangedEventArgs args)
    {
        CameraComponent.enabled = true;
    }

    private void OnPreSceneChangeEvent(object sender, Game.Utilities.EventManager.PreSceneChangeEventArgs args)
    {
        CameraComponent.enabled = false;
    }

    public void ResetGravity()
    {
        characterUp = target.parent.up;
        float angle = Vector3.Angle(worldUp, characterUp);
        Vector3 axis = Vector3.Cross(worldUp, characterUp);
        targetSpace = Quaternion.AngleAxis(angle, axis);
    }

    public void UpdateGravity()
    {
        characterUp = target.parent.up;
        Vector3 currentUp = targetSpace * worldUp;
        targetSpace = Quaternion.FromToRotation(currentUp, characterUp) * targetSpace;
    }

    public void RestartExecution()
    {
        gamePaused = false;
    }

    public void StopExecution()
    {
        gamePaused = true;
    }

    void RealignPlayer()
    {
        if (state == eCameraState.HomeDoor)
            return; // Dans ce cas, osef de tout le reste
        // TODO: During a camera realignment, wait before realigning player
        target.LookAt(target.position + Vector3.ProjectOnPlane(my.forward, characterUp), characterUp); // Reoriente the character's rotator
    }

    /// <summary>
    /// Hard reset de la caméra, la place immédiatement à sa position par défaut (utilisé quand le player spawn).
    /// </summary>
    void PlaceBehindPlayerNoLerp(float? argYaw = null)
    {
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
    void AutomatedMovement()
    {
        if (autoAdjustPitch)
            pitch = Mathf.LerpAngle(pitch, targetPitch, deltaTime / autoDamp);
        if (autoAdjustYaw) {
            yaw = Mathf.LerpAngle(yaw, targetYaw, deltaTime / autoDamp);
        }
        if (state == eCameraState.Resetting)
        {
            if (((autoAdjustYaw && Mathf.Abs(Mathf.DeltaAngle(yaw, targetYaw)) < 1f) || !autoAdjustYaw)
                && ((autoAdjustPitch && Mathf.Abs(Mathf.DeltaAngle(pitch, targetPitch)) < 1f) || !autoAdjustPitch))
            {
                StopCurrentReset();
            }
        }
    }

    void StopCurrentReset()
    {
        state = eCameraState.Default;
        resetType = eResetType.None;
    }

    /// <summary>
    /// Allows the camera to reset automatically.
    /// </summary>
    /// <param name="allow"> Whether or not to allow the automatic reset. </param>
    /// <param name="immediate"> If true, a reset takes place immediately, else, wait for resetTime. </param>
    void AllowAutoReset(bool allow, bool immediate = false)
    {
        if (photoMode) {
            canAutoReset = false;
            return;
        }

        canAutoReset = allow;
        if (resetType != eResetType.POI) // si je suis dja en train de reset je change pas les options de reset
            lastInput = immediate ? 0 : Time.time;

        if (immediate)
            StopCurrentReset();
    }

    void EvaluatePosition()
    {
        if (state == eCameraState.HomeDoor)
        {
            return;
        }
        else if (state == eCameraState.Overriden)
        {
            camPosition = overridePos;
            return;
        }

        float distanceFromAngle = Mathf.Lerp(0, 1, distanceFromRotation.Evaluate(pitchRotationLimit.InverseLerp(pitch)));
        idealDistance = 1 + zoomValue * distanceFromAngle + additionalDistance;
        
        if (canZoom) Zoom(Input.GetAxis("DebugVertical"));

        offset.x = Mathf.Lerp(offsetClose.x, offsetFar.x, currentDistance / maxDistance);
        offset.y = Mathf.Lerp(offsetClose.y, offsetFar.y, currentDistance / maxDistance);

        Vector3 targetPos = target.position;

        Vector3 targetWithOffset = targetPos + my.right * offset.x + my.up * offset.y;

        CheckForCollision(targetPos, targetWithOffset);
        negDistance.z = -currentDistance;

        targetWithOffset += GetContextualOffset(camRotation * negDistance + targetWithOffset);

        camPosition = camRotation * negDistance + targetWithOffset;
    }

    Vector3 lastFrameOffset;
    Vector3 velocity = Vector3.zero;
    Vector3 lastFrameCamPos;
    void SmoothMovement()
    {
        float t = smoothMovement ? deltaTime / smoothDamp : 1;
        // damping value is the inverse of speed, we simply give the camera a speed of 1/smoothDamp
        // so deltaTime * speed = deltaTime * 1/smoothDamp = deltaTime/smoothDamp

        if (float.IsNaN(lastFrameCamPos.x) || float.IsNaN(lastFrameCamPos.y) || float.IsNaN(lastFrameCamPos.z))
            lastFrameCamPos = my.position;

        if (float.IsNaN(camPosition.x) || float.IsNaN(camPosition.y) || float.IsNaN(camPosition.z))
            camPosition = my.position;

        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFov, fovDamp * deltaTime);
        my.position = SmoothApproach(my.position, lastFrameCamPos, camPosition, t);
        my.rotation = Quaternion.Lerp(my.rotation, camRotation, t); // TODO: only local space calculation?

        if (dontSmoothNextFrame)
        {
            my.position = camPosition;
            my.rotation = camRotation;
            dontSmoothNextFrame = false;
        }

        lastFrameOffset = target.position - my.position;
        lastFrameCamPos = camPosition;
    }

    Vector3 SmoothApproach(Vector3 pastPosition, Vector3 pastTargetPosition, Vector3 targetPosition, float t)
    {
        if (t == 0) return pastPosition;

        Vector3 v = (targetPosition - pastTargetPosition) / t;
        Vector3 f = pastPosition - pastTargetPosition + v;
        //print("smooth approach v: " + v + " | f: " + f + " | targetPos: " + targetPosition + " | t: " + t + " | Exp(t): " + Mathf.Exp(t));
        return targetPosition - v + f * Mathf.Exp(-t);
    }
    #endregion

    #region Inputs and States

    void GetInputsAndStates()
    {
        playerState = player.CurrentState;
        playerVelocity = target.InverseTransformVector((Quaternion.AngleAxis(Vector3.Angle(worldUp, target.up), (Vector3.Cross(worldUp, target.up) != Vector3.zero ? Vector3.Cross(worldUp, target.up) : Vector3.forward))) * player.MovementInfo.velocity);
        // TODO: ask someone to make the player velocity ACTUALLY local and not just rotated from the up vector

        input.x = Input.GetAxis("Mouse X") + Input.GetAxis("RightStick X");
        input.y = Input.GetAxis("Mouse Y") + Input.GetAxis("RightStick Y");

        if (Time.time < ignoreInputEnd)
        {
            input.x *= 0;
            input.y *= 0;
        }

        if (state == eCameraState.HomeDoor)
        {
            HomeDoorState();
            return; // Dans ce cas, osef de tout le reste
        }

        if (eclipseFX)
            eclipseFX.camSpeed = input;

        bool isGrounded = (playerState & (ePlayerState.move | ePlayerState.stand | ePlayerState.slide | ePlayerState.wallRun | ePlayerState.hover)) != 0;
        float slopeValue = CheckGroundAndReturnSlopeValue();

        // TODO: Il nous faut ptet une fonction SetState() pour pouvoir faire des trucs uniquement lors d'un changement de State

        // TODO: place FOV action somewhere that makes more sense ??
        targetFov = fovBasedOnPitch.Lerp(fovFromRotation.Evaluate(pitchRotationLimit.InverseLerp(pitch)));

        if (playerState == ePlayerState.dash)
        { // DASH

            float dot = Vector3.Dot(target.forward, target.parent.forward);
            if (dot < dashDotLimit)
                ResetCamera(slopeValue, dashDamp);

            targetFov += dashFovSupplement;
            additionalDistance = dashDistance;
        }

        if (inverseFacingDirection && Input.GetAxis("Vertical") >= 0)
        {
            startFacingDirection ^= true;
            inverseFacingDirection = false;
        }

        if (input.sqrMagnitude != 0)
        { // Contrôle manuel
            state = eCameraState.PlayerControl;
            resetType = eResetType.None;
            manualPitch = pitch - slopeValue;
            SetTargetRotation(null, null, smoothDamp);

        }
        else if (state != eCameraState.Resetting && !photoMode)
        {

            if (state == eCameraState.PlayerControl)
            { // Si on était en control Manuel avant
                if (!photoMode)
                    AllowAutoReset(true, false); // On autorise l'auto reset
                state = eCameraState.Default; // On passe en default state
            }

            // Auto reset après quelques secondes sans Input
            if (Time.time > lastInput + timeBeforeAutoReset && canAutoReset)
            {
                if (overriden)
                    state = eCameraState.Overriden;
                else if (axisAligned.sqrMagnitude != 0)
                    AlignWithAxis();
                else if (nearPOI)
                    LookAtTargetPOI();
            }

            if (isGrounded)
                GroundStateCamera(slopeValue);
            else
                AirStateCamera(slopeValue);

            if (playerState == ePlayerState.wallRun)
                WallRunCamera();

            if (playerState == ePlayerState.glide || playerState == ePlayerState.graviswap)
            {
                SetTargetRotation(-2 * playerVelocity.y + defaultPitch, GetYawBehindPlayer(), resetDamp);
                state = eCameraState.FollowBehind;
            }
            if (playerState == ePlayerState.phantom)
            {
                deltaTime = Time.unscaledDeltaTime;
                SetTargetRotation(-2 * playerVelocity.y + defaultPitch, GetYawBehindPlayer(), resetDamp);
                additionalDistance = 5;
                state = eCameraState.FollowBehind;
            }
        }

        if (Input.GetButton("ResetCamera"))
        {
            ResetCamera(slopeValue);
            targetFov += resetCameraFovSupplement;
        }
    }

    void ResetCamera(float slopeValue = 0, float? damp = null)
    {
        manualPitch = defaultPitch;
        zoomValue = distance;
        offsetFar = trueOffsetFar;

        AllowAutoReset(true);

        if (damp == null)
            damp = resetDamp;

        facingTime = -1;
        state = eCameraState.Resetting;

        bool isFalling = playerState == ePlayerState.air && playerVelocity.y < 0;

        bool aboveGround = Physics.Raycast(target.position + (playerVelocity.z * Vector3.forward + playerVelocity.x * Vector3.right) / 4, -target.up, maxJumpHeight, controller.collisionMask);
        // dans les airs, la caméra pointe vers le bas
        if (isFalling && !aboveGround)
        { // on n'utilise pas isGrounded ici car cet état est spécifique au fait de tomber
            resetType = eResetType.ManualAir;
            SetTargetRotation(pitchRotationLimit.max, GetYawBehindPlayer(), (float)damp);
        }
        else
        { // sinon, elle se met derrière le joueur
            resetType = eResetType.ManualGround;
            SetTargetRotation(defaultPitch + slopeValue, GetYawBehindPlayer(), (float)damp);
        }
    }

    void GroundStateCamera(float slopeValue)
    {

        if (additionalDistance != 0 && !inPanorama)
            additionalDistance = 0;

        if (resetType == eResetType.ManualAir)
            StopCurrentReset();

        if (state == eCameraState.Air)
        { // Si on était dans les airs avant
            if (!onEdgeOfCliff)
                manualPitch = defaultPitch; // On reset le pitch si on n'est pas au bord d'une falaise
            AllowAutoReset(true, true);
        }

        if (state < eCameraState.Overriden)
        {
            if (playerVelocity != Vector3.zero)
            {
                state = eCameraState.Default;
                SetTargetRotation(slopeValue + manualPitch, null, resetDamp);

            }
            else
            {
                state = eCameraState.Idle;
                SetTargetRotation(slopeValue + manualPitch, null, resetDamp);
                inverseFacingDirection = false;
                startFacingDirection = currentFacingDirection;
            }
        }
    }

    void AirStateCamera(float slopeValue)
    {
        if (state >= eCameraState.Overriden)
            return;

        state = eCameraState.Air;
        AllowAutoReset(true);

        bool isFalling = playerVelocity.y < 0;

        if (isFalling && !photoMode)
        { // je suis en train de tomber
            if (additionalDistance > -distanceReductionWhenFalling) // alors je zoom vers le perso
                additionalDistance -= deltaTime / autoResetDamp;

            bool aboveGround = Physics.Raycast(target.position + (playerVelocity.z * Vector3.forward + playerVelocity.x * Vector3.right) / 4, -target.up, maxJumpHeight, controller.collisionMask);
            if (!aboveGround) // je suis au-dessus du vide donc je me penche vers le bas
                SetTargetRotation(pitchRotationLimit.max, null, fallingDamp);

        }
        else
        { // je suis en train de sauter
            additionalDistance = 0;

            if (manualPitch <= defaultPitch + slopeValue) // ma caméra a manuellement été pointée vers le haut donc je la replace à son pitch par défaut
                manualPitch = targetPitch = defaultPitch + slopeValue;
        }
    }

    [Header("WallRun")]
    [SerializeField] float facingWallBuffer = 0.5f;
    [SerializeField] float wallRunDamp = 0.5f;

    void WallRunCamera()
    {
        Vector3 newYaw = Vector3.Cross(target.parent.up, controller.collisions.lastWallNormal);
        
        if (Vector3.Dot(newYaw, target.parent.forward) < 0)
            newYaw *= -1;
        
        float dot = Vector3.Dot(newYaw, playerVelocity.normalized);

        if (Mathf.Abs(dot) < facingWallBuffer)
            dot = 0;
        
        newYaw = Vector3.Lerp(Quaternion.Inverse(targetSpace) * my.forward, newYaw, dot);
        
        resetType = eResetType.WallRun;
        AllowAutoReset(true, true);
        SetTargetRotation(defaultPitch, GetRotationTowardsDirection(newYaw).y, wallRunDamp);
        state = eCameraState.WallRun;
    }

    #endregion

    #region PhotoMode
    
    bool photoMode = false;
    Vector2 trueOffsetFar;
    string photoPath;

    PostProcessingBehaviour postProcess;
    PostProcessingProfile defaultProfile;
    int currentFilter = 0;
    bool postProcessWasOverriden;

    [SerializeField] PostProcessingProfile[] filters;


    void DebugCameraMovement()
    {
        if (Input.GetButtonDown("Back") || Input.GetKeyDown(KeyCode.F8))
        {
            photoMode ^= true;
            canZoom ^= true;

            if (photoMode)
            {
                StartCoroutine(_StartPhotoMode());
            }
            else
            {
                StartCoroutine(_StopPhotoMode());
            }
        }

        if (photoMode)
        {
            if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("MenuButton"))
            {
                StartCoroutine(_StopPhotoMode());

            } else if (Input.GetButtonDown("Jump")) {
                TakePicture();
            }

            deltaTime = Time.unscaledDeltaTime;

            Vector2 debugMove = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            offsetFar += debugMove * 0.2f;
        }

    }

    WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

    IEnumerator _StartPhotoMode()
    {
        yield return endOfFrame;

        StartPhotoMode();
    }

    IEnumerator _StopPhotoMode()
    {
        yield return endOfFrame;

        StopPhotoMode();
    }

    void StartPhotoMode()
    {
        photoMode = true;

        Time.timeScale = 0;
        deltaTime = Time.unscaledDeltaTime;

        AllowAutoReset(false, true);
        StopCurrentReset();

        autoAdjustPitch = false;
        autoAdjustYaw = false;

        Game.Utilities.EventManager.SendGamePausedEvent(this, new Game.Utilities.EventManager.GamePausedEventArgs(true));

        currentDistance = zoomValue = distance;
        lastHitDistance = idealDistance;

        gameController.UiController.SwitchState(Game.UI.MenuType.PhotoMode);
    }

    void StopPhotoMode()
    {
        photoMode = false;

        currentDistance = zoomValue = distance;
        offsetFar = new Vector2(0, 1);
        Time.timeScale = 1;
        Game.Utilities.EventManager.SendGamePausedEvent(this, new Game.Utilities.EventManager.GamePausedEventArgs(false));

        gameController.UiController.SwitchState(Game.UI.MenuType.HUD);
    }

    void TakePicture()
    {
        gameController.UiController.PhotoModeController.SetVisible(false, true);

        StartCoroutine(_WhiteFlash());

        if (!Directory.Exists(photoPath))
            Directory.CreateDirectory(photoPath);

        string date = System.DateTime.Now.ToString();
        date = date.Replace("/", "-");
        date = date.Replace(" ", "_");
        date = date.Replace(":", "-");
        ScreenCapture.CaptureScreenshot(photoPath + date + ".png");

    }

    ColorOverlay whiteScreen;

    IEnumerator _WhiteFlash()
    {
        if (!whiteScreen)
            whiteScreen = GetComponent<ColorOverlay>();

        whiteScreen.color = Color.white;
        whiteScreen.intensity = 0;
        whiteScreen.blend = ColorOverlay.BlendMode.Normal;

        float flashHalfTime = 0.05f;

        for (float elapsed = 0; elapsed < flashHalfTime; elapsed += Time.unscaledDeltaTime)
        {
            whiteScreen.intensity = Mathf.Pow(elapsed / flashHalfTime, 2);

            yield return null;
        }

        flashHalfTime = 0.1f;

        gameController.PlayerController.InteractionController.SetNeedleActive(false);
        for (float elapsed = 0; elapsed < flashHalfTime; elapsed += Time.unscaledDeltaTime)
        {
            whiteScreen.intensity = 1 - Mathf.Pow(elapsed / flashHalfTime, 2);

            yield return null;
        }
        whiteScreen.intensity = 0;

    }

    #endregion

    #region Rotation

    void DoRotation()
    {
        if (state == eCameraState.HomeDoor)
            return; // Dans ce cas, osef de tout le reste

        rotationSpeed.x = Mathf.Lerp(minRotationSpeed.x, maxRotationSpeed.x, currentDistance / idealDistance);
        rotationSpeed.y = Mathf.Lerp(minRotationSpeed.y, maxRotationSpeed.y, currentDistance / idealDistance);

        float clampedX = Mathf.Clamp(input.x * (idealDistance / currentDistance), -mouseSpeedLimit.x, mouseSpeedLimit.x); // Avoid going too fast (makes weird lerp)
        if (invertAxis.x) clampedX = -clampedX;
        yaw += clampedX * rotationSpeed.x * deltaTime;

        if (input.x == 0 && !photoMode)
            yaw += VelocityInfluence() * rotationSpeed.x * deltaTime;

        float clampedY = Mathf.Clamp(input.y * (idealDistance / currentDistance), -mouseSpeedLimit.y, mouseSpeedLimit.y); // Avoid going too fast (makes weird lerp)
        if (invertAxis.y) clampedY = -clampedY;
        pitch -= clampedY * rotationSpeed.y * deltaTime;
        pitch = pitchRotationLimit.Clamp(pitch);

        if (state != eCameraState.PlayerControl)
            AutomatedMovement();

        if (float.IsNaN(pitch)) pitch = defaultPitch;

        if (float.IsNaN(yaw)) yaw = GetYawBehindPlayer();
        
        camRotation = targetSpace * Quaternion.Euler(pitch, yaw, 0);
    }

    void SetTargetRotation(Vector2 rotation, float damp)
    {
        autoAdjustPitch = true;
        autoAdjustYaw = true;
        targetPitch = rotation.x;
        targetYaw = rotation.y;
        autoDamp = damp;
    }

    void SetTargetRotation(float? newTargetPitch, float? newTargetYaw, float damp)
    {
        autoAdjustPitch = newTargetPitch != null;
        autoAdjustYaw = newTargetYaw != null;
        targetPitch = newTargetPitch ?? targetPitch;
        targetYaw = newTargetYaw ?? targetYaw;
        autoDamp = damp;
    }

    Vector3 worldForward = new Vector3(0, 0, 1);
    Vector3 worldUp = new Vector3(0, 1, 0);

    Vector2 GetRotationTowardsPoint(Vector3 point)
    {
        Vector3 direction = point - camPosition;
        float distance = direction.magnitude; // TODO: check if that's actually necessary?
        direction /= distance;
        return GetRotationTowardsDirection(direction);
    }

    Vector2 GetRotationTowardsDirection(Vector3 direction)
    {
        Quaternion q = Quaternion.LookRotation(direction, target.up);
        return Quaternion.Inverse(targetSpace) * new Vector2(q.eulerAngles.x, q.eulerAngles.y);
    }

    float GetPitchTowardsPoint(Vector3 point)
    {
        return GetRotationTowardsPoint(point).x;
    }

    float GetYawTowardsPoint(Vector3 point)
    {
        return GetRotationTowardsPoint(point).y;
    }

    float GetYawBehindPlayer()
    {
        return SignedAngle(targetSpace * worldForward, target.parent.rotation * worldForward, target.up);
    }

    float SignedAngle(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * 57.29578f; // Radian to Degrees constant
    }
    #endregion

    #region Collisions

    bool blockedByAWall;
    float lastHitDistance;
    void CheckForCollision(Vector3 targetPos, Vector3 targetWithOffset)
    {
        Vector3 rayStart = targetPos;

        negDistance.z = -idealDistance;
        Vector3 rayEnd = camRotation * negDistance + targetWithOffset;

        RaycastHit hit;
        blockedByAWall = Physics.SphereCast(rayStart, rayRadius, rayEnd - rayStart, out hit, idealDistance, blockingLayer);
        //Debug.DrawLine(rayStart, rayEnd, Color.yellow);

        if (blockedByAWall && hit.distance > 0) // If we hit something, hitDistance cannot be 0, nor higher than idealDistance
            lastHitDistance = Mathf.Min(hit.distance - rayRadius, idealDistance);
        float fixedDistance = blockedByAWall ? lastHitDistance : idealDistance;
        
        // If collide, use collisionDamp to quickly get in position and not be blocked by a wall
        // If not colliding, slowly get back to the idealPosition using noCollisionDamp
        currentDistance = Mathf.Lerp(currentDistance, fixedDistance, fixedDistance < currentDistance + .1f ? deltaTime / dampWhenColliding : deltaTime / dampAfterColliding);
    }
    #endregion

    #region Additional Rotation Factors

    [Header("Additional Factors")]
    public float playerVelocityInfluence = 0.1f;
    public float velocityInfluenceDelay = 2f;
    public float minimumPlayerVelocity = 3f;
    public float uTurnInfluence = 2f;
    public float cornerInfluence = 2f;
    public float wallDistanceCheck = 4;
    public AnimationCurve velocityInfluenceCurve;

    float lastVelocityX;
    Vector3 turnDirection;
    float velocityInfluenceTimer, cornerInfluenceTimer;

    /// <summary>
    /// Calculates additional camera movement based on player's movements
    /// </summary>
    float VelocityInfluence()
    {

        if (Physics.Raycast(target.position, target.parent.forward, wallDistanceCheck, blockingLayer))
            return 0;

        bool uTurn = playerVelocity.z < -minimumPlayerVelocity;
        bool turn = Mathf.Abs(playerVelocity.x) > minimumPlayerVelocity;

        if (velocityInfluenceTimer > 0 && !uTurn && (!turn || Sign(playerVelocity.x) != Sign(lastVelocityX)))
            velocityInfluenceTimer = 0;
        else if (velocityInfluenceTimer < velocityInfluenceDelay)
            velocityInfluenceTimer += deltaTime;

        float resolvedInfluence = lastVelocityX = playerVelocity.x;

        if (uTurn)
            resolvedInfluence += uTurnInfluence * Sign(lastVelocityX);

        resolvedInfluence *= playerVelocityInfluence * velocityInfluenceCurve.Evaluate(velocityInfluenceTimer);

        /*if (turn && !uTurn) // Corner Influence
        {
            if (cornerInfluenceTimer < 1)
                cornerInfluenceTimer += deltaTime;
            resolvedInfluence += CornerInfluence() * cornerInfluenceTimer;
        } else if (cornerInfluenceTimer > 0)
            cornerInfluenceTimer -= deltaTime;*/

        return resolvedInfluence;
    }

    float CornerInfluence()
    {
        float cornerInfluence = 0;
        Quaternion fromTo = Quaternion.FromToRotation(target.forward, target.parent.forward);

        turnDirection.x = playerVelocity.x;
        Vector3 rotation = fromTo * turnDirection;
        Vector3 offset = worldForward * Mathf.Abs(turnDirection.x);

        Vector3 forward = target.TransformVector(fromTo * worldForward);

        //Vector3 transformedAnticipated = target.TransformVector(rotation + offset);
        Vector3 transformedDirection = target.TransformVector(rotation);
        Vector3 transformedDelayed = target.TransformVector(rotation - offset);

        //Ray anticipatedCheck = new Ray(target.position, transformedAnticipated);
        Ray cornerCheck = new Ray(target.position + forward + characterUp, transformedDirection);
        Ray delayedCheck = new Ray(target.position + forward + characterUp, transformedDelayed);

        RaycastHit hit;

        //if (Physics.Raycast(anticipatedCheck, wallDistanceCheck, blockingLayer))
        //	resolvedInfluence += cornerInfluence * Sign(lastVelocityX);
        if (Physics.Raycast(delayedCheck, out hit, wallDistanceCheck, blockingLayer))
            cornerInfluence += this.cornerInfluence * Sign(lastVelocityX);
        if (Physics.Raycast(cornerCheck, out hit, wallDistanceCheck, blockingLayer))
            cornerInfluence += this.cornerInfluence * Sign(lastVelocityX);

        // TODO: check for wall in the opposite direction
        // if there are walls on both sides, reset camera and return 0 ?

        /*if (hit.normal.sqrMagnitude != 0) {
			print("playerVelocity: " + playerVelocity + " wallNormal: " + hit.normal);
			SetTargetRotation(null, GetRotationTowardsDirection(Vector3.Cross(target.up, hit.normal)).y, resetDamp);
		}*/

        return cornerInfluence;
    }

    float Sign(float v)
    {
        return v >= 0 ? 1 : -1;
    }

    #endregion

    #region Slopes and Cliffs

    [Header("Slopes")]
    public float limitVertical = 0.7f;
    public float minSlopeLength = 1;

    /// <summary>
    /// Checks whether we're on a cliff or on a slope and returns the value of that slope
    /// </summary>
    float CheckGroundAndReturnSlopeValue()
    {
        Vector3 groundNormal = controller.collisions.currentGroundNormal;
        Vector3 targetUp = target.up;
        Vector3 targetPos = target.position;
        Vector3 playerForward = player.transform.forward;

        if (groundNormal.y < limitVertical)
            groundNormal = targetUp; // Si on est quasi à la verticale, on considère qu'on est contre un mur, on repasse en caméra normale

        // Si on est au sol et qu'il n'y a pas de mur devant
        if ((playerState & (ePlayerState.move | ePlayerState.stand)) != 0 && !Physics.Raycast(targetPos, player.transform.forward, 1, controller.collisionMask))
        {

            RaycastHit groundInFront;
            float yOffset = (1 - groundNormal.y) * 10;
            float depth = cliffMinDepth + yOffset;
            Vector3 rayStart = targetPos + playerForward * distanceToCheckGroundForward + targetUp * yOffset;

            if (Physics.Raycast(rayStart, -targetUp, out groundInFront, depth, controller.collisionMask))
            {

                Debug.DrawRay(rayStart, -targetUp * depth, Color.red);
                NotOnEdgeOfCliff(); // Y a du sol devant donc on n'est pas au bord d'une falaise
                Vector3 groundInFrontNormal = Quaternion.Inverse(targetSpace) * groundInFront.normal;

                RaycastHit groundFurther;

                if (groundInFrontNormal.y > 0.999f)
                    groundNormal = targetUp; // Si devant c'est environ du sol plat, on reset slopeValue; pas besoin de calculs en plus

                else if (groundInFrontNormal.y > limitVertical)
                { // si la pente devant n'est pas quasi verticale (ie un mur) et a plus de x degrés

                    yOffset = (1 - groundInFrontNormal.y) * 10;
                    depth = cliffMinDepth + yOffset;
                    rayStart = targetPos + playerForward * (distanceToCheckGroundForward + minSlopeLength) + targetUp * yOffset;

                    // on check entre le sol actuel et le sol devant pour voir la taille d'une pente
                    if (Physics.Raycast(rayStart, -targetUp, out groundFurther, depth, controller.collisionMask))
                    {

                        Vector3 groundFurtherNormal = Quaternion.Inverse(targetSpace) * groundFurther.normal;
                        Debug.DrawRay(rayStart, -targetUp * depth, Color.red);

                        // si groundfurther ~= groundNormal, on ignore groundinfront, c'est un petit bump
                        if (Vector3.Dot(groundNormal, groundFurtherNormal) < 0.999f)
                        {
                            // on fait la moyenne entre les pentes
                            groundNormal += groundInFront.normal + groundFurther.normal;
                            groundNormal.x /= 3;
                            groundNormal.y /= 3;
                            groundNormal.z /= 3;
                        }
                    }

                } // Sinon : ne rien faire, on garde le sol actuel

            }
            else
            {// on est au sol, y a pas de mur devant, et y a pas de sol devant non plus, donc on est au bord d'une falaise
                onEdgeOfCliff = true;
                AllowAutoReset(false);
                // TODO: pencher la caméra automatiquement lorsque l'on arrive près d'un bord (mais : seulement si le sol actuel est plat ?)
            }

        }
        else // soit on n'est pas au sol, soit on est au sol mais y a un mur devant, donc on n'est pas au bord d'une falaise
            NotOnEdgeOfCliff();

        return Vector3.Dot(Vector3.ProjectOnPlane(my.forward, target.parent.up), groundNormal) * 60;
        // Ici on recalcule le forward en aplatissant celui de la caméra pour éviter des erreurs quand le perso tourne
    }

    void NotOnEdgeOfCliff()
    {
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

    void Zoom(float value)
    {
        if (value != 0) zoomValue = zoomDistance.Clamp(zoomValue - value * zoomSpeed);
    }

    void ZoomFromCeiling()
    {
        RaycastHit hit;
        if (Physics.Raycast(target.position, target.up, out hit, maxDistance, blockingLayer))
        {

            zoomValue = hit.distance;
            SetTargetRotation(defaultPitch, null, autoResetDamp);
            AllowAutoReset(true);

        }
        else if (zoomValue != distance)
            zoomValue = distance;
    }

    IEnumerator _ZoomAt(float value, float damp)
    {
        while (Mathf.Abs(zoomValue - value) > 0.01f)
        {
            zoomValue = Mathf.Lerp(zoomValue, value, deltaTime / damp);
            yield return null;
        }
    }

    /// <summary>
    /// Zoom in a (0,1) value between minimum and maximum distance
    /// </summary>
    public void ZoomAt(float value, float damp)
    {
        zoomRoutine = StartCoroutine(_ZoomAt(value, damp));
    }

    public void ResetZoom()
    {
        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);
        zoomValue = distance;
    }
    #endregion

    #region Panorama Mode
    float panoramaTimer = 0;
    bool inPanorama = false;

    void DoPanorama()
    {
        if (!Input.anyKey && input.sqrMagnitude == 0 && playerVelocity == Vector3.zero)
            panoramaTimer += deltaTime;
        else
        {
            panoramaTimer = 0;
            if (inPanorama)
            {
                additionalDistance = 0;
                inPanorama = false;
            }
        }
        if (panoramaTimer >= timeToTriggerPanorama)
        {
            additionalDistance = Mathf.Lerp(additionalDistance, panoramaAdditionalDistance, deltaTime / panoramaDezoomDamp);
            inPanorama = true;
        }
    }

    #endregion

    #region Contextual Offset
    Vector3 contextualOffset;
    bool cameraBounce;

    public void SetVerticalOffset(float verticalOffset)
    {
#if FALSE
        recoilIntensity = recoilOnImpact * verticalOffset;
        contextualOffset.y = -verticalOffset;
        cameraBounce = true;
#endif
    }

    Vector3 GetContextualOffset(Vector3 potentialPosition)
    {
        //Vector3 offset = new Vector3(0, 0, 0);

        if (onEdgeOfCliff)
        { // Appliquer l'offset contextuel sur le bord des falaises
            float cliffOffsetDistance = Mathf.Max(0, pitch) / cliffOffsetDivision.Lerp(currentDistance / maxDistance);
            //Debug.DrawLine(potentialPosition, potentialPosition + player.transform.forward * cliffOffsetDistance, Color.white);

            RaycastHit hit; // tirer un rayon pour voir s'il y a un mur en travers de l'offset prévu, réduire l'offset si c'est le cas
            if (Physics.Linecast(potentialPosition, potentialPosition + player.transform.forward * cliffOffsetDistance, out hit, blockingLayer))
                cliffOffsetDistance = Mathf.Min(cliffOffsetDistance, hit.distance - rayRadius);

            contextualOffset = Vector3.Lerp(contextualOffset, player.transform.forward * cliffOffsetDistance, deltaTime / autoResetDamp);
        }
        else
            contextualOffset = Vector3.Lerp(contextualOffset, Vector3.zero, deltaTime / autoResetDamp);
        return contextualOffset;
        /*if (cameraBounce) {
            offset += contextualOffset.y * target.up * recoilIntensity;
            contextualOffset.y = Mathf.Lerp(contextualOffset.y, 0, deltaTime / smoothDamp);

            targetFov += contextualOffset.y * recoilFOVImpact;

            if (Mathf.Abs(contextualOffset.y) < .01f)
                cameraBounce = false;
        }
        else
            contextualOffset = Vector3.Lerp(contextualOffset, Vector3.zero, deltaTime / autoResetDamp);*/
        //return offset + contextualOffset;
    }
    #endregion

    #region Home Door

    [Header("Home Door")]
    [SerializeField] float homeDoorMaxZoom = 10;
    [SerializeField] float homeDoorFov = 40;

    float lastFrameZoomSign;
    Vector3 homeDoorPosition, homeDoorForward, homePosition;
    bool dontSmoothNextFrame;

    public void LookAtHomeDoor(Vector3 doorPosition, Vector3 doorForward, Vector3 homePosition)
    {
        AllowAutoReset(true, true);
        state = eCameraState.HomeDoor;
        homeDoorPosition = doorPosition;
        homeDoorForward = doorForward;
        this.homePosition = homePosition;
        lastFrameZoomSign = 1;
        targetFov = homeDoorFov; // Change FOV
    }

    public void StopLookingAtHomeDoor()
    {
        state = eCameraState.Default;

        ResetCamera(); // Reset the camera, ignoring any previous value
        yaw = targetYaw;
        pitch = targetPitch;
    }

    void HomeDoorState()
    {
        Vector3 playerPos = target.position;

        if ((playerPos - homeDoorPosition).sqrMagnitude > homeDoorMaxZoom * homeDoorMaxZoom
            && (playerPos - homePosition).sqrMagnitude > homeDoorMaxZoom * homeDoorMaxZoom)
            StopLookingAtHomeDoor(); // Si le joueur sort de la zone autour des portes, on reprend le comportement normal

        bool playerPassedPortal = (playerPos - homeDoorPosition).sqrMagnitude > (playerPos - homePosition).sqrMagnitude;
        Vector3 forward = playerPassedPortal ? Vector3.forward : homeDoorForward;
        Vector3 projected = Vector3.Project((playerPassedPortal ? homePosition : homeDoorPosition) - playerPos, forward);
        float playerPosValue = 2 * projected.magnitude / homeDoorMaxZoom;

        if (playerPassedPortal || (!playerPassedPortal && (my.position - playerPos).sqrMagnitude > (my.position - homeDoorPosition).sqrMagnitude))
            playerPosValue *= -1;
        float trueZoom = (homeDoorMaxZoom / 2) * playerPosValue + homeDoorMaxZoom / 2 - 0.01f;

        bool camPassedPortal = trueZoom < 0;

        Vector3 targetPos = camPassedPortal ? homePosition : homeDoorPosition;
        Vector3 otherSide = camPassedPortal ? homeDoorPosition : homePosition;

        if (Mathf.Sign(trueZoom) != lastFrameZoomSign)
        { // La caméra passe le portail, on switch de position
            dontSmoothNextFrame = true;
            Vector3 offset = my.position - otherSide;
            //my.position = targetPos + offset;
            //my.rotation = Quaternion.LookRotation(forward);

            offset = lastFrameCamPos - otherSide;
            lastFrameCamPos = targetPos + offset;
            // on change la position de la caméra lors de la frame précédente également pour éviter un lerp
        }
        lastFrameZoomSign = Mathf.Sign(trueZoom);

        targetPos.y += 2;
        forward = camPassedPortal ? Vector3.forward : homeDoorForward;

        RaycastHit hit;
        bool hitAWall = Physics.Raycast(targetPos, -forward, out hit, trueZoom, blockingLayer);
        if (hitAWall) // si y a un mur on évite de se mettre au travers
            trueZoom = hit.distance;

        camPosition = targetPos - forward * trueZoom;
        camRotation = Quaternion.LookRotation(forward);

        //

        bool alterPlayerForward = playerPassedPortal && !camPassedPortal;

        Vector3 characterUp = target.parent.up;
        forward = alterPlayerForward ? Vector3.forward : Vector3.ProjectOnPlane(my.forward, characterUp);
        target.LookAt(target.position + forward, characterUp); // Reoriente the character's rotator
    }

    #endregion

    #region Triggers

    public void EnterTrigger(CameraControlTrigger trigger)
    {
        currentTrigger = trigger;

        if (trigger.editZoom)
            ZoomAt(trigger.zoomValue, trigger.damp);

        switch (trigger.mode)
        {
            case CameraControlTrigger.CameraControl.PointOfInterest:
                SetPointOfInterest(trigger.target.position);
                break;
            case CameraControlTrigger.CameraControl.AlignWithForwardAxis:
                SetAxisAlignment(trigger.transform.forward, !trigger.lookInForwardDirection);
                break;
            case CameraControlTrigger.CameraControl.OverrideCameraTransform:
                OverrideCamera(trigger.target.position, trigger.target.eulerAngles, trigger.damp);
                break;
            default: break;
        }
        enablePanoramaMode = !trigger.disablePanoramaMode;
        ignoreInputEnd = Time.time + trigger.ignoreInput;
    }

    public void ExitTrigger(CameraControlTrigger trigger)
    {
        if (trigger != currentTrigger) return; // si le trigger duquel on sort n'est pas celui actif, rien à faire

        if (trigger.editZoom)
            ResetZoom();

        switch (trigger.mode)
        {
            case CameraControlTrigger.CameraControl.PointOfInterest:
                ClearPointOfInterest(trigger.target.position);
                break;
            case CameraControlTrigger.CameraControl.AlignWithForwardAxis:
                RemoveAxisAlignment(trigger.transform.forward);
                break;
            case CameraControlTrigger.CameraControl.OverrideCameraTransform:
                StopOverride();
                break;
            default: break;
        }
        enablePanoramaMode = true;
        currentTrigger = null;
        ignoreInputEnd = 0;
    }

    bool startFacingDirection, currentFacingDirection, inverseFacingDirection;
    float maxInverseFacingTime = 0.5f; // TODO: softcode that
    float facingTime = -1;
    float ignoreInputEnd;

    bool FacingDirection(Vector3 direction)
    {
        float dot = Vector3.Dot(target.parent.forward, direction);
        bool temp = currentFacingDirection ? dot > -0.8f : dot > 0.8f;
        if (facingTime == -1)
        {
            facingTime = 0;
            if (Input.GetAxis("Vertical") <= 0)
                temp ^= true;
            return startFacingDirection = currentFacingDirection = temp;

        }
        else if (temp != currentFacingDirection && playerVelocity.sqrMagnitude > 0.01f)
        {
            facingTime += deltaTime;
            if (facingTime >= maxInverseFacingTime)
            {
                facingTime = 0;
                inverseFacingDirection ^= true;
                return currentFacingDirection = temp;
            }
        }
        else
            facingTime = 0;
        return currentFacingDirection;
    }

    #region Point of Interest

    bool nearPOI, alwaysLookAt = true;
    Vector3 targetPOI;
    public void SetPointOfInterest(Vector3 point)
    {
        nearPOI = true;
        targetPOI = point;
        AllowAutoReset(true, true);
        facingTime = -1; // allow immediate alignment
    }

    public void ClearPointOfInterest(Vector3 point)
    {
        if (targetPOI != point)
            return; // Attempting to clear a point that is not currently set
        nearPOI = false;
        state = eCameraState.Default;
        resetType = eResetType.None;
    }

    bool IcanSee(Vector3 point)
    {
        //Debug.DrawLine(camPosition, point, Color.magenta, 0.5f);
        return !Physics.Linecast(camPosition, point, blockingLayer);
    }

    void LookAtTargetPOI()
    {
        if (alwaysLookAt || IcanSee(targetPOI) && FacingDirection((targetPOI - target.position).normalized))
        {
            state = eCameraState.LookAt;
            resetType = eResetType.POI;
            SetTargetRotation(GetRotationTowardsPoint(targetPOI), autoResetDamp);
        }
        else
            resetType = eResetType.None;
    }
    #endregion

    #region Align with Axis

    Vector3 axisAligned = Vector3.zero;
    bool faceBothWays;

    public void SetAxisAlignment(Vector3 direction, bool faceBothWays)
    {
        this.faceBothWays = faceBothWays;
        axisAligned = direction;
        AllowAutoReset(true, true);
        facingTime = -1; // allow instant alignment
        inverseFacingDirection = false;
    }

    void AlignWithAxis()
    {
        state = eCameraState.LookAt;
        if (!faceBothWays || FacingDirection(axisAligned))
            SetTargetRotation(GetRotationTowardsDirection(axisAligned), axisAlignDamp);
        else
            SetTargetRotation(GetRotationTowardsDirection(-axisAligned), axisAlignDamp);
        targetPitch += defaultPitch;
    }

    public void RemoveAxisAlignment(Vector3 direction)
    {
        if (axisAligned != direction)
            return; // Attempting to clear a direction that isn't currently set
        axisAligned = Vector3.zero;
    }
    #endregion

    #region Override Position and Rotation

    Vector3 overridePos, overrideRot;
    float overrideDamp;
    bool overriden;

    public void OverrideCamera(Vector3 position, Vector3 rotation, float damp)
    {
        state = eCameraState.Overriden;
        overriden = true;
        overridePos = position;
        overrideRot = rotation;
        autoDamp = overrideDamp = damp;
    }

    public void StopOverride()
    {
        overriden = false;
    }

    #endregion

    #endregion

}
