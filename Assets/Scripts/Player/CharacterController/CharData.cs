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
            float leftStickDeadMaxValue;
            public float LeftStickDeadMaxValue { get { return leftStickDeadMaxValue; } }

            [SerializeField]
            float defaultTransitionSpeed;
            public float DefaultTransitionSpeed { get { return defaultTransitionSpeed; } }

            [SerializeField]
            float defaultMaxSpeed;
            public float DefaultMaxSpeed { get { return defaultMaxSpeed; } }

            [SerializeField]
            float gravityStrength;
            public float GravityStrength { get { return gravityStrength; } }

            [SerializeField]
            float turnSpeed;
            public float TurnSpeed { get { return turnSpeed; } }

            public void OnValidate()
            {
                leftStickDeadMaxValue = Mathf.Clamp01(leftStickDeadMaxValue);
                defaultTransitionSpeed = Mathf.Clamp(defaultTransitionSpeed, 0, float.MaxValue);
                defaultMaxSpeed = Mathf.Clamp(defaultMaxSpeed, 0, float.MaxValue);
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

            public void OnValidate()
            {
                speed = Mathf.Clamp(speed, 0, float.MaxValue);
                sprintCoefficient = Mathf.Clamp(sprintCoefficient, 0, float.MaxValue);
            }
        }

        #endregion move

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