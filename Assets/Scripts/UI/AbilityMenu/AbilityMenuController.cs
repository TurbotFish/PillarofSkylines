using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.AbilityMenu
{
    public class AbilityMenuController : MonoBehaviour, IUiState
    {
        [SerializeField]
        TopBarView topBarView;

        [SerializeField]
        LeftColumnView leftColumnView;

        [SerializeField]
        float defaultSelectionDelay = 0.2f;

        Player.PlayerModel playerModel;

        public bool IsActive { get; private set; }

        List<Player.eAbilityType> abilityOrder = new List<Player.eAbilityType>();
        int selectedAbilityIndex = 0;
        Player.eAbilityType selectedAbility { get { return abilityOrder[selectedAbilityIndex]; } }

        float selectionDelayTimer = 0;
        bool activationButtonDown = false;

        //###########################################################

        #region monobehaviour methods

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

            for (int i = 0; i < abilityOrder.Count; i++)
            {
                var abilityType = abilityOrder[i];
                eAbilityElementState state;

                if (!playerModel.CheckAbilityUnlocked(abilityType))
                {
                    state = eAbilityElementState.Locked;
                }
                else if (playerModel.CheckAbilityActive(abilityType))
                {
                    state = eAbilityElementState.Activated;
                }
                else
                {
                    state = eAbilityElementState.Deactivated;
                }

                leftColumnView.SetAbilityState(abilityType, state);
            }

            //**********************************************

            float stickValue = Input.GetAxis("Vertical");

            if (Input.GetButton("Jump") && !activationButtonDown)
            {
                if (playerModel.CheckAbilityActive(selectedAbility))
                {
                    playerModel.DeactivateAbility(selectedAbility);
                }
                else
                {
                    playerModel.ActivateAbility(selectedAbility);
                }

                this.activationButtonDown = true;
            }
            else if (!Input.GetButton("Jump"))
            {
                activationButtonDown = false;
            }

            //**********************************************

            if (Mathf.Approximately(stickValue, 0f))
            {
                selectionDelayTimer = 0f;
            }

            if (selectionDelayTimer > 0)
            {
                selectionDelayTimer -= Time.unscaledDeltaTime;

                if (selectionDelayTimer <= 0)
                {
                    selectionDelayTimer = 0;
                }
            }
            else
            {
                if (stickValue < -0.8f)
                {
                    selectedAbilityIndex++;

                    if (selectedAbilityIndex >= abilityOrder.Count)
                    {
                        selectedAbilityIndex = 0;
                    }

                    leftColumnView.SetAbilitySelected(playerModel.AbilityData.GetAbility(selectedAbility));

                    selectionDelayTimer += defaultSelectionDelay;
                }
                else if (stickValue > 0.8f)
                {
                    selectedAbilityIndex--;

                    if (selectedAbilityIndex < 0)
                    {
                        selectedAbilityIndex = abilityOrder.Count - 1;
                    }

                    leftColumnView.SetAbilitySelected(playerModel.AbilityData.GetAbility(selectedAbility));

                    selectionDelayTimer += defaultSelectionDelay;
                }
            }

            //###########################################################
        }

        #endregion monobehaviour methods

        //###########################################################

        void IUiState.Initialize(Player.PlayerModel playerModel)
        {
            this.playerModel = playerModel;

            topBarView.Initialize(playerModel.Favours);

            foreach (var ability in playerModel.AbilityData.GetAllAbilities())
            {
                abilityOrder.Add(ability.Type);
                leftColumnView.CreateAbilityElement(ability, playerModel.CheckAbilityGroupUnlocked(ability.Group));
            }

            leftColumnView.SetAbilitySelected(playerModel.AbilityData.GetAbility(selectedAbility));
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

        //###########################################################
    }
} //end of namespace