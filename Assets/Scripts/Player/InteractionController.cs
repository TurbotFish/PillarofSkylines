using Game.GameControl;
using Game.LevelElements;
using Game.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// This class handles picking up of objects in the world.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class InteractionController : MonoBehaviour
    {
        //########################################################################

        // -- ATTRIBUTES

        [SerializeField] private GameObject playerNeedle;
        public EchoSystem.Echo currentEcho;

        private GameController gameController;

        private bool isActive = false;

        private List<IInteractable> nearbyInteractableObjects = new List<IInteractable>();
        private IInteractable currentInteractableObject;

        /// <summary>
        /// Helps making sure the element turning off the UI is the same as the one turning it on.
        /// </summary>
        private string lastTag;

        //########################################################################

        // -- INITIALIZATION

        public void Initialize(GameController gameController)
        {
            this.gameController = gameController;

            nearbyInteractableObjects.Clear();
            currentInteractableObject = null;

            Utilities.EventManager.OnMenuSwitchedEvent += OnMenuSwitchedEventHandler;
            Utilities.EventManager.SceneChangedEvent += OnSceneChangedEventHandler;
            Utilities.EventManager.PreSceneChangeEvent += PreSceneChangedEventHandler;
        }

        //########################################################################

        // -- INQUIRIES

        //########################################################################

        // -- OPERATIONS

        private void Update()
        {
            if (!isActive)
            {
                return;
            }

            nearbyInteractableObjects.RemoveAll(item => item == null);
            SetCurrentInteractableObject();

            if (Input.GetButtonDown("Interact"))
            {
                if (currentInteractableObject != null)
                {
                    currentInteractableObject.OnInteraction();
                }
                else if (gameController.PlayerModel.CheckAbilityActive(AbilityType.Echo))
                {
					currentEcho = gameController.EchoManager.CreateEcho(true);

                    currentEcho.transform.SetParent(gameController.PlayerController.CharController.MyTransform);
                }
            }
            else if (Input.GetButtonDown("Drift") && !Input.GetButtonUp("Drift"))
            {
                gameController.EchoManager.Drift();
            }

            if (Input.GetButtonUp("Interact"))
            {
                if (currentInteractableObject == null && currentEcho != null)
                {

                    currentEcho.MyTransform.SetParent(null);
					gameController.EchoManager.PlaceEcho ();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var interactableObject = other.GetComponent<IInteractable>();

            if (interactableObject != null)
            {
                if (!nearbyInteractableObjects.Contains(interactableObject))
                {
                    nearbyInteractableObjects.Add(interactableObject);
                    interactableObject.OnPlayerEnter();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var interactableObject = other.GetComponent<IInteractable>();

            if (interactableObject != null)
            {
                if (nearbyInteractableObjects.Contains(interactableObject))
                {
                    nearbyInteractableObjects.Remove(interactableObject);
                    interactableObject.OnPlayerExit();
                }
            }
        }

        private void SetCurrentInteractableObject()
        {
            IInteractable nearestInteractableObject = null;
            float smallestDistance = float.MaxValue;
            Vector3 playerPosition = gameController.PlayerController.CharController.MyTransform.position;

            foreach (var interactableObject in nearbyInteractableObjects)
            {
                if (!interactableObject.IsInteractable())
                {
                    continue;
                }

                float newDistance = Vector3.Distance(playerPosition, interactableObject.Transform.position);
                if (newDistance < smallestDistance)
                {
                    nearestInteractableObject = interactableObject;
                    smallestDistance = newDistance;
                }
            }

            if (nearestInteractableObject != currentInteractableObject)
            {
                if (currentInteractableObject != null)
                {
                    currentInteractableObject.OnHoverEnd();
                }

                currentInteractableObject = nearestInteractableObject;

                if (currentInteractableObject != null)
                {
                    currentInteractableObject.OnHoverBegin();
                }
            }
        }

        private void ShowUiMessage(string message, string tag)
        {
            lastTag = tag;
            Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(true, message));
        }

        private void ShowAnnounceMessage(string message, float time)
        {
            Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(true, message, UI.eMessageType.Announcement, "", time));
        }

        private void ShowImportantMessage(string message, string description)
        {
            Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(true, message, UI.eMessageType.Important, description));
        }

        private void HideUiMessage()
        {
            Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(false));
        }

        private void HideUiMessage(string tag)
        {
            if (tag == lastTag)
                Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(false));
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnMenuSwitchedEventHandler(object sender, Utilities.EventManager.OnMenuSwitchedEventArgs args)
        {
            if (!isActive && args.NewUiState == UI.MenuType.HUD)
            {
                isActive = true;
                //Debug.Log("InteractionController activated!");
            }
            else if (isActive && args.PreviousUiState == UI.MenuType.HUD)
            {
                isActive = false;
                //Debug.Log("InteractionController deactivated!");
            }
        }

        /// <summary>
        /// Called before a level switch.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PreSceneChangedEventHandler(object sender, Utilities.EventManager.PreSceneChangeEventArgs args)
        {
            nearbyInteractableObjects.Clear();

            HideUiMessage();
        }

        /// <summary>
        /// "Reset everything!"  "Everything?"  "EVERYTHING!"
        /// </summary>
        private void OnSceneChangedEventHandler(object sender, Utilities.EventManager.SceneChangedEventArgs args)
        {        
        }
    }
}
//end of namespace