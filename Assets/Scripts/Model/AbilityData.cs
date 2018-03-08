﻿using System.Collections.Generic;
using UnityEngine;

namespace Game.Model
{
    [CreateAssetMenu(menuName = "ScriptableObjects/AbilityData", fileName = "AbilityData")]
    public class AbilityData : ScriptableObject
    {
        [Header("Details")]
        //double jump
        [SerializeField]
        Ability doubleJump = new Ability(eAbilityType.DoubleJump);
        public Ability DoubleJump { get { return doubleJump; } }

        //ground rise
        [SerializeField]
        Ability groundRise = new Ability(eAbilityType.GroundRise);
        public Ability GroundRise { get { return groundRise; } }

        //echo boost
        [SerializeField]
        Ability echoBoost = new Ability(eAbilityType.EchoBoost);
        public Ability EchoBoost { get { return echoBoost; } }

        //echo distance
        [SerializeField]
        Ability echoDistance = new Ability(eAbilityType.EchoDistance);
        public Ability EchoDistance { get { return echoDistance; } }

        //glide
        [SerializeField]
        Ability glide = new Ability(eAbilityType.Glide);
        public Ability Glide { get { return glide; } }

        //wall run
        [SerializeField]
        Ability wallRun = new Ability(eAbilityType.WallRun);
        public Ability WallRun { get { return wallRun; } }

        //dash
        [SerializeField]
        Ability dash = new Ability(eAbilityType.Dash);
        public Ability Dash { get { return dash; } }

        //hover
        [SerializeField]
        Ability hover = new Ability(eAbilityType.Hover);
        public Ability Hover { get { return hover; } }

        //###########################################################

        #region methods

        public Ability GetAbility(eAbilityType ability)
        {
            switch (ability)
            {
                case eAbilityType.DoubleJump:
                    return DoubleJump;
                case eAbilityType.Dash:
                    return Dash;
                case eAbilityType.Glide:
                    return Glide;
                case eAbilityType.WallRun:
                    return wallRun;
                case eAbilityType.GroundRise:
                    return groundRise;
                case eAbilityType.EchoBoost:
                    return echoBoost;
                case eAbilityType.EchoDistance:
                    return echoDistance;
                case eAbilityType.Hover:
                    return hover;
                default:
                    return null;
            }
        }

        public List<Ability> GetAllAbilities()
        {
            return new List<Ability>()
            {
                doubleJump,
                groundRise,
                echoBoost,
                echoDistance,
                glide,
                wallRun,
                dash,
                hover
            };
        }

        #endregion methods

        //###########################################################

        void OnValidate()
        {
            foreach(var ability in GetAllAbilities())
            {
                ability.OnValidate();
            }
        }
    }
} //end of namespace