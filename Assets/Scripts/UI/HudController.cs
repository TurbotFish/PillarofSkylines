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

    public class HudController : MonoBehaviour, IUiMenu
    {
        
        [Header("Help"), SerializeField]
        TMPro.TextMeshProUGUI helpMessage;
        [SerializeField] float helpFadeTime = 0.1f;

        GameObject helpPanel;
        CanvasGroup helpRenderer;


        [Header("Important"), SerializeField]
        TMPro.TextMeshProUGUI importantTitle;
        [SerializeField]
        TMPro.TextMeshProUGUI importantDescription;
        [SerializeField] float importantFadeTime = 0.5f;
        
        GameObject importantPanel;
        CanvasGroup importantRenderer;


        [Header("Announce"), SerializeField]
        TMPro.TextMeshProUGUI announcement;
        [SerializeField]
        TMPro.TextMeshProUGUI announcementDescription;
        [SerializeField] float announceFadeTime = 0.5f;

        GameObject announcePanel;
        CanvasGroup announceRenderer;
        float announceTime;
        bool announcePanelActive;


        public CanvasGroup myRenderer;
        public bool IsActive { get; private set; }
        
        //###########################################################

        #region monobehaviour methods
            
        void Start()
        {
            //myRenderer = GetComponent<CanvasGroup>();

            helpPanel = helpMessage.transform.parent.gameObject;
            helpPanel.SetActive(false);
            helpRenderer = helpPanel.GetComponent<CanvasGroup>();

            importantPanel = importantTitle.transform.parent.gameObject;
            importantPanel.SetActive(false);
            importantRenderer = importantPanel.GetComponent<CanvasGroup>();

            announcePanel = announcement.transform.parent.gameObject;
            announcePanel.SetActive(false);
            announceRenderer = announcePanel.GetComponent<CanvasGroup>();

            Utilities.EventManager.OnShowHudMessageEvent += OnShowHudMessageEventHandler;
        }
        
        void Update()
        {
            if (!IsActive)
                return;

            if (announcePanelActive && announceTime > 0)
            {
                announceTime -= Time.unscaledDeltaTime;
                if (announceTime <= 0) {
                    announcePanelActive = false;
                    Display(announceRenderer, false, announceFadeTime);
                }
            }

            if (Input.GetButtonDown("MenuButton"))
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(MenuType.PauseMenu));
                return;
            }
            else if (Input.GetButtonDown("Back"))
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(MenuType.HelpMenu));
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
        
        void Display(CanvasGroup canvas, bool active, float fadeTime) {
            StartCoroutine(_Display(canvas, active, fadeTime));
        }

        IEnumerator _Display(CanvasGroup canvas, bool active, float fadeTime)
        {
            if (active)
                canvas.gameObject.SetActive(true);

            for (float elapsed = 0; elapsed < fadeTime; elapsed += Time.unscaledDeltaTime)
            {
                float t = elapsed / fadeTime;
                if (!active) t = 1 - t;
                canvas.alpha = t;
                yield return null;
            }

            if (!active)
                canvas.gameObject.SetActive(false);
            else
                canvas.alpha = 1;
        }

        //###########################################################

        void IUiMenu.Initialize(IGameController gameController)
        {
            helpMessage.text = "";
        }

        void IUiMenu.Activate(Utilities.EventManager.OnShowMenuEventArgs args)
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;
            myRenderer.alpha = 1;
            StopAllCoroutines();
            gameObject.SetActive(true);          
        }

        void IUiMenu.Deactivate()
        {
            if (!IsActive)
            {
                return;
            }

            IsActive = false;

            if (myRenderer)
            {
                Display(myRenderer, false, 0.5f);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        //###########################################################

        void OnShowHudMessageEventHandler(object sender, Utilities.EventManager.OnShowHudMessageEventArgs args)
        {
            //Debug.LogFormat("showHudMessageEvent: show={0}, message={1}", args.Show.ToString(), args.Message);

            if (!IsActive)
                return;

            switch (args.MessageType)
            {
                default:
                case eMessageType.Help:
                    Display(helpRenderer, args.Show, helpFadeTime);
                    if (args.Show)
                        helpMessage.text = args.Message;
                    break;

                case eMessageType.Important:
                    Display(importantRenderer, args.Show, importantFadeTime);
                    if (args.Show)
                    {
                        importantTitle.text = args.Message;
                        importantDescription.text = args.Description;
                    }
                    break;

                case eMessageType.Announcement:
                    announcePanelActive = args.Show;
                    Display(announceRenderer, args.Show, announceFadeTime);
                    if (args.Show)
                    {
                        announcement.text = args.Message;
                        announcementDescription.text = args.Description;
                        announceTime = args.Time;
                    }
                    else
                        announceTime = 0;
                    break;
            }
        }

        //###########################################################
    }
} //end of namespace