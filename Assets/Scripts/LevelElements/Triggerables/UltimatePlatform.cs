using System.Collections;
using Game.GameControl;
using UnityEngine;
using System.Collections.Generic;
using Game.Model;

namespace Game.LevelElements
{
    [RequireComponent(typeof(MovingPlatform))]
    public class UltimatePlatform : TriggerableObject
    {
        //###########################################################

        [Header("Step By Step Movement")]

        [HideInInspector]
        public List<Vector3> localPositions;

        public float timeToMove = 1;
        public float easing = 1;
        public bool looping = true;

        [HideInInspector]
        public int currentPoint = 0;
        platformState currentState = platformState.newOrder;
        Transform my;
        bool goingForward = true;
        MovingPlatform platform;
        Vector3 initialPosition;
        float elapsed;

        //###########################################################

        #region public methods

        public override void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            base.Initialize(gameController, isCopy);

            //(PersistentDataObject as PersistentStepByStepTriggerable).State = currentState;

            my = transform;
            platform = GetComponent<MovingPlatform>();

            localPositions.Add(Vector3.zero);
            foreach (Transform child in transform)
            {
                localPositions.Add(new Vector3(child.localPosition.x * my.lossyScale.x, child.localPosition.y * my.lossyScale.y, child.localPosition.z * my.lossyScale.z));
            }

            initialPosition = my.localPosition;
            elapsed = timeToMove;

        }

        #endregion public methods

        //###########################################################

        #region protected methods

        protected override void Activate()
        {
            Debug.LogFormat("Door \"{0}\": Activate called!", name);
            
        }

        protected override void Deactivate()
        {
            Debug.LogFormat("Door \"{0}\": Deactivate called!", name);
            
        }

        /*protected override PersistentTriggerable CreatePersistentObject()
        {
            return new PersistentStepByStepTriggerable(this);
        }*/

        #endregion protected methods

        //###########################################################

        #region private methods

       private void Update()
        {
            if (!isInitialized)
                return;

            if (currentState == platformState.newOrder && Triggered)
            {
                Move(localPositions[currentPoint % (localPositions.Count)], localPositions[(currentPoint + (goingForward ? 1 : -1)) % (localPositions.Count)]);

                print("moving order : " + localPositions[currentPoint % (localPositions.Count)] + " to " + localPositions[(currentPoint + (goingForward? 1:-1)) % (localPositions.Count)]);
                
                currentPoint += (goingForward?1:-1);
                if (currentPoint == localPositions.Count && looping)
                {
                    currentPoint = 0;
                }
                if (currentPoint == localPositions.Count-1 && !looping)
                {
                    goingForward = false;
                }
                if (currentPoint == 0 && !looping)
                {
                    goingForward = true;
                }
                currentState = platformState.moving;
            }
        }

        private void Move(Vector3 startPos, Vector3 endPos)
        { 
            StopAllCoroutines();
            StartCoroutine(_Move(initialPosition + startPos, initialPosition + endPos));
        }

        private IEnumerator _Move(Vector3 startPos, Vector3 endPos)
        {
            elapsed = timeToMove - elapsed;
            while ( elapsed < timeToMove)
            {
                if (Triggered)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / timeToMove;

                    Vector3 movement = Vector3.Lerp(startPos, endPos, (Mathf.Pow(t, easing) / (Mathf.Pow(t, easing) + Mathf.Pow(1 - t, easing)))) - my.localPosition;
                    platform.Move(movement);
                }
                yield return null;
            }

            my.localPosition = endPos;
            elapsed = timeToMove;
            currentState = platformState.newOrder;
        }

        #endregion private methods

        //###########################################################
    }

    enum platformState
    {
        moving,
        newOrder,
        waiting,
        disabled
    }
} //end of namespace