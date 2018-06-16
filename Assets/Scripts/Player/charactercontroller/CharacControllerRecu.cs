using UnityEngine;

namespace Game.Player.CharacterController
{
    public class CharacControllerRecu : MonoBehaviour
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
        /// <summary>
        /// The layer mask used to detect obstacles (doesn't detect clouds).
        /// </summary>
        [Tooltip("The layer mask used to detect obstacles (doesn't detect clouds).")]
        public LayerMask collisionMaskNoCloud;

        #endregion cast variables

        //#############################################################################

        public CollisionInfo collisions;
        public CapsuleCollider favourCollider;
        Transform myTransform;
        CharController myPlayer;

        /// <summary>
        /// The number of collisions detected on this frame.
        /// </summary>
        int collisionNumber;
        Collider[] wallsOverPlayer;

        /// <summary>
        /// The moving platform the player is currently on (null if not on a moving platform).
        /// </summary>
        MovingPlatform[] currentPFs;

        [HideInInspector]
        public Gravifloor currentGravifloor;

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

        void Start()
        {
            favourCollider = GetComponentInChildren<CapsuleCollider>();
            myTransform = transform;
            myPlayer = GetComponent<CharController>();
            favourCollider.center = center;
            favourCollider.radius = radius;
            favourCollider.height = height + radius * 2;
            capsuleHeightModifier = new Vector3(0, height, 0);
            collisions.collisionLayer = collisionMask;

            Utilities.EventManager.PreSceneChangeEvent += OnPreSceneChange;
        }

        private void OnPreSceneChange(object sender, Utilities.EventManager.PreSceneChangeEventArgs args)
        {
            collisions.Reset();
        }

        //#############################################################################

        public Vector3 Move(Vector3 velocity)
        {


            var pos1 = myTransform.position;

			belowLastFrame = collisions.below;
            if (collisions.side)
            {
                collisions.lastWallNormal = collisions.currentWallNormal;
            } else
            {
                collisions.lastWallNormal = Vector3.zero;
            }
			collisions.Reset();


            playerAngle = (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, transform.up), (Vector3.Cross(Vector3.up, transform.up) != Vector3.zero ? Vector3.Cross(Vector3.up, transform.up) : Vector3.forward)));
            collisions.initialVelocityOnThisFrame = velocity;

#if UNITY_EDITOR
            Debug.DrawRay(myTransform.position + playerAngle * center, velocity / Time.deltaTime, Color.green);
#endif

            collisionNumber = 0;
            //Recursively check if the movement meets obstacles
            velocity = CollisionDetection(velocity, myTransform.position + playerAngle * center, new RaycastHit());
            //Debug.Log("col num : " + collisionNumber);

            

            /*
            if (rebordOffset != Vector3.zero)
            {
                transform.position += rebordOffset;
                print("rebord is : " + rebordOffset);
                rebordOffset = Vector3.zero;
            }*/
            if (climbingStep)
            {
                transform.position += stepOffset;
                stepOffset = Vector3.zero;
            }

            Vector3 finalVelocity = Vector3.zero;
            //Check if calculated movement will end up in a wall, if so try to adjust movement
            wallsOverPlayer = Physics.OverlapCapsule(myTransform.position + playerAngle * (center - capsuleHeightModifier / 2) + velocity, myTransform.position + playerAngle * (center + capsuleHeightModifier / 2) + velocity
                , radius*.99f, collisionMaskNoCloud);
            

            if (wallsOverPlayer.Length == 0)
            {
                finalVelocity = ConfirmMovement(velocity);
            }
            else
            {
                //Debug.LogWarning("The player's inside " + wallsOverPlayer.Length + " wall(s) : " + wallsOverPlayer[0].name);
                insideWallOnThisFrame = true;
                finalVelocity = AdjustPlayerPosition(velocity);
            }

            if (timerBeforeForgetVBC > 0)
            {
                timerBeforeForgetVBC -= Time.deltaTime;
                if (velocityBeforeCollision != Vector3.zero && !Physics.CapsuleCast(
                    (myTransform.position + playerAngle * center) - (playerAngle * capsuleHeightModifier / 2),
                    (myTransform.position + playerAngle * center) + (playerAngle * capsuleHeightModifier / 2),
                    radius,
                    velocityBeforeCollision,
                    out hit,
                    velocityBeforeCollision.magnitude,
                    ((velocityBeforeCollision.y > 0 || myPlayer.CurrentState == ePlayerState.glide) ? collisionMaskNoCloud : collisionMask)))
                {
                    //Debug.Log("managed to add velocity from before collision");
                    //Debug.Log("added VBC : " + velocityBeforeCollision + " final velocity : " + finalVelocity);

                    finalVelocity = (Quaternion.AngleAxis(Vector3.Angle(transform.up, Vector3.up), (Vector3.Cross(transform.up, Vector3.up) != Vector3.zero ? Vector3.Cross(transform.up, Vector3.up) : Vector3.forward))) * velocityBeforeCollision.normalized * finalVelocity.magnitude;
                    velocityBeforeCollision = Vector3.zero;
                }
                if (timerBeforeForgetVBC < 0f)
                {
                    velocityBeforeCollision = Vector3.zero;
                }
            }

            //Update collision informations
            CollisionUpdate(velocity);
            
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
            //Debug.Log("char. velocity before : " + velocity);
            var result = (Quaternion.AngleAxis(Vector3.Angle(transform.up, Vector3.up), (Vector3.Cross(transform.up, Vector3.up) != Vector3.zero ? Vector3.Cross(transform.up, Vector3.up) : Vector3.forward))) * velocity /*/ Time.deltaTime*/;
            //Debug.Log("char. velocity after : " + result);

            return result;
            //return velocity;
        }

        Vector3 AdjustPlayerPosition(Vector3 velocity)
        {

			Vector3 result = Vector3.zero;
            Vector3 OutOfWallDirection = Vector3.zero;
            foreach (Collider wall in wallsOverPlayer)
            {
                //                print("collider : " + wall.name);
                bool foundSmth = false;
                Vector3 position = myTransform.position + velocity + playerAngle * center;
				int security = 5;
				do
                {
					security--;
                    foundSmth = Physics.Linecast(position, wall.transform.position, out hit, collisionMask);
                    if (foundSmth)
                        position = hit.point + (wall.transform.position - hit.point) * .01f;
				} while (foundSmth && !wall.gameObject.Equals(hit.transform.gameObject) && security > 0);

                if (foundSmth)
                {
                    OutOfWallDirection += hit.normal;
                }
                else
                {
                    OutOfWallDirection += transform.up;
                }
            }

            Physics.Raycast(myTransform.position + velocity + playerAngle * center, -OutOfWallDirection, out hit, radius + height / 2, collisionMask);
            OutOfWallDirection = OutOfWallDirection.normalized * (radius + height / 2);

            if (Physics.CapsuleCast(myTransform.position + velocity + playerAngle * (center - capsuleHeightModifier / 2) + OutOfWallDirection, myTransform.position + velocity + playerAngle * (center + capsuleHeightModifier / 2) + OutOfWallDirection
				, radius, -OutOfWallDirection, out hit, radius + height, collisionMask))
            {
				//print("hit distance : " + hit.distance + "distance adj : " + ((radius + height) - hit.distance));
                myTransform.Translate(OutOfWallDirection * ((radius + height) - hit.distance)/2, Space.World);
                result = ConfirmMovement(velocity);
            }
            else
            {
                myTransform.Translate(OutOfWallDirection, Space.World);
				result = ConfirmMovement(velocity);
            }

            if (belowLastFrame && !insideWallOnThisFrame && myPlayer.CurrentState != ePlayerState.jetpack)
			{
				print("must be below");
				int security = 5;
				while (!Physics.SphereCast(myTransform.position + playerAngle * (center - capsuleHeightModifier / 2) + myTransform.up * skinWidth * 2, radius, -myTransform.up, out hit, skinWidth * 4, collisionMask) && security > 0) {
					print("déplacement !");
					security--;
					myTransform.Translate(-myTransform.up * skinWidth * 2, Space.World);
				}
			}

            if (collisions.lastWallNormal != Vector3.zero && myPlayer.CurrentState == ePlayerState.wallRun)
            {
                print("must be on a wall");
                int security = 5;
                while (!Physics.CapsuleCast(myTransform.position + playerAngle * (center - capsuleHeightModifier / 2),
                                            myTransform.position + playerAngle * (center + capsuleHeightModifier / 2),
                                            radius,
                                            -collisions.lastWallNormal,
                                            out hit,
                                            skinWidth * 2,
                                            collisionMask) && security > 0)
                {
                    security--;
                    myTransform.Translate(-collisions.lastWallNormal * skinWidth * 2, Space.World);
                }
            }

            return result;
        }


        void CollisionUpdate(Vector3 velocity)
        {
            if (currentPFs != null)
            {
                foreach (MovingPlatform PF in currentPFs)
                {
                    PF.RemovePlayer();
                }
                currentPFs = null;
            }
            // EN TEST POUR BIEN RESTER AU SOL, à voir ce que ça vaut
            
            if (myPlayer.CurrentState == ePlayerState.stand || myPlayer.CurrentState == ePlayerState.move)
            {
                if (Physics.SphereCast(myTransform.position + myTransform.up * (radius + skinWidth), radius, -myTransform.up, out hit2, myPlayer.CharData.Physics.MaxStepHeight, collisionMask) && velocity.y < .2f)
                {
                    transform.position += -myTransform.up * (hit2.distance - skinWidth);
                }
            }

            //Send casts to check if there's stuff around the player and set bools depending on the results
            collisions.below = Physics.SphereCast(myTransform.position + playerAngle * (center - capsuleHeightModifier / 2) + myTransform.up * skinWidth * 2, radius, -myTransform.up, out hit, skinWidth * 4, collisionMask) || climbingStep;
            Debug.DrawRay(myTransform.position + playerAngle * (center - capsuleHeightModifier / 2) + myTransform.up * (skinWidth * 2-radius), -myTransform.up * skinWidth * 4, Color.blue);

            if (Physics.Raycast(myTransform.position, hit.point - myTransform.position, out hit2, height*2, collisionMask))
            {
                if (Vector3.Angle(hit.normal, hit2.normal) > 1f)
                    collisions.cornerNormal = true;
            }

            if (collisions.below && !climbingStep)
            {
                collisions.currentGroundNormal = hit.normal;
                if (currentPFs == null && hit.collider.CompareTag("MovingPlatform"))
                {
                    currentPFs = hit.collider.GetComponentsInParent<MovingPlatform>();
                    foreach (MovingPlatform PF in currentPFs)
                    {
                      PF.AddPlayer(myPlayer, hit.point);
                    }
                }
                if (hit.collider.CompareTag("Gravifloor") && (currentGravifloor == null || currentGravifloor != hit.collider.GetComponent<Gravifloor>()))
                {
                    currentGravifloor = hit.collider.GetComponent<Gravifloor>();
                    //if (currentGravifloor && Vector3.Dot(-currentGravifloor.gravityDirection, myTransform.up) > 0.6f)
                        currentGravifloor.AddPlayer(myPlayer, -hit.normal);
                }
                if (hit.collider.CompareTag("SlipperySlope"))
                {
                    collisions.SlippySlope = true;
                }
                else
                {
                    collisions.SlippySlope = false;
                }
                if (hit.collider.CompareTag("NotSlipperySlope"))
                {
                    collisions.NotSlippySlope = true;
                }
                else
                {
                    collisions.NotSlippySlope = false;
                }
            }

            if (currentGravifloor != null && (!collisions.below || !hit.collider.CompareTag("Gravifloor")))
            {
                currentGravifloor.RemovePlayer(!collisions.below);
                currentGravifloor = null;
            }

            if (collisions.below && !belowLastFrame)
            {
                myPlayer.myCamera.SetVerticalOffset(Vector3.Project(collisions.initialVelocityOnThisFrame, myTransform.up).magnitude);
				myPlayer.fxManager.ImpactPlay (Vector3.Project (collisions.initialVelocityOnThisFrame/Time.deltaTime, myTransform.up).magnitude);
            }

            collisions.above = Physics.SphereCast(myTransform.position + playerAngle * (center + capsuleHeightModifier / 2) - myTransform.up * skinWidth * 2, radius, myTransform.up, out hit, skinWidth * 4, collisionMask);
            if (collisions.above)
            {
                if (hit.collider.CompareTag("cloud"))
                {
                    collisions.above = false;
                }
                if (currentPFs == null && hit.collider.CompareTag("MovingPlatform"))
                {
                    currentPFs = hit.collider.GetComponentsInParent<MovingPlatform>();
                    foreach (MovingPlatform PF in currentPFs)
                    {
                        PF.AddPlayer(myPlayer, hit.point);
                    }
                }
            }

            //***********************************************************
            //side collision

            if (collisions.lastWallNormal != Vector3.zero)
            { //here we check if the player is still "touching" the wall he previously collided with
                RaycastHit searchHit;

                bool colliding = Physics.CapsuleCast(
                                                 myTransform.position + playerAngle * (center - capsuleHeightModifier / 2),
                                                 myTransform.position + playerAngle * (center + capsuleHeightModifier / 2),
                                                 radius,
                                                 -collisions.lastWallNormal,
                                                 out searchHit,
                                                 skinWidth * 2,
                                                 collisionMask
                                             );

                //Debug.Log("colliding : " + colliding);
                int security = 2;
                while (!colliding && security > 0 && !collisions.below && myPlayer.CurrentState == ePlayerState.wallRun)
                {
                    myTransform.position += -collisions.lastWallNormal * skinWidth * 2;

                    colliding = Physics.CapsuleCast(
                                                myTransform.position + playerAngle * (center - capsuleHeightModifier / 2),
                                                myTransform.position + playerAngle * (center + capsuleHeightModifier / 2),
                                                radius,
                                                -collisions.lastWallNormal,
                                                out searchHit,
                                                skinWidth * 2,
                                                collisionMask
                                            );
                    security--;
                }
                //Debug.Log("colliding after movement : " + colliding);
                if (!colliding)
                {
                    collisions.side = false;
                } else
                {
                    collisions.currentWallNormal = searchHit.normal;
                    collisions.currentWallHit = searchHit;
                    collisions.side = true;
                }
            } else 
            { //if the player is not touching the previous wall anymore, check for a new collision
                collisions.side = Physics.CapsuleCast(
                    myTransform.position + playerAngle * (center - capsuleHeightModifier / 2),
                    myTransform.position + playerAngle * (center + capsuleHeightModifier / 2),
                    radius,
                    Vector3.ProjectOnPlane(
                        collisions.initialVelocityOnThisFrame,
                        (collisions.below ? collisions.currentGroundNormal : myTransform.up)
                    ),
                    out sideHit,
                    skinWidth * 2,
                    collisionMask
                );

                if (collisions.side)
                {
                    //Debug.LogErrorFormat("hitName = {0}; hitNormal={1}", sideHit.collider.name, sideHit.normal);
                    collisions.currentWallNormal = sideHit.normal;
                    collisions.currentWallHit = sideHit;
                }
            }

            if (collisions.side)
            { //register with moving platforms
                if (currentPFs == null && sideHit.collider.CompareTag("MovingPlatform"))
                {
                    currentPFs = sideHit.collider.GetComponentsInParent<MovingPlatform>();
                    foreach (MovingPlatform PF in currentPFs)
                    {
                        PF.AddPlayer(myPlayer, sideHit.point);
                    }
                }
            }

            //***********************************************************
        } //end of CollisionUpdate


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
                ((veloNorm.y > 0 || myPlayer.CurrentState == ePlayerState.glide) ? collisionMaskNoCloud : collisionMask))
                )
            {

                if (collisionNumber == 0 && velocityBeforeCollision == Vector3.zero && (myPlayer.CurrentState != ePlayerState.slide || myPlayer.stateMachine.timeInCurrentState < 0.05f) && myPlayer.CurrentState != ePlayerState.wallRun
                    && (Vector3.Dot(velocity, myTransform.up) < -.9f || Vector3.Dot(velocity, myTransform.up) > .9f))
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
               


                #region step detection
                //Detect the obstacle met from above to check if it's a step
                //Debug.DrawRay(myTransform.position + movementVector + myTransform.up * (height + radius * 2) + Vector3.ProjectOnPlane(hit.point - myTransform.position, myTransform.up).normalized * (radius + skinWidth), -myTransform.up * (height + radius * 2), Color.red);
                //Debug.DrawRay(myTransform.position + movementVector + myTransform.up * (height + radius * 2), Vector3.ProjectOnPlane(hit.point - myTransform.position, myTransform.up).normalized * (radius + skinWidth), Color.red);

                if (
                    (myPlayer.CurrentState == ePlayerState.move || climbingStep) &&
                    (
                    Physics.Raycast(myTransform.position + movementVector + myTransform.up * (height + radius * 2) + Vector3.ProjectOnPlane(hit.point - myTransform.position, myTransform.up).normalized * (radius * 2 + skinWidth)
                                        , -myTransform.up, out hit2, height + radius * 2, collisionMask) || 
                    Physics.Raycast(myTransform.position + movementVector + myTransform.up * (height + radius * 2) + Vector3.ProjectOnPlane(hit.point - myTransform.position, myTransform.up).normalized * (radius + skinWidth)
                                        , -myTransform.up, out hit2, height + radius * 2, collisionMask)
                                        )
                                        &&
                    !Physics.Raycast(myTransform.position + movementVector + myTransform.up * (height + radius * 2), Vector3.ProjectOnPlane(hit.point - myTransform.position, myTransform.up), (radius*2 + skinWidth), collisionMask))
                {
                    collisions.stepHeight = (height + radius * 2) - hit2.distance;
                    // Once checked if it's a step, check if it's not too high, and if it's not a slope
                    //					print("detected collision at " + hit2.point + ", new ground angle : " + Vector3.Angle(hit2.normal, myTransform.up) + ", step height : " + collisions.stepHeight + ", dot product : " + Vector3.Dot(hit.normal, hit2.normal));
                    if (Vector3.Angle(hit2.normal, myTransform.up) < myPlayer.CharData.Physics.MaxSlopeAngle
                        && collisions.stepHeight < myPlayer.CharData.Physics.MaxStepHeight
                        && Vector3.Dot(hit.normal, hit2.normal) < .95f
                        && Vector3.Dot(hit.normal, hit2.normal) >= -.01f
                        )
                    {
                        stepOffset = myTransform.up * collisions.stepHeight;
                        //print("step added : " + stepOffset.y);
                        climbingStep = true;
                    }
                    else
                    {
                        //						print("stopped climbing 1");
                        climbingStep = false;
                    }
                }
                else
                {
                    //					print("stopped climbing 2");
                    climbingStep = false;
                }
                #endregion step detection
                    
                //Stop the process if the script detected a lot of collisions
                if (collisionNumber < (climbingStep ? 4 : 5))
                {
                    Vector3 reflection = new Vector3();
                    //if it's the first obstacle met on this frame, project the extra velocity on the plane parallel to this obstacle
                    //if it's not the first, project on the line parallel to both the current obstacle and the previous one
                    //if the player is on the ground, count the ground as a first collision
                    //once the extra velocity has been projected, Call the function once more to check if this new velocity meets obstacle and do everything again

                    //Debug.Log("hit.normal " + hit.normal + " collisions.currentGroundNormal " + collisions.currentGroundNormal + " collisions.below " + collisions.below + " belowLastFrame " + belowLastFrame);
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
                                return movementVector;
                                reflection = CollisionDetection(Vector3.ProjectOnPlane(extraVelocity, hit.normal), position + movementVector, hit);
                            }
                        }
                        else
                        {
                            if ((collisions.below || belowLastFrame) && Vector3.Dot(hit.normal, collisions.currentGroundNormal) < 0)
                            {
                                //print("recu 3");
                                reflection = CollisionDetection(Vector3.Project(extraVelocity, Vector3.Cross(hit.normal, collisions.currentGroundNormal)), position + movementVector, hit);
                            }
                            else
                            {
                                //print("recu 4");
                                reflection = CollisionDetection(Vector3.ProjectOnPlane(extraVelocity, hit.normal), position + movementVector, hit);
                            }
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
            public bool side, SlippySlope, NotSlippySlope;

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
                side = SlippySlope = cornerNormal = NotSlippySlope = false;
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