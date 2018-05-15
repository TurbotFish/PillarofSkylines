﻿using Game.GameControl;
using Game.Model;
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

        PlayerModel playerModel;

        //
        World.ePillarId pillarId;
        bool canEnterPillar = false;
        private IGameController GameController;

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
            if (!IsActive)
            {
                return;
            }

            if (Input.GetButtonDown("MenuButton"))
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(eUiState.HUD));
            }
            else if (canEnterPillar && Input.GetButtonDown("Interact"))
            {
                if (playerModel.UnlockPillar(pillarId))
                {
                    GameController.SwitchToPillar(pillarId);
                }             
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        void IUiState.Initialize(IGameController gameController)
        {
            GameController = gameController;
            playerModel = GameController.PlayerModel;
        }

        void IUiState.Activate(Utilities.EventManager.OnShowMenuEventArgs args)
        {
            if (IsActive)
            {
                return;
            }
            else if (!(args is Utilities.EventManager.OnShowPillarEntranceMenuEventArgs))
            {
                throw new System.Exception("ERROR!");
            }

            IsActive = true;
            gameObject.SetActive(true);


            pillarId = (args as Utilities.EventManager.OnShowPillarEntranceMenuEventArgs).PillarId;

            int cost = playerModel.GetPillarEntryPrice(pillarId);
            costPanelView.Initialize(cost);

            if (playerModel.PillarKeysCount < cost)
            {
                warningMessage.SetActive(true);
            }
            else
            {
                warningMessage.SetActive(false);
                canEnterPillar = true;
            }
        }

        void IUiState.Deactivate()
        {
            bool wasActive = IsActive;

            IsActive = false;
            gameObject.SetActive(false);
        }

        //###########################################################
    }
} //end of namespace