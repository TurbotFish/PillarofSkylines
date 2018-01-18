using UnityEngine;

namespace Game.Player {
    /// <summary>
    /// This class handles picking up of objects in the world.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class InteractionController : MonoBehaviour
    {
        //
        PlayerModel playerModel;
        CharacterController.CharController myPlayer;

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
        World.SpawnPointSystem.SpawnPointManager spawnPointManager;
        Transform airParticle, airOrigin;

        HomeBeacon homeBeacon;

        //
        bool isActive = false;
        bool isInteractButtonDown = false;
        bool isDriftButtonDown = false;

        //########################################################################

        #region initialization
        
		public void Initialize(PlayerModel playerModel, CharacterController.CharController player, EchoSystem.EchoManager echoManager)
        {
            this.playerModel = playerModel;
			myPlayer = player;
            this.echoManager = echoManager;

            spawnPointManager = FindObjectOfType<World.SpawnPointSystem.SpawnPointManager>(); //TODO: Fix that

            Utilities.EventManager.OnMenuSwitchedEvent += OnMenuSwitchedEventHandler;
            Utilities.EventManager.SceneChangedEvent += OnSceneChangedEventHandler;
        }

        #endregion initialization
        
        //########################################################################

        #region input handling
            
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

                    playerModel.hasNeedle ^= true;

                    if (playerModel.hasNeedle)
                        needleSlotForDrift = needleSlotCollider;
                    
                    Utilities.EventManager.SendEclipseEvent(this, new Utilities.EventManager.EclipseEventArgs(playerModel.hasNeedle));

                    ShowUiMessage(playerModel.hasNeedle ? "Press [X] to plant the needle" : "Press [X] to take the needle");
                }
                //eye
                else if (eyeInRange) {
                    eyeInRange = false;
                    playerModel.hasNeedle = false;
                    Utilities.EventManager.SendLeavePillarEvent(this, new Utilities.EventManager.LeavePillarEventArgs(true));

                    HideUiMessage();
                }
                //home beacon
                else if (homeBeacon && homeBeacon.activated) {
                    var eventArgs = new Utilities.EventManager.TeleportPlayerEventArgs(homeBeacon.destination.position, Quaternion.identity, false);
                    Utilities.EventManager.SendTeleportPlayerEvent(this, eventArgs);
                }

            }
            else if (!Input.GetButton("Interact") && isInteractButtonDown)
            {
                isInteractButtonDown = false;
            }

			// drift input handling
            float driftInput = Input.GetAxis("Drift") + (Input.GetButtonUp("Drift") ? 1 : 0);
            if (driftInput > 0.5f && !isDriftButtonDown)  {
                isDriftButtonDown = true;
            }
            else if (driftInput < 0.4f && isDriftButtonDown) {
                isDriftButtonDown = false;

                // stop eclipse
                if (playerModel.hasNeedle) {

                    print("Needle Slot for Drift: " + needleSlotForDrift + " Collider: " + needleSlotCollider);

                    playerModel.hasNeedle = false;

                    if (needleSlotForDrift) {
                        foreach (Transform child in needleSlotForDrift.transform)
                            child.gameObject.SetActive(true);

                        var eventArgs = new Utilities.EventManager.TeleportPlayerEventArgs(needleSlotForDrift.transform.position, false);
                        Utilities.EventManager.SendTeleportPlayerEvent(this, eventArgs);
                    }
                    needleSlotCollider = needleSlotForDrift;

                    Utilities.EventManager.SendEclipseEvent(this, new Utilities.EventManager.EclipseEventArgs(false));

                    // stop air particles
                } else if (airParticle) {
                    airParticle.parent = airOrigin;
                    airParticle.transform.localPosition = Vector3.zero;
                    airParticle = null;
                }
            }
			
            // stop air particle if grounded
            if (airParticle && (myPlayer.CurrentState & (CharacterController.ePlayerState.move | CharacterController.ePlayerState.slide | CharacterController.ePlayerState.stand)) != 0) {
                airParticle.parent = airOrigin;
                airParticle.transform.localPosition = Vector3.zero;
                airParticle = null;
            }
        }

        #endregion input handling

        //########################################################################
        //########################################################################

        #region collision handling
            
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
                        // si on est dans l'éclipse on peut poser peu importe lequel c'est
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
						other.GetComponent<WindTunnelPart>().AddPlayer(myPlayer);
						break;
                    // air particle
                    case "AirParticle":
                        if (!airParticle) {
                            airOrigin = other.transform;
                            airParticle = airOrigin.GetChild(0);
                            airParticle.parent = myPlayer.transform;
                            airParticle.localPosition = new Vector3(0,1,0);
                        }
                        break;
                    // air particle
                    case "AirReceptor":
                        if (airParticle) {
                            other.GetComponent<AirReceptor>().Activate();
                            Destroy(airParticle.gameObject);
                            Destroy(airOrigin.gameObject);
                            airParticle = null;
                            airOrigin = null;
                        }
                        break;
                    // doorHome
                    case "DoorHome": {
                            // check if player is on the right side of door
                            float dot = Vector3.Dot(other.transform.forward, myPlayer.transform.forward);
                            
                            if (dot > 0.4f) {
                                // take offset from exact door position
                                Vector3 offset = myPlayer.transform.position - other.transform.position;
                                Vector3 targetPoint = spawnPointManager.GetHomeSpawnPoint() + other.transform.parent.InverseTransformDirection(offset);
                                // calculate new rotation
                                Vector3 newForward = other.transform.parent.InverseTransformDirection(myPlayer.transform.forward);
                                // teleport to Home
                                var eventArgs = new Utilities.EventManager.TeleportPlayerEventArgs(targetPoint, Quaternion.LookRotation(newForward), false); //TODO: make it take rotation as well
                                Utilities.EventManager.SendTeleportPlayerEvent(this, eventArgs);
                            }
                            break;
                        }
                    // doorHome
                    case "DoorBackHome": {
                            // check if player is on the right side of door
                            float dot = Vector3.Dot(other.transform.forward, myPlayer.transform.forward);
                            
                            if (dot < -0.4f) {
                                // take offset from exact door position
                                Vector3 offset = myPlayer.transform.position - other.transform.position;
                                Transform destination = other.GetComponent<HomeBeacon>().destination;
                                Vector3 targetPoint = destination.position + destination.parent.TransformDirection(offset);
                                // calculate new rotation
                                Vector3 newForward = destination.parent.TransformDirection( myPlayer.transform.forward);
                                // teleport to temporary Door
                                var eventArgs = new Utilities.EventManager.TeleportPlayerEventArgs(targetPoint, Quaternion.LookRotation(newForward), false); //TODO: make it take rotation as well
                                Utilities.EventManager.SendTeleportPlayerEvent(this, eventArgs);
                            }
                            break;
                        }
                    // HomeBeacon
                    case "HomeBeacon":
                        homeBeacon = other.GetComponent<HomeBeacon>();
                        if (homeBeacon.activated)
                            ShowUiMessage("Press [X] to teleport");
                        break;
                    // Trigger Activator
                    case "TriggerActivator":
                        other.GetComponent<TriggerSystem.TimedActivator>().manager.Activate();
                        break;
                    //other
                    default:
                        Debug.LogWarningFormat("InteractionController: unhandled tag: \"{0}\"", other.tag);
                        break;
                }
            }
        }
        
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
                    // HomeBeacon
                    case "HomeBeacon":
                        homeBeacon = null;

                        HideUiMessage();
                        break;
                    // Trigger Activator
                    case "TriggerActivator":
                        other.GetComponent<TriggerSystem.TimedActivator>().manager.StartTimer();
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

        class PillarEntranceInfo {
            public bool IsPillarEntranceInRange = false;
            public World.Interaction.PillarEntrance CurrentPillarEntrance = null;
        }

        //########################################################################
    }
}
//end of namespace