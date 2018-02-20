using Game.GameControl;
using UnityEngine;

namespace Game.LevelElements
{
    public class FeedbackObject : TriggerableObject
    {
        //###########################################################

        [Header("Material")]
        [SerializeField]
        private Material on;

        [SerializeField]
        private Material off;

        [Header("Particles")]
        [SerializeField]
        private ParticleSystem onActive;

        [SerializeField]
        private ParticleSystem onUnactive;

        private Renderer rend;

        //###########################################################

        #region public methods

        public override void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            base.Initialize(gameController, isCopy);

            rend = GetComponent<Renderer>();
            rend.sharedMaterial = off;
        }

        #endregion public methods

        //###########################################################

        #region protected methods

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

        #endregion protected methods

        //###########################################################
    }
} //end of namespace