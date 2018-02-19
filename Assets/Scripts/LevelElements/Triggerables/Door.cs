using System.Collections;
using UnityEngine;

namespace Game.LevelElements
{
    [RequireComponent(typeof(MovingPlatform))]
    public class Door : TriggerableObject
    {
        //###########################################################

        [Header("Door")]
        public Vector3 offsetWhenOpen;
        Vector3 localPositionWhenOpen, localPositionWhenClosed;

        public float timeToMove = 1;
        Transform my;

        MovingPlatform platform;

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

            platform = GetComponent<MovingPlatform>();

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
            localPositionWhenOpen = localPositionWhenClosed + offsetWhenOpen;
            Move(localPositionWhenClosed, localPositionWhenOpen);
        }

        protected override void Deactivate()
        {
            localPositionWhenClosed = localPositionWhenOpen - offsetWhenOpen;
            Move(localPositionWhenOpen, localPositionWhenClosed);
        }

        private void Move(Vector3 startPos, Vector3 endPos)
        {
            StopAllCoroutines();
            StartCoroutine(_Move(startPos, endPos));
        }

        private IEnumerator _Move(Vector3 startPos, Vector3 endPos)
        {
            for (elapsed = timeToMove - elapsed; elapsed < timeToMove; elapsed += Time.deltaTime)
            {
                float t = elapsed / timeToMove;


                Vector3 movement = Vector3.Lerp(startPos, endPos, t) - my.localPosition;
                platform.Move(movement);
                yield return null;
            }
            my.localPosition = endPos;
        }

        //###########################################################
    }
} //end of namespace