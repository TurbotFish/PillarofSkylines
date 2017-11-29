using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.PillarEntranceMenu
{
    public class PillarEntranceMenuController : MonoBehaviour, IUiState
    {
        [SerializeField]
        CostPanelView costPanelView;

        [SerializeField]
        GameObject warningMessage;

        Player.PlayerModel playerModel;

        //
        World.ePillarId pillarId;
        bool canEnterPillar = false;

        //
        public bool IsActive { get; private set; }

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

            if (Input.GetButtonDown("MenuButton"))
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(eUiState.HUD));
            }

            if (this.canEnterPillar && Input.GetButtonDown("Sprint"))
            {
                this.playerModel.Favours -= this.playerModel.PillarData.GetPillarEntryPrice(this.pillarId);
                Utilities.EventManager.SendEnterPillarEvent(this, new Utilities.EventManager.EnterPillarEventArgs(this.pillarId));
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        void IUiState.Initialize(Player.PlayerModel playerModel)
        {
            this.playerModel = playerModel;
        }

        void IUiState.Activate(Utilities.EventManager.OnShowMenuEventArgs args)
        {
            if (this.IsActive)
            {
                return;
            }
            else if (!(args is Utilities.EventManager.OnShowPillarEntranceMenuEventArgs))
            {
                throw new System.Exception("ERROR!");
            }

            this.IsActive = true;
            this.gameObject.SetActive(true);


            this.pillarId = (args as Utilities.EventManager.OnShowPillarEntranceMenuEventArgs).PillarId;

            int cost = this.playerModel.PillarData.GetPillarEntryPrice(this.pillarId);
            this.costPanelView.Initialize(cost);

            if (this.playerModel.Favours < cost)
            {
                this.warningMessage.SetActive(true);
            }
            else
            {
                this.warningMessage.SetActive(false);
                this.canEnterPillar = true;
            }
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