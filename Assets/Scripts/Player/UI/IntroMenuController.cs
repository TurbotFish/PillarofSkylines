using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.UI
{
    public class IntroMenuController : MonoBehaviour, IUiState
    {
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

        void IUiState.Activate()
        {
            if (this.IsActive)
            {
                return;
            }

            this.IsActive = true;
            this.gameObject.SetActive(true);

            Utilities.EventManager.SendOnMenuOpenedEvent(this, new Utilities.EventManager.OnMenuOpenedEventArgs(eUiState.Intro));
            Time.timeScale = 0.1f;
        }

        void IUiState.Deactivate()
        {
            bool wasActive = this.IsActive;

            this.IsActive = false;
            this.gameObject.SetActive(false);

            if (wasActive)
            {
                Utilities.EventManager.SendOnMenuClosedEvent(this, new Utilities.EventManager.OnMenuClosedEventArgs(eUiState.Intro));
                Time.timeScale = 1;
            }
        }

        //###########################################################
    }
} //end of namespace