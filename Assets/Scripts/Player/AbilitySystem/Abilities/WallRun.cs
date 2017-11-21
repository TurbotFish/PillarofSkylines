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

        [Header("Specific Variables")]
        [Tooltip("The max dot product value that triggers a wall run.\n" +
            " '-1': the player faces the wall.\n" +
            " '0': the player forward and the wall are perpendicular.\n" +
            " '1' the player faces away from the wall.")]
        [SerializeField]
        float triggerDotProduct = -0.5f;
        public float TriggerDotProduct { get { return triggerDotProduct; } }

        [Tooltip("")]
        [SerializeField]
        float wallDriftTargetSpeed = 10f;
        public float WallDriftTargetSpeed { get { return wallDriftTargetSpeed; } }

        [Tooltip("")]
        [SerializeField]
        float wallDriftSlowdownFactor = 0.8f;
        public float WallDriftSlowdownFactor { get { return wallDriftSlowdownFactor; } }

        //###########################################################

        public override void OnValidate()
        {
            base.OnValidate();
        }
    }
} //end of namespace