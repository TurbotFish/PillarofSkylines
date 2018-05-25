//using Game.GameControl;
//using Game.Model;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Game.UI.AbilityMenu
//{
//    public class AbilityMenuController : MonoBehaviour, IUiMenu
//    {
//        const float CIRCLE_INTERVAL = 360f / 8f;

//        //##################################################################

//        [Tooltip("If the total value of the 2 axes of the left stick is less than this value, no slot is selected.")]
//        [SerializeField] float stickDeadWeight;

//        [Header("")]
//        [SerializeField] private CenterView centerView;

//        [SerializeField] private List<SlotView> abilityViews = new List<SlotView>();

//        [Space, SerializeField] TMPro.TextMeshProUGUI helpMessage;

//        [Space, SerializeField] TMPro.TextMeshProUGUI descriptionHelpMessage;

//        [SerializeField] private Cursor cursor;

//        [Header("Colours")]
//        [SerializeField] private Color lockedAbilityColour;

//        [SerializeField] private Color availableAbilityColour;

//        [SerializeField] private Color activeAbilityColour;

//        [SerializeField] private Color pillarLockedAbilityColour;


//        PlayerModel playerModel;

//        bool isEchoAbilityActive;
//        int selectedSlotIndex = -1;

//        bool activationButtonDown = false;
//        Vector2 previousStickPos = Vector2.zero;

//        //##################################################################

//        public bool IsActive { get; private set; }

//        public Color LockedAbilityColour { get { return lockedAbilityColour; } }
//        public Color AvailableAbilityColour { get { return availableAbilityColour; } }
//        public Color ActiveAbilityColour { get { return activeAbilityColour; } }
//        public Color PillarLockedAbilityColour { get { return pillarLockedAbilityColour; } }

//        SlotView SelectedSlot
//        {
//            get
//            {
//                if (selectedSlotIndex == -1) { return null; }

//                return abilityViews[selectedSlotIndex];
//            }
//        }

//        //##################################################################

//        void IUiMenu.Initialize(IGameController gameController)
//        {
//            playerModel = gameController.PlayerModel;

//            for (int i = 0; i < abilityViews.Count; i++)
//            {
//                abilityViews[i].Initialize(playerModel, this);
//            }

//            centerView.Initialize(playerModel, this);
//            helpMessage.text = "";
//        }

//        void IUiMenu.Activate(Utilities.EventManager.OnShowMenuEventArgs args)
//        {
//            if (IsActive)
//            {
//                return;
//            }

//            IsActive = true;
//            gameObject.SetActive(true);
//            centerView.Activate();

//            isEchoAbilityActive = playerModel.CheckAbilityActive(AbilityType.Echo);

//            //not tutorial
//            if (isEchoAbilityActive)
//            {
//                foreach (var slot in abilityViews)
//                {
//                    slot.Activate();
//                }
//            }
//            //tutorial
//            else
//            {
//                foreach (var slot in abilityViews)
//                {
//                    slot.Deactivate();
//                }

//                centerView.SetContent(playerModel.AbilityData.GetAbility(AbilityType.Echo));
//            }

//            UpdateInputMessage();
//        }

//        void IUiMenu.Deactivate()
//        {
//            centerView.Deactivate();
//            foreach (var slot in abilityViews)
//            {
//                slot.Deactivate();
//            }

//            gameObject.SetActive(false);
//            IsActive = false;
//        }

//        //##################################################################

//        #region update

//        void Update()
//        {
//            if (!IsActive)
//            {
//                return;
//            }

//            //exit ability menu
//            if (Input.GetButtonDown("MenuButton") || Input.GetButtonDown("Cancel"))
//            {
//                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(MenuType.HUD));
//                return;
//            }
//            //show help menu
//            else if (Input.GetButtonDown("Back"))
//            {
//                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(MenuType.HelpMenu));
//                return;
//            }

//            //**********************************************
//            //handle ability activation

//            ////not tutorial
//            //if (isEchoAbilityActive)
//            //{
//            //    if (Input.GetButtonDown("Jump") && !activationButtonDown && SelectedSlot != null)
//            //    {
//            //        ActivateSelectedAbility();

//            //        activationButtonDown = true;
//            //    }
//            //    else if (!Input.GetButton("Jump"))
//            //    {
//            //        activationButtonDown = false;
//            //    }
//            //}
//            ////tutorial
//            //else
//            //{
//            //    if (Input.GetButtonDown("Jump"))
//            //    {
//            //        if (playerModel.ActivateAbility(eAbilityType.Echo))
//            //        {
//            //            foreach (var slot in abilityViews)
//            //            {
//            //                slot.Activate();
//            //            }

//            //            SetSelectedSlot(-1);

//            //            isEchoAbilityActive = true;
//            //        }
//            //    }
//            //}

//            //**********************************************
//            //handle ability selection

//            if (isEchoAbilityActive)
//            {
//                var leftStick = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

//                if ((previousStickPos - leftStick).magnitude > 0.05f) //ignore stick if it is not moving
//                {
//                    float weight = Mathf.Abs(leftStick.x) + Mathf.Abs(leftStick.y);

//                    if (weight < stickDeadWeight)
//                    {
//                        SetSelectedSlot(-1);
//                    }
//                    else
//                    {
//                        float angle = Vector2.SignedAngle(Vector2.up, leftStick.normalized);

//                        if (angle < 0f)
//                        {
//                            angle *= -1;
//                        }
//                        else
//                        {
//                            angle = 360f - angle;
//                        }

//                        angle += CIRCLE_INTERVAL * 0.5f;

//                        if (angle >= 360f)
//                        {
//                            angle -= 360f;
//                        }

//                        SetSelectedSlot((int)(angle / CIRCLE_INTERVAL));

//                        //Debug.LogFormat("AbilityMenuController: Update: stick={0} ; angle={1} ; slot = {2}", leftStick, angle, selectedSlotIndex);
//                    }

//                    previousStickPos = leftStick;
//                }
//            }

//            //**********************************************
//        }

//        #endregion update

//        //##################################################################

//        public void OnPointerEnterSlot(SlotView slot)
//        {
//            int slotIndex = abilityViews.IndexOf(slot);

//            if (slotIndex != selectedSlotIndex)
//            {
//                SetSelectedSlot(slotIndex);
//            }
//        }

//        public void OnPointerExitSlot(SlotView slot)
//        {
//            int slotIndex = abilityViews.IndexOf(slot);

//            if (slotIndex == selectedSlotIndex)
//            {
//                SetSelectedSlot(-1);
//            }
//        }

//        public void OnPointerClickSlot(SlotView slot)
//        {
//            int slotIndex = abilityViews.IndexOf(slot);

//            if (slotIndex == selectedSlotIndex)
//            {
//                ActivateSelectedAbility();
//            }
//        }

//        //##################################################################

//        void SetSelectedSlot(int slotIndex)
//        {
//            //Debug.LogFormat("AbilityMenuController: SetSelectedSlot: index={0}", slotIndex);

//            selectedSlotIndex = slotIndex;

//            for (int i = 0; i < abilityViews.Count; i++)
//            {
//                if (i == selectedSlotIndex)
//                {
//                    cursor.GoTo(abilityViews[i].transform.position);
//                    abilityViews[i].SetSelected(true);
//                }
//                else
//                {
//                    abilityViews[i].SetSelected(false);
//                }
//            }

//            if (SelectedSlot == null)
//            {
//                centerView.SetContent(null);
//                cursor.gameObject.SetActive(false);
//            }
//            else
//            {
//                cursor.gameObject.SetActive(true);
//                centerView.SetContent(playerModel.AbilityData.GetAbility(SelectedSlot.AbilityType));
//            }

//            UpdateInputMessage();
//        }

//        void ActivateSelectedAbility()
//        {
//            Debug.LogWarning("WIP: this still exists but does not do anything!");
//            //if (SelectedSlot == null)
//            //    return;

//            //print(" check if : " + SelectedSlot.AbilityType + " is active : " + playerModel.CheckAbilityActive(SelectedSlot.AbilityType));

//            //if (playerModel.CheckAbilityActive(SelectedSlot.AbilityType))
//            //{
//            //    playerModel.DeactivateAbility(SelectedSlot.AbilityType);
//            //}
//            //else
//            //{
//            //    playerModel.ActivateAbility(SelectedSlot.AbilityType);
//            //}

//            //UpdateInputMessage();
//        }

//        void UpdateInputMessage()
//        {
//            helpMessage.color = new Color(1, 1, 1);

//            //tutorial
//            if (!isEchoAbilityActive)
//            {
//                ////not enough favours
//                //if (playerModel.GetCurrencyAmount(eCurrencyType.Favour) < playerModel.AbilityData.GetAbility(eAbilityType.Echo).ActivationPrice)
//                //{
//                //    helpMessage.text = "";
//                //    descriptionHelpMessage.text = "You don't have enough Favours to unlock this ability";
//                //}
//                //enough favours
//                //else
//                //{
//                    helpMessage.text = "[A] Place Favour";
//                    descriptionHelpMessage.text = "Press [A] to start using this ability";
//                    helpMessage.color = ActiveAbilityColour;
//                //}
//            }
//            //no slot selected
//            else if (SelectedSlot == null)
//            {
//                helpMessage.text = "";
//            }
//            //ability is active
//            else if (playerModel.CheckAbilityActive(SelectedSlot.AbilityType))
//            {
//                helpMessage.text = "[A] Remove Favour";
//                descriptionHelpMessage.text = "Press [A] to stop using this ability";
//            }
//            ////ability is locked
//            //else if (!playerModel.CheckAbilityUnlocked(SelectedSlot.AbilityType))
//            //{
//            //    helpMessage.text = "Destroy a Pillar to Unlock";
//            //    descriptionHelpMessage.text = "Destroy a Pillar to unlock this ability";
//            //}
//            ////not enough favours
//            //else if (playerModel.GetCurrencyAmount(eCurrencyType.Favour) < playerModel.AbilityData.GetAbility(SelectedSlot.AbilityType).ActivationPrice)
//            //{
//            //    helpMessage.text = "";
//            //    descriptionHelpMessage.text = "You don't have enough Favours to unlock this ability";
//            //}
//            //enough favours
//            else
//            {
//                helpMessage.text = "[A] Place Favour";
//                descriptionHelpMessage.text = "Press [A] to start using this ability";
//                helpMessage.color = ActiveAbilityColour;
//            }

//        }

//        //##################################################################
//    }
//} //end of namespace