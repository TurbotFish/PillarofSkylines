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
        int selectedSlotIndex = 0;
        SlotView SelectedSlot
        {
            get
            {
                if (selectedSlotIndex == -1) { return null; }

                return slotList[selectedSlotIndex];
            }
        }

        bool activationButtonDown = false;

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
            if (Input.GetButtonDown("MenuButton"))
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(eUiState.HUD));
                return;
            }

            //**********************************************
            //handle ability activation

            if (Input.GetButton("Jump") && !activationButtonDown && SelectedSlot != null)
            {
                if (playerModel.CheckAbilityActive(SelectedSlot.AbilityType))
                {
                    Debug.LogFormat("Ability Menu: deactivating ability {0}", SelectedSlot.AbilityType);
                    playerModel.DeactivateAbility(SelectedSlot.AbilityType);
                }
                else
                {
                    Debug.LogFormat("Ability Menu: activating ability {0}", SelectedSlot.AbilityType);
                    playerModel.ActivateAbility(SelectedSlot.AbilityType);
                }

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

            float weight = Mathf.Abs(leftStick.x) + Mathf.Abs(leftStick.y);

            if (weight < stickDeadWeight)
            {
                selectedSlotIndex = -1;

                centerView.SetContent(null);
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

                selectedSlotIndex = (int)(angle / CIRCLE_INTERVAL);

                centerView.SetContent(playerModel.AbilityData.GetAbility(SelectedSlot.AbilityType));

                Debug.LogFormat("AbilityMenuController: Update: stick={0} ; angle={1} ; slot = {2}", leftStick, angle, selectedSlotIndex);
            }

            //setting the selection state of all the slots
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

            //**********************************************
        }

        #endregion update



        //##################################################################
    }
} //end of namespace