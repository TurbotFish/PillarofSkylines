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
        [SerializeField] private Sprite ActiveSprite;

        //###########################################################

        // -- ATTRIBUTES

        private Image ImageComponent;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(PlayerModel model)
        {
            ImageComponent = GetComponent<Image>();

            SetSprite(model.GetAbilityState(AbilityType));

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
                    break;
                case AbilityState.active:
                    ImageComponent.sprite = ActiveSprite;
                    break;
            }
        }
    }
} // end of namespace