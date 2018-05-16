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
        [SerializeField] GameObject playerNeedle;
        public EchoSystem.Echo currentEcho;

        //
        IGameController gameController;

        //
        bool isPickupInRange = false;
        Pickup currentPickup;
        PillarEntranceInfo pillarEntranceInfo = new PillarEntranceInfo();
        bool pillarExitInRange = false;
        bool needleInRange = false;
        Collider needleSlotCollider, needleSlotForDrift;
        bool needleSlotInRange = false;
        bool eyeInRange = false;
        Transform airParticle, airOrigin;

        //
        bool isActive = false;
        bool isInteractButtonDown = false;
        bool isDriftButtonDown = false;

        private List<IInteractable> nearbyInteractableObjects = new List<IInteractable>();
        private IInteractable currentInteractableObject;

        //########################################################################

        #region initialization

        public void Initialize(IGameController gameController)
        {
            this.gameController = gameController;

            nearbyInteractableObjects.Clear();
            currentInteractableObject = null;

            Utilities.EventManager.OnMenuSwitchedEvent += OnMenuSwitchedEventHandler;
            Utilities.EventManager.SceneChangedEvent += OnSceneChangedEventHandler;
            Utilities.EventManager.PreSceneChangeEvent += PreSceneChangedEventHandler;
        }

        #endregion initialization

        //########################################################################

        #region operations

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
                else if(gameController.PlayerModel.CheckAbilityActive(eAbilityType.Echo))
                {
                    currentEcho = gameController.EchoManager.CreateEcho(true);
                    currentEcho.transform.SetParent(gameController.PlayerController.CharController.MyTransform);
                }
            }
            else if (Input.GetButtonDown("Drift"))
            {
                gameController.EchoManager.Drift();
            }

            if (Input.GetButtonUp("Interact"))
            {
                if (currentInteractableObject == null && currentEcho != null) {
                    currentEcho.MyTransform.SetParent(null);
                }
            }

            // stop air particle if grounded
            if (airParticle && (gameController.PlayerController.CharController.CurrentState & (CharacterController.ePlayerState.move | CharacterController.ePlayerState.slide | CharacterController.ePlayerState.stand)) != 0)
            {
                Debug.Log("this should not be here!");
                airParticle.parent = airOrigin;
                airParticle.transform.localPosition = Vector3.zero;
                airParticle = null;
            }

            return;
            //éééééééééééé

            if (Input.GetButtonDown("Interact") && !isInteractButtonDown)
            {
                isInteractButtonDown = true;

                ////favour
                //if (isPickupInRange)
                //{
                //    if (!currentPickup.IsPickedUp)
                //    {
                //        currentPickup.OnInteraction();
                //    }

                //    //clean up
                //    isPickupInRange = false;
                //    currentPickup = null;
                //    HideUiMessage("Favour");
                //}
                ////pillar entrance
                //else if (pillarEntranceInfo.IsPillarEntranceInRange)
                //{
                //    if (gameController.PlayerModel.CheckIsPillarUnlocked(pillarEntranceInfo.CurrentPillarEntrance.PillarId))
                //    {
                //        Utilities.EventManager.SendEnterPillarEvent(this, new Utilities.EventManager.EnterPillarEventArgs(pillarEntranceInfo.CurrentPillarEntrance.PillarId));
                //    }
                //    else
                //    {
                //        Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowPillarEntranceMenuEventArgs(pillarEntranceInfo.CurrentPillarEntrance.PillarId));
                //    }
                //}
                ////pillar exit
                //else if (pillarExitInRange)
                //{
                //    Utilities.EventManager.SendLeavePillarEvent(this, new Utilities.EventManager.LeavePillarEventArgs(false));
                //}
                ////needle
                //else if (needleInRange || needleSlotInRange)
                //{

                //    foreach (Transform child in needleSlotCollider.transform)
                //        child.gameObject.SetActive(gameController.PlayerModel.hasNeedle);

                //    gameController.PlayerModel.hasNeedle ^= true;

                //    playerNeedle.SetActive(gameController.PlayerModel.hasNeedle);

                //    if (gameController.PlayerModel.hasNeedle)
                //        needleSlotForDrift = needleSlotCollider;

                //    Utilities.EventManager.SendEclipseEvent(this, new Utilities.EventManager.EclipseEventArgs(gameController.PlayerModel.hasNeedle));

                //    ShowUiMessage(gameController.PlayerModel.hasNeedle ? "[X]: Plant Needle" : "[X]: Take Needle", needleInRange ? "Needle" : "NeedleSlot");
                //}
                ////eye
                //else if (eyeInRange)
                //{
                //    eyeInRange = false;
                //    gameController.PlayerModel.hasNeedle = false;
                //    Utilities.EventManager.SendLeavePillarEvent(this, new Utilities.EventManager.LeavePillarEventArgs(true));

                //    HideUiMessage("Eye");
                //}

            }
            else if (!Input.GetButton("Interact") && isInteractButtonDown)
            {
                isInteractButtonDown = false;
            }

            // drift input handling
            float driftInput = Input.GetAxis("Drift") + (Input.GetButtonUp("Drift") ? 1 : 0);
            if (driftInput > 0.5f && !isDriftButtonDown)
            {
                isDriftButtonDown = true;
            }
            else if (driftInput < 0.4f && isDriftButtonDown)
            {
                isDriftButtonDown = false;

                // stop eclipse
                if (gameController.PlayerModel.hasNeedle)
                {

                    print("Needle Slot for Drift: " + needleSlotForDrift + " Collider: " + needleSlotCollider);

                    gameController.PlayerModel.hasNeedle = false;

                    if (needleSlotForDrift)
                    {
                        foreach (Transform child in needleSlotForDrift.transform)
                            child.gameObject.SetActive(true);

                        var eventArgs = new Utilities.EventManager.TeleportPlayerEventArgs(needleSlotForDrift.transform.position, false);
                        Utilities.EventManager.SendTeleportPlayerEvent(this, eventArgs);
                    }
                    needleSlotCollider = needleSlotForDrift;

                    Utilities.EventManager.SendEclipseEvent(this, new Utilities.EventManager.EclipseEventArgs(false));

                    // stop air particles
                }
                else if (airParticle)
                {
                    airParticle.parent = airOrigin;
                    airParticle.transform.localPosition = Vector3.zero;
                    airParticle = null;
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

            return;
            //éééééééééééé

            if (other.gameObject.layer == LayerMask.NameToLayer("PickUps"))
            {
                switch (other.tag)
                {
                    ////favour
                    //case "Favour":
                    //    if (!isPickupInRange)
                    //    {
                    //        currentPickup = other.GetComponent<Pickup>();

                    //        if (!currentPickup.IsPickedUp)
                    //        {
                    //            isPickupInRange = true;

                    //            ShowUiMessage("[X]: Accept " + currentPickup.PickupName, other.tag);
                    //        }
                    //        else
                    //        {
                    //            currentPickup = null;
                    //        }
                    //    }
                    //    break;
                    ////pillar entrance
                    //case "Pillar":
                    //    var pillarEntrance = other.GetComponent<PillarEntrance>();
                    //    if (pillarEntrance != null)
                    //    {
                    //        if (!gameController.PlayerModel.CheckIsPillarDestroyed(pillarEntrance.PillarId))
                    //        {
                    //            pillarEntranceInfo.IsPillarEntranceInRange = true;
                    //            pillarEntranceInfo.CurrentPillarEntrance = pillarEntrance;

                    //            ShowUiMessage("[X]: Enter Pillar", other.tag);
                    //        }
                    //        break;
                    //    }

                    //    var pillarExit = other.GetComponent<PillarExit>();
                    //    if (pillarExit != null)
                    //    {
                    //        pillarExitInRange = true;

                    //        ShowUiMessage("[X]: Exit Pillar", other.tag);
                    //        break;
                    //    }

                    //    break;
                    ////needle
                    //case "Needle":
                    //    needleInRange = true;
                    //    needleSlotCollider = other;

                    //    ShowUiMessage(gameController.PlayerModel.hasNeedle ? "[X]: Plant Needle" : "[X]: Take Needle", other.tag);
                    //    break;
                    ////needle slot
                    //case "NeedleSlot":
                    //    // si on est dans l'éclipse on peut poser peu importe lequel c'est
                    //    // sinon on ne peut retirer que s'il a l'aiguille
                    //    if (gameController.PlayerModel.hasNeedle || needleSlotCollider && needleSlotCollider == other)
                    //    {
                    //        needleSlotInRange = true;
                    //        needleSlotCollider = other;

                    //        ShowUiMessage(gameController.PlayerModel.hasNeedle ? "[X]: Plant Needle" : "[X]: Take Needle", other.tag);
                    //    }
                    //    break;
                    ////eye
                    //case "Eye":
                    //    if (gameController.PlayerModel.hasNeedle)
                    //    {
                    //        eyeInRange = true;
                    //        ShowUiMessage("[X]: Plant Needle", other.tag);
                    //    }
                    //    break;
                    ////echo
                    //case "Echo":
                    //    other.GetComponent<EchoSystem.Echo>().Break(true);
                    //    break;
                    ////echo breaker
                    //case "EchoBreaker":
                    //    gameController.EchoManager.BreakAll();
                    //    break;
                    //echo seed
                    //case "EchoSeed":
                    //    gameController.EchoManager.CreateEcho(false);
                    //    Destroy(other.gameObject);
                    //    break;
                    ////wind
                    //case "Wind":
                    //    Utilities.EventManager.SendWindTunnelEnteredEvent(this, new Utilities.EventManager.WindTunnelPartEnteredEventArgs(other.GetComponent<WindTunnelPart>()));
                    //    if (gameController.PlayerController.CharController.stateMachine.CurrentState != CharacterController.ePlayerState.windTunnel)
                    //    {
                    //        gameController.PlayerController.CharController.stateMachine.ChangeState(
                    //            new CharacterController.States.WindTunnelState(gameController.PlayerController.CharController, gameController.PlayerController.CharController.stateMachine)
                    //        );
                    //    }
                    //    break;
                    // air particle
                    case "AirParticle":
                        if (!airParticle)
                        {
                            airOrigin = other.transform;
                            airParticle = airOrigin.GetChild(0);
                            airParticle.parent = gameController.PlayerController.CharController.transform;
                            airParticle.localPosition = new Vector3(0, 1, 0);
                        }
                        break;
                    // air particle
                    case "AirReceptor":
                        if (airParticle)
                        {
                            other.GetComponent<LevelElements.AirReceptor>().Activate();
                            Destroy(airParticle.gameObject);
                            Destroy(airOrigin.gameObject);
                            airParticle = null;
                            airOrigin = null;
                        }
                        break;
                        //// Trigger Activator
                        //case "TriggerActivator":
                        //    other.GetComponent<TimedActivator>().manager.Activate();
                        //    break;
                        // Tutorial Message
                        //case "TutoBox":
                        //    UI.TutoBox tutoBox = other.GetComponent<UI.TutoBox>();
                        //    if (tutoBox.messageType == UI.eMessageType.Important)
                        //    {
                        //        ShowImportantMessage(tutoBox.message, tutoBox.description);
                        //        Destroy(other.gameObject);
                        //    }
                        //    else if (tutoBox.messageType == UI.eMessageType.Announcement)
                        //    {
                        //        ShowAnnounceMessage(tutoBox.message, tutoBox.time);
                        //        Destroy(other.gameObject);
                        //    }
                        //    else
                        //        ShowUiMessage(tutoBox.message, other.tag);
                        //    break;
                        //// CameraControlTrigger
                        //case "CameraControlTrigger":
                        //    gameController.CameraController.PoS_Camera.EnterTrigger(other.GetComponent<CameraControlTrigger>());
                        //    break;
                        //// Miscelanous Interactive Elements
                        //case "Interactible":
                        //    other.GetComponent<Interactible>().EnterTrigger(transform);
                        //    break;
                        //// Zones where the player can only walk
                        //case "NoRunZone":
                        //    gameController.PlayerController.CharController.isInsideNoRunZone = true;
                        //    break;
                        ////other
                        //default:
                        //    Debug.LogWarningFormat("InteractionController: unhandled tag: \"{0}\"", other.tag);
                        //    break;
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

            return;
            //éééééééééééé

            if (other.gameObject.layer == LayerMask.NameToLayer("PickUps"))
            {
                switch (other.tag)
                {
                    ////favour
                    //case "Favour":
                    //    isPickupInRange = false;
                    //    currentPickup = null;

                    //    HideUiMessage(other.tag);
                    //    break;
                    ////pillar entrance
                    //case "Pillar":
                    //    pillarEntranceInfo.IsPillarEntranceInRange = false;
                    //    pillarEntranceInfo.CurrentPillarEntrance = null;

                    //    pillarExitInRange = false;

                    //    HideUiMessage(other.tag);
                    //    break;
                    ////needle
                    //case "Needle":
                    //    needleInRange = false;

                    //    HideUiMessage(other.tag);
                    //    break;
                    ////needle slot
                    //case "NeedleSlot":
                    //    needleSlotInRange = false;

                    //    HideUiMessage(other.tag);
                    //    break;
                    ////eye
                    //case "Eye":
                    //    eyeInRange = false;

                    //    HideUiMessage(other.tag);
                    //    break;
                    ////echo
                    //case "Echo":
                    //    other.GetComponent<EchoSystem.Echo>().isActive = true;
                    //    break;
                    ////wind
                    //case "Wind":
                    //    Debug.Log("coucou");
                    //    Utilities.EventManager.SendWindTunnelExitedEvent(this, new Utilities.EventManager.WindTunnelPartExitedEventArgs(other.GetComponent<WindTunnelPart>()));
                    //    break;
                    //// HomeBeacon
                    //case "Beacon":
                    //    beacon = null;
                    //    HideUiMessage(other.tag);
                    //    break;
                    //// Trigger Activator
                    //case "TriggerActivator":
                    //    other.GetComponent<TimedActivator>().manager.StartTimer();
                    //    break;
                    // Tutorial Message
                    //case "TutoBox":
                    //    HideUiMessage(other.tag);
                    //    break;
                    //// Home
                    //case "Home":
                    //    echoManager.atHome = false;
                    //    break;
                    //// CameraControlTrigger
                    //case "CameraControlTrigger":
                    //    gameController.CameraController.PoS_Camera.ExitTrigger(other.GetComponent<CameraControlTrigger>());
                    //    break;
                    //// Miscelanous Interactive Elements
                    //case "Interactible":
                    //    other.GetComponent<Interactible>().ExitTrigger(transform);
                    //    break;
                    // Zones where the player can only walk
                    //case "NoRunZone":
                    //    gameController.PlayerController.CharController.isInsideNoRunZone = false;
                    //    break;
                    ////other
                    //default:
                    //    Debug.LogWarningFormat("InteractionController: unhandled tag: \"{0}\"", other.tag);
                    //    break;
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

        #endregion operations

        //########################################################################
        //########################################################################

        #region helper methods

        /// <summary>
        /// Helps making sure the element turning off the UI is the same as the one turning it on.
        /// </summary>
        string lastTag;

        void ShowUiMessage(string message, string tag)
        {
            lastTag = tag;
            Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(true, message));
        }

        void ShowAnnounceMessage(string message, float time)
        {
            Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(true, message, UI.eMessageType.Announcement, "", time));
        }

        void ShowImportantMessage(string message, string description)
        {
            Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(true, message, UI.eMessageType.Important, description));
        }

        void HideUiMessage()
        {
            Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(false));
        }

        void HideUiMessage(string tag)
        {
            if (tag == lastTag)
                Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(false));
        }

        #endregion helper methods

        //########################################################################
        //########################################################################

        #region event handlers

        /// <summary>
        /// 
        /// </summary>
        void OnMenuSwitchedEventHandler(object sender, Utilities.EventManager.OnMenuSwitchedEventArgs args)
        {
            if (!isActive && args.NewUiState == UI.eUiState.HUD)
            {
                isActive = true;
                //Debug.Log("InteractionController activated!");
            }
            else if (isActive && args.PreviousUiState == UI.eUiState.HUD)
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
        void OnSceneChangedEventHandler(object sender, Utilities.EventManager.SceneChangedEventArgs args)
        {
            //***
            this.isPickupInRange = false;
            this.currentPickup = null;

            this.needleInRange = false;
            this.needleSlotCollider = null;

            this.eyeInRange = false;

            this.pillarEntranceInfo.IsPillarEntranceInRange = false;
            this.pillarEntranceInfo.CurrentPillarEntrance = null;

            this.pillarExitInRange = false;
            //***          
        }

        #endregion event handlers

        //########################################################################
        //########################################################################

        class PillarEntranceInfo
        {
            public bool IsPillarEntranceInRange = false;
            public PillarEntrance CurrentPillarEntrance = null;
        }

        //########################################################################
    }
}
//end of namespace