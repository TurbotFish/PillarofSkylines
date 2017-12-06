using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.AbilityMenu
{
    public class AbilityElementView : MonoBehaviour
    {
        //##################################################################

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

        //##################################################################

        public void Initialize(Player.AbilitySystem.Ability ability, bool unlocked)
        {
            this.abilityIcon.sprite = ability.Icon;
            this.abilityName.text = ability.Name;

            if (unlocked)
            {
                backgroundImage.color = deactivatedColour;
            }
            else
            {
                backgroundImage.color = lockedColour;
            }

            backgroundOutline.enabled = false;

            AbilityType = ability.Type;
        }

        //##################################################################

        public void SetState(eAbilityElementState state)
        {
            switch (state)
            {
                case eAbilityElementState.Activated:
                    backgroundImage.color = activatedColour;
                    break;
                case eAbilityElementState.Deactivated:
                    backgroundImage.color = deactivatedColour;
                    break;
                case eAbilityElementState.Locked:
                    backgroundImage.color = lockedColour;
                    break;
                default:
                    Debug.LogErrorFormat("AbilityElementView: SetState: not implemented case: {0}", state.ToString());
                    break;
            }
        }

        public void SetActivated()
        {
            backgroundImage.color = activatedColour;
        }

        public void SetDeactivated()
        {
            backgroundImage.color = deactivatedColour;
        }

        public void SetSelected(bool selected)
        {
            if (selected)
            {
                backgroundOutline.enabled = true;
            }
            else
            {
                backgroundOutline.enabled = false;
            }
        }

        public void SetLocked()
        {
            backgroundImage.color = lockedColour;
        }
    }
}