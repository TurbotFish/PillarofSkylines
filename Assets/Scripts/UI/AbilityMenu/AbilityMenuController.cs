using Game.GameControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.AbilityMenu
{
    public class AbilityMenuController : MonoBehaviour, IUiState
    {
        //##################################################################

        const float CIRCLE_INTERVAL = 360f / 8f;

        [Tooltip("If the total value of the 2 axes of the left stick is less than this value, no slot is selected.")]
        [SerializeField]
        float stickDeadWeight;

        [Header("")]
        [SerializeField]
        CenterView centerView;

        [SerializeField]
        List<SlotView> abilityViews = new List<SlotView>();

        //[SerializeField]
        //GroupView orangeGroup;

        //[SerializeField]
        //GroupView yellowGroup;

        //[SerializeField]
        //GroupView blueGroup;

        //[SerializeField]
        //GroupView greenGroup;

        [Header("")]
        [SerializeField]
        Color lockedAbilityColour;
        public Color LockedAbilityColour { get { return lockedAbilityColour; } }

        [SerializeField]
        Color availableAbilityColour;
        public Color AvailableAbilityColour { get { return availableAbilityColour; } }

        [SerializeField]
        Color activeAbilityColour;
        public Color ActiveAbilityColour { get { return activeAbilityColour; } }

        [Space, SerializeField]
        TMPro.TextMeshProUGUI helpMessage;

        [Space, SerializeField]
        TMPro.TextMeshProUGUI descriptionHelpMessage;

        public Cursor cursor;

        Player.PlayerModel playerModel;

        public bool IsActive { get; private set; }

        //List<SlotView> slotList = new List<SlotView>();
        int selectedSlotIndex = -1;
        SlotView SelectedSlot
        {
            get
            {
                if (selectedSlotIndex == -1) { return null; }

                return abilityViews[selectedSlotIndex];
            }
        }

        bool activationButtonDown = false;
        Vector2 previousStickPos = Vector2.zero;

        //##################################################################

        void IUiState.Initialize(IGameControllerBase gameController)
        {
            playerModel = gameController.PlayerModel;

            //slotList.AddRange(orangeGroup.Slots);
            //slotList.AddRange(yellowGroup.Slots);
            //slotList.AddRange(blueGroup.Slots);
            //slotList.AddRange(greenGroup.Slots);

            for (int i = 0; i < abilityViews.Count; i++)
            {
                abilityViews[i].Initialize(playerModel, this);
            }

            centerView.Initialize(playerModel, this);
            helpMessage.text = "";
        }

        void IUiState.Activate(Utilities.EventManager.OnShowMenuEventArgs args)
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;
            gameObject.SetActive(true);
        }

        void IUiState.Deactivate()
        {
            IsActive = false;
            gameObject.SetActive(false);
        }

        //##################################################################

        #region update

        void Update()
        {
            if (!IsActive)
                return;

            //exit ability menu
            if (Input.GetButtonDown("MenuButton") || Input.GetButtonDown("Cancel"))
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(eUiState.HUD));
                return;
            }
            else if (Input.GetButtonDown("Back"))
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(eUiState.HelpMenu));
                return;
            }

            //**********************************************
            //handle ability activation

            if (Input.GetButton("Jump") && !activationButtonDown && SelectedSlot != null)
            {
                ActivateSelectedAbility();

                activationButtonDown = true;
            }
            else if (!Input.GetButton("Jump"))
            {
                activationButtonDown = false;
            }

            //**********************************************
            //handle ability selection

            var leftStick = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if ((previousStickPos - leftStick).magnitude > 0.05f) //ignore stick if it is not moving
            {
                float weight = Mathf.Abs(leftStick.x) + Mathf.Abs(leftStick.y);

                if (weight < stickDeadWeight)
                {
                    SetSelectedSlot(-1);
                }
                else
                {
                    float angle = Vector2.SignedAngle(Vector2.up, leftStick.normalized);

                    if (angle < 0f)
                    {
                        angle *= -1;
                    }
                    else
                    {
                        angle = 360f - angle;
                    }

                    angle += CIRCLE_INTERVAL * 0.5f;

                    if (angle >= 360f)
                    {
                        angle -= 360f;
                    }

                    SetSelectedSlot((int)(angle / CIRCLE_INTERVAL));

                    //Debug.LogFormat("AbilityMenuController: Update: stick={0} ; angle={1} ; slot = {2}", leftStick, angle, selectedSlotIndex);
                }

                previousStickPos = leftStick;
            }

            //**********************************************
        }

        #endregion update

        //##################################################################

        public void OnPointerEnterSlot(SlotView slot)
        {
            int slotIndex = abilityViews.IndexOf(slot);

            if (slotIndex != selectedSlotIndex)
            {
                SetSelectedSlot(slotIndex);
            }
        }

        public void OnPointerExitSlot(SlotView slot)
        {
            int slotIndex = abilityViews.IndexOf(slot);

            if (slotIndex == selectedSlotIndex)
            {
                SetSelectedSlot(-1);
            }
        }

        public void OnPointerClickSlot(SlotView slot)
        {
            int slotIndex = abilityViews.IndexOf(slot);

            if (slotIndex == selectedSlotIndex)
            {
                ActivateSelectedAbility();
            }
        }

        //##################################################################

        void SetSelectedSlot(int slotIndex)
        {
            //Debug.LogFormat("AbilityMenuController: SetSelectedSlot: index={0}", slotIndex);

            selectedSlotIndex = slotIndex;

            for (int i = 0; i < abilityViews.Count; i++)
            {
                if (i == selectedSlotIndex)
                {
                    cursor.GoTo(abilityViews[i].transform.position);
                    abilityViews[i].SetSelected(true);
                }
                else
                {
                    abilityViews[i].SetSelected(false);
                }
            }

            if (SelectedSlot == null)
            {
                centerView.SetContent(null);
                cursor.gameObject.SetActive(false);
            }
            else
            {
                cursor.gameObject.SetActive(true);
                centerView.SetContent(playerModel.AbilityData.GetAbility(SelectedSlot.AbilityType));
            }

            UpdateInputMessage();
        }

        void ActivateSelectedAbility()
        {
            if (SelectedSlot == null)
                return;

            print(" check if : " + SelectedSlot.AbilityType + " is active : " + playerModel.CheckAbilityActive(SelectedSlot.AbilityType));

            if (playerModel.CheckAbilityActive(SelectedSlot.AbilityType))
                playerModel.DeactivateAbility(SelectedSlot.AbilityType);
            else
                playerModel.ActivateAbility(SelectedSlot.AbilityType);
            UpdateInputMessage();
        }

        void UpdateInputMessage()
        {
            helpMessage.color = new Color(1, 1, 1);
            if (SelectedSlot == null)
                helpMessage.text = "";
            else if (playerModel.CheckAbilityActive(SelectedSlot.AbilityType))
            {
                helpMessage.text = "[A] Remove Favour";
                descriptionHelpMessage.text = "Press [A] to stop using this ability";
            }
            else
            {
                if (!playerModel.CheckAbilityUnlocked(SelectedSlot.AbilityType))
                {
                    helpMessage.text = "Destroy a Pillar to Unlock";
                    descriptionHelpMessage.text = "Destroy a Pillar to unlock this ability";
                }
                else if (playerModel.Favours <= 0)
                {
                    helpMessage.text = "";
                    descriptionHelpMessage.text = "You don't have enough Favours to unlock this ability";
                }
                else 
                {
                    helpMessage.text = "[A] Place Favour";
                    descriptionHelpMessage.text = "Press [A] to start using this ability";
                    helpMessage.color = ActiveAbilityColour;
                }

            }
        }

        //##################################################################
    }
} //end of namespace