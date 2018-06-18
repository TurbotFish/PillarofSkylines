using Game.GameControl;
using UnityEngine;

namespace Game.LevelElements
{
    public class LightReceptor : Trigger
    {
        //###########################################################

        [SerializeField]
        Transform lookAtTarget;

        Renderer rend;

        [SerializeField]
        Material on;

        [SerializeField]
        Material off;

        //###########################################################

        #region editor methods

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();

            rend = GetComponent<Renderer>();
            if (lookAtTarget)
            {
                transform.LookAt(lookAtTarget);
                lookAtTarget.LookAt(transform);
            }
        }

#endif

        #endregion editor methods

        //###########################################################

        #region public methods

        public override void Initialize(GameController gameController)
        {
            base.Initialize(gameController);

            rend = GetComponent<Renderer>();
            SetTriggerState(false);
            rend.sharedMaterial = off;
        }

        /// <summary>
        /// Activates or deactivates the LightReceptor.
        /// </summary>
        /// <param name="newState"></param>
        /// <param name="inverse"></param>
        public void SetToggle(bool newState, bool inverse)
        {
            Debug.LogFormat("LightReceptor: SetToggle: newState={0}", newState);
            SetTriggerState(newState);

            if (TriggerState == inverse)
            {
                rend.sharedMaterial = on;
            }
            else
            {
                rend.sharedMaterial = off;
            }
        }

        protected override void OnTriggerStateChanged(bool old_state, bool new_state)
        {
        }

        #endregion public methods

        //###########################################################
    }
} //end of namespace