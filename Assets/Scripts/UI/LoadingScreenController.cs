using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class LoadingScreenController : MonoBehaviour, IUiState
    {
        Player.PlayerModel playerModel;

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
}