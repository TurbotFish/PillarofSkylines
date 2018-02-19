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

        private void OnValidate()
        {
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

        #region monobehaviour methods

        private void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            rend = GetComponent<Renderer>();
            TriggerState = false;
            rend.sharedMaterial = off;
        }

        #endregion monobehaviour methods

        //###########################################################

        #region public methods

        public void SetToggle(bool newState, bool inverse)
        {
            TriggerState = newState;
            if (TriggerState == inverse)
                rend.sharedMaterial = on;
            else
                rend.sharedMaterial = off;
        }

        #endregion public methods

        //###########################################################
    }
} //end of namespace