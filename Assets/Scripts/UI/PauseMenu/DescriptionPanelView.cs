using Game.GameControl;
using Game.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class DescriptionPanelView : MonoBehaviour
    {
        [SerializeField] private Text NameTextComponent;
        [SerializeField] private Text DescriptionTextComponent;
        [SerializeField] private Image IconImageComponent;

        private IGameController GameController;

        public void Initialize(IGameController game_controller)
        {
            GameController = game_controller;
        }

        public void ShowAbility(AbilityType ability)
        {
            var ability_data = GameController.PlayerModel.AbilityData.GetAbility(ability);

            NameTextComponent.text = ability_data.Name;
            DescriptionTextComponent.text = ability_data.Description;
            IconImageComponent.sprite = ability_data.Icon;
        }
    }
}