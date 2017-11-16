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
		global::Player myPlayer;

        //
        bool favourPickUpInRange = false;
        World.Interaction.Favour favour;

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
		public void InitializeFavourController(PlayerModel playerModel, global::Player player)
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
                    if (!this.favour.FavourPickedUp)
                    {
                        //pick up favour
                        this.playerModel.Favours++;

                        //play favour pick up animation
                        PlayMakerFSM[] temp = this.favour.MyTransform.parent.GetComponents<PlayMakerFSM>();
                        foreach (var fsm in temp)
                        {
                            if (fsm.FsmName == "Faveur_activation")
                            {
                                fsm.FsmVariables.GetFsmBool("Fav_activated").Value = true;
                            }
                        }

                        //send event
                        Utilities.EventManager.SendFavourPickedUpEvent(this, new Utilities.EventManager.FavourPickedUpEventArgs(this.favour.InstanceId));
                    }

                    //clean up
                    this.favourPickUpInRange = false;
                    this.favour = null;
                    HideUiMessage();
                }
                //pillar entrance
                else if (this.pillarEntranceInfo.IsPillarEntranceInRange)
                {
                    Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowPillarEntranceMenuEventArgs(this.pillarEntranceInfo.CurrentPillarEntrance.PillarId));
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
                        if (!this.favourPickUpInRange)
                        {
                            this.favour = other.GetComponent<World.Interaction.Favour>();

                            if (!this.favour.FavourPickedUp)
                            {
                                this.favourPickUpInRange = true;

                                ShowUiMessage("Press [X] to pick up a Favour!");
                            }
                            else
                            {
                                this.favour = null;
                            }
                        }                     
                        break;
                    //pillar entrance
                    case "Pillar":
                        var pillarEntrance = other.GetComponent<World.Interaction.PillarEntrance>();

                        if (!this.playerModel.IsPillarDestroyed(pillarEntrance.PillarId) && this.playerModel.Favours >= this.playerModel.PillarData.GetPillarEntryPrice(pillarEntrance.PillarId))
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
                        this.favourPickUpInRange = false;
                        this.favour = null;

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

        static string DetermineFavourId(Collider favourCollider)
        {
            return string.Format("{0}-{1}", favourCollider.name, favourCollider.transform.position);
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
        void OnSceneChangedEventHandler(object sender, Utilities.EventManager.OnSceneChangedEventArgs args)
        {
            this.favourPickUpInRange = false;
            this.favour = null;

            this.needleInRange = false;
            this.needlePickedUpCollider = null;

            this.eyeInRange = false;

            this.pillarEntranceInfo.IsPillarEntranceInRange = false;
            this.pillarEntranceInfo.CurrentPillarEntrance = null;

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