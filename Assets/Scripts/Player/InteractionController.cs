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
        //########################################################################

        //
        PlayerModel playerModel;
        Game.Player.CharacterController.Character myPlayer;

        //
        bool favourPickUpInRange = false;
        World.Interaction.Favour favour;

        PillarEntranceInfo pillarEntranceInfo = new PillarEntranceInfo();
        bool pillarExitInRange = false;

        bool needleInRange = false;
        Collider needlePickedUpCollider;

        bool needleSlotInRange = false;

        bool eyeInRange = false;

        //
        bool isActive = false;
        bool isInteractButtonDown = false;
        bool isDriftButtonDown = false;

        //########################################################################

        #region initialization

        /// <summary>
        /// 
        /// </summary>
		public void InitializeFavourController(PlayerModel playerModel, Game.Player.CharacterController.Character player)
        {
            this.playerModel = playerModel;
            myPlayer = player;

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
            {
                return;
            }

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

                        //play favour pick up animation
                        PlayMakerFSM[] temp = favour.MyTransform.parent.GetComponents<PlayMakerFSM>();
                        foreach (var fsm in temp)
                        {
                            if (fsm.FsmName == "Faveur_activation")
                            {
                                fsm.FsmVariables.GetFsmBool("Fav_activated").Value = true;
                            }
                        }

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
                else if (needleInRange)
                {
                    needleInRange = false;
                    needlePickedUpCollider.enabled = false;

                    playerModel.hasNeedle = true;

                    Utilities.EventManager.SendEclipseEvent(this, new Utilities.EventManager.EclipseEventArgs(true));

                    HideUiMessage();
                }
                //needle
                else if (needleSlotInRange)
                {
                    this.needleSlotInRange = false;

                    playerModel.hasNeedle ^= true;

                    Utilities.EventManager.SendEclipseEvent(this, new Utilities.EventManager.EclipseEventArgs(playerModel.hasNeedle));

                    HideUiMessage();
                }
                //eye
                else if (eyeInRange)
                {
                    eyeInRange = false;

                    playerModel.hasNeedle = false;

                    Utilities.EventManager.SendLeavePillarEvent(this, new Utilities.EventManager.LeavePillarEventArgs(true));
                    Debug.Log("oeil mort");

                    HideUiMessage();
                }
            }
            else if (!Input.GetButton("Interact") && isInteractButtonDown)
            {
                isInteractButtonDown = false;
            }

            if (Input.GetButton("Drift") && !isDriftButtonDown && playerModel.hasNeedle)
            {
                isDriftButtonDown = true;
                playerModel.hasNeedle = false;
                needlePickedUpCollider.enabled = true;
                Utilities.EventManager.SendEclipseEvent(this, new Utilities.EventManager.EclipseEventArgs(false));
            }
            else if (!Input.GetButton("Drift") && isDriftButtonDown)
            {
                isDriftButtonDown = false;
            }
        }

        #endregion input handling

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
                        if (pillarExit != null)
                        {
                            pillarExitInRange = true;

                            ShowUiMessage("Press [X] to leave the Pillar!");
                            break;
                        }

                        break;
                    //needle
                    case "Needle":
                        needleInRange = true;
                        needlePickedUpCollider = other;

                        ShowUiMessage("Press [X] to take the needle");
                        break;
                    //needle slot
                    case "NeedleSlot":
                        needleSlotInRange = true;

                        ShowUiMessage(playerModel.hasNeedle ? "Press [X] to plant the needle" : "Press [X] to take the needle");
                        break;
                    //eye
                    case "Eye":
                        if (playerModel.hasNeedle)
                        {
                            eyeInRange = true;

                            ShowUiMessage("Press [X] to plant the needle");
                        }
                        break;
                    //echo
                    case "Echo":
                        Debug.LogWarning("Echo destruction on collision not coded yet");
                        ///Destroy(other.gameObject);
                        break;
                    //wind
                    case "Wind":
                        other.GetComponent<WindTunnelPart>().AddPlayer(myPlayer);
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
                        needlePickedUpCollider = null;

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
        /// "Reset everything!"  "Everything?"  "EVERYTHING!"
        /// </summary>
        void OnSceneChangedEventHandler(object sender, Utilities.EventManager.SceneChangedEventArgs args)
        {
            favourPickUpInRange = false;
            favour = null;

            needleInRange = false;
            needlePickedUpCollider = null;

            eyeInRange = false;

            pillarEntranceInfo.IsPillarEntranceInRange = false;
            pillarEntranceInfo.CurrentPillarEntrance = null;

            pillarExitInRange = false;

            HideUiMessage();
        }

        #endregion event handlers

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