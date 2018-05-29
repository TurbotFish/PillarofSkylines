using System.Collections;
using Game.GameControl;
using UnityEngine;

namespace Game.LevelElements
{
    public class RetractingDoor : TriggerableObject
    {
        //###########################################################

        [Header("Door")]
        public Bool3 retractingAxis;
        public float timeToRetract = 1;
        public bool startOpen;

        private Transform my;
        private Vector3 startScale;

        //###########################################################

        #region public methods

        public override void Initialize(GameController gameController)
        {
            base.Initialize(gameController);

            my = transform;
            startScale = my.localScale;
            SetTriggered(false);

            if (startOpen)
            {
                Vector3 targetScale = new Vector3(retractingAxis.x ? 0 : 1,
                                                  retractingAxis.y ? 0 : 1,
                                                  retractingAxis.z ? 0 : 1);
                my.localScale = targetScale;
            }
        }

        #endregion public methods

        //###########################################################

        #region protected methods

        protected override void Activate()
        {
            Toggle(startOpen);
        }

        protected override void Deactivate()
        {
            Toggle(!startOpen);
        }

        #endregion protected methods

        //###########################################################

        #region private methods

        private void Toggle(bool yo)
        {
            if (yo)
            {
                Retract(startScale);
            }
            else
            {
                Vector3 targetScale = new Vector3(retractingAxis.x ? 0 : 1,
                                                  retractingAxis.y ? 0 : 1,
                                                  retractingAxis.z ? 0 : 1);
                Retract(targetScale);
            }
        }

        private void Retract(Vector3 targetScale)
        {
            StopAllCoroutines();
            StartCoroutine(_Retract(targetScale));
        }

        private IEnumerator _Retract(Vector3 targetScale)
        {

            Vector3 startScale = my.localScale;
            for (float elapsed = 0; elapsed < timeToRetract; elapsed += Time.deltaTime)
            {

                my.localScale = Vector3.Lerp(startScale, targetScale, elapsed / timeToRetract);
                yield return null;
            }
            my.localScale = targetScale;
        }

        #endregion private methods

        //###########################################################
    }
} //end of namespace