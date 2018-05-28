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

        public Transform MyTransform { get { if (my == null) { my = transform; } return my; } }

        //###########################################################

        #region public methods

        public override void Initialize(GameController gameController)
        {
            base.Initialize(gameController);

            if (my == null)
            {
                my = transform;
            }

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
                MyTransform.localEulerAngles = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }
            MyTransform.localEulerAngles = endPos;
        }

        #endregion private methods

        //###########################################################
    }
} //end of namespace