using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.UI
{
    public class HudController : MonoBehaviour, IUiState
    {
        [SerializeField]
        TMPro.TextMeshProUGUI messageView;

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

            this.messageView.text = "";
        }

        void IUiState.Deactivate()
        {
            this.IsActive = false;
            this.gameObject.SetActive(false);
        }

        //###########################################################

        public void ShowMessage(string message)
        {
            this.messageView.text = message;
        }

        public void HideMessage()
        {
            this.messageView.text = "";
        }
    }
} //end of namespace