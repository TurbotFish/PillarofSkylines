using Game.Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Game.UI.PauseMenu
{
    public class AbilityEntryView : MonoBehaviour, IEntryView, ISelectHandler, IDeselectHandler
    {
        //###########################################################

        // -- CONSTANTS

        [SerializeField] private AbilityType AbilityType;
        [SerializeField] private Text NameTextComponent;
        [SerializeField] private Image IconImageComponent;
        [SerializeField] private Image BackgroundImageComponent;

        [SerializeField] Color activeColor = Color.white;
        [SerializeField] Color inactiveColor = Color.gray;

        [SerializeField] float selectedWidth = 620;
        [SerializeField] float deselectedWidth = 400;

        [SerializeField] float transitionDuration = .1f;

        public float entranceDuration = 0.1f;

        //###########################################################

        // -- ATTRIBUTES

        private bool isInitialized;
        private PlayerModel Model;
        private SkillsMenuController SkillsMenuController;
        private RectTransform myTransform;
        public AbilityListView containerList;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(PlayerModel model, SkillsMenuController skills_menu_controller)
        {
            isInitialized = true;

            Model = model;
            SkillsMenuController = skills_menu_controller;

            myTransform = transform as RectTransform;
            
            NameTextComponent.text = Model.AbilityData.GetAbility(AbilityType).Name;
            IconImageComponent.sprite = Model.AbilityData.GetAbility(AbilityType).Icon;

            Utilities.EventManager.AbilityStateChangedEvent += OnAbilityStateChanged;

            SetVisualsFromState();
            CloseVisuals();
        }

        //###########################################################

        // -- CALLBACKS

        public void OnSelect(BaseEventData eventData)
        {
            if (eventData.selectedObject == this.gameObject)
            {
                containerList.SetMovable(myTransform);
                SkillsMenuController.OnAbilitySelected(AbilityType);
                SetVisuals(true);
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SetVisuals(false);
        }
        
        void OnDisable()
        {
            if (isInitialized)
                CloseVisuals();
        }

        private void OnAbilityStateChanged(object sender, Utilities.EventManager.AbilityStateChangedEventArgs args)
        {
            if (args.AbilityType == AbilityType)
            {
                SetVisualsFromState();
            }
        }

        //###########################################################

        // -- OPERATIONS

        void SetVisualsFromState()
        {
            switch (Model.GetAbilityState(AbilityType))
            {
                case AbilityState.inactive:
                    IconImageComponent.color = inactiveColor;
                    NameTextComponent.color = inactiveColor;
                    BackgroundImageComponent.color = inactiveColor;
                    break;
                case AbilityState.active:
                    IconImageComponent.color = activeColor;
                    NameTextComponent.color = activeColor;
                    BackgroundImageComponent.color = activeColor;
                    break;
            }
            CloseVisuals();
        }

        void CloseVisuals()
        {
            NameTextComponent.color *= new Color(1, 1, 1, 0);
            IconImageComponent.color += new Color(0, 0, 0, 1);
            myTransform.sizeDelta = new Vector2(deselectedWidth, myTransform.sizeDelta.y);
        }

        private void SetVisuals(bool selected)
        {
            StartCoroutine(_Transition(selected));
        }
        
        IEnumerator _Transition(bool selected)
        {
            Color color = IconImageComponent.color;
            Vector2 size = myTransform.sizeDelta;
            
            color.a = selected ? 1 : 0;
            IconImageComponent.color = color;
            
            color.a = 1-color.a;
            NameTextComponent.color = color;
            
            size.x = selected ? deselectedWidth : selectedWidth;
            myTransform.sizeDelta = size;

            for(float elapsed = 0; elapsed < transitionDuration; elapsed += Time.unscaledDeltaTime)
            {
                float t = elapsed / transitionDuration;

                color.a = selected ? 1-t : t;

                IconImageComponent.color = color;
                
                color.a = 1 - color.a;
                NameTextComponent.color = color;

                size.x = Mathf.Lerp(selected ? deselectedWidth : selectedWidth, selected ? selectedWidth : deselectedWidth, elapsed/transitionDuration);
                myTransform.sizeDelta = size;

                yield return null;
            }

            color.a = selected ? 0 : 1;
            IconImageComponent.color = color;

            color.a = 1 - color.a;
            NameTextComponent.color = color;

            size.x = selected ? selectedWidth : deselectedWidth;
            myTransform.sizeDelta = size;
        }
        
        public void Hide()
        {
            Vector3 pos = myTransform.anchoredPosition;
            pos.x = -1000;
            myTransform.anchoredPosition = pos;
        }

        public void Entrance()
        {
            StartCoroutine(_Entrance());
        }

        IEnumerator _Entrance()
        {
            Vector3 pos = myTransform.anchoredPosition;

            for(float elapsed = 0; elapsed < entranceDuration; elapsed+=Time.unscaledDeltaTime)
            {
                pos.x = Mathf.Lerp(-550, 0, elapsed / entranceDuration);
                myTransform.anchoredPosition = pos;
                yield return null;
            }

            pos.x = 0;
            myTransform.anchoredPosition = pos;
        }

    }
} // end of namespace