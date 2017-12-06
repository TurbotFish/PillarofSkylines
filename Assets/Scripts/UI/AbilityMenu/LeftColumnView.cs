﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.UI.AbilityMenu
{
    public class LeftColumnView : MonoBehaviour
    {
        [SerializeField]
        SelectedAbilityView selectedAbilityView;

        [SerializeField]
        GameObject abilityElementPrefab;

        [SerializeField]
        Transform abilityListContentTransform;

        List<AbilityElementView> abilityElements = new List<AbilityElementView>();

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetAbilityActive(Player.AbilitySystem.Ability ability, bool active)
        {
            var abilities = from abilityElement in this.abilityElements where abilityElement.AbilityType == ability.Type select abilityElement;

            if (active)
            {
                abilities.First().SetActivated();
            }
            else
            {
                abilities.First().SetDeactivated();
            }
        }

        public void SetAbilityState(Player.eAbilityType type, AbilityMenu.eAbilityElementState state)
        {
            var abilities = from abilityElement in this.abilityElements
                            where abilityElement.AbilityType == type
                            select abilityElement;

            abilities.First().SetState(state);
        }

        public void SetAbilitySelected(Player.AbilitySystem.Ability ability)
        {
            foreach(var abilityElement in this.abilityElements)
            {
                if(abilityElement.AbilityType == ability.Type)
                {
                    abilityElement.SetSelected(true);
                }
                else
                {
                    abilityElement.SetSelected(false);
                }
            }

            this.selectedAbilityView.ShowAbilityInfo(ability);
        }

        public void CreateAbilityElement(Player.AbilitySystem.Ability ability, bool unlocked)
        {
            var go = Instantiate(this.abilityElementPrefab, this.abilityListContentTransform);
            var script = go.GetComponent<AbilityElementView>();

            script.Initialize(ability, unlocked);
            this.abilityElements.Add(script);
        }
    }
} //end of namespace