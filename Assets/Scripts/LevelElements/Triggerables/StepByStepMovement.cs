using System.Collections;
using Game.GameControl;
using UnityEngine;
using System.Collections.Generic;
using Game.Model;

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

        [HideInInspector]
        public int currentState = 0;
        Transform my;
        MovingPlatform platform;
        float elapsed;

        //###########################################################

        #region public methods

        public override void Initialize(GameController gameController)
        {
            base.Initialize(gameController);

            (PersistentDataObject as StepByStepTriggerablePersistentData).State = currentState;

            my = transform;
            platform = GetComponent<MovingPlatform>();

            startPosition = my.localPosition;
            elapsed = timeToMove;

        }

        public override void SetTriggered(bool triggered, bool initializing)
        {
            //print("am trigered : " + name + " state : " + currentState);
            
            if (initializing)
            {
                //print("hey i do nothing i'm being initialized lol");
                currentState = (PersistentDataObject as StepByStepTriggerablePersistentData).State;
                transform.localPosition = startPosition + offsets[currentState];
            }
            else
            {
                ChangeLevel(triggered);
            }
        }

        #endregion public methods

        //###########################################################

        #region protected methods

        protected override void Activate()
        {
            Debug.LogFormat("Door \"{0}\": Activate called!", name);
            /*
            if (currentState < offsets.Count - 1)
            {
                beforeLocalPosition = startPosition + offsets[currentState];
                currentState++;
                (PersistentDataObject as PersistentStepByStepTriggerable).State++;
                afterLocalPosition = startPosition + offsets[currentState];
                Move(beforeLocalPosition, afterLocalPosition);
            }*/
        }

        protected override void Deactivate()
        {
            Debug.LogFormat("Door \"{0}\": Deactivate called!", name);
            /*
            if (currentState > 0)
            {
                beforeLocalPosition = startPosition + offsets[currentState];
                currentState--;
                (PersistentDataObject as PersistentStepByStepTriggerable).State--;
                afterLocalPosition = startPosition + offsets[currentState];
                Move(beforeLocalPosition, afterLocalPosition);
            }*/
        }

        protected override TriggerablePersistentData CreatePersistentObject()
        {
            return new StepByStepTriggerablePersistentData(this);
        }

        #endregion protected methods

        //###########################################################

        #region private methods

        private void ChangeLevel (bool up)
        {
            beforeLocalPosition = startPosition + offsets[currentState];
            if (up && currentState < offsets.Count - 1)
            {
                currentState++;
                (PersistentDataObject as StepByStepTriggerablePersistentData).State++;
            }
            else if (!up && currentState > 0)
            {
                currentState--;
                (PersistentDataObject as StepByStepTriggerablePersistentData).State--;
            }
            else
            {
                return;
            }
            afterLocalPosition = startPosition + offsets[currentState];
            Move(beforeLocalPosition, afterLocalPosition);
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

        #endregion private methods

        //###########################################################
    }
} //end of namespace