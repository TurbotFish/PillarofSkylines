using System.Collections;
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
        Transform my;
        Vector3 startScale;

        //###########################################################

        protected override void Awake()
        {
            base.Awake();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            my = transform;
            startScale = my.localScale;
            triggered = false;
            if (startOpen)
            {
                Vector3 targetScale = new Vector3(retractingAxis.x ? 0 : 1,
                                                  retractingAxis.y ? 0 : 1,
                                                  retractingAxis.z ? 0 : 1);
                my.localScale = targetScale;
            }
        }

        //###########################################################

        protected override void Activate()
        {
            Toggle(startOpen);
        }

        protected override void Deactivate()
        {
            Toggle(!startOpen);
        }

        //###########################################################

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

        //###########################################################
    }
} //end of namespace