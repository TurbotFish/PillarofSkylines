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
		Player myPlayer;

        //
        bool favourPickUpInRange = false;
        Collider favourPickUpCollider;

        PillarEntranceInfo pillarEntranceInfo = new PillarEntranceInfo();

        bool needleInRange = false;
        Collider needlePickedUpCollider;

        bool eyeInRange = false;

        //
        bool isActive = false;
        bool isSprintButtonDown = false;
        bool isDriftButtonDown = false;

        //########################################################################

        #region initialization

        /// <summary>
        /// 
        /// </summary>
		public void InitializeFavourController(PlayerModel playerModel, Player player)
        {
            this.playerModel = playerModel;
			myPlayer = player;

            Utilities.EventManager.OnMenuSwitchedEvent += OnMenuSwitchedEventHandler;
            Utilities.EventManager.OnSceneChangedEvent += OnSceneChangedEventHandler;
        }

        #endregion initialization

        //########################################################################
        //########################################################################

        #region input handling

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            if (!this.isActive)
            {
                return;
            }

            if (Input.GetButton("Sprint") && !this.isSprintButtonDown)
            {
                this.isSprintButtonDown = true;

                //favour
                if (this.favourPickUpInRange)
                {
                    //pick up favour
                    this.favourPickUpCollider.enabled = false;
                    this.playerModel.Favours++;

                    //play favour pick up animation
                    PlayMakerFSM[] temp = favourPickUpCollider.transform.parent.GetComponents<PlayMakerFSM>();
                    foreach (var fsm in temp)
                    {
                        if (fsm.FsmName == "Faveur_activation")
                        {
                            fsm.FsmVariables.GetFsmBool("Fav_activated").Value = true;
                        }
                    }

                    //
                    this.favourPickUpInRange = false;
                    this.favourPickUpCollider = null;

                    HideUiMessage();
                }
                //pillar entrance
                else if (this.pillarEntranceInfo.IsPillarEntranceInRange)
                {
                    Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(UI.eUiState.End));
                }
                else if (this.needleInRange)
                {
                    this.needleInRange = false;
                    this.needlePickedUpCollider.enabled = false;

                    playerModel.hasNeedle = true;

                    Utilities.EventManager.SendOnEclipseEvent(this, new Utilities.EventManager.OnEclipseEventArgs(true));

                    HideUiMessage();
                }
                else if (this.eyeInRange)
                {
                    this.eyeInRange = false;

                    playerModel.hasNeedle = false;

                    Utilities.EventManager.SendOnEyeKilledEvent(this);
                    Debug.Log("oeil mort");

                    HideUiMessage();
                }
            }
            else if (!Input.GetButton("Sprint") && this.isSprintButtonDown)
            {
                this.isSprintButtonDown = false;
            }

            if (Input.GetButton("Drift") && !this.isDriftButtonDown && playerModel.hasNeedle)
            {
                this.isDriftButtonDown = true;
                playerModel.hasNeedle = false;
                this.needlePickedUpCollider.enabled = true;
                Utilities.EventManager.SendOnEclipseEvent(this, new Utilities.EventManager.OnEclipseEventArgs(false));
            }
            else if (!Input.GetButton("Drift") && this.isDriftButtonDown)
            {
                this.isDriftButtonDown = false;
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
                        this.favourPickUpInRange = true;
                        this.favourPickUpCollider = other;

                        ShowUiMessage("Press [X] to pick up a Favour!");
                        break;
                    //pillar entrance
                    case "Pillar":
                        var pillarEntrance = other.GetComponent<World.Interaction.PillarEntrance>();

                        if (!this.playerModel.IsPillarDestroyed(pillarEntrance.PillarId) && this.playerModel.Favours >= pillarEntrance.EntryPrice)
                        {
                            this.pillarEntranceInfo.IsPillarEntranceInRange = true;
                            this.pillarEntranceInfo.CurrentPillarEntrance = pillarEntrance;

                            ShowUiMessage("Press [X] to enter the Pillar!");
                        }
                        break;
                    //needle
                    case "Needle":
                        this.needleInRange = true;
                        this.needlePickedUpCollider = other;

                        ShowUiMessage("Press [X] to take the needle");
                        break;
                    //eye
                    case "Eye":
                        if (playerModel.hasNeedle)
                        {
                            this.eyeInRange = true;

                            ShowUiMessage("Press [X] to plant the needle");
                        }
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
                        this.favourPickUpInRange = false;
                        this.favourPickUpCollider = null;

                        HideUiMessage();
                        break;
                    //pillar entrance
                    case "Pillar":
                        this.pillarEntranceInfo.IsPillarEntranceInRange = false;
                        this.pillarEntranceInfo.CurrentPillarEntrance = null;

                        HideUiMessage();
                        break;
                    //needle
                    case "Needle":
                        this.needleInRange = false;
                        this.needlePickedUpCollider = null;

                        HideUiMessage();
                        break;
                    //eye
                    case "Eye":
                        this.eyeInRange = false;

                        HideUiMessage();
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
            else
            {
                Debug.LogWarningFormat("InteractionController: isActive={0}, previousUiState={1}, newUiState={2}", this.isActive, args.PreviousUiState, args.NewUiState);
            }
        }

        /// <summary>
        /// "Reset everything!"  "Everything?"  "EVERYTHING!"
        /// </summary>
        void OnSceneChangedEventHandler(object sender, Utilities.EventManager.OnSceneChangedEventArgs args)
        {
            this.favourPickUpInRange = false;
            this.favourPickUpCollider = null;

            this.needleInRange = false;
            this.needlePickedUpCollider = null;

            this.eyeInRange = false;

            this.pillarEntranceInfo.IsPillarEntranceInRange = false;
            this.pillarEntranceInfo.CurrentPillarEntrance = null;
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