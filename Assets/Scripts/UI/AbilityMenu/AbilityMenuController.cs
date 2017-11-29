using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.AbilityMenu
{
    public class AbilityMenuController : MonoBehaviour, IUiState
    {
        //##################################################################

        const float CIRCLE_INTERVAL = 360f / 12f;

        //[SerializeField]
        //TopBarView topBarView;

        //[SerializeField]
        //LeftColumnView leftColumnView;

        [SerializeField]
        float defaultSelectionDelay = 0.2f;

        [Tooltip("If the total value of the 2 axes of the left stick is less than this value, no slot is selected.")]
        [SerializeField]
        float stickDeadWeight;

        [Header("")]
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
        public Color LockedAbilityColour { get { return LockedAbilityColour; } }

        [SerializeField]
        Color availableAbilityColour;
        public Color AvailableAbilityColour { get { return availableAbilityColour; } }

        [SerializeField]
        Color activeAbilityColour;
        public Color ActiveAbilityColour { get { return activeAbilityColour; } }



        Player.PlayerModel playerModel;

        public bool IsActive { get; private set; }

        //List<Player.eAbilityType> abilityOrder = new List<Player.eAbilityType>();
        //Player.eAbilityType SelectedAbility { get { return abilityOrder[selectedAbilityIndex]; } }

        List<SlotView> slotList = new List<SlotView>();
        int selectedSlotIndex = 0;
        SlotView SelectedSlot
        {
            get
            {
                if (selectedSlotIndex == -1)
                {
                    return null;
                }

                return slotList[selectedSlotIndex];
            }
        }

        float selectionDelayTimer = 0;
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


            //topBarView.Initialize(playerModel.Favours);

            //foreach (var ability in playerModel.AbilityData.GetAllAbilities())
            //{
            //    abilityOrder.Add(ability.Type);
            //    leftColumnView.CreateAbilityElement(ability, playerModel.CheckAbilityGroupUnlocked(ability.Group));
            //}

            //leftColumnView.SetAbilitySelected(playerModel.AbilityData.GetAbility(SelectedAbility));
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
            //updating the state of the ability element views

            //for (int i = 0; i < abilityOrder.Count; i++)
            //{
            //    var abilityType = abilityOrder[i];
            //    eAbilityElementState state;

            //    if (!playerModel.CheckAbilityUnlocked(abilityType))
            //    {
            //        state = eAbilityElementState.Locked;
            //    }
            //    else if (playerModel.CheckAbilityActive(abilityType))
            //    {
            //        state = eAbilityElementState.Activated;
            //    }
            //    else
            //    {
            //        state = eAbilityElementState.Deactivated;
            //    }

            //    leftColumnView.SetAbilityState(abilityType, state);
            //}

            //**********************************************
            //handle ability activation

            if (Input.GetButton("Jump") && !activationButtonDown && SelectedSlot != null)
            {
                if (playerModel.CheckAbilityActive(SelectedSlot.AbilityType))
                {
                    playerModel.DeactivateAbility(SelectedSlot.AbilityType);
                }
                else
                {
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

            var leftStick = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            float weight = Mathf.Abs(leftStick.x) + Mathf.Abs(leftStick.y);

            if(weight < stickDeadWeight)
            {
                selectedSlotIndex = -1;
            }
            else
            {
                float angle = Vector2.SignedAngle(Vector2.up, leftStick.normalized);
                if(angle < 180f)
                {
                    angle = 360f + angle;
                }
                angle += CIRCLE_INTERVAL * 0.5f;

                int slot = Mathf.RoundToInt(angle % CIRCLE_INTERVAL);

                Debug.LogFormat("AbilityMenuController: Update: selected slot = {0}", slot);
            }


            //float stickValue = Input.GetAxis("Vertical");

            //if (Mathf.Approximately(stickValue, 0f))
            //{
            //    selectionDelayTimer = 0f;
            //}

            //if (selectionDelayTimer > 0)
            //{
            //    selectionDelayTimer -= Time.unscaledDeltaTime;

            //    if (selectionDelayTimer <= 0)
            //    {
            //        selectionDelayTimer = 0;
            //    }
            //}
            //else
            //{
            //    if (stickValue < -0.8f)
            //    {
            //        selectedSlotIndex++;

            //        if (selectedSlotIndex >= abilityOrder.Count)
            //        {
            //            selectedSlotIndex = 0;
            //        }

            //        leftColumnView.SetAbilitySelected(playerModel.AbilityData.GetAbility(SelectedAbility));

            //        selectionDelayTimer += defaultSelectionDelay;
            //    }
            //    else if (stickValue > 0.8f)
            //    {
            //        selectedSlotIndex--;

            //        if (selectedSlotIndex < 0)
            //        {
            //            selectedSlotIndex = abilityOrder.Count - 1;
            //        }

            //        leftColumnView.SetAbilitySelected(playerModel.AbilityData.GetAbility(SelectedAbility));

            //        selectionDelayTimer += defaultSelectionDelay;
            //    }
            //}

            //**********************************************
        }

        #endregion update



        //##################################################################
    }
} //end of namespace