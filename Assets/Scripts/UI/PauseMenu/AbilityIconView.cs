using Game.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.PauseMenu
{
    public class AbilityIconView : MonoBehaviour
    {
        //###########################################################

        // -- CONSTANTS

        [SerializeField] private AbilityType AbilityType;
        [SerializeField] private Sprite InactiveSprite;

        //###########################################################

        // -- ATTRIBUTES

        private Image ImageComponent;

        private PlayerModel Model;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(PlayerModel model)
        {
            Model = model;

            ImageComponent = GetComponent<Image>();

            SetSprite(Model.GetAbilityState(AbilityType));

            Utilities.EventManager.AbilityStateChangedEvent += OnAbilityStateChanged;
        }

        private void OnDestroy()
        {
            Utilities.EventManager.AbilityStateChangedEvent -= OnAbilityStateChanged;
        }

        //###########################################################

        // -- OPERATIONS

        private void OnAbilityStateChanged(object sender, Utilities.EventManager.AbilityStateChangedEventArgs args)
        {
            if (args.AbilityType == AbilityType)
            {
                SetSprite(args.AbilityState);
            }
        }

        private void SetSprite(AbilityState ability_state)
        {
            switch (ability_state)
            {
                case AbilityState.inactive:
                    ImageComponent.sprite = InactiveSprite;
                    ImageComponent.color = Color.gray;
                    break;
                case AbilityState.active:
                    ImageComponent.sprite = Model.AbilityData.GetAbility(AbilityType).Icon;
                    ImageComponent.color = Color.white;
                    break;
            }
        }
    }
} // end of namespace