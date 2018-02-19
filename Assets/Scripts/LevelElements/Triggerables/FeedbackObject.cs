using UnityEngine;

namespace Game.LevelElements
{
    public class FeedbackObject : TriggerableObject
    {
        //###########################################################

        [Header("Material")]
        [SerializeField]
        Material on;
        [SerializeField] Material off;

        [Header("Particles")]
        [SerializeField]
        ParticleSystem onActive;
        [SerializeField] ParticleSystem onUnactive;

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
            rend.sharedMaterial = off;
        }

        //###########################################################

        protected override void Activate()
        {
            rend.sharedMaterial = on;
            if (onActive)
                onActive.Play();
        }

        protected override void Deactivate()
        {
            rend.sharedMaterial = off;
            if (onUnactive)
                onUnactive.Play();
        }

        //###########################################################
    }
} //end of namespace