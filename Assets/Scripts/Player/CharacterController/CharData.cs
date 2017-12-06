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

            public void OnValidate()
            {
                leftStickDeadMaxValue = Mathf.Clamp01(leftStickDeadMaxValue);
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