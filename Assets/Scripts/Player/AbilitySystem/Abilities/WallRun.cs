using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.AbilitySystem
{
    [Serializable]
    public class WallRun : Ability
    {
        public override eAbilityType Type { get { return eAbilityType.WallRun; } }

        [SerializeField]
        GeneralData general = new GeneralData();
        public GeneralData General { get { return general; } }

        [SerializeField]
        WallJumpData wallJump = new WallJumpData();
        public WallJumpData WallJump { get { return wallJump; } }

        [SerializeField]
        WallDriftData wallDrift = new WallDriftData();
        public WallDriftData WallDrift { get { return wallDrift; } }

        [SerializeField]
        WallRunHorizontalData wallRunHorizontal = new WallRunHorizontalData();
        public WallRunHorizontalData WallRunHorizontal { get { return wallRunHorizontal; } }

        [SerializeField]
        WallRunVerticalData wallRunVertical = new WallRunVerticalData();
        public WallRunVerticalData WallRunVertical { get { return wallRunVertical; } }

        //###########################################################

        public override void OnValidate()
        {
            base.OnValidate();

            general.OnValidate();
            wallJump.OnValidate();
            wallDrift.OnValidate();
            wallRunHorizontal.OnValidate();
            wallRunVertical.OnValidate();
        }

        //########################################################### 

        [Serializable]
        public class GeneralData
        {
            [Tooltip(
            "The max dot product value that triggers a wall run." +
            "\n'-1': the player faces the wall." +
            "\n'0': the player forward and the wall are perpendicular." +
            "\n'1' the player faces away from the wall.")]
            [SerializeField]
            float triggerDotProduct;
            public float TriggerDotProduct { get { return triggerDotProduct; } }

            [Tooltip("The minimal value of the vertical axis of the left stick required to activate or stay in wall run mode.")]
            [SerializeField]
            float stickMinVerticalTrigger;
            public float StickMinVerticalTrigger { get { return stickMinVerticalTrigger; } }

            [Tooltip("The maximal value (absolute) of the horizontal axis of the left stick required to activate or stay in wall run mode.")]
            [SerializeField]
            float stickMaxHorizontalTrigger;
            public float StickMaxHorizontalTrigger { get { return stickMaxHorizontalTrigger; } }

            //******************************

            public void OnValidate()
            {
                triggerDotProduct = Mathf.Clamp(triggerDotProduct, -1, 1);
                stickMinVerticalTrigger = Mathf.Clamp01(stickMinVerticalTrigger);
                stickMaxHorizontalTrigger = Mathf.Clamp01(stickMaxHorizontalTrigger);
            }
        }

        [Serializable]
        public class WallDriftData
        {
            //wall drift target speed
            [Tooltip("The speed the player tends towards during a wall drift.")]
            [SerializeField]
            float targetSpeed;
            public float TargetSpeed { get { return targetSpeed; } }

            //wall drift slowdown factor
            [Tooltip(
                "'1': instant slowdown" +
                "\n'0': no slowdown")]
            [SerializeField]
            float slowdownFactor;
            public float SlowdownFactor { get { return slowdownFactor; } }

            //******************************

            public void OnValidate()
            {
                slowdownFactor = Mathf.Clamp01(slowdownFactor);
            }
        }

        [Serializable]
        public class WallJumpData
        {
            //ignore gravity duration
            [Tooltip("The duration during which gravity is ignored.")]
            [SerializeField]
            float ignoreGravityDuration;
            public float IgnoreGravityDuration { get { return ignoreGravityDuration; } }

            //ignore stick duration
            [Tooltip("The duration during which the player character cannot turn.")]
            [SerializeField]
            float ignoreStickDuration;
            public float IgnoreStickDuration { get { return ignoreStickDuration; } }

            //wall jump strength
            [Tooltip("The initial \"speed\" of the jump.")]
            [SerializeField]
            float strength;
            public float Strength { get { return strength; } }

            //******************************

            public void OnValidate()
            {

            }
        }

        [Serializable]
        public class WallRunHorizontalData
        {
            //wall run horizontal time
            [Tooltip("The duration of a horizontal wall run.")]
            [SerializeField]
            float duration;
            public float Duration { get { return duration; } }

            //wall run horizontal target speed
            [Tooltip("The speed the player tends towards during a horizontal wall run.")]
            [SerializeField]
            float targetSpeed;
            public float TargetSpeed { get { return targetSpeed; } }

            //wall run horizontal gravity multiplier
            [Tooltip("The rate by which gravity is reduced during a horizontal wall run.")]
            [SerializeField]
            float gravityMultiplier;
            public float GravityMultiplier { get { return gravityMultiplier; } }

            //******************************

            public void OnValidate()
            {
                gravityMultiplier = Mathf.Clamp01(gravityMultiplier);
            }
        }

        [Serializable]
        public class WallRunVerticalData
        {
            //wall run vertical min speed
            [Tooltip("The minimal speed required to activate a vertical wall run.")]
            [SerializeField]
            float minTriggerSpeed;
            public float MinTriggerSpeed { get { return minTriggerSpeed; } }

            //wall run vertical target speed
            [Tooltip("The speed the player tends towards during a vertical wall run.")]
            [SerializeField]
            float targetSpeed;
            public float TargetSpeed { get { return targetSpeed; } }

            //wall run vertical acceleration factor
            [Tooltip("How \"fast\" the player accelerates.")]
            [SerializeField]
            float accelerationFactor;
            public float AccelerationFactor { get { return accelerationFactor; } }

            //wall run vertical slowdown factor
            [Tooltip("How \"fast\" the player slows down.")]
            [SerializeField]
            float slowdownFactor;
            public float SlowdownFactor { get { return slowdownFactor; } }

            //wall run vertical time
            [Tooltip("The base duration of a vertical wall run.")]
            [SerializeField]
            float baseDuration;
            public float BaseDuration { get { return baseDuration; } }

            //wall run vertical duration multiplier
            [Tooltip("The speed of the player multiplied by this value is added to the duration.")]
            [SerializeField]
            float durationMultiplier;
            public float DurationMultiplier { get { return durationMultiplier; } }

            //******************************

            public void OnValidate()
            {
                accelerationFactor = Mathf.Clamp01(accelerationFactor);
                slowdownFactor = Mathf.Clamp01(slowdownFactor);
            }
        }

    } //end of class
} //end of namespace