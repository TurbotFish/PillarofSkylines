using Game.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.PauseMenu
{
    public class DescriptionPanelView : MonoBehaviour
    {
        //###########################################################

        // -- CONSTANTS

        [SerializeField] private Text NameTextComponent;
        [SerializeField] private Text DescriptionTextComponent;
        [SerializeField] private Image IconImageComponent;

        //###########################################################

        // -- ATTRIBUTES

        private PlayerModel Model;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(PlayerModel model)
        {
            Model = model;
        }

        //###########################################################

        // -- OPERATIONS

        public void ShowAbility(AbilityType ability)
        {
            var ability_data = Model.AbilityData.GetAbility(ability);

            NameTextComponent.text = ability_data.Name;
            DescriptionTextComponent.text = ability_data.Description;
            IconImageComponent.sprite = ability_data.Icon;
        }
    }
} // end of namespace