using Game.LevelElements;
using UnityEngine;

namespace Game.UI
{
    public class TutoBox : MonoBehaviour, IInteractable
    {
        //########################################################################

        public eMessageType messageType;

        public string message = "EXAMPLE TUTORIAL MESSAGE";
        [TextArea, ConditionalHide("messageType", 1)]
        public string description = "Example Description.";
        [ConditionalHide("messageType", 2)]
        public float time = 2;

        //########################################################################

        #region inquiries

        public Transform Transform { get { return transform; } }

        public bool IsInteractable()
        {
            return false;
        }

        #endregion inquiries

        //########################################################################

        #region operations

        public void OnPlayerEnter()
        {
            if (messageType == eMessageType.Important)
            {
                var messageEventArgs = new Utilities.EventManager.OnShowHudMessageEventArgs(true, message, messageType, description);
                Utilities.EventManager.SendShowHudMessageEvent(this, messageEventArgs);

                Destroy(gameObject);
            }
            else if (messageType == eMessageType.Announcement)
            {
                var messageEventArgs = new Utilities.EventManager.OnShowHudMessageEventArgs(true, message, messageType, "");
                Utilities.EventManager.SendShowHudMessageEvent(this, messageEventArgs);

                Destroy(gameObject);
            }
            else
            {
                var messageEventArgs = new Utilities.EventManager.OnShowHudMessageEventArgs(true, message);
                Utilities.EventManager.SendShowHudMessageEvent(this, messageEventArgs);
            }
        }

        public void OnPlayerExit()
        {
            Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(false));
        }

        public void OnHoverBegin()
        {
        }

        public void OnHoverEnd()
        {
        }

        public void OnInteraction()
        {
        }

        #endregion operations

        //########################################################################
    }
} // end of namespace