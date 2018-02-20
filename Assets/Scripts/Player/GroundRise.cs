﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController
{
    public class GroundRise : MovingPlatform
    {

        float height;
        float strength;
        bool flat;
        Vector3 playerUp;

        GameObject goUp;
        GameObject goFlat;

        float currHeight = 0f;
        bool finishedMoving = false;
        float timeRemaining;

        public void Initialize(Vector3 position, Vector3 playerUp, CharController player, Vector3 velocity)
        {
            CharData.GroundRiseData grRiseData = player.CharData.GroundRise;
            RaycastHit hit;
            strength = grRiseData.Strength;
            this.playerUp = playerUp;

            //print("test : " + Vector3.ProjectOnPlane(velocity, playerUp).magnitude + " > " + grRiseData.VelocityToFlat);
            if (Vector3.ProjectOnPlane(velocity, playerUp).magnitude > grRiseData.VelocityToFlat)
            {
                flat = true;
                active = false;
            }

            if (Physics.Raycast(position + (playerUp*.01f), -playerUp, out hit, grRiseData.Range, player.tempPhysicsHandler.collisionMaskNoCloud))
            {
                if (hit.transform.CompareTag("Untagged"))
                {
                    Debug.Log("oui groundrise !");
                    transform.position = hit.point;
                    if (player.CollisionInfo.below && !flat)
                        currPlayer = player;
                    transform.rotation = player.MyTransform.rotation;
                } else
                {
                    return;
                }
            } else
            {
                return;
            }
            goUp = transform.GetChild(0).gameObject;
            goFlat = transform.GetChild(1).gameObject;

            if (flat)
            {
                height = grRiseData.FlatLength;
                goFlat.SetActive(true);
                transform.localScale = new Vector3(transform.localScale.x, grRiseData.FlatLength, transform.localScale.z);
                transform.Rotate(90 - grRiseData.FlatAngle, 0, 0);
                transform.position += Vector3.ProjectOnPlane(velocity, playerUp).normalized;
            }
            else
            {
                height = grRiseData.Height;
                transform.localScale = new Vector3(transform.localScale.x, height, transform.localScale.z);
                goUp.SetActive(true);
            }
            timeRemaining = grRiseData.Duration;

        }

        public override void Start()
        {
            
            base.Start();
        }

        void Update()
        {
            if (!finishedMoving)
            {
                if (flat)
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
                {
                    currHeight += strength * Time.deltaTime;
                    if (currHeight > height)
                    {
                        transform.Translate(Vector3.up * (height - (currHeight - (strength * Time.deltaTime))), Space.Self);
                        finishedMoving = true;

                        if (currPlayer)
                            currPlayer.ImmediateMovement(playerUp * (height - (currHeight - (strength * Time.deltaTime))), true);

                    }
                    else
                    {
                        transform.Translate(Vector3.up * strength * Time.deltaTime, Space.Self);

                        if (currPlayer)
                            currPlayer.ImmediateMovement(playerUp * strength * Time.deltaTime, true);
                    }
                }
            }
            else
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining < 0f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
