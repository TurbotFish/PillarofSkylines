using System.Collections;
using Game.GameControl;
using UnityEngine;
using System.Collections.Generic;

namespace Game.LevelElements
{
    [RequireComponent(typeof(MovingPlatform))]
    public class StepByStepMovement : TriggerableObject
    {
        //###########################################################

        [Header("Step By Step Movement")]
        public List<Vector3> offsets;

        List<Vector3> localPositions;
        Vector3 beforeLocalPosition, afterLocalPosition, startPosition;

        public float timeToMove = 1;

        int currentState = 0;
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

            startPosition = my.localPosition;
            elapsed = timeToMove;

        }

        public override void SetTriggered(bool triggered)
        {
            print("am trigered : " + name + " state : " + currentState);
            if (triggered)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }

        }

        #endregion public methods

        //###########################################################

        #region protected methods

        protected override void Activate()
        {
            Debug.LogFormat("Door \"{0}\": Activate called!", name);

            if (currentState < offsets.Count - 1)
            {
                beforeLocalPosition = startPosition + offsets[currentState];
                currentState++;
                afterLocalPosition = startPosition + offsets[currentState];
                Move(beforeLocalPosition, afterLocalPosition);
            }

        }

        protected override void Deactivate()
        {
            Debug.LogFormat("Door \"{0}\": Deactivate called!", name);

            if (currentState > 0)
            {
                beforeLocalPosition = startPosition + offsets[currentState];
                currentState--;
                afterLocalPosition = startPosition + offsets[currentState];
                Move(beforeLocalPosition, afterLocalPosition);
            }
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