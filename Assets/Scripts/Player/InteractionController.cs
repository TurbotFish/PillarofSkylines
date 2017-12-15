using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayMaker;

namespace Game.Player
{
    /// <summary>
    /// This class handles picking up of objects in the world.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class InteractionController : MonoBehaviour
    {
        //
        PlayerModel playerModel;
        Game.Player.CharacterController.CharController myPlayer;

        //
        bool favourPickUpInRange = false;
        World.Interaction.Favour favour;

        PillarEntranceInfo pillarEntranceInfo = new PillarEntranceInfo();
        bool pillarExitInRange = false;

        bool needleInRange = false;
        Collider needleSlotCollider, needleSlotForDrift;

        bool needleSlotInRange = false;

        bool eyeInRange = false;

        EchoSystem.EchoManager echoManager;

        //
        bool isActive = false;
        bool isInteractButtonDown = false;
        bool isDriftButtonDown = false;

        //########################################################################

        #region initialization

        /// <summary>
        /// 
        /// </summary>
		public void Initialize(PlayerModel playerModel, CharacterController.CharController player, EchoSystem.EchoManager echoManager)
        {
            this.playerModel = playerModel;
			myPlayer = player;
            this.echoManager = echoManager;

            Utilities.EventManager.OnMenuSwitchedEvent += OnMenuSwitchedEventHandler;
            Utilities.EventManager.SceneChangedEvent += OnSceneChangedEventHandler;
        }

        #endregion initialization
        
        //########################################################################

        #region input handling

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            if (!isActive)
                return;

            if (Input.GetButton("Interact") && !isInteractButtonDown)
            {
                isInteractButtonDown = true;

                //favour
                if (favourPickUpInRange)
                {
                    if (!favour.FavourPickedUp)
                    {
                        //pick up favour
                        playerModel.ChangeFavourAmount(1);
                        
                        //send event
                        Utilities.EventManager.SendFavourPickedUpEvent(this, new Utilities.EventManager.FavourPickedUpEventArgs(favour.InstanceId));
                    }

                    //clean up
                    favourPickUpInRange = false;
                    favour = null;
                    HideUiMessage();
                }
                //pillar entrance
                else if (pillarEntranceInfo.IsPillarEntranceInRange)
                {
                    Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowPillarEntranceMenuEventArgs(pillarEntranceInfo.CurrentPillarEntrance.PillarId));
                }
                //pillar exit
                else if (pillarExitInRange)
                {
                    Utilities.EventManager.SendLeavePillarEvent(this, new Utilities.EventManager.LeavePillarEventArgs(false));
                }
                //needle
                else if (needleInRange || needleSlotInRange) {

                    foreach(Transform child in needleSlotCollider.transform)
                        child.gameObject.SetActive(playerModel.hasNeedle);

                    if (playerModel.hasNeedle)
                        needleSlotForDrift = needleSlotCollider;
                    
                    playerModel.hasNeedle ^= true;

                    Utilities.EventManager.SendEclipseEvent(this, new Utilities.EventManager.EclipseEventArgs(playerModel.hasNeedle));

                    ShowUiMessage(playerModel.hasNeedle ? "Press [X] to plant the needle" : "Press [X] to take the needle");
                }
                //eye
                else if (eyeInRange)
                {
                    eyeInRange = false;

                    playerModel.hasNeedle = false;

                    Utilities.EventManager.SendLeavePillarEvent(this, new Utilities.EventManager.LeavePillarEventArgs(true));

                    HideUiMessage();
                }
            }
            else if (!Input.GetButton("Interact") && isInteractButtonDown)
            {
                isInteractButtonDown = false;
            }

            // stop eclipse with drift
            float driftInput = Input.GetAxis("Right Trigger");
            if (driftInput > 0.7f && !isDriftButtonDown && playerModel.hasNeedle)
            {
                isDriftButtonDown = true;
                playerModel.hasNeedle = false;

                if (needleSlotForDrift) {
                    foreach (Transform child in needleSlotForDrift.transform)
                        child.gameObject.SetActive(true);

                    var eventArgs = new Utilities.EventManager.TeleportPlayerEventArgs(needleSlotForDrift.transform.position, Quaternion.identity, false);
                    Utilities.EventManager.SendTeleportPlayerEvent(this, eventArgs);
                }
                needleSlotCollider = needleSlotForDrift;

                Utilities.EventManager.SendEclipseEvent(this, new Utilities.EventManager.EclipseEventArgs(false));
            }
            else if (driftInput < 0.6f && isDriftButtonDown)
            {
                isDriftButtonDown = false;
            }
        }

        #endregion input handling

        //########################################################################
        //########################################################################

        #region collision handling

        /// <summary>
        /// 
        /// </summary>
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("PickUps"))
            {
                switch (other.tag)
                {
                    //favour
                    case "Favour":
                        if (!favourPickUpInRange)
                        {
                            favour = other.GetComponent<World.Interaction.Favour>();

                            if (!favour.FavourPickedUp)
                            {
                                favourPickUpInRange = true;

                                ShowUiMessage("Press [X] to pick up a Favour!");
                            }
                            else
                            {
                                favour = null;
                            }
                        }                     
                        break;
                    //pillar entrance
                    case "Pillar":
                        var pillarEntrance = other.GetComponent<World.Interaction.PillarEntrance>();
                        if (pillarEntrance != null)
                        {
                            if (!playerModel.IsPillarDestroyed(pillarEntrance.PillarId))
                            {
                                pillarEntranceInfo.IsPillarEntranceInRange = true;
                                pillarEntranceInfo.CurrentPillarEntrance = pillarEntrance;

                                ShowUiMessage("Press [X] to enter the Pillar!");
                            }
                            break;
                        }

                        var pillarExit = other.GetComponent<World.Interaction.PillarExit>();
                        if(pillarExit != null)
                        {
                            pillarExitInRange = true;

                            ShowUiMessage("Press [X] to leave the Pillar!");
                            break;
                        }

                        break;
                    //needle
                    case "Needle":
                        needleInRange = true;
                        needleSlotCollider = other;

                        ShowUiMessage(playerModel.hasNeedle ? "Press [X] to plant the needle" : "Press [X] to take the needle");
                        break;
                    //needle slot
                    case "NeedleSlot":


                        // si on est dans l'éclipse on peut poser peut importe lequel c'est
                        // sinon on ne peut retirer que s'il a l'aiguille

                        if (playerModel.hasNeedle || needleSlotCollider && needleSlotCollider == other) {
                            needleSlotInRange = true;
                            needleSlotCollider = other;

                            ShowUiMessage(playerModel.hasNeedle ? "Press [X] to plant the needle" : "Press [X] to take the needle");
                        }

                        break;
                    //eye
                    case "Eye":
                        if (playerModel.hasNeedle) {
                            eyeInRange = true;
                            ShowUiMessage("Press [X] to plant the needle");
                        }
						break;
                    //echo
                    case "Echo":
                        other.GetComponent<EchoSystem.Echo>().Break();
                        break;
                    //echo breaker
                    case "EchoBreaker":
                        echoManager.BreakAll();
                        break;
                    //echo breaker
                    case "EchoSeed":
                        echoManager.CreateEcho(false);
                        Destroy(other.gameObject);
                        break;
                    //wind
                    case "Wind":
                        Debug.LogWarning("commented code!");
						//other.GetComponent<WindTunnelPart>().AddPlayer(myPlayer);
						break;
                    //other
                    default:
                        Debug.LogWarningFormat("InteractionController: unhandled tag: \"{0}\"", other.tag);
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("PickUps"))
            {
                switch (other.tag)
                {
                    //favour
                    case "Favour":
                        favourPickUpInRange = false;
                        favour = null;

                        HideUiMessage();
                        break;
                    //pillar entrance
                    case "Pillar":
                        pillarEntranceInfo.IsPillarEntranceInRange = false;
                        pillarEntranceInfo.CurrentPillarEntrance = null;

                        pillarExitInRange = false;

                        HideUiMessage();
                        break;
                    //needle
                    case "Needle":
                        needleInRange = false;

                        HideUiMessage();
                        break;
                    //needle slot
                    case "NeedleSlot":
                        needleSlotInRange = false;

                        HideUiMessage();
                        break;
                    //eye
                    case "Eye":
                        eyeInRange = false;

                        HideUiMessage();
						break;
                    //eye
                    case "Echo":
                        other.GetComponent<EchoSystem.Echo>().isActive = true;
                        break;
                    //wind
                    case "Wind":
						other.GetComponent<WindTunnelPart>().RemovePlayer();
						break;
                    //other
                    default:
                        Debug.LogWarningFormat("InteractionController: unhandled tag: \"{0}\"", other.tag);
                        break;
                }
            }
        }

        #endregion collision handling

        //########################################################################
        //########################################################################

        #region helper methods

        void ShowUiMessage(string message)
        {
            Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(true, message));
        }

        void HideUiMessage()
        {
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
            if (!this.isActive && args.NewUiState == UI.eUiState.HUD)
            {
                this.isActive = true;
                Debug.Log("InteractionController activated!");
            }
            else if (this.isActive && args.PreviousUiState == UI.eUiState.HUD)
            {
                this.isActive = false;
                Debug.Log("InteractionController deactivated!");
            }
        }

        /// <summary>
        /// "Reset everything!"  "Everything?"  "EVERYTHING!"
        /// </summary>
        void OnSceneChangedEventHandler(object sender, Utilities.EventManager.SceneChangedEventArgs args)
        {
            this.favourPickUpInRange = false;
            this.favour = null;

            this.needleInRange = false;
            this.needleSlotCollider = null;

            this.eyeInRange = false;

            this.pillarEntranceInfo.IsPillarEntranceInRange = false;
            this.pillarEntranceInfo.CurrentPillarEntrance = null;

            this.pillarExitInRange = false;

            HideUiMessage();
        }

        #endregion event handlers

        //########################################################################
        //########################################################################

        class PillarEntranceInfo
        {
            public bool IsPillarEntranceInRange = false;
            public World.Interaction.PillarEntrance CurrentPillarEntrance = null;
        }

        //########################################################################
    }
}
//end of namespace