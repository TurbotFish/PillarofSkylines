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
        public float comingBackMultiplicator = 1;

        Transform my; //use the property "MyTransform" instead of this! The property is guranteed to be initialized!
        MovingPlatform platform;
        float elapsed;
        bool comingBack = false;
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
            //Debug.LogFormat("Door \"{0}\": Activate called!", name);
            comingBack = false;
            localPositionWhenOpen = localPositionWhenClosed + offsetWhenOpen;
            Move(localPositionWhenClosed, localPositionWhenOpen);
        }

        protected override void Deactivate()
        {
            //Debug.LogFormat("Door \"{0}\": Deactivate called!", name);
            comingBack = true;
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
            for (elapsed = timeToMove - elapsed; elapsed < timeToMove; elapsed += Time.deltaTime * (comingBack ? comingBackMultiplicator : 1))
            {
                float t = elapsed / timeToMove;

                Vector3 movement = Vector3.Lerp(startPos, endPos, t) - MyTransform.localPosition;
                platform.Move(movement);
                yield return null;
            }

            MyTransform.localPosition = endPos;
        }

        #endregion private methods

        //###########################################################
    }
} //end of namespace