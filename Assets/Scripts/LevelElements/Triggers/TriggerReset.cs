using UnityEngine;
using System.Collections;
using Game.GameControl;

namespace Game.LevelElements
{
    [RequireComponent(typeof(BoxCollider))]
    public class TriggerReset : Trigger
    {
        //###########################################################

        [SerializeField]
        private string tagToActivate = "Player";
        [SerializeField]
        private bool definitiveActivation;
        [SerializeField]
        private float delayBeforeDeactivation;

        [SerializeField]
        private bool changeMaterial;

        [ConditionalHide("changeMaterial"), SerializeField]
        private int materialID = 0;

        [ConditionalHide("changeMaterial"), SerializeField]
        private Material on, off;

        [ConditionalHide("changeMaterial"), SerializeField]
        private new Renderer renderer;

        //###########################################################

        #region monobehaviour methods

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == tagToActivate)
            {
                /*if (Toggle)
                    TriggerState ^= true;
                else*/
                TriggerState = false;

                if (changeMaterial)
                {
                    Material[] sharedMaterialsCopy = renderer.sharedMaterials;
                    sharedMaterialsCopy[materialID] = TriggerState ? on : off;
                    renderer.sharedMaterials = sharedMaterialsCopy;
                }
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        #region public methods

        public override void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            base.Initialize(gameController, isCopy);

            GetComponent<BoxCollider>().isTrigger = true;
        }

        #endregion public methods

        //###########################################################

        /*private IEnumerator OnTriggerExit(Collider other) {
            if (definitiveActivation || Toggle) yield break;
            if (other.tag == tagToActivate) {
                yield return new WaitForSeconds(delayBeforeDeactivation);
                TriggerState = false;
            }
        }*/

        /*protected override void OnDrawGizmos() {
            base.OnDrawGizmos();
            BoxCollider box = GetComponent<BoxCollider>();
            box.isTrigger = true;

            if (Targets == null || Targets.Count == 0)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;

            Gizmos.DrawWireCube(box.center, box.size);
        }*/

    }
} //end of namespace