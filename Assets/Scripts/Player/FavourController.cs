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
            if (this.favourPickUpInRange && Input.GetKeyDown(KeyCode.F))
            {
                this.favourPickUpCollider.enabled = false;
                this.playerModel.Favours++;

                LeaveFavourPickUpZone();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.GetMask("PickUps"))
            {
                switch (other.tag)
                {
                    case "favour":
                        this.favourPickUpInRange = true;
                        this.favourPickUpCollider = other;

                        //show UI text
                        Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(true, "Press [F] to pick up favour!"));
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