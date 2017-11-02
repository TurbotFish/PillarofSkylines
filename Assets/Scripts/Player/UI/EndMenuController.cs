using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.UI
{
    public class EndMenuController : MonoBehaviour, IUiState
    {
        PlayerModel playerModel;

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
                if (Application.isEditor)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                }
                else
                {
                    Application.Quit();
                }
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        void IUiState.Initialize(PlayerModel playerModel)
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