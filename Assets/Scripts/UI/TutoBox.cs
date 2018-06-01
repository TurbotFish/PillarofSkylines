using Game.GameControl;
using Game.LevelElements;
using Game.World;
using UnityEngine;

namespace Game.UI
{
    public class TutoBox : MonoBehaviour, IInteractable, IWorldObject
    {
        //########################################################################

        public eMessageType messageType;

        public string message = "EXAMPLE TUTORIAL MESSAGE";
        [TextArea, ConditionalHide("messageType", 1)]
        public string description = "Example Description.";
        [ConditionalHide("messageType", 2)]
        public float time = 2;

        //########################################################################

        // -- ATTRIBUTES

        private GameController GameController;

        //########################################################################

        // -- INITIALIZATION

        public void Initialize(GameController gameController)
        {
            GameController = gameController;
        }

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
            if (messageType == eMessageType.Help)
            {
                GameController.UiController.Hud.ShowHelpMessage(message, "TutoBox");
            }
            else
            {
                GameController.UiController.Hud.ShowAnnouncmentMessage(message, description);

                //Destroy(this.gameObject);
            }
        }

        public void OnPlayerExit()
        {
            GameController.UiController.Hud.HideHelpMessage("TutoBox");
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