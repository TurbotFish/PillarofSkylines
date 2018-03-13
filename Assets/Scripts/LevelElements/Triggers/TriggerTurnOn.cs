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

        public override void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            base.Initialize(gameController, isCopy);

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