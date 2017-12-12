using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.AbilityMenu
{
    public class AbilityMenuController : MonoBehaviour, IUiState
    {
        //##################################################################

        const float CIRCLE_INTERVAL = 360f / 12f;

        [Tooltip("If the total value of the 2 axes of the left stick is less than this value, no slot is selected.")]
        [SerializeField]
        float stickDeadWeight;

        [Header("")]
        [SerializeField]
        CenterView centerView;

        [SerializeField]
        GroupView orangeGroup;

        [SerializeField]
        GroupView yellowGroup;

        [SerializeField]
        GroupView blueGroup;

        [SerializeField]
        GroupView greenGroup;

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



        Player.PlayerModel playerModel;

        public bool IsActive { get; private set; }

        List<SlotView> slotList = new List<SlotView>();
        int selectedSlotIndex = -1;
        SlotView SelectedSlot
        {
            get
            {
                if (selectedSlotIndex == -1) { return null; }

                return slotList[selectedSlotIndex];
            }
        }

        bool activationButtonDown = false;
        Vector2 previousStickPos = Vector2.zero;

        //##################################################################

        void IUiState.Initialize(Player.PlayerModel playerModel)
        {
            this.playerModel = playerModel;

            slotList.AddRange(orangeGroup.Slots);
            slotList.AddRange(yellowGroup.Slots);
            slotList.AddRange(blueGroup.Slots);
            slotList.AddRange(greenGroup.Slots);

            for (int i = 0; i < slotList.Count; i++)
            {
                slotList[i].Initialize(playerModel, this, playerModel.AbilityData.AbilitySlots[i]);
            }

            centerView.Initialize(playerModel, this);
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
            {
                return;
            }

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

            //var leftStick = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
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
            //Debug.Log("aaa");
            int slotIndex = slotList.IndexOf(slot);

            if (slotIndex != selectedSlotIndex)
            {
                //Debug.Log("aaa ok");
                SetSelectedSlot(slotIndex);
            }
        }

        public void OnPointerExitSlot(SlotView slot)
        {
            //Debug.Log("bbb");
            int slotIndex = slotList.IndexOf(slot);

            if (slotIndex == selectedSlotIndex)
            {
                //Debug.Log("bbb ok");
                SetSelectedSlot(-1);
            }
        }

        public void OnPointerClickSlot(SlotView slot)
        {
            //Debug.Log("ccc");
            int slotIndex = slotList.IndexOf(slot);

            if (slotIndex == selectedSlotIndex)
            {
                //Debug.Log("ccc ok");
                ActivateSelectedAbility();
            }
        }

        //##################################################################

        void SetSelectedSlot(int slotIndex)
        {
            //Debug.LogFormat("AbilityMenuController: SetSelectedSlot: index={0}", slotIndex);

            selectedSlotIndex = slotIndex;

            for (int i = 0; i < slotList.Count; i++)
            {
                if (i == selectedSlotIndex)
                {
                    slotList[i].SetSelected(true);
                }
                else
                {
                    slotList[i].SetSelected(false);
                }
            }

            if (SelectedSlot == null)
            {
                centerView.SetContent(null);
            }
            else
            {
                centerView.SetContent(playerModel.AbilityData.GetAbility(SelectedSlot.AbilityType));
            }
        }

        void ActivateSelectedAbility()
        {
            if (SelectedSlot == null)
            {
                return;
            }

            if (playerModel.CheckAbilityActive(SelectedSlot.AbilityType))
            {
                //Debug.LogFormat("Ability Menu: deactivating ability {0}", SelectedSlot.AbilityType);
                playerModel.DeactivateAbility(SelectedSlot.AbilityType);
            }
            else
            {
                //Debug.LogFormat("Ability Menu: activating ability {0}", SelectedSlot.AbilityType);
                playerModel.ActivateAbility(SelectedSlot.AbilityType);
            }
        }
    }
} //end of namespace