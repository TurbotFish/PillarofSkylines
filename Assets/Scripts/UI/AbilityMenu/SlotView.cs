//using Game.Model;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//namespace Game.UI.AbilityMenu
//{
//    public class SlotView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
//    {
//        //##################################################################

//        [SerializeField] private AbilityType ability;

//        [SerializeField] private Image backgroundImage;
//        [SerializeField] private Outline backgroundOutline;
//        [SerializeField] private Image abilityImage;

//        private PlayerModel playerModel;
//        private AbilityMenuController menuController;
//        private bool isActive;

//        //##################################################################

//        public void Initialize(PlayerModel playerModel, AbilityMenuController menuController)
//        {
//            this.playerModel = playerModel;
//            this.menuController = menuController;

//            abilityImage.sprite = playerModel.AbilityData.GetAbility(ability).Icon;
//            SetBackgroundColour(playerModel.GetAbilityState(ability));
//            backgroundOutline.enabled = false;

//            //Utilities.EventManager.AbilityStateChangedEvent += OnAbilityStateChangedEventHandler;
//        }

//        //##################################################################

//        public AbilityType AbilityType { get { return ability; } }

//        //##################################################################

//        public void Activate()
//        {
//            isActive = true;
//            gameObject.SetActive(true);
//        }

//        public void Deactivate()
//        {
//            gameObject.SetActive(false);
//            isActive = false;
//        }

//        public void SetSelected(bool selected)
//        {
//            backgroundOutline.enabled = selected;
//        }

//        private void Update()
//        {
//            if (!isActive)
//            {
//                return;
//            }

//            var abilityState = playerModel.GetAbilityState(ability);
//            SetBackgroundColour(abilityState);
//        }

//        //void OnAbilityStateChangedEventHandler(object sender, Utilities.EventManager.AbilityStateChangedEventArgs args)
//        //{
//        //    if (args.AbilityType != AbilityType)
//        //    {
//        //        return;
//        //    }

//        //    SetBackgroundColour(args.AbilityState);
//        //}

//        private void SetBackgroundColour(AbilityState abilityState)
//        {
//            switch (abilityState)
//            {
//                case AbilityState.active:
//                    backgroundImage.color = menuController.ActiveAbilityColour;
//                    break;
//                case AbilityState.available:
//                    backgroundImage.color = menuController.AvailableAbilityColour;
//                    break;
//                case AbilityState.locked:
//                    backgroundImage.color = menuController.LockedAbilityColour;
//                    break;
//                case AbilityState.pillarLocked:
//                    backgroundImage.color = menuController.PillarLockedAbilityColour;
//                    break;
//                default:
//                    Debug.LogError("No colour defined for this ability state: " + abilityState);
//                    break;
//            }
//        }

//        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
//        {
//            Debug.LogError("Use an XBox controller you peasant!");
//            menuController.OnPointerEnterSlot(this);
//        }

//        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
//        {
//            Debug.LogError("Use an XBox controller you peasant!");
//            menuController.OnPointerExitSlot(this);
//        }

//        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
//        {
//            Debug.LogError("Use an XBox controller you peasant!");
//            menuController.OnPointerClickSlot(this);
//        }
//    }
//} //end of namespace