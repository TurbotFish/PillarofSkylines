using System.Collections;
using UnityEngine;

namespace Game.LevelElements
{
    public class ChildTriggerer : TriggerableObject
    {
        //###########################################################

        [Header("Child Triggerer")]
        public Vector3 offsetWhenOpen;
        Vector3 localPositionWhenOpen, localPositionWhenClosed;

        public float timeToMove = 1;
        Transform my;

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
                localPositionWhenOpen = my.localPosition;
                elapsed = 0;
            }
            else
            {
                localPositionWhenClosed = my.localPosition;
                elapsed = timeToMove;
            }
        }

        //###########################################################

        protected override void Activate()
        {

            if (transform.childCount == 0)
                return;

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        protected override void Deactivate()
        {
            localPositionWhenClosed = localPositionWhenOpen - offsetWhenOpen;
            Move(localPositionWhenOpen, localPositionWhenClosed);
        }

        void Move(Vector3 startPos, Vector3 endPos)
        {
            StopAllCoroutines();
            StartCoroutine(_Move(startPos, endPos));
        }

        float elapsed;
        IEnumerator _Move(Vector3 startPos, Vector3 endPos)
        {

            for (elapsed = timeToMove - elapsed; elapsed < timeToMove; elapsed += Time.deltaTime)
            {
                float t = elapsed / timeToMove;
                my.localPosition = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }
            my.localPosition = endPos;
        }

        //###########################################################
    }
} //end of namespace