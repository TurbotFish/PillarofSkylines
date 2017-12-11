using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.AbilitySystem
{
    [Serializable]
    public class WallRun : Ability
    {
        //###########################################################

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

        public WallRun() : base(eAbilityType.WallRun)
        {

        }

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
            float dirTrigger;
            public float DirectionTrigger { get { return dirTrigger; } }

            [Tooltip("The minimal value of the dot product between the direction of the wall and the left stick required to activate or stay in wall run mode.")]
            [SerializeField]
            float stickTrigger;
            public float StickTrigger { get { return stickTrigger; } }

            //******************************

            public void OnValidate()
            {
                dirTrigger = Mathf.Clamp(dirTrigger, -1, 1);
                stickTrigger = Mathf.Clamp(stickTrigger, -1, 1);
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
        public class WallDriftData
        {
            //wall drift target speed
            [Tooltip("The speed the player tends towards during a wall drift.")]
            [SerializeField]
            float targetSpeed;
            public float TargetSpeed { get { return targetSpeed; } }

            //wall drift slowdown factor
            [Tooltip("The factor by which the player accelerates or deccelerates." +
                "\n'1': instantly switches to target speed" +
                "\n'0': never changes speed")]
            [SerializeField]
            float lerpFactor;
            public float LerpFactor { get { return lerpFactor; } }

            //******************************

            public void OnValidate()
            {
                targetSpeed = Mathf.Clamp(targetSpeed, 0, float.MaxValue);
                lerpFactor = Mathf.Clamp01(lerpFactor);
            }
        }

        [Serializable]
        public class WallRunHorizontalData
        {
            //minimal speed
            [Tooltip("The minimal horizontal speed the player needs to have to continue the horizontal wall run.")]
            [SerializeField]
            float minSpeed;
            public float MinSpeed { get { return minSpeed; } }

            //momentum
            [Tooltip("The factor of the previous velocity that is carried over")]
            [SerializeField]
            float momentum;
            public float Momentum { get { return momentum; } }

            //forward speed
            [Tooltip("")]
            [SerializeField]
            float forwardSpeed;
            public float ForwardSpeed { get { return forwardSpeed; } }

            //wall run horizontal gravity multiplier
            [Tooltip("The gravity strength.")]
            [SerializeField]
            float gravity;
            public float Gravity { get { return gravity; } }

            //******************************

            public void OnValidate()
            {
                minSpeed = Mathf.Clamp(minSpeed, 0, float.MaxValue);
                momentum = Mathf.Clamp01(momentum);
                forwardSpeed = Mathf.Clamp(forwardSpeed, 0, float.MaxValue);
                gravity = Mathf.Clamp(gravity, 0, float.MaxValue);
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