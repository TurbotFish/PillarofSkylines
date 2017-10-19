using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.UI.AbilityMenu
{
    public class AbilityMenuController : MonoBehaviour, IUiState
    {
        [SerializeField]
        TopBarView topBarView;

        [SerializeField]
        LeftColumnView leftColumnView;

        [SerializeField]
        float defaultSelectionDelay = 0.2f;

        PlayerModel playerModel;

        public bool IsActive { get; private set; }

        List<eAbilityType> abilityOrder = new List<eAbilityType>();
        int selectedAbilityIndex = 0;

        float selectionDelayTimer = 0;
        bool activationButtonDown = false;

        //###########################################################

        #region monobehaviour methods

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!IsActive)
            {
                return;
            }

            float stickValue = Input.GetAxis("Vertical");

            if (Input.GetButton("Jump") && !this.activationButtonDown)
            {
                this.activationButtonDown = true;
                this.leftColumnView.SetAbilityActive(this.playerModel.AbilityData.GetAbility(this.abilityOrder[this.selectedAbilityIndex]));
            }
            else if (!Input.GetButton("Jump"))
            {
                this.activationButtonDown = false;
            }

            if (this.selectionDelayTimer > 0)
            {
                this.selectionDelayTimer -= Time.deltaTime;

                if (this.selectionDelayTimer < 0)
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

                    this.leftColumnView.SetAbilitySelected(this.abilityOrder[this.selectedAbilityIndex]);

                    this.defaultSelectionDelay += this.defaultSelectionDelay;
                }
                else if (stickValue > 0.8f)
                {
                    this.selectedAbilityIndex--;

                    if (this.selectedAbilityIndex < 0)
                    {
                        this.selectedAbilityIndex = this.abilityOrder.Count - 1;
                    }

                    this.leftColumnView.SetAbilitySelected(this.abilityOrder[this.selectedAbilityIndex]);

                    this.selectionDelayTimer += this.defaultSelectionDelay;
                }
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        void IUiState.Initialize(PlayerModel playerModel)
        {
            this.playerModel = playerModel;

            this.topBarView.Initialize(playerModel.Favours);

            foreach (var ability in playerModel.AbilityData.GetAllAbilities())
            {
                this.abilityOrder.Add(ability.Type);
                this.leftColumnView.CreateAbilityElement(ability, playerModel.CheckAbilityGroupUnlocked(ability.Group));
            }

            this.leftColumnView.SetAbilitySelected(this.abilityOrder[this.selectedAbilityIndex]);
        }

        void IUiState.Activate()
        {
            if (this.IsActive)
            {
                return;
            }

            this.IsActive = true;
            this.gameObject.SetActive(true);

            Utilities.EventManager.SendOnMenuOpenedEvent(this, new Utilities.EventManager.OnMenuOpenedEventArgs(eUiState.AbilityMenu));
        }

        void IUiState.Deactivate()
        {
            bool wasActive = this.IsActive;

            this.IsActive = false;
            this.gameObject.SetActive(false);

            if (wasActive)
            {
                Utilities.EventManager.SendOnMenuClosedEvent(this, new Utilities.EventManager.OnMenuClosedEventArgs(eUiState.AbilityMenu));
            }
        }

        //###########################################################
    }
} //end of namespace