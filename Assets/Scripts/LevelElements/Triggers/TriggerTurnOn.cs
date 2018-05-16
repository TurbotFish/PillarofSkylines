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

        public override void Initialize(IGameController gameController)
        {
            base.Initialize(gameController);

            GetComponent<BoxCollider>().isTrigger = true;
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