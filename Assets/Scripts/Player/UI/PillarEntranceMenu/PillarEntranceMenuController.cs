using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.UI.PillarEntranceMenu
{
    public class PillarEntranceMenuController : MonoBehaviour, IUiState
    {
        [SerializeField]
        CostPanelView costPanelView;

        PlayerModel playerModel;

        public bool IsActive { get; private set; }
        bool isMenuButtonDown = false;

        //###########################################################

        #region monobehaviour methods

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!this.IsActive)
            {
                return;
            }

            if (Input.GetButton("Menu") && !this.isMenuButtonDown)
            {
                this.isMenuButtonDown = true;

                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(eUiState.HUD));
            }
            else if (!Input.GetButton("Menu") && this.isMenuButtonDown)
            {
                this.isMenuButtonDown = false;
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        void IUiState.Initialize(PlayerModel playerModel)
        {
            this.playerModel = playerModel;
        }

        void IUiState.Activate()
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
            bool wasActive = this.IsActive;

            this.IsActive = false;
            this.gameObject.SetActive(false);
        }

        //###########################################################
    }
} //end of namespace