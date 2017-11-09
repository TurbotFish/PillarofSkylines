﻿using System.Collections;
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
        Player.eAbilityType selectedAbility { get { return this.abilityOrder[this.selectedAbilityIndex]; } }

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

            if (Input.GetButtonDown("MenuButton"))
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(eUiState.HUD));
                return;
            }

            //###########################################################

            float stickValue = Input.GetAxis("Vertical");

            if (Input.GetButton("Jump") && !this.activationButtonDown)
            {
                if(!this.playerModel.CheckAbilityActive(this.selectedAbility) && this.playerModel.ActivateAbility(this.selectedAbility))
                {
                    this.leftColumnView.SetAbilityActive(this.playerModel.AbilityData.GetAbility(this.selectedAbility), true);
                }
                else if(this.playerModel.CheckAbilityActive(this.selectedAbility) && this.playerModel.DeactivateAbility(this.selectedAbility))
                {
                    this.leftColumnView.SetAbilityActive(this.playerModel.AbilityData.GetAbility(this.selectedAbility), false);
                }

                this.activationButtonDown = true;
            }
            else if (!Input.GetButton("Jump"))
            {
                this.activationButtonDown = false;
            }

            //###########################################################

            if (Mathf.Approximately(stickValue, 0f))
            {
                this.selectionDelayTimer = 0f;
            }

            if (this.selectionDelayTimer > 0)
            {
                this.selectionDelayTimer -= Time.unscaledDeltaTime;

                if (this.selectionDelayTimer <= 0)
                {
                    this.selectionDelayTimer = 0;
                }
            }
            else
            {
                if (stickValue < -0.8f)
                {
                    this.selectedAbilityIndex++;

                    if (this.selectedAbilityIndex >= this.abilityOrder.Count)
                    {
                        this.selectedAbilityIndex = 0;
                    }

                    this.leftColumnView.SetAbilitySelected(this.playerModel.AbilityData.GetAbility(this.selectedAbility));

                    this.selectionDelayTimer += this.defaultSelectionDelay;
                }
                else if (stickValue > 0.8f)
                {
                    this.selectedAbilityIndex--;

                    if (this.selectedAbilityIndex < 0)
                    {
                        this.selectedAbilityIndex = this.abilityOrder.Count - 1;
                    }

                    this.leftColumnView.SetAbilitySelected(this.playerModel.AbilityData.GetAbility(this.selectedAbility));

                    this.selectionDelayTimer += this.defaultSelectionDelay;
                }
            }

            //###########################################################
        }

        #endregion monobehaviour methods

        //###########################################################

        void IUiState.Initialize(Player.PlayerModel playerModel)
        {
            this.playerModel = playerModel;

            this.topBarView.Initialize(playerModel.Favours);

            foreach (var ability in playerModel.AbilityData.GetAllAbilities())
            {
                this.abilityOrder.Add(ability.Type);
                this.leftColumnView.CreateAbilityElement(ability, playerModel.CheckAbilityGroupUnlocked(ability.Group));
            }

            this.leftColumnView.SetAbilitySelected(this.playerModel.AbilityData.GetAbility(this.selectedAbility));
        }

        void IUiState.Activate(Utilities.EventManager.OnShowMenuEventArgs args)
        {
            if (this.IsActive)
            {
                return;
            }

            this.IsActive = true;
            this.gameObject.SetActive(true);

            
        }

        void IUiState.Deactivate()
        {
            //bool wasActive = this.IsActive;

            this.IsActive = false;
            this.gameObject.SetActive(false);

            //if (wasActive)
            //{
            //    Utilities.EventManager.SendOnMenuClosedEvent(this, new Utilities.EventManager.OnMenuClosedEventArgs(eUiState.AbilityMenu));
            //}
        }

        //###########################################################
    }
} //end of namespace