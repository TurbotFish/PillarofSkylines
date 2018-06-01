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

        // -- CONSTANTS

        [SerializeField] private GameObject playerNeedle;

        //########################################################################

        // -- ATTRIBUTES

        public EchoSystem.Echo currentEcho;

        private GameController gameController;

        private bool IsGamePaused = true;

        private List<IInteractable> nearbyInteractableObjects = new List<IInteractable>();
        private IInteractable currentInteractableObject;

        //########################################################################

        // -- INITIALIZATION

        public void Initialize(GameController gameController)
        {
            this.gameController = gameController;

            nearbyInteractableObjects.Clear();
            currentInteractableObject = null;

            Utilities.EventManager.GamePausedEvent += OnGamePausedEvent;
            Utilities.EventManager.SceneChangedEvent += OnSceneChangedEventHandler;
            Utilities.EventManager.PreSceneChangeEvent += PreSceneChangedEventHandler;
        }

        public void OnDestroy()
        {
            Utilities.EventManager.GamePausedEvent -= OnGamePausedEvent;
            Utilities.EventManager.SceneChangedEvent -= OnSceneChangedEventHandler;
            Utilities.EventManager.PreSceneChangeEvent -= PreSceneChangedEventHandler;
        }

        //########################################################################

        // -- OPERATIONS

        private void Update()
        {
            if (IsGamePaused)
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
            else if (!Input.GetButton("Interact"))
            {
                if (currentInteractableObject == null && currentEcho != null)
                {
                    currentEcho.MyTransform.SetParent(null);
                    currentEcho = null;

                    gameController.EchoManager.PlaceEcho();
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

        /// <summary>
        /// Handles the GamePaused event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnGamePausedEvent(object sender, Utilities.EventManager.GamePausedEventArgs args)
        {
            IsGamePaused = args.PauseActive;
        }

        /// <summary>
        /// Called before a level switch.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PreSceneChangedEventHandler(object sender, Utilities.EventManager.PreSceneChangeEventArgs args)
        {
            nearbyInteractableObjects.Clear();

            gameController.UiController.Hud.HideHelpMessage();
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