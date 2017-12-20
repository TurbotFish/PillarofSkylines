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
        GlideData glide = new GlideData();
        public GlideData Glide { get { return glide; } }

        [SerializeField]
        WallRunData wallRun = new WallRunData();
        public WallRunData WallRun { get { return wallRun; } }

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

            public void OnValidate()
            {
                stickDeadMaxVal = Mathf.Clamp01(stickDeadMaxVal);
                transitionSpeed = Mathf.Clamp(transitionSpeed, 0, float.MaxValue);
                maxSpeed = Mathf.Clamp(maxSpeed, 0, float.MaxValue);
                gravityStrength = Mathf.Clamp(gravityStrength, 0, float.MaxValue);
                turnSpeed = Mathf.Clamp(turnSpeed, 0, float.MaxValue);
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
            float canStillJumpTimer;
            public float CanStillJumpTimer { get { return canStillJumpTimer; } }

            public void OnValidate()
            {
                speed = Mathf.Clamp(speed, 0, float.MaxValue);
                sprintCoefficient = Mathf.Clamp(sprintCoefficient, 0, float.MaxValue);
                maxSpeed = Mathf.Clamp(maxSpeed, 0, float.MaxValue);
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
            float minJumpTime;
            public float MinJumpTime { get { return minJumpTime; } }

            [SerializeField]
            float maxJumpTime;
            public float MaxJumpTime { get { return maxJumpTime; } }

            [SerializeField]
            float speed;
            public float Speed { get { return speed; } }

            [SerializeField]
            float transitionSpeed;
            public float TransitionSpeed { get { return transitionSpeed; } }

            public void OnValidate()
            {
                strength = Mathf.Clamp(strength, 0, float.MaxValue);
                minJumpTime = Mathf.Clamp(minJumpTime, 0, float.MaxValue);
                maxJumpTime = Mathf.Clamp(maxJumpTime, 0, float.MaxValue);
                speed = Mathf.Clamp(speed, 0, float.MaxValue);
                transitionSpeed = Mathf.Clamp(transitionSpeed, 0, float.MaxValue);
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

        #region glide

        [System.Serializable]
        public class GlideData
        {
            [SerializeField]
            float horizontalAngle;
            public float HorizontalAngle { get { return horizontalAngle; } }

            public void OnValidate()
            {
                horizontalAngle = Mathf.Clamp(horizontalAngle, 0, float.MaxValue);
            }
        }

        #endregion glide

        //*******************************************

        #region wall run

        [System.Serializable]
        public class WallRunData
        {

            public void OnValidate()
            {

            }
        }

        #endregion wall run

        //*******************************************

        #endregion subClasses

        //#############################################################################
    }
} //end of namespace