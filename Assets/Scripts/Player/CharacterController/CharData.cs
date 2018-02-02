using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController
{
    [CreateAssetMenu(menuName = "ScriptableObjects/CharData", fileName = "CharData")]
    public class CharData : ScriptableObject
    {
        //#############################################################################

        #region inspector variables

        [SerializeField]
        GeneralData general = new GeneralData();
        public GeneralData General { get { return general; } }

        [SerializeField]
        PhysicsData physics = new PhysicsData();
        public PhysicsData Physics { get { return physics; } }

        [SerializeField]
        MoveData move = new MoveData();
        public MoveData Move { get { return move; } }

        [SerializeField]
        StandData stand = new StandData();
        public StandData Stand { get { return stand; } }

        [SerializeField]
        FallData fall = new FallData();
        public FallData Fall { get { return fall; } }

        [SerializeField]
        JumpData jump = new JumpData();
        public JumpData Jump { get { return jump; } }

        [SerializeField]
        DashData dash = new DashData();
        public DashData Dash { get { return dash; } }

        [SerializeField]
        SlideData slide = new SlideData();
        public SlideData Slide { get { return slide; } }

        [SerializeField]
        GlideData glide = new GlideData();
        public GlideData Glide { get { return glide; } }
        
        [SerializeField]
        WallRunData wallRun = new WallRunData();
        public WallRunData WallRun { get { return wallRun; } }
        
        [SerializeField]
        EchoBoostData echoBoost = new EchoBoostData();
        public EchoBoostData EchoBoost { get { return echoBoost; } }

        [SerializeField]
        HoverData hover = new HoverData();
        public HoverData Hover { get { return hover; } }

        #endregion inspector variables

        //#############################################################################

        #region on validate

        void OnValidate()
        {
            general.OnValidate();
            physics.OnValidate();
            move.OnValidate();
            stand.OnValidate();
            fall.OnValidate();
            jump.OnValidate();
            dash.OnValidate();
            slide.OnValidate();
            glide.OnValidate();
            wallRun.OnValidate();
        }

        #endregion on validate

        //#############################################################################

        #region subClasses

        //*******************************************

        #region general

        [System.Serializable]
        public class GeneralData
        {
            [SerializeField]
            float stickDeadMaxVal;
            public float StickDeadMaxVal { get { return stickDeadMaxVal; } }

            [SerializeField]
            float transitionSpeed;
            public float TransitionSpeed { get { return transitionSpeed; } }

            [SerializeField]
            float maxSpeed;
            public float MaxSpeed { get { return maxSpeed; } }

            [SerializeField]
            float gravityStrength;
            public float GravityStrength { get { return gravityStrength; } }

            [SerializeField]
            float turnSpeed;
            public float TurnSpeed { get { return turnSpeed; } }

            [SerializeField]
            float minSlopeAngle;
            public float MinSlopeAngle { get { return minSlopeAngle; } }

            [SerializeField]
            float maxSlopeAngle;
            public float MaxSlopeAngle { get { return maxSlopeAngle; } }

            [SerializeField]
            float minWallAngle;
            public float MinWallAngle { get { return minWallAngle; } }
            
            public void OnValidate()
            {
                stickDeadMaxVal = Mathf.Clamp01(stickDeadMaxVal);
                transitionSpeed = Mathf.Clamp(transitionSpeed, 0, float.MaxValue);
                maxSpeed = Mathf.Clamp(maxSpeed, 0, float.MaxValue);
                gravityStrength = Mathf.Clamp(gravityStrength, 0, float.MaxValue);
                turnSpeed = Mathf.Clamp(turnSpeed, 0, float.MaxValue);
                minSlopeAngle = Mathf.Clamp(minSlopeAngle, 0, 360);
                maxSlopeAngle = Mathf.Clamp(maxSlopeAngle, 0, 360);
                minWallAngle = Mathf.Clamp(minWallAngle, 0, 360);
            }
        }

        #endregion general

        //*******************************************

        #region physics

        [System.Serializable]
        public class PhysicsData
        {
            [SerializeField]
            float maxStepHeight;
            public float MaxStepHeight { get { return maxStepHeight; } }

            [SerializeField]
            float maxSlopeAngle;
            public float MaxSlopeAngle { get { return maxSlopeAngle; } }

            public void OnValidate()
            {
                maxStepHeight = Mathf.Clamp(maxStepHeight, 0, float.MaxValue);
            }
        }

        #endregion physics

        //*******************************************

        #region move

        [System.Serializable]
        public class MoveData
        {
            [SerializeField]
            float speed;
            public float Speed { get { return speed; } }

            [SerializeField]
            float sprintCoefficient;
            public float SprintCoefficient { get { return sprintCoefficient; } }

            [SerializeField]
            float maxSpeed;
            public float MaxSpeed { get { return maxSpeed; } }

            [SerializeField]
            float transitionSpeed;
            public float TransitionSpeed { get { return transitionSpeed; } }

            [SerializeField]
            float slipperyGroundTransitionSpeed;
            public float SlipperyGroundTransitionSpeed { get { return slipperyGroundTransitionSpeed; } }

            [SerializeField]
            float canStillJumpTimer;
            public float CanStillJumpTimer { get { return canStillJumpTimer; } }

            public void OnValidate()
            {
                speed = Mathf.Clamp(speed, 0, float.MaxValue);
                sprintCoefficient = Mathf.Clamp(sprintCoefficient, 0, float.MaxValue);
                maxSpeed = Mathf.Clamp(maxSpeed, 0, float.MaxValue);
                transitionSpeed = Mathf.Clamp(transitionSpeed, 0, float.MaxValue);
                canStillJumpTimer = Mathf.Clamp(canStillJumpTimer, 0, float.MaxValue);
            }
        }

        #endregion move

        //*******************************************

        #region stand

        [System.Serializable]
        public class StandData
        {
            [SerializeField]
            float slowFactor;
            public float SlowdownFactor { get { return slowFactor; } }

            [SerializeField]
            float transitionSpeed;
            public float TransitionSpeed { get { return transitionSpeed; } }

            public void OnValidate()
            {
                slowFactor = Mathf.Clamp01(slowFactor);
                transitionSpeed = Mathf.Clamp(transitionSpeed, 0, float.MaxValue);
            }
        }

        #endregion stand

        //*******************************************

        #region fall

        [System.Serializable]
        public class FallData
        {
            [SerializeField]
            float speed;
            public float Speed { get { return speed; } }

            [SerializeField]
            float maxSpeed;
            public float MaxSpeed { get { return maxSpeed; } }

            [SerializeField]
            float transitionSpeed;
            public float TransitionSpeed { get { return transitionSpeed; } }

            public void OnValidate()
            {
                speed = Mathf.Clamp(speed, 0, float.MaxValue);
                maxSpeed = Mathf.Clamp(maxSpeed, 0, float.MaxValue);
                transitionSpeed = Mathf.Clamp(transitionSpeed, 0, float.MaxValue);
            }
        }

        #endregion fall

        //*******************************************

        #region jump

        [System.Serializable]
        public class JumpData
        {
            [SerializeField]
            float strength;
            public float Strength { get { return strength; } }

            [SerializeField]
            float minStrength;
            public float MinStrength { get { return minStrength; } }

            [SerializeField]
            float aerialJumpCoeff;
            public float AerialJumpCoeff { get { return aerialJumpCoeff; } }

            [SerializeField]
            float speed;
            public float Speed { get { return speed; } }

            [SerializeField]
            float impactOfCurrentSpeed;
            public float ImpactOfCurrentSpeed { get { return impactOfCurrentSpeed; } }

            [SerializeField]
            float transitionSpeed;
            public float TransitionSpeed { get { return transitionSpeed; } }

            [SerializeField]
            int maxAerialJumps;
            public int MaxAerialJumps { get { return maxAerialJumps; } set { maxAerialJumps = Mathf.Max(0, value); } }

            public void OnValidate()
            {
                strength = Mathf.Clamp(strength, 0, float.MaxValue);
                minStrength = Mathf.Clamp(minStrength, 0, float.MaxValue);
                aerialJumpCoeff = Mathf.Clamp(aerialJumpCoeff, 0, float.MaxValue);
                speed = Mathf.Clamp(speed, 0, float.MaxValue);
                transitionSpeed = Mathf.Clamp(transitionSpeed, 0, float.MaxValue);
                maxAerialJumps = Mathf.Max(0, maxAerialJumps);
            }
        }

        #endregion jump

        //*******************************************

        #region dash

        [System.Serializable]
        public class DashData
        {
            [SerializeField]
            float speed;
            public float Speed { get { return speed; } }

            [SerializeField]
            float time;
            public float Time { get { return time; } }

            [SerializeField]
            float cooldown;
            public float Cooldown { get { return cooldown; } }

            [SerializeField]
            float transitionSpeed;
            public float TransitionSpeed { get { return transitionSpeed; } }

            public void OnValidate()
            {
                speed = Mathf.Clamp(speed, 0, float.MaxValue);
                time = Mathf.Clamp(time, 0, float.MaxValue);
                cooldown = Mathf.Clamp(cooldown, 0, float.MaxValue);
            }
        }

        #endregion dash

        //*******************************************

        #region slide

        [System.Serializable]
        public class SlideData
        {
            [SerializeField]
            float transitionSpeed;
            public float TransitionSpeed { get { return transitionSpeed; } }

            [SerializeField]
            float minimalSpeed;
            public float MinimalSpeed { get { return minimalSpeed; } }

            [SerializeField]
            float control;
            public float Control { get { return control; } }

            public void OnValidate()
            {
                transitionSpeed = Mathf.Clamp(transitionSpeed, 0, float.MaxValue);
                minimalSpeed = Mathf.Clamp(minimalSpeed, 0, float.MaxValue);
                control = Mathf.Clamp(control, 0, float.MaxValue);
            }
        }

        #endregion slide

        //*******************************************

        #region glide

        [System.Serializable]
        public class GlideData
        {
            [SerializeField]
            float minAngle;
            public float MinAngle { get { return minAngle; } }

            [SerializeField]
            float maxAngle;
            public float MaxAngle { get { return maxAngle; } }

            [SerializeField]
            float baseAngle;
            public float BaseAngle { get { return baseAngle; } }

            [SerializeField]
            float baseSpeed;
            public float BaseSpeed { get { return baseSpeed; } }

            [SerializeField]
            float vertUpAngleCtrl;
            public float VertUpAngleCtrl { get { return vertUpAngleCtrl; } }

            [SerializeField]
            float vertDownAngleCtrl;
            public float VertDownAngleCtrl { get { return vertDownAngleCtrl; } }

            [SerializeField]
            float noInputImpactCoeff;
            public float NoInputImpactCoeff { get { return noInputImpactCoeff; } }

            [SerializeField]
            AnimationCurve upwardDecceleration;
            public AnimationCurve UpwardDecceleration { get { return upwardDecceleration; } }

            [SerializeField]
            AnimationCurve downwardAcceleration;
            public AnimationCurve DownwardAcceleration { get { return downwardAcceleration; } }

            [SerializeField]
            float speedSmooth;
            public float SpeedSmooth { get { return speedSmooth; } }

            [SerializeField]
            float stallSpeed;
            public float StallSpeed { get { return stallSpeed; } }

            [SerializeField]
            float minHorizAngle;
            public float MinHorizAngle { get { return minHorizAngle; } }

            [SerializeField]
            float maxHorizAngle;
            public float MaxHorizAngle { get { return maxHorizAngle; } }

            [SerializeField]
            float horizComingBack;
            public float HorizComingBack { get { return horizComingBack; } }

            [SerializeField]
            float horizAngleCtrl;
            public float HorizAngleCtrl { get { return horizAngleCtrl; } }
            
            [SerializeField]
            float exitInertiaTime;
            public float ExitInertiaTime { get { return exitInertiaTime; } }

            public void OnValidate()
            {
            }
        }

        #endregion glide
        
        //*******************************************

        #region wall run

        [System.Serializable]
        public class WallRunData
        {
            [SerializeField]
            float transitionSpeed;
            public float TransitionSpeed { get { return transitionSpeed; } }

            [SerializeField]
            float gravityModifier;
            public float GravityModifier { get { return gravityModifier; } }

            [SerializeField]
            float maxTriggerAngle = 80;
            public float MaxTriggerAngle { get { return maxTriggerAngle; } }

            [SerializeField]
            float timerBeforeAirControl;
            public float TimerBeforeAirControl { get { return timerBeforeAirControl; } }

            [SerializeField]
            float speedMultiplier;
            public float SpeedMultiplier { get { return speedMultiplier; } }

            [SerializeField]
            float speed;
            public float Speed { get { return speed; } }

            [SerializeField]
            float timeToUnstick;
            public float TimeToUnstick { get { return timeToUnstick; } }

            public void OnValidate()
            {
                transitionSpeed = Mathf.Clamp(transitionSpeed, 0, float.MaxValue);
                gravityModifier = Mathf.Clamp01(gravityModifier);
            }
        }

        #endregion wall run

        //*******************************************

        #region echo boost

        [System.Serializable]
        public class EchoBoostData
        {
            [SerializeField]
            float duration;
            public float Duration { get { return duration; } }

            [SerializeField]
            float lerpSpeed;
            public float LerpSpeed { get { return lerpSpeed; } }

            [SerializeField]
            float speedMultiplier;
            public float SpeedMultiplier { get { return speedMultiplier; } }

            [SerializeField]
            float jumpMultiplier;
            public float JumpMultiplier { get { return jumpMultiplier; } }

            public void OnValidate()
            {
                duration = Mathf.Clamp(duration, 0, float.MaxValue);
                lerpSpeed = Mathf.Clamp(lerpSpeed, 0, float.MaxValue);
                speedMultiplier = Mathf.Clamp(speedMultiplier, 0, float.MaxValue);
                jumpMultiplier = Mathf.Clamp(jumpMultiplier, 0, float.MaxValue);
            }
        }

        #endregion echo boost

        //*******************************************

        #region hover

        [System.Serializable]
        public class HoverData
        {
            [SerializeField]
            float duration;
            public float Duration { get { return duration; } }

            public void OnValidate()
            {
                duration = Mathf.Clamp(duration, 0, float.MaxValue);
            }
        }

        #endregion hover

        //*******************************************

        #endregion subClasses

        //#############################################################################
    }
} //end of namespace