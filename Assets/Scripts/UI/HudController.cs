using UnityEngine;
using Game.GameControl;
using System.Collections;

namespace Game.UI
{

    public enum eMessageType
    {
        Help = 0,
        Important,
        Announcement
    }

    public class HudController : MonoBehaviour, IUiState
    {
        [SerializeField]
        TMPro.TextMeshProUGUI helpMessage;
        GameObject helpPanel;
        
        [SerializeField]
        TMPro.TextMeshProUGUI importantTitle;
        [SerializeField]
        TMPro.TextMeshProUGUI importantDescription;
        GameObject importantPanel;
        
        [Header("Announce"), SerializeField]
        TMPro.TextMeshProUGUI announcement;
        [SerializeField]
        TMPro.TextMeshProUGUI announcementDescription;
        [SerializeField] float fadeTime = 0.5f;

        GameObject announcePanel;
        CanvasGroup announceRenderer;
        float announceTime;

        public bool IsActive { get; private set; }


        //###########################################################

        #region monobehaviour methods
            
        void Start()
        {
            helpPanel = helpMessage.transform.parent.gameObject;
            helpPanel.SetActive(false);

            importantPanel = importantTitle.transform.parent.gameObject;
            importantPanel.SetActive(false);

            announcePanel = announcement.transform.parent.gameObject;
            announcePanel.SetActive(false);
            announceRenderer = announcePanel.GetComponent<CanvasGroup>();

            Utilities.EventManager.OnShowHudMessageEvent += OnShowHudMessageEventHandler;
        }
        
        void Update()
        {
            if (!IsActive)
                return;

            if (announcePanel.activeSelf && announceTime > 0)
            {
                announceTime -= Time.unscaledDeltaTime;
                if (announceTime <= 0)
                    Display(announceRenderer, false);
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
            else if (Input.GetButtonDown("Interact") && importantPanel.activeSelf)
            {
                Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(false, "", eMessageType.Important));
                return;
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        bool flag;

        void Display(CanvasGroup canvas, bool active) {
            StartCoroutine(_Display(canvas, active));
        }

        IEnumerator _Display(CanvasGroup canvas, bool active)
        {
            if (flag) yield break;

            flag = true;

            if (active)
                canvas.gameObject.SetActive(true);

            for (float elapsed = 0; elapsed < fadeTime; elapsed += Time.unscaledDeltaTime)
            {
                float t = elapsed / fadeTime;
                if (!active) t = 1 - t;
                canvas.alpha = t;
                yield return null;
            }

            flag = false;

            if (!active)
                canvas.gameObject.SetActive(false);
        }

        //###########################################################

        void IUiState.Initialize(IGameControllerBase gameController)
        {
            helpMessage.text = "";
        }

        void IUiState.Activate(Utilities.EventManager.OnShowMenuEventArgs args)
        {
            if (IsActive)
                return;
            IsActive = true;
            gameObject.SetActive(true);          
        }

        void IUiState.Deactivate()
        {
            IsActive = false;
            gameObject.SetActive(false);
        }

        //###########################################################

        void OnShowHudMessageEventHandler(object sender, Utilities.EventManager.OnShowHudMessageEventArgs args)
        {
            //Debug.LogFormat("showHudMessageEvent: show={0}, message={1}", args.Show.ToString(), args.Message);

            switch (args.MessageType)
            {
                default:
                case eMessageType.Help:
                    helpPanel.SetActive(args.Show);
                    if (args.Show)
                        helpMessage.text = args.Message;
                    else
                        helpMessage.text = "";
                    break;

                case eMessageType.Important:
                    importantPanel.SetActive(args.Show);
                    if (args.Show)
                    {
                        importantTitle.text = args.Message;
                        importantDescription.text = args.Description;
                    }
                    else
                    {
                        importantTitle.text = "";
                        importantDescription.text = "";
                    }
                    break;

                case eMessageType.Announcement:
                    Display(announceRenderer, args.Show);
                    if (args.Show)
                    {
                        announcement.text = args.Message;
                        announcementDescription.text = args.Description;
                        announceTime = args.Time;
                    }
                    else
                    {
                        announcement.text = "";
                        announceTime = 0;
                    }
                    break;
            }
        }

        //###########################################################
    }
} //end of namespace