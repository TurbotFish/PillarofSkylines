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
            float triggerDotProduct = -0.5f;
            public float TriggerDotProduct { get { return triggerDotProduct; } }

            [Tooltip("")]
            [SerializeField]
            float stickMinVerticalTrigger = 0.4f;
            public float StickMinVerticalTrigger { get { return stickMinVerticalTrigger; } }

            [Tooltip("")]
            [SerializeField]
            float stickMaxHorizontalTrigger = 0.4f;
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
            [Tooltip("")]
            [SerializeField]
            float targetSpeed = 10f;
            public float TargetSpeed { get { return targetSpeed; } }

            //wall drift slowdown factor
            [Tooltip(
                "'1': instant slowdown" +
                "\n'0': no slowdown")]
            [SerializeField]
            float slowdownFactor = 0.8f;
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
            //wall jump time
            [Tooltip("")]
            [SerializeField]
            float duration = 1f;
            public float Duration { get { return duration; } }

            //wall jump strength
            [Tooltip("")]
            [SerializeField]
            float strength = 30f;
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
            [Tooltip("")]
            [SerializeField]
            float duration = 4f;
            public float Duration { get { return duration; } }

            //wall run horizontal target speed
            [Tooltip("")]
            [SerializeField]
            float targetSpeed = 8;
            public float TargetSpeed { get { return targetSpeed; } }

            //wall run horizontal gravity multiplier
            [Tooltip("")]
            [SerializeField]
            float gravityMultiplier = 0.2f;
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
            [Tooltip("")]
            [SerializeField]
            float minTriggerSpeed = 1f;
            public float MinTriggerSpeed { get { return minTriggerSpeed; } }

            //wall run vertical target speed
            [Tooltip("")]
            [SerializeField]
            float targetSpeed = 4;
            public float TargetSpeed { get { return targetSpeed; } }

            //wall run vertical acceleration factor
            [Tooltip("")]
            [SerializeField]
            float accelerationFactor = 0.8f;
            public float AccelerationFactor { get { return accelerationFactor; } }

            //wall run vertical slowdown factor
            [Tooltip("")]
            [SerializeField]
            float slowdownFactor = 0.6f;
            public float SlowdownFactor { get { return slowdownFactor; } }

            //wall run vertical time
            [Tooltip("")]
            [SerializeField]
            float duration = 4f;
            public float Duration { get { return duration; } }

            //******************************

            public void OnValidate()
            {
                accelerationFactor = Mathf.Clamp01(accelerationFactor);
                slowdownFactor = Mathf.Clamp01(slowdownFactor);
            }
        }

    } //end of class
} //end of namespace