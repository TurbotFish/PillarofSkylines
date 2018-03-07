using System.Collections;
using Game.GameControl;
using UnityEngine;

namespace Game.LevelElements
{
    public class RotationGiver : TriggerableObject
    {
        //###########################################################

        [Header("Child Triggerer")]
        [SerializeField]
        private Vector3 offsetWhenOpen;

        [SerializeField]
        private float timeToMove = 1;

        private Vector3 localPositionWhenOpen, localPositionWhenClosed;       
        private Transform my;
        private float elapsed;

        //###########################################################

        #region public methods

        public override void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            base.Initialize(gameController, isCopy);

            my = transform;

            if (Triggered)
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

        #endregion public methods

        //###########################################################

        #region protected methods

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
                my.localEulerAngles = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }
            my.localEulerAngles = endPos;
        }

        #endregion private methods

        //###########################################################
    }
} //end of namespace