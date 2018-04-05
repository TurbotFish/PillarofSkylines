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

       // [HideInInspector]
        public List<Vector3> waypoints;
        public List<float> waitTime;

        public List<float> timeToMove;
        public float easing = 1;
        public bool looping = true;
        public bool finishMovement = true;

        [HideInInspector]
        public int currentPoint = 0;
        platformState currentState = platformState.newOrder;
        Transform my;
        bool goingForward = true;
        bool finishingMovement;
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

            waypoints.Add(Vector3.zero);
            foreach (Transform child in transform.GetChild(0))
            {
                waypoints.Add(new Vector3(child.localPosition.x * my.lossyScale.x, child.localPosition.y * my.lossyScale.y, child.localPosition.z * my.lossyScale.z));
            }

            if (waitTime.Count < waypoints.Count)
            {
                int size = waypoints.Count - waitTime.Count;
                for (int i = 0; i < size; i++)
                {
                    waitTime.Add(0f);
                }
            }

            if (timeToMove.Count < waypoints.Count)
            {
                int size = waypoints.Count - timeToMove.Count;
                for (int i = 0; i < size; i++)
                {
                    timeToMove.Add(1f);
                }
            }

            initialPosition = my.localPosition;
            elapsed = timeToMove[0];

        }

        #endregion public methods

        //###########################################################

        #region protected methods

        protected override void Activate()
        {
            Debug.LogFormat("Platform \"{0}\": Activate called!", name);
            
        }

        protected override void Deactivate()
        {
            Debug.LogFormat("Platform \"{0}\": Deactivate called!", name);
            if (finishMovement)
            {
                finishingMovement = true;
            }
            else
            {
                currentState = platformState.disabled;
            }
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

            if (currentState == platformState.newOrder && (Triggered || finishingMovement))
            {

                Move(waypoints[currentPoint % (waypoints.Count)], waypoints[(currentPoint + (goingForward ? 1 : -1)) % (waypoints.Count)], timeToMove[currentPoint]);

                print("moving order : " + waypoints[currentPoint % (waypoints.Count)] + " to " + waypoints[(currentPoint + (goingForward? 1:-1)) % (waypoints.Count)]);
                
                currentPoint += (goingForward?1:-1);
                if (currentPoint == waypoints.Count && looping)
                {
                    currentPoint = 0;
                }
                if (currentPoint == waypoints.Count-1 && !looping)
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

        private void Move(Vector3 startPos, Vector3 endPos, float timeMoving)
        {
            elapsed = timeMoving;
            StopAllCoroutines();
            StartCoroutine(_Move(initialPosition + startPos, initialPosition + endPos, timeMoving));
        }

        private IEnumerator _Move(Vector3 startPos, Vector3 endPos, float timeMoving)
        {
            elapsed = timeMoving - elapsed;

			currentState = platformState.waiting;

			if (waitTime.Count > currentPoint)
				yield return new WaitForSeconds(waitTime[currentPoint]);


            while (elapsed < timeMoving)
            {
                if (Triggered || finishingMovement)
                {
                    currentState = platformState.moving;
                    elapsed += Time.deltaTime;
                    float t = elapsed / timeMoving;

                    Vector3 movement = Vector3.Lerp(startPos, endPos, (Mathf.Pow(t, easing) / (Mathf.Pow(t, easing) + Mathf.Pow(1 - t, easing)))) - my.localPosition;
                    platform.Move(movement);
                }
                yield return null;
            }

            my.localPosition = endPos;

           

            if (finishingMovement && currentPoint == 0)
            {
                finishingMovement = false;
            }

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