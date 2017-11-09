using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.AbilityMenu
{
    public class AbilityElementView : MonoBehaviour
    {
        [SerializeField]
        Outline backgroundOutline;

        [SerializeField]
        Image backgroundImage;

        [SerializeField]
        Image abilityIcon;

        [SerializeField]
        Text abilityName;

        [SerializeField]
        Color lockedColour;

        [SerializeField]
        Color deactivatedColour;

        [SerializeField]
        Color activatedColour;

        public Player.eAbilityType AbilityType { get; private set; }

        public void Initialize(Player.AbilitySystem.Ability ability, bool unlocked)
        {
            this.abilityIcon.sprite = ability.Icon;
            this.abilityName.text = ability.Name;

            if (unlocked)
            {
                this.backgroundImage.color = this.deactivatedColour;
            }
            else
            {
                this.backgroundImage.color = this.lockedColour;
            }

            this.backgroundOutline.enabled = false;

            this.AbilityType = ability.Type;
        }

        public void SetActivated()
        {
            this.backgroundImage.color = this.activatedColour;
        }

        public void SetDeactivated()
        {
            this.backgroundImage.color = this.deactivatedColour;
        }

        public void SetSelected(bool selected)
        {
            if (selected)
            {
                this.backgroundOutline.enabled = true;
            }
            else
            {
                this.backgroundOutline.enabled = false;
            }
        }
    }
}