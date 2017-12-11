using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace Game.Player.AbilitySystem
{
    [CreateAssetMenu(menuName = "ScriptableObjects/AbilityData", fileName = "AbilityData")]
    public class AbilityData : ScriptableObject
    {
        [Header("UI Ability Slots")]
        [Tooltip("List with the abilities as they appear in the ability menu. It starts with the topmost slot and continues clockwise.")]
        [SerializeField]
        List<eAbilityType> abilitySlots = new List<eAbilityType>(12);
        public List<eAbilityType> AbilitySlots { get { return abilitySlots; } }

        [Header("Details")]
        //double jump
        [SerializeField]
        DoubleJump doubleJump = new DoubleJump();
        public DoubleJump DoubleJump { get { return doubleJump; } }

        //glide
        [SerializeField]
        Glide glide = new Glide();
        public Glide Glide { get { return glide; } }

        //dash
        [SerializeField]
        Dash dash = new Dash();
        public Dash Dash { get { return dash; } }

        //tomb finder
        [SerializeField]
        TombFinder tombFinder = new TombFinder();
        public TombFinder TombFinder { get { return tombFinder; } }

        //wall run
        [SerializeField]
        WallRun wallRun = new WallRun();
        public WallRun WallRun { get { return wallRun; } }

        //echo trampolin
        [SerializeField]
        Ability echoTrampolin = new Ability(eAbilityType.EchoTrampolin);
        public Ability EchoTrampolin { get { return echoTrampolin; } }

        //distant echo
        [SerializeField]
        Ability distantEcho = new Ability(eAbilityType.DistantEcho);
        public Ability DistantEcho { get { return distantEcho; } }

        //aim drift
        [SerializeField]
        Ability aimDrift = new Ability(eAbilityType.AimDrift);
        public Ability AimDrift { get { return aimDrift; } }

        //super jump
        [SerializeField]
        Ability superJump = new Ability(eAbilityType.SuperJump);
        public Ability SuperJump { get { return superJump; } }

        //phantom
        [SerializeField]
        Ability phantom = new Ability(eAbilityType.Phantom);
        public Ability Phantom { get { return phantom; } }

        //kinematic inversion
        [SerializeField]
        Ability kinematicInversion = new Ability(eAbilityType.KinematicInversion);
        public Ability KinematicInversion { get { return kinematicInversion; } }

        //echo jump
        [SerializeField]
        Ability echoJump = new Ability(eAbilityType.EchoJump);
        public Ability EchoJump { get { return echoJump; } }

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
                case eAbilityType.TombFinder:
                    return tombFinder;
                case eAbilityType.EchoTrampolin:
                    return echoTrampolin;
                case eAbilityType.DistantEcho:
                    return distantEcho;
                case eAbilityType.AimDrift:
                    return aimDrift;
                case eAbilityType.SuperJump:
                    return superJump;
                case eAbilityType.Phantom:
                    return phantom;
                case eAbilityType.KinematicInversion:
                    return kinematicInversion;
                case eAbilityType.EchoJump:
                    return echoJump;

                default:
                    return null;
            }
        }

        public List<Ability> GetAllAbilities()
        {
            return new List<Ability>()
            {
                doubleJump,
                dash,
                glide,
                wallRun,
                tombFinder,
                echoTrampolin,
                distantEcho,
                aimDrift,
                superJump,
                phantom,
                kinematicInversion,
                echoJump
            };
        }

        #endregion methods

        //###########################################################

        void OnValidate()
        {
            while (abilitySlots.Count != 12)
            {
                if (abilitySlots.Count > 12)
                {
                    abilitySlots.RemoveAt(abilitySlots.Count - 1);
                }
                else if (abilitySlots.Count < 12)
                {
                    abilitySlots.Add(0);
                }
            }

            doubleJump.OnValidate();
            dash.OnValidate();
            glide.OnValidate();
            wallRun.OnValidate();
            tombFinder.OnValidate();
            echoTrampolin.OnValidate();
            distantEcho.OnValidate();
            aimDrift.OnValidate();
            superJump.OnValidate();
            phantom.OnValidate();
            kinematicInversion.OnValidate();
            echoJump.OnValidate();
        }
    }
} //end of namespace