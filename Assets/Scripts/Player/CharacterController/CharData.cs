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
        WallRunData wallRun = new WallRunData();
        public WallRunData WallRun { get { return wallRun; } }

        #endregion inspector variables

        //#############################################################################

        #region on validate

        void OnValidate()
        {
            wallRun.OnValidate();
        }

        #endregion on validate

        //#############################################################################

        #region subClasses

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

            public void OnValidate()
            {
                leftStickDeadMaxValue = Mathf.Clamp01(leftStickDeadMaxValue);
                defaultTransitionSpeed = Mathf.Clamp(defaultTransitionSpeed, 0, float.MaxValue);
                defaultMaxSpeed = Mathf.Clamp(defaultMaxSpeed, 0, float.MaxValue);
                gravityStrength = Mathf.Clamp(gravityStrength, 0, float.MaxValue);
            }
        }

        [System.Serializable]
        public class WallRunData
        {

            public void OnValidate()
            {

            }
        }

        #endregion subClasses

        //#############################################################################
    }
} //end of namespace