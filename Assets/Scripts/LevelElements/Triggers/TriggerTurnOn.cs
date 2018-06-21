using UnityEngine;
using Game.GameControl;

namespace Game.LevelElements
{
    [RequireComponent(typeof(BoxCollider))]
    public class TriggerTurnOn : Trigger
    {
        //###########################################################      

        [SerializeField]
        string tagToActivate = "Player";

        [SerializeField]
        bool setState = true;

        //###########################################################

        public override void Initialize(GameController gameController)
        {
            base.Initialize(gameController);

            GetComponent<BoxCollider>().isTrigger = true;
        }

        protected override void OnTriggerStateChanged(bool old_state, bool new_state)
        {

        }

        //###########################################################

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == tagToActivate)
            {
                SetTriggerState(setState, true);

            }
        }

        //###########################################################
    }
}