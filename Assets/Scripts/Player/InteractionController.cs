﻿using System.Collections;
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

        //
        bool favourPickUpInRange = false;
        Collider favourPickUpCollider;

		//
		bool needleInRange = false;
		Collider needlePickedUpCollider;
		bool eyeInRange = false;

		//
		bool pillarInRange = false;

        /// <summary>
        /// 
        /// </summary>
        public void InitializeFavourController(PlayerModel playerModel)
        {
            this.playerModel = playerModel;
        }

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {

            if (Input.GetButtonDown("Sprint")) {
                if (this.favourPickUpInRange)
                {
                    this.favourPickUpCollider.enabled = false;
                    this.playerModel.Favours++;

					//favourPickUpCollider.transform.parent.GetComponent<PlayMakerFSM>().Fsm.f
					PlayMakerFSM[] temp = favourPickUpCollider.transform.parent.GetComponents<PlayMakerFSM>();
					foreach (var fsm in temp) {
						if (fsm.FsmName == "Faveur_activation") {
							fsm.FsmVariables.GetFsmBool ("Fav_activated").Value = true;
						}
					}

                    LeaveFavourPickUpZone();
                }
                else if (this.pillarInRange)
                {
                    Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(UI.eUiState.End));

                    this.pillarInRange = false;

                    //hide UI text
                    Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(false));
                }
				else if (this.needleInRange)
				{
					this.needleInRange = false;
					this.needlePickedUpCollider.enabled = false;

					playerModel.hasNeedle = true;

					Utilities.EventManager.SendOnEclipseEvent(this, new Utilities.EventManager.OnEclipseEventArgs(true));

					//hide UI text
					Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(false));
				}
				else if (this.eyeInRange)
				{
					this.eyeInRange = false;

					playerModel.hasNeedle = false;

					Utilities.EventManager.SendOnEyeKilledEvent(this);
					Debug.Log("oeil mort");

					//hide UI text
					Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(false));
				}
            }

			if (Input.GetButtonUp("Drift")) {
				playerModel.hasNeedle = false;
				this.needlePickedUpCollider.enabled = true;
				Utilities.EventManager.SendOnEclipseEvent(this, new Utilities.EventManager.OnEclipseEventArgs(false));
			}
        }

        /// <summary>
        /// 
        /// </summary>
        void OnTriggerEnter(Collider other)
        {
            //Debug.LogFormat("trigger enter: name={0}, layer={1}, tag={2}, pickUpLayerId={3}", other.name, other.gameObject.layer, other.tag, LayerMask.NameToLayer("PickUps"));

            if (other.gameObject.layer == LayerMask.NameToLayer("PickUps"))
            {
                //Debug.LogFormat("trigger enter check 1");

                switch (other.tag)
				{
                    case "Favour":
                        //Debug.LogFormat("trigger enter check 2");

                        this.favourPickUpInRange = true;
						this.favourPickUpCollider = other;

						//show UI text
						Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(true, "Press [X] to pick up a Favour!"));

						break;
					case "Pillar":
						if(this.playerModel.GetAllActiveAbilities().Count + this.playerModel.Favours >= 3)
						{
							this.pillarInRange = true;

							//show UI text
							Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(true, "Press [X] to enter the Pillar!"));
						}

						break;
					case "Needle":
						this.needleInRange = true;
						this.needlePickedUpCollider = other;

						//show UI text
						Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(true, "Press [X] to take the needle"));

						break;
					case "Eye":
						if (playerModel.hasNeedle)
						{
							this.eyeInRange = true;
							//show UI text
							Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(true, "Press [X] to plant the needle"));
						}
						break;
                    default:
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
                    case "favour":
                        LeaveFavourPickUpZone();
                        break;
					case "Needle":
						this.needleInRange = false;
						this.needlePickedUpCollider = null;

						//hide UI text
						Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(false));
						break;
					case "Eye":
						this.eyeInRange = false;

						//hide UI text
						Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(false));
						break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void LeaveFavourPickUpZone()
        {
            this.favourPickUpInRange = false;
            this.favourPickUpCollider = null;

            //hide UI text
            Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(false));
        }
    }
}
//end of namespace