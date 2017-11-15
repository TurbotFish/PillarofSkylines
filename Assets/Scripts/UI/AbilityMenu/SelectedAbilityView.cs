using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.AbilityMenu
{
    public class SelectedAbilityView : MonoBehaviour
    {
        [SerializeField]
        Image abilityIcon;

        [SerializeField]
        Text abilityNameText;

        [SerializeField]
        Text abilityDescriptionText;

        public void ShowAbilityInfo(Player.AbilitySystem.Ability ability)
        {
            this.abilityIcon.sprite = ability.Icon;
            this.abilityNameText.text = ability.Name;
            this.abilityDescriptionText.text = ability.Description;
        }
    }
}