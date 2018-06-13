using Game.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class PillarMarkIconView : MonoBehaviour
    {
        //###########################################################

        // -- CONSTANTS

        [SerializeField] int order;
        [SerializeField] private Color ActiveColor;
        [SerializeField] private Color InactiveColor;

        //###########################################################

        // -- ATTRIBUTES

        private Image ImageComponent;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(PlayerModel model)
        {
            ImageComponent = GetComponent<Image>();

            //SetSprite(model.GetPillarMarkState(PillarMarkId));

            Utilities.EventManager.PillarMarkStateChangedEvent += OnPillarMarkStateChanged;
        }

        public void OnDestroy()
        {
            Utilities.EventManager.PillarMarkStateChangedEvent -= OnPillarMarkStateChanged;
        }

        //###########################################################

        // -- OPERATIONS

        private void OnPillarMarkStateChanged(object sender, Utilities.EventManager.PillarMarkStateChangedEventArgs args)
        {
            if (order == args.PillarMarkAmount) {
                SetSprite(args.PillarMarkState);
            }
        }

        private void SetSprite(PillarMarkState state)
        {
            switch (state)
            {
                case PillarMarkState.inactive:
                    ImageComponent.color = InactiveColor;
                    break;
                case PillarMarkState.active:
                    ImageComponent.color = ActiveColor;
                    break;
            }
        }
    }
} // end of namespace