using UnityEngine;

namespace Game.Player.CharacterController
{
    public class PhantomController : MonoBehaviour
    {
        //#############################################################################

        #region collider properties

        /// <summary>
        /// The center of the capsule used as the player collider.
        /// </summary>
        [Tooltip("The center of the capsule used as the player collider.")]
        public Vector3 center = new Vector3(0f, 0f, 0f);
        /// <summary>
        /// The radius of the capsule used as the player collider.
        /// </summary>
        [Tooltip("The radius of the capsule used as the player collider.")]
        public float radius = .5f;
        /// <summary>
        /// The height difference between the position and the center of the upper sphere of the capsule.
        /// </summary>
        [Tooltip("The height of the capsule.")]
        public float height = 1f;

        #endregion collider properties

        //#############################################################################

        #region cast variables

        /// <summary>
        /// The height difference between the two points of the capsule.
        /// </summary>
        [HideInInspector]
        public Vector3 capsuleHeightModifier;
        /// <summary>
        /// The safety margin used when casting for obstacles.
        /// </summary>
        [Tooltip("The safety margin used when casting for obstacles.")]
        public float skinWidth = 0.02f;
        /// <summary>
        /// The layer mask used to detect obstacles.
        /// </summary>
        [Tooltip("The layer mask used to detect obstacles.")]
        public LayerMask collisionMask;

        #endregion cast variables

        //#############################################################################
        
        [HideInInspector]
        public Transform myTransform;

        /// <summary>
        /// The number of collisions detected on this frame.
        /// </summary>
        int collisionNumber;
        Collider[] wallsOverPlayer;
        
		bool belowLastFrame;
        bool climbingStep;
        bool insideWallOnThisFrame;
        Vector3 stepOffset;
        Vector3 rebordOffset = Vector3.zero;

        [HideInInspector]
        public Quaternion playerAngle;
        RaycastHit hit;
        RaycastHit hit2;
        RaycastHit hit3;
        Vector3 wallDir;
        Vector3 velocityBeforeCollision = Vector3.zero;
        float timerBeforeForgetVBC = 0f;
        RaycastHit sideHit;

        //#############################################################################

        void Awake()
        {
            myTransform = transform;
            capsuleHeightModifier = new Vector3(0, height, 0);
        }

        //#############################################################################

        public Vector3 Move(Vector3 velocity)
        {

            var pos1 = myTransform.position;

            playerAngle = (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, transform.up), (Vector3.Cross(Vector3.up, transform.up) != Vector3.zero ? Vector3.Cross(Vector3.up, transform.up) : Vector3.forward)));

#if UNITY_EDITOR
            Debug.DrawRay(myTransform.position + playerAngle * center, velocity / Time.deltaTime, Color.green);
#endif

            collisionNumber = 0;
            //Recursively check if the movement meets obstacles
            velocity = CollisionDetection(velocity, myTransform.position + playerAngle * center, new RaycastHit());
            //Debug.Log("col num : " + collisionNumber);

            
            if (climbingStep)
            {
                transform.position += stepOffset;
                stepOffset = Vector3.zero;
            }

            Vector3 finalVelocity = Vector3.zero;
            //Check if calculated movement will end up in a wall, if so try to adjust movement

            finalVelocity = ConfirmMovement(velocity);

            if (timerBeforeForgetVBC > 0)
            {
                timerBeforeForgetVBC -= Time.deltaTime;
                /*if (velocityBeforeCollision != Vector3.zero)
                {
                    Debug.Log("checking for velocity before collision : " + velocityBeforeCollision);
                    Debug.DrawRay((myTransform.position + playerAngle * center), velocityBeforeCollision, Color.red);
                }*/
                if (velocityBeforeCollision != Vector3.zero && !Physics.CapsuleCast(
                    (myTransform.position + playerAngle * center) - (playerAngle * capsuleHeightModifier / 2),
                    (myTransform.position + playerAngle * center) + (playerAngle * capsuleHeightModifier / 2),
                    radius,
                    velocityBeforeCollision,
                    out hit,
                    velocityBeforeCollision.magnitude,
                    collisionMask))
                {
                    //Debug.Log("managed to add velocity from before collision");
                    finalVelocity = velocityBeforeCollision.normalized * finalVelocity.magnitude;
                    velocityBeforeCollision = Vector3.zero;
                }
                if (timerBeforeForgetVBC < 0f)
                {
                    velocityBeforeCollision = Vector3.zero;
                }
            }
            

            //**
            var pos2 = myTransform.position;
            if((pos2-pos1).magnitude > 0.01f)
            {
                //Debug.LogFormat("move={0}", (pos2 - pos1).magnitude.ToString());
            }
            //**

            return finalVelocity;
        }

        //#############################################################################

        Vector3 ConfirmMovement(Vector3 velocity)
        {

            var pos1 = myTransform.position;
            myTransform.Translate(velocity, Space.World);
            var pos2 = myTransform.position;
            
            /*
            if (myPlayer.CurrentState == ePlayerState.move)
            {
                Debug.LogFormat("input={0}", velocity.ToString());
                Debug.LogFormat("translation={0}; magnitude={1}", (pos2 - pos1).ToString(), (pos2 - pos1).magnitude.ToString());
                Debug.LogFormat("angle:{0}", Vector3.Angle(transform.up, Vector3.up));
                Debug.LogFormat("axis:{0}", Vector3.Cross(transform.up, Vector3.up));
                Debug.LogFormat("velocity / deltaTime = {0}", velocity / Time.deltaTime);
            }*/

            var result = (Quaternion.AngleAxis(Vector3.Angle(transform.up, Vector3.up), (Vector3.Cross(transform.up, Vector3.up) != Vector3.zero ? Vector3.Cross(Vector3.up, transform.up) : Vector3.forward))) * velocity /*/ Time.deltaTime*/;

            return result;
            //return velocity;
        }
        

        //Recursively check if the movement meets obstacles
        Vector3 CollisionDetection(Vector3 velocity, Vector3 position, RaycastHit oldHit)
        {
            Vector3 movementVector = Vector3.zero;
            RaycastHit hit;
            Vector3 veloNorm = velocity.normalized;
            float rayLength = velocity.magnitude;
            Vector3 newOrigin = position;

            //print("velocity : " + velocity*100 + "initial velocity : " + collisions.initialVelocityOnThisFrame*100);

            //Send a first capsule cast in the direction of the velocity
            if (Physics.CapsuleCast(
                newOrigin - (playerAngle * capsuleHeightModifier / 2),
                newOrigin + (playerAngle * capsuleHeightModifier / 2),
                radius,
                velocity,
                out hit,
                rayLength,
                collisionMask))
            {

                if (collisionNumber == 0 && velocityBeforeCollision == Vector3.zero)
                {
                    //Debug.Log("hey the velocity before collision is : " + velocity);
                    timerBeforeForgetVBC = 0.05f;
                    velocityBeforeCollision = velocity;
                }

                //print("met smth ! name : " + hit.collider.name + " at : " + hit.point + " distance : " + hit.distance);

                //When an obstacle is met, remember the amount of movement needed to get to the obstacle
                movementVector += veloNorm * (hit.distance - skinWidth);

                //Get the remaining velocity after getting to the obstacle
                Vector3 extraVelocity = (velocity - movementVector);

                collisionNumber++;
               
                
                //Stop the process if the script detected a lot of collisions
                if (collisionNumber < (climbingStep ? 4 : 5))
                {
                    Vector3 reflection = new Vector3();
                    //if it's the first obstacle met on this frame, project the extra velocity on the plane parallel to this obstacle
                    //if it's not the first, project on the line parallel to both the current obstacle and the previous one
                    //if the player is on the ground, count the ground as a first collision
                    //once the extra velocity has been projected, Call the function once more to check if this new velocity meets obstacle and do everything again


                    if (climbingStep)
                    {
                        // If the controller detected the player is going up a step, send a new detection from above the step
                        reflection = CollisionDetection(extraVelocity, position + movementVector + stepOffset, hit);
                    }
                    else
                    {
                        if (collisionNumber > 1)
                        {
                            if (Vector3.Dot(hit.normal, oldHit.normal) < 0)
                            {
                                //print("recu 1");
                                reflection = CollisionDetection(Vector3.Project(extraVelocity, Vector3.Cross(hit.normal, oldHit.normal)), position + movementVector, hit);
                            }
                            else
                            {
                                //print("recu 2");
                                reflection = CollisionDetection(Vector3.ProjectOnPlane(extraVelocity, hit.normal), position + movementVector, hit);
                            }
                        }
                        else
                        {
                            //print("recu 4");
                            reflection = CollisionDetection(Vector3.ProjectOnPlane(extraVelocity, hit.normal), position + movementVector, hit);
                        }
                    }
                    //Add all the reflections calculated together
                    movementVector += reflection;
                }
            }
            else
            {

                if (collisionNumber == 0)
                {
                    //print("stopped climbing 3");
                    climbingStep = false;
                }

                //if no obstacle is met, add the reamining velocity to the movement vector
                movementVector += velocity;
            }

            if (collisionNumber > 4)
            {
                Debug.LogWarning("whoa that was a lot of collisions there (" + collisionNumber + ").");
            }
            //			print("coll nmber : " + collisionNumber);
            //Debug.DrawRay(newOrigin, movementVector, Color.red);


            
            //return the movement vector calculated
            return movementVector;
        }

        //#############################################################################

        /// <summary>
        /// The struct containing three bools describing collision informations.
        /// </summary>
        public struct CollisionInfo
        {
            public bool above, below;
            public bool side, SlippySlope;

            public bool cornerNormal;
            public float stepHeight;

            public LayerMask collisionLayer;

            public Vector3 initialVelocityOnThisFrame;

            public Vector3 currentGroundNormal;
            public Vector3 currentWallNormal;
            public Vector3 lastWallNormal;

            public RaycastHit currentWallHit;

            public void Reset()
            {
                if (!below)
                    currentGroundNormal = Vector3.zero;
                above = below = false;
                side = SlippySlope = cornerNormal = false;
                currentWallNormal = Vector3.zero;
                currentWallHit= new RaycastHit();
            }

        }

        //#############################################################################

        void OnDrawGizmosSelected()
        {
            Quaternion playerAngle = (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, transform.up), (Vector3.Cross(Vector3.up, transform.up) != Vector3.zero ? Vector3.Cross(Vector3.up, transform.up) : Vector3.forward)));
            Gizmos.color = new Color(1, 0, 1, 0.75F);
            Vector3 upPosition = transform.position + playerAngle * (center + capsuleHeightModifier / 2);
            Vector3 downPosition = transform.position + playerAngle * (center - capsuleHeightModifier / 2);
            Gizmos.DrawWireSphere(upPosition, radius);
            Gizmos.DrawWireSphere(downPosition, radius);
        }

        //#############################################################################
    }
} //end of namespace