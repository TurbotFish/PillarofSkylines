using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.AbilitySystem
{
    [Serializable]
    public class WallRun : Ability
    {
        public override eAbilityType Type { get { return eAbilityType.TombFinder; } }

        //*************************************************************
        [Header("Wall Drift")]

        //
        [Tooltip(
            "The max dot product value that triggers a wall run." +
            "\n'-1': the player faces the wall." +
            "\n'0': the player forward and the wall are perpendicular." +
            "\n'1' the player faces away from the wall.")]
        [SerializeField]
        float triggerDotProduct = -0.5f;
        public float TriggerDotProduct { get { return triggerDotProduct; } }

        //wall drift target speed
        [Tooltip("")]
        [SerializeField]
        float wallDriftTargetSpeed = 10f;
        public float WallDriftTargetSpeed { get { return wallDriftTargetSpeed; } }

        //wall drift slowdown factor
        [Tooltip(
            "'1': instant slowdown" +
            "\n'0': no slowdown")]
        [SerializeField]
        float wallDriftSlowdownFactor = 0.8f;
        public float WallDriftSlowdownFactor { get { return wallDriftSlowdownFactor; } }

        //*************************************************************
        [Header("Wall Jump")]

        //wall jump time
        [Tooltip("")]
        [SerializeField]
        float wallJumpTime = 1f;
        public float WallJumpTime { get { return wallJumpTime; } }

        //wall jump strength
        [Tooltip("")]
        [SerializeField]
        float wallJumpStrength = 30f;
        public float WallJumpStrength { get { return wallJumpStrength; } }

        //*************************************************************
        [Header("Wall Run Horizontal")]

        //wall jump horizontal time
        [Tooltip("")]
        [SerializeField]
        float wallRunHorizontalTime = 4f;
        public float WallRunHorizontalTime { get { return wallRunHorizontalTime; } }

        //wall run horizontal target speed
        [Tooltip("")]
        [SerializeField]
        float wallRunHorizontalTargetSpeed = 8;
        public float WallRunHorizontalTargetSpeed { get { return wallRunHorizontalTargetSpeed; } }

        //wall run horizontal gravity multiplier
        [Tooltip("")]
        [SerializeField]
        float wallRunHorizontalGravityMultiplier = 0.2f;
        public float WallRunHorizontalGravityMultiplier { get { return wallRunHorizontalGravityMultiplier; } }

        //###########################################################

        public override void OnValidate()
        {
            base.OnValidate();

            triggerDotProduct = Mathf.Clamp(triggerDotProduct, -1, 1);
            wallDriftSlowdownFactor = Mathf.Clamp01(wallDriftSlowdownFactor);

            wallRunHorizontalGravityMultiplier = Mathf.Clamp01(wallRunHorizontalGravityMultiplier);
        }
    }
} //end of namespace