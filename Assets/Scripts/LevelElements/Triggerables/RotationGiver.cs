using System.Collections;
using UnityEngine;

namespace Game.LevelElements
{
    public class RotationGiver : TriggerableObject
    {
        //###########################################################

        [Header("Child Triggerer")]
        public Vector3 offsetWhenOpen;
        Vector3 localPositionWhenOpen, localPositionWhenClosed;

        public float timeToMove = 1;
        Transform my;

        float elapsed;

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

            if (triggered)
            {
                localPositionWhenOpen = my.localEulerAngles;
                elapsed = 0;
            }
            else
            {
                localPositionWhenClosed = my.localEulerAngles;
                elapsed = timeToMove;
            }
        }

        //###########################################################

        protected override void Activate()
        {

            localPositionWhenOpen = localPositionWhenClosed + offsetWhenOpen;
            Move(localPositionWhenClosed, localPositionWhenOpen);
        }

        protected override void Deactivate()
        {
            localPositionWhenClosed = localPositionWhenOpen - offsetWhenOpen;
            Move(localPositionWhenOpen, localPositionWhenClosed);
        }

        //###########################################################

        void Move(Vector3 startPos, Vector3 endPos)
        {
            StopAllCoroutines();
            StartCoroutine(_Move(startPos, endPos));
        }

        IEnumerator _Move(Vector3 startPos, Vector3 endPos)
        {

            for (elapsed = timeToMove - elapsed; elapsed < timeToMove; elapsed += Time.deltaTime)
            {
                float t = elapsed / timeToMove;
                my.localEulerAngles = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }
            my.localEulerAngles = endPos;
        }

        //###########################################################
    }
} //end of namespace