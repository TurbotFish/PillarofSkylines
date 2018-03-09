using UnityEngine;
using System.Collections;
using Game.GameControl;

namespace Game.LevelElements
{
    [RequireComponent(typeof(BoxCollider))]
    public class TriggerBox : Trigger
    {
        //###########################################################

        [SerializeField]
        string tagToActivate = "Player";
        [SerializeField]
        bool definitiveActivation;
        [SerializeField]
        float delayBeforeDeactivation;

        [SerializeField]
        bool changeMaterial;

        [ConditionalHide("changeMaterial"), SerializeField]
        int materialID = 0;

        [ConditionalHide("changeMaterial"), SerializeField]
        Material on, off;

        [ConditionalHide("changeMaterial"), SerializeField]
        new Renderer renderer;

        //###########################################################

        #region editor methods

#if UNITY_EDITOR

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            BoxCollider box = GetComponent<BoxCollider>();
            box.isTrigger = true;

            if (Targets == null || Targets.Count == 0)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;

            Gizmos.DrawWireCube(box.center, box.size);
        }

#endif

        #endregion editor methods

        //###########################################################

        #region monobehaviour methods

        private void OnTriggerEnter(Collider other)
        {
            print("helo");
            if (other.tag == tagToActivate && Mathf.Abs(Vector3.Dot(other.transform.up, transform.up)) > 0.9f)
            {
                if (Toggle)
                    SetTriggerState(!TriggerState);
                else
                    SetTriggerState(true);

                if (changeMaterial)
                {
                    Material[] sharedMaterialsCopy = renderer.sharedMaterials;
                    sharedMaterialsCopy[materialID] = TriggerState ? on : off;
                    renderer.sharedMaterials = sharedMaterialsCopy;
                }
            }
        }

        private IEnumerator OnTriggerExit(Collider other)
        {
            if (definitiveActivation || Toggle) yield break;
            if (other.tag == tagToActivate)
            {
                yield return new WaitForSeconds(delayBeforeDeactivation);
                SetTriggerState(false);
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
    }
} //end of namespace