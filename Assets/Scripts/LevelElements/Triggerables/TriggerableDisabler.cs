using UnityEngine;

namespace Game.LevelElements
{
    public class TriggerableDisabler : TriggerableObject
    {
        //###########################################################

        [Header("Disabler")]
        [SerializeField]
        bool disabledByDefault;

        [SerializeField] GameObject[] objects;

        Renderer rend;

        //###########################################################

        private void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            rend = GetComponent<Renderer>();
            foreach (GameObject go in objects)
                go.SetActive(!disabledByDefault);
        }

        //###########################################################

        protected override void Activate()
        {
            foreach (GameObject go in objects)
                go.SetActive(disabledByDefault);
        }

        protected override void Deactivate()
        {
            foreach (GameObject go in objects)
                go.SetActive(!disabledByDefault);
        }

        //###########################################################
    }
} //end of namespace