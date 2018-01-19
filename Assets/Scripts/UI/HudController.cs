using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game.UI
{
    public class HudController : MonoBehaviour, IUiState
    {
        [SerializeField]
        TMPro.TextMeshProUGUI messageView;
        GameObject messagePanel;

        //Player.PlayerModel playerModel;

        public bool IsActive { get; private set; }


        //###########################################################

        #region monobehaviour methods

        // Use this for initialization
        void Start()
        {
            messagePanel = this.messageView.transform.parent.gameObject;
            messagePanel.SetActive(false);
            Utilities.EventManager.OnShowHudMessageEvent += OnShowHudMessageEventHandler;
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
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(eUiState.AbilityMenu));
                return;
            }
            else if (Input.GetButtonDown("Back"))
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(eUiState.HelpMenu));
                return;
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        void IUiState.Initialize(Player.PlayerModel playerModel)
        {
            //this.playerModel = playerModel;

            this.messageView.text = "";
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
            this.IsActive = false;
            this.gameObject.SetActive(false);
        }

        //###########################################################

        void OnShowHudMessageEventHandler(object sender, Utilities.EventManager.OnShowHudMessageEventArgs args)
        {
            Debug.LogFormat("showHudMessageEvent: show={0}, message={1}", args.Show.ToString(), args.Message);

            if (args.Show)
            {
                messagePanel.SetActive(true);
                this.messageView.text = args.Message;
            }
            else
            {
                this.messageView.text = "";
                messagePanel.SetActive(false);
            }
        }

        //###########################################################
    }
} //end of namespace