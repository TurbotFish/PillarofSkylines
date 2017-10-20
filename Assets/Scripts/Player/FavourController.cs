using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(Collider))]
    public class FavourController : MonoBehaviour
    {
        [SerializeField]
        PlayerModel playerModel;

        bool favourPickUpInRange = false;
        Collider favourPickUpCollider;

        bool pillarInRange = false;

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

            if (Input.GetButton("Sprint")) {
                if (this.favourPickUpInRange)
                {
                    this.favourPickUpCollider.enabled = false;
                    this.playerModel.Favours++;

                    LeaveFavourPickUpZone();
                }
                else if (this.pillarInRange)
                {
                    Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(UI.eUiState.End));

                    this.pillarInRange = false;

                    //hide UI text
                    Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(false));
                }
            }
        }

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
                    default:
                        break;
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.GetMask("PickUps"))
            {
                switch (other.tag)
                {
                    case "favour":
                        LeaveFavourPickUpZone();
                        break;
                    default:
                        break;
                }
            }
        }

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