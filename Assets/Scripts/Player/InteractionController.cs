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

        private GameController GameController;

        private bool IsGamePaused = true;

        private List<IInteractable> NearbyInteractableObjects = new List<IInteractable>();
        private IInteractable CurrentInteractableObject;

        private List<IInteractable> InteractablesEnteredDuringPause = new List<IInteractable>();

        //########################################################################

        // -- INITIALIZATION

        public void Initialize(GameController gameController)
        {
            this.GameController = gameController;

            NearbyInteractableObjects.Clear();
            CurrentInteractableObject = null;

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

            NearbyInteractableObjects.RemoveAll(item => item == null);
            SetCurrentInteractableObject();

            if (Input.GetButtonDown("Interact"))
            {
                if (CurrentInteractableObject != null)
                {
                    CurrentInteractableObject.OnInteraction();
                }
                else if (GameController.PlayerModel.CheckAbilityActive(AbilityType.Echo))
                {
                    currentEcho = GameController.EchoManager.CreateEcho(true);

                    currentEcho.transform.SetParent(GameController.PlayerController.CharController.MyTransform);
                }
            }
            else if (Input.GetButtonDown("Drift") && !Input.GetButtonUp("Drift"))
            {
                GameController.EchoManager.Drift();
            }
            else if (!Input.GetButton("Interact"))
            {
                if (CurrentInteractableObject == null && currentEcho != null)
                {
                    GameController.EchoManager.PlaceEcho(currentEcho.MyTransform);
                    currentEcho.MyTransform.SetParent(null);
                    currentEcho = null;
                }
            }
        }

        /// <summary>
        /// On Trigger Enter.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            var interactable_object = other.GetComponent<IInteractable>();

            if (interactable_object != null)
            {
                if (!IsGamePaused)
                {
                    if (!NearbyInteractableObjects.Contains(interactable_object))
                    {
                        NearbyInteractableObjects.Add(interactable_object);
                        interactable_object.OnPlayerEnter();
                    }
                }
                else
                {
                    if (!InteractablesEnteredDuringPause.Contains(interactable_object))
                    {
                        InteractablesEnteredDuringPause.Add(interactable_object);
                    }
                }
            }
        }

        /// <summary>
        /// On Trigger Exit.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            var interactableObject = other.GetComponent<IInteractable>();

            if (interactableObject != null)
            {
                if (NearbyInteractableObjects.Contains(interactableObject))
                {
                    NearbyInteractableObjects.Remove(interactableObject);
                    interactableObject.OnPlayerExit();
                }

                if (InteractablesEnteredDuringPause.Contains(interactableObject))
                {
                    InteractablesEnteredDuringPause.Remove(interactableObject);
                }
            }
        }

        private void SetCurrentInteractableObject()
        {
            IInteractable nearestInteractableObject = null;
            float smallestDistance = float.MaxValue;
            Vector3 playerPosition = GameController.PlayerController.CharController.MyTransform.position;

            foreach (var interactableObject in NearbyInteractableObjects)
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

            if (nearestInteractableObject != CurrentInteractableObject)
            {
                if (CurrentInteractableObject != null)
                {
                    CurrentInteractableObject.OnHoverEnd();
                }

                CurrentInteractableObject = nearestInteractableObject;

                if (CurrentInteractableObject != null)
                {
                    CurrentInteractableObject.OnHoverBegin();
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

            if (!IsGamePaused)
            {
                foreach(var interactable_object in InteractablesEnteredDuringPause)
                {
                    if (!NearbyInteractableObjects.Contains(interactable_object))
                    {
                        NearbyInteractableObjects.Add(interactable_object);
                        interactable_object.OnPlayerEnter();
                    }
                }
                InteractablesEnteredDuringPause.Clear();
            }
        }

        /// <summary>
        /// Called before a level switch.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PreSceneChangedEventHandler(object sender, Utilities.EventManager.PreSceneChangeEventArgs args)
        {
            foreach(var interactable_object in NearbyInteractableObjects)
            {
                interactable_object.OnPlayerExit();
            }
            NearbyInteractableObjects.Clear();

            GameController.UiController.Hud.HideHelpMessage();
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