using System.Collections;
using Game.GameControl;
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

        #region public methods

        public override void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            base.Initialize(gameController, isCopy);

            my = transform;
            platform = GetComponent<MovingPlatform>();

            if (Triggered)
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

        #endregion public methods

        //###########################################################

        #region protected methods

        protected override void Activate()
        {
            Debug.LogFormat("Door \"{0}\": Activate called!", name);

            localPositionWhenOpen = localPositionWhenClosed + offsetWhenOpen;
            Move(localPositionWhenClosed, localPositionWhenOpen);
        }

        protected override void Deactivate()
        {
            Debug.LogFormat("Door \"{0}\": Deactivate called!", name);

            localPositionWhenClosed = localPositionWhenOpen - offsetWhenOpen;
            Move(localPositionWhenOpen, localPositionWhenClosed);
        }

        #endregion protected methods

        //###########################################################

        #region private methods

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

        #endregion private methods

        //###########################################################
    }
} //end of namespace