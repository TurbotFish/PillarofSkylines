using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController
{
    [RequireComponent(typeof(MovingPlatform))]
    public class GroundRise : MovingPlatform
    {

        float height;
        float strength;
        bool flat;
        Vector3 playerUp;

        GameObject goUp;
        GameObject goFlat;

        MovingPlatform platform;

        [SerializeField] LayerMask collisionMask;

        [SerializeField] ParticleSystem endParticles;

        float currHeight = 0f;
        bool finishedMoving = false;
        float timeRemaining;

        FallAndDie[] rocks;

        public void Initialize(Vector3 position, Vector3 playerUp, CharController player, Vector3 velocity)
        {
            CharData.GroundRiseData grRiseData = player.CharData.GroundRise;
            RaycastHit hit;
            strength = grRiseData.Strength;
            this.playerUp = playerUp;

            platform = GetComponent<MovingPlatform>();
            goUp = transform.GetChild(0).gameObject;
            goFlat = transform.GetChild(1).gameObject;

            rocks = GetComponentsInChildren<FallAndDie>();

            foreach (FallAndDie rock in rocks) {

                rock.fallDirection = -transform.up;
                rock.fallTime = 1;
                rock.fallingSpeed = 11;
                rock.gravity = 50;

                rock.waitBeforeFalling = Random.Range(0, 0.5f);

            }

            //print("test : " + Vector3.ProjectOnPlane(velocity, playerUp).magnitude + " > " + grRiseData.VelocityToFlat);
            /*if (Vector3.ProjectOnPlane(velocity, playerUp).magnitude > grRiseData.VelocityToFlat)
            {
                flat = true;
                active = false;
            }*/

            if (Physics.Raycast(position + (playerUp*.01f), -playerUp, out hit, grRiseData.Range, player.tempPhysicsHandler.collisionMaskNoCloud))
            {
                if (!hit.transform.CompareTag("MovingPlatform") && !hit.transform.CompareTag("GroundRise"))
                {
                    transform.position = hit.point;
                    /*if (player.CurrentState == ePlayerState.slide)
                    {
                        flat = true;
                        goFlat.tag = "SlipperySlope";
                    }*/
                    if (player.CollisionInfo.below && !flat && Vector3.Distance(position, transform.position) < 0.2f)
                        currPlayer = player;
                    transform.rotation = player.MyTransform.rotation;
                } else
                {
                    return;
                }
            }
            else
            {
                return;
            }

            /*if (flat)
            {
                height = grRiseData.FlatLength;
                goFlat.SetActive(true);
                transform.localScale = new Vector3(transform.localScale.x, grRiseData.FlatLength, transform.localScale.z);

                float groundAngle = Vector3.Angle(playerUp, player.CollisionInfo.currentGroundNormal);
                transform.Translate(Vector3.forward);
                if (groundAngle + (90 - grRiseData.FlatAngle) < 90 && goFlat.CompareTag("SlipperySlope"))
                {
                    transform.Rotate(groundAngle, 0, 0);
                    transform.Rotate(90 - groundAngle, 0, 0);
                }
                else
                {
                    transform.Rotate(90 - grRiseData.FlatAngle, 0, 0);
                }
            }
            else
            {*/
                height = grRiseData.Height;
                goUp.SetActive(true);
                float maxheight = height + player.tempPhysicsHandler.height * 2;
                if (Physics.BoxCast(goUp.transform.position + goUp.transform.up * 0.5f, new Vector3(goUp.transform.localScale.x / 2.5f, 0.01f, goUp.transform.localScale.z / 2.5f), playerUp, out hit, Quaternion.identity, maxheight, collisionMask))
                {
                    height -= maxheight - hit.distance;
                    print("boxcast hit " + hit.collider);
                }
            //}
            timeRemaining = grRiseData.Duration;
        }

        protected override void Start()
        {            
            base.Start();
        }

        void Update()
        {
            if (!finishedMoving)
            {
                /*if (flat)
                {
                    currHeight += strength * Time.deltaTime;
                    if (currHeight > height)
                    {
                        transform.Translate(Vector3.up * (height - (currHeight - (strength * Time.deltaTime))), Space.Self);
                        finishedMoving = true;
                    }
                    else
                    {
                        transform.Translate(Vector3.up * strength * Time.deltaTime, Space.Self);
                    }
                }
                else
                {*/
                    Collider[] cols = Physics.OverlapBox(goUp.transform.position, new Vector3(goUp.transform.lossyScale.x / 2, goUp.transform.lossyScale.y / 2, goUp.transform.lossyScale.z / 2), Quaternion.identity);
                    foreach (Collider col in cols)
                    {
                        if (col.CompareTag("Player"))
                        {
                            col.transform.parent.position = new Vector3(col.transform.position.x, transform.position.y, col.transform.position.z);
                        }
                    }
                    
                    currHeight += strength * Time.deltaTime;
                    if (currHeight > height)
                    {
                        transform.Translate(Vector3.up * (height - (currHeight - (strength * Time.deltaTime))), Space.Self);
                        finishedMoving = true;

                        /*if (currPlayer)
                        {
                            if (currPlayer.MovementInfo.velocity.y * Time.deltaTime < strength * Time.deltaTime)
                                currPlayer.SetVelocity(new Vector3(currPlayer.MovementInfo.velocity.x, strength, currPlayer.MovementInfo.velocity.z), true);
                        }*/
                    }
                    else
                    {
                        RaycastHit hit;
                        if (Physics.BoxCast(transform.position, new Vector3(goUp.transform.localScale.x / 2, 0.01f, goUp.transform.localScale.z / 2), playerUp, out hit, Quaternion.identity, strength * Time.deltaTime))
                        {
                            if (hit.transform.CompareTag("Player"))
                            {
                                currPlayer = hit.transform.GetComponentInParent<CharController>();
                            } else
                            {
                                currPlayer = null;
                            }
                        } else
                        {
                            currPlayer = null;
                        }

                    platform.Move(Vector3.up * strength * Time.deltaTime);

                        //transform.Translate(Vector3.up * strength * Time.deltaTime, Space.Self);

                        /*
                        if (currPlayer)
                        {
                            Debug.Log("there's a player on me :o");
                            if (currPlayer.MovementInfo.velocity.y < strength + 0.1f)
                            {
                                Debug.Log("Go up player");
                                currPlayer.SetVelocity(new Vector3(currPlayer.MovementInfo.velocity.x, strength + 0.1f, currPlayer.MovementInfo.velocity.z), true);
                            }
                        }*/
                    }
                //}
            }
            else
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining < 0f)
                {
                    foreach (FallAndDie rock in rocks)
                    {
                        rock.GetComponent<BackAndForthMovement>().enabled = false;
                        rock.transform.parent = null;
                        rock.Trigger();
                    }
                    endParticles.transform.parent = null;
                    endParticles.Play();

                    Destroy(gameObject);
                }
            }
        }
    }
}
