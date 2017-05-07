using UnityEngine;
using UnityEngine.SceneManagement;

public class ThirdPersonController : MonoBehaviour {

    public Transform rotator; // Used to get the rotation of the camera

    [Header("Movement")]
    public float walkForce = 10;
    public float runForce = 20;
    public float rotationSpeed = 12;
    public bool strafe;

    [Header("Jump")]
    public int numberOfJumps = 1;
    public float jumpForce = 5f;
    public float maxJumpForce = 13;
    public float jumpFactor = 10;
    public float gravity = 9.8f;
    public float maxFallingSpeed = 5f;

    float xForce, zForce;
    Vector3 direction;
    Vector3 distToGround;
    KeyCode jumpKey = KeyCode.Space;

    new ThirdPersonCamera camera;

    CharacterController controller;
    GameObject platform;
    Animator animator;
    ParticleSystem doubleJumpParticles, dashParticles;

    float verticalVelocity;
    bool isGrounded;
    float reachedMaxFallingSpeed;

    float lastForce;
    float currentForce {
        get {
            if (dashing) lastForce = dashForce;
            else
            if (controller.isGrounded)
                lastForce = Input.GetKey(KeyCode.LeftShift) ? runForce : walkForce;
            return lastForce;
        }
    }
    
    void Start() {
        camera = FindObjectOfType<ThirdPersonCamera>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        distToGround.y = - (GetComponent<Collider>().bounds.extents.y + .1f);
        
        if (FXManager.instance) {
            doubleJumpParticles = FXManager.instance.doubleJumpFX;
            doubleJumpParticles.transform.parent = null;
            dashParticles = FXManager.instance.dashFX;
            dashParticles.Stop();
        }
    }

    void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #region Jump
    int jumpsRemaining;
    bool jumping;
    void Jump() {
        if (jumpsRemaining == numberOfJumps && !controller.isGrounded)
            jumpsRemaining--; // The first jump can only be done on the ground
        if (jumpsRemaining > 0) {
            jumpsRemaining--;
            jumping = true;
            verticalVelocity = jumpForce;
            if(!controller.isGrounded && doubleJumpParticles) {
                if (doubleJumpParticles.isPlaying) {
                    doubleJumpParticles.Stop();
                    doubleJumpParticles.Clear();
                }
                doubleJumpParticles.transform.position = transform.position;
                doubleJumpParticles.Play();
            }
        }
    }
    #endregion

    #region Impact
    [Header("Impact")]
    public float fallImpact = 8;
    public MinMax impact = new MinMax(0.7f, 3);

    void Impact() {
        float impactStrength = Mathf.Min(reachedMaxFallingSpeed * fallImpact + impact.min, impact.max);
        camera.temporaryOffset = new Vector2(0, -impactStrength);
    }
    #endregion

    #region Dash
    bool dashing;
    [Header("Dash")]
    public float dashForce = 20;
    public float dashDuration = 0.5f;
    float timeDashed;

    void StartDash() {
        dashing = true;
        timeDashed = 0;
        if (dashParticles) dashParticles.Play();
    }
    #endregion

    void DoVerticalVelocity() {
        reachedMaxFallingSpeed = verticalVelocity <= -maxFallingSpeed ? reachedMaxFallingSpeed + deltaTime : 0;

        if (Input.GetKeyUp(jumpKey)) jumping = false;

        if (Input.GetKeyDown(jumpKey) && jumpsRemaining > 0) {
            Jump();
        }
        else if (Input.GetKey(jumpKey) && jumping && verticalVelocity < maxJumpForce) {
            verticalVelocity += jumpFactor * Time.deltaTime;
            if (verticalVelocity > maxJumpForce) jumping = false;
        }
        else if (controller.isGrounded) {
            if (reachedMaxFallingSpeed > 0)
                Impact();
            jumpsRemaining = numberOfJumps;
            verticalVelocity = -gravity * deltaTime;

        } else if (reachedMaxFallingSpeed == 0)
            verticalVelocity -= gravity * deltaTime;
    }

    float deltaTime;
    void Update() {
        deltaTime = Time.deltaTime;
        
        DoVerticalVelocity();
        
        xForce = Input.GetAxis("Horizontal") * currentForce;
        zForce = Input.GetAxis("Vertical") * currentForce;

        if (Input.GetKeyDown(KeyCode.V) && !dashing)
            StartDash();

        if (dashing) {
            zForce = dashForce;
            verticalVelocity = 0;
            timeDashed += Time.deltaTime;
            if (timeDashed > dashDuration) {
                dashing = false;
                if (dashParticles) dashParticles.Stop();
            }
        }

        if (strafe) {
            if (xForce != 0 || zForce != 0) // If we are moving, rotate in the direction of the camera
                transform.rotation = Quaternion.Lerp(transform.rotation, rotator.rotation, deltaTime * rotationSpeed);

            direction.x = deltaTime * xForce;
            direction.y = deltaTime * verticalVelocity;
            direction.z = deltaTime * zForce;
            direction = transform.TransformDirection(direction);
        } else {

            direction.x = xForce;
            direction.y = verticalVelocity;
            direction.z = zForce;
            direction = rotator.TransformPoint(direction);

            Quaternion rot = Quaternion.LookRotation(direction - transform.position);
            rot.eulerAngles = new Vector3(0, rot.eulerAngles.y, 0);

            if (xForce != 0 || zForce != 0)
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, deltaTime * rotationSpeed);

            // rotate in the direction we are moving, no direction.x value
            direction.x = 0;
            direction.y = verticalVelocity;
            direction.z = Mathf.Min(Mathf.Abs(zForce) + Mathf.Abs(xForce), currentForce);
            direction = transform.TransformDirection(direction);
        }

        controller.Move(deltaTime * direction);
        UpdateAnimator(controller.velocity);
        isGrounded = Physics.Linecast(transform.position, transform.position + distToGround);

		Debug.DrawLine(controller.transform.position + Vector3.up, controller.transform.position+ controller.velocity.normalized * 5, Color.blue);
    }


    [SerializeField]
    float m_RunCycleLegOffset = 0.2f;
    [SerializeField]
    float m_AnimSpeedMultiplier = 1f;
    const float k_Half = 0.5f;
    void UpdateAnimator(Vector3 move) {
		
        //Hotfixes
        //if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
       // isGrounded = controller.isGrounded;
		RaycastHit hit;
		if (Physics.Raycast(transform.GetComponent<CharacterController>().bounds.min, Vector3.down,out hit, 1f))
		{
			if (hit.collider.tag != "Player")
			{
				isGrounded = true;
				//Debug.Log("grounded");
			}
		}
		else
		{
			isGrounded = false;
		}
		//isGrounded = true;
        //animator.applyRootMotion = controller.isGrounded;
        animator.applyRootMotion = false;

        //float m_TurnAmount = Mathf.Atan2(move.x, move.z);

        // update the animator parameters
        animator.SetFloat("Forward", move.z, 0.1f, Time.deltaTime);
        //animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
        //animator.SetBool("Crouch", m_Crouching);
        animator.SetBool("OnGround", isGrounded);
        if (!isGrounded) {
			animator.SetFloat("Jump", verticalVelocity);
        }

        // calculate which leg is behind, so as to leave that leg trailing in the jump animation
        // (This code is reliant on the specific run cycle offset in our animations,
        // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
        float runCycle =
            Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
        float jumpLeg = (runCycle < k_Half ? 1 : -1) * move.z;
        if (isGrounded) {
            animator.SetFloat("JumpLeg", jumpLeg);
        }

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (isGrounded && move.magnitude > 0) {
            animator.speed = m_AnimSpeedMultiplier;
        } else {
            // don't use that while airborne
            animator.speed = 1;
        }
    }
}
