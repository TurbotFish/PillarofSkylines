using System.Collections;
using Game.GameControl;
using UnityEngine;

namespace Game.LevelElements {

    public class DissolveDoor : TriggerableObject {
        
        public float timeToDissolve = 1;

        public Material closedMat, openMat;
        public Renderer myRenderer;
        public Collider myCollider;

        Transform my; //use the property "MyTransform" instead of this! The property is guranteed to be initialized!
        float elapsed;

        //###########################################################

        public Transform MyTransform { get { if (my == null) { my = transform; } return my; } }

        //###########################################################

        #region public methods

        public override void Initialize(GameController gameController)
        {
            base.Initialize(gameController);
            
            if (Triggered) {
                elapsed = 0;
            }
            else {
                elapsed = timeToDissolve;
            }
        }

        #endregion public methods

        //###########################################################

        #region protected methods

        protected override void Activate() {
            Debug.LogFormat("Door \"{0}\": Activate called!", name);
            Dissolve(closedMat, openMat, true);
            myCollider.enabled = false;
        }

        protected override void Deactivate() {
            Debug.LogFormat("Door \"{0}\": Deactivate called!", name);
            Dissolve(openMat, closedMat, false);
        }

        #endregion protected

        //###########################################################

        #region private methods

        private void Dissolve(Material startMat, Material endMat, bool enable)
        {
            StopAllCoroutines();
            StartCoroutine(_Dissolve(startMat, endMat, enable));
        }

        private IEnumerator _Dissolve(Material startMat, Material endMat, bool enable) {
            for (elapsed = timeToDissolve - elapsed; elapsed < timeToDissolve; elapsed += Time.deltaTime)
            {
                float t = elapsed / timeToDissolve;

                myRenderer.material.Lerp(startMat, endMat, t);

                yield return null;
            }
            myRenderer.sharedMaterial = endMat;
            if (!enable)
                myCollider.enabled = true;
        }

        #endregion private methods


    }
}