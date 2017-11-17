﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Player.AbilitySystem
{
    [CreateAssetMenu(menuName = "ScriptableObjects/AbilityData", fileName = "AbilityData")]
    public class AbilityData : ScriptableObject
    {
        //double jump
        [SerializeField]
        DoubleJump doubleJump = new DoubleJump();
        public DoubleJump DoubleJump { get { return this.doubleJump; } }

        //glide
        [SerializeField]
        Glide glide = new Glide();
        public Glide Glide { get { return this.glide; } }

        //dash
        [SerializeField]
        Dash dash = new Dash();
        public Dash Dash { get { return this.dash; } }

        //tomb finder
        [SerializeField]
        TombFinder tombFinder = new TombFinder();
        public TombFinder TombFinder { get { return this.tombFinder; } }

        //###########################################################

        #region methods

        public Ability GetAbility(eAbilityType ability)
        {
            switch (ability)
            {
                case eAbilityType.DoubleJump:
                    return this.DoubleJump;
                case eAbilityType.Glide:
                    return this.Glide;
                case eAbilityType.Dash:
                    return this.Dash;
                case eAbilityType.TombFinder:
                    return this.tombFinder;
                default:
                    throw new NotImplementedException();
            }
        }

        public List<Ability> GetAllAbilities()
        {
            return new List<Ability>()
            {
                this.doubleJump,
                this.glide,
                this.dash,
                this.tombFinder
            };
        }

        #endregion methods

        //###########################################################

        void OnValidate()
        {
            this.doubleJump.OnValidate();
            this.glide.OnValidate();
            this.dash.OnValidate();
        }
    }
} //end of namespace