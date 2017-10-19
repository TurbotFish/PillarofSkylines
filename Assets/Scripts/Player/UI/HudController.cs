using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game.Player.UI
{
    public class HudController : MonoBehaviour, IUiState
    {
        [SerializeField]
        TMPro.TextMeshProUGUI messageView;

        public bool IsActive { get; private set; }

        List<HudMessageRequest> hudMessageRequestList = new List<HudMessageRequest>();
        HudMessageRequest currentHudMessageRequest = null;

        //###########################################################

        #region monobehaviour methods

        // Use this for initialization
        void Start()
        {
            this.messageView.gameObject.SetActive(false);
            Utilities.EventManager.OnShowHudMessageEvent += OnShowHudMessageEventHandler;
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

        void OnShowHudMessageEventHandler(object sender, Utilities.EventManager.OnShowHudMessageEventArgs args)
        {
            if (args.Show)
            {
                if (this.currentHudMessageRequest == null)
                {
                    this.currentHudMessageRequest = new HudMessageRequest(sender, args);

                    this.messageView.gameObject.SetActive(true);
                    this.messageView.text = args.Message;
                }
                else
                {
                    this.hudMessageRequestList.Add(new HudMessageRequest(sender, args));
                }
            }
            else
            {
                var requests = from item in this.hudMessageRequestList where item.sender == sender select item;

                foreach (var request in requests)
                {
                    this.hudMessageRequestList.Remove(request);
                }

                if (this.currentHudMessageRequest.sender == sender)
                {
                    if (this.hudMessageRequestList.Count > 0)
                    {
                        this.currentHudMessageRequest = this.hudMessageRequestList[0];
                        this.hudMessageRequestList.RemoveAt(0);

                        this.messageView.text = this.currentHudMessageRequest.args.Message;
                    }
                    else
                    {
                        this.currentHudMessageRequest = null;

                        this.messageView.text = "";
                        this.messageView.gameObject.SetActive(false);
                    }
                }
            }
        }

        //###########################################################

        class HudMessageRequest
        {
            public object sender;
            public Utilities.EventManager.OnShowHudMessageEventArgs args;

            public HudMessageRequest(object sender, Utilities.EventManager.OnShowHudMessageEventArgs args)
            {
                this.sender = sender;
                this.args = args;
            }
        }
    }
} //end of namespace