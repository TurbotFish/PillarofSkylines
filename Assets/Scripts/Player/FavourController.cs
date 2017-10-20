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

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {


            if (this.favourPickUpInRange && Input.GetButton("Sprint"))
            {
                this.favourPickUpCollider.enabled = false;
                this.playerModel.Favours++;

                LeaveFavourPickUpZone();
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
                        Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(true, "Press [RT] to pick up favour!"));
                        break;
                    case "Pillar":
                        if(this.playerModel.GetAllActiveAbilities().Count + this.playerModel.Favours >= 3)
                        {
                            Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(UI.eUiState.End));
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