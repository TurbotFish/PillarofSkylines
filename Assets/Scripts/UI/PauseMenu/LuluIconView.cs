using Game.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.PauseMenu
{
    public class LuluIconView : MonoBehaviour
    {
        //###########################################################

        // -- CONSTANTS

        [SerializeField] private int LuluNumber;
        [SerializeField] private Sprite InactiveSprite;
        [SerializeField] private Sprite ActiveSprite;

        //###########################################################

        // -- ATTRIBUTES

        private Image ImageComponent;

        private PlayerModel Model;
        bool initialized = false;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(PlayerModel model)
        {
            Model = model;

            ImageComponent = GetComponent<Image>();

            initialized = true;

            SetSprite();
        }

        private void OnEnable()
        {
            if (initialized)
                SetSprite();
        }

        //###########################################################

        // -- OPERATIONS
        
        private void SetSprite()
        {
            if (Model.GetFireflyCount(true) >= LuluNumber)
                ImageComponent.sprite = ActiveSprite;
            else
                ImageComponent.sprite = InactiveSprite;
        }
        
    }
}