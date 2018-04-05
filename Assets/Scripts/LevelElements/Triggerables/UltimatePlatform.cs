using System.Collections;
using Game.GameControl;
using UnityEngine;
using System.Collections.Generic;
using Game.Model;

namespace Game.LevelElements
{
    public enum eUltimatePlatformState
    {
        moving,
        waiting,
        disabled
    }

    [RequireComponent(typeof(MovingPlatform))]
    public class UltimatePlatform : TriggerableObject
    {
        //###########################################################

        [Header("Step By Step Movement")]
        public List<float> waitTime;
        public List<float> timeToMove;
        public float easing = 1;
        public bool looping = true;
        public bool finishMovement = true;

        private Transform myTransform;
        private MovingPlatform platform;
        private PersistentUltimatePlatform persistentUltimatePlatform;

        private List<Vector3> waypoints = new List<Vector3>();
        private Vector3 initialPosition;

        private int currentPoint = 0; //the starting point of the current segment
        private int nextPoint = 1; //the target point of the current segment
        private eUltimatePlatformState currentState = eUltimatePlatformState.waiting;
        private eUltimatePlatformState previousState = eUltimatePlatformState.waiting;
        private bool goingForward = true;
        private bool finishingMovement;
        private float elapsed;

        //###########################################################

        #region public methods

        public override void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            if (!isInitialized) //this is here to make sure its only called once in the editor version (where the GO is deactivated instead of being unloaded)
            {
                myTransform = transform;
                platform = GetComponent<MovingPlatform>();
                initialPosition = myTransform.localPosition;

                //filling waypoint list
                waypoints.Clear();
                waypoints.Add(Vector3.zero);

                foreach (Transform child in transform.GetChild(0))
                {
                    waypoints.Add(new Vector3(child.localPosition.x * myTransform.lossyScale.x, child.localPosition.y * myTransform.lossyScale.y, child.localPosition.z * myTransform.lossyScale.z));
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
            }

            base.Initialize(gameController, isCopy);

            //persistence
            persistentUltimatePlatform = PersistentDataObject as PersistentUltimatePlatform;

            if (persistentUltimatePlatform.MasterSetOnce)
            {
                myTransform.localPosition = persistentUltimatePlatform.LocalPosition;
                currentPoint = persistentUltimatePlatform.CurrentPoint;
                currentState = persistentUltimatePlatform.CurrentState;
                previousState = persistentUltimatePlatform.PreviousState;
                goingForward = persistentUltimatePlatform.GoingForward;
                finishingMovement = persistentUltimatePlatform.FinishingMovement;
                elapsed = persistentUltimatePlatform.Elapsed;
            }
        }

        #endregion public methods

        //###########################################################

        #region unity methods

        private void Update()
        {
            if (!isInitialized)
            {
                return;
            }

            if (persistentUltimatePlatform.Master == null)
            {
                persistentUltimatePlatform.Master = this;
                persistentUltimatePlatform.MasterSetOnce = true;
            }

            if (persistentUltimatePlatform.Master.Equals(this))
            {
                if (currentState != eUltimatePlatformState.disabled)
                {
                    elapsed += Time.deltaTime;
                }

                if (currentState == eUltimatePlatformState.waiting)
                {
                    if (elapsed >= waitTime[currentPoint] && (Triggered || finishingMovement))
                    {
                        elapsed = 0;
                        currentState = eUltimatePlatformState.moving;
                    }
                } //end of waiting state
                else if (currentState == eUltimatePlatformState.moving)
                {
                    if (elapsed >= timeToMove[currentPoint])
                    {
                        elapsed = 0;
                        myTransform.localPosition = initialPosition + waypoints[nextPoint];

                        //update "currentPoint"
                        currentPoint = nextPoint;

                        if (looping)
                        {
                            nextPoint++;
                            if (nextPoint == waypoints.Count)
                            {
                                nextPoint = 0;
                            }
                        }
                        else //not looping
                        {
                            nextPoint += goingForward ? 1 : -1;

                            if (nextPoint == waypoints.Count)
                            {
                                nextPoint -= 2; //same as currentPoint-1
                                goingForward = false;
                            }
                            else if (nextPoint == -1)
                            {
                                nextPoint = 1;
                                goingForward = true;
                            }
                        }

                        persistentUltimatePlatform.CurrentPoint = currentPoint;
                        persistentUltimatePlatform.NextPoint = nextPoint;

                        //update state
                        if (finishingMovement && currentPoint == 0)
                        {
                            currentState = eUltimatePlatformState.disabled;
                            finishingMovement = false;
                        }
                        else
                        {
                            currentState = eUltimatePlatformState.waiting;
                        }
                    }
                    else
                    {
                        float t = elapsed / timeToMove[currentPoint];
                        Vector3 lerp = Vector3.Lerp(
                            initialPosition + waypoints[currentPoint],
                            initialPosition + waypoints[nextPoint],
                            (Mathf.Pow(t, easing) / (Mathf.Pow(t, easing) + Mathf.Pow(1 - t, easing)))
                        );
                        Vector3 movement = lerp - myTransform.localPosition;
                        platform.Move(movement);
                    }
                } //end of moving state

                //persistence
                persistentUltimatePlatform.LocalPosition = myTransform.localPosition;
                persistentUltimatePlatform.CurrentPoint = currentPoint;
                persistentUltimatePlatform.NextPoint = nextPoint;
                persistentUltimatePlatform.GoingForward = goingForward;
                persistentUltimatePlatform.FinishingMovement = finishingMovement;
                persistentUltimatePlatform.Elapsed = elapsed;
            }
            else //instance is not the current master
            {
                myTransform.localPosition = persistentUltimatePlatform.LocalPosition;

                currentPoint = persistentUltimatePlatform.CurrentPoint;
                currentState = persistentUltimatePlatform.CurrentState;
                goingForward = persistentUltimatePlatform.GoingForward;
                finishingMovement = persistentUltimatePlatform.FinishingMovement;
                elapsed = persistentUltimatePlatform.Elapsed;
            }

            //éééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééé
            //if (currentState == eUltimatePlatformState.newOrder && (Triggered || finishingMovement))
            //{
            //    Move(waypoints[currentPoint % (waypoints.Count)], waypoints[(currentPoint + (goingForward ? 1 : -1)) % (waypoints.Count)], timeToMove[currentPoint]);

            //    print("moving order : " + waypoints[currentPoint % (waypoints.Count)] + " to " + waypoints[(currentPoint + (goingForward ? 1 : -1)) % (waypoints.Count)]);

            //    currentPoint += (goingForward ? 1 : -1);
            //    if (currentPoint == waypoints.Count && looping)
            //    {
            //        currentPoint = 0;
            //    }
            //    if (currentPoint == waypoints.Count - 1 && !looping)
            //    {
            //        goingForward = false;
            //    }
            //    if (currentPoint == 0 && !looping)
            //    {
            //        goingForward = true;
            //    }
            //    currentState = eUltimatePlatformState.moving;
            //}
            //éééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééé
        }

        private void OnDisable()
        {
            if (persistentUltimatePlatform != null && persistentUltimatePlatform.Master.Equals(this))
            {
                persistentUltimatePlatform.Master = null;
            }
        }

        #endregion unity methods

        //###########################################################

        #region protected methods

        protected override void Activate()
        {
            Debug.LogFormat("Platform \"{0}\": Activate called!", name);

            currentState = eUltimatePlatformState.moving;
            finishingMovement = false;
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
                currentState = eUltimatePlatformState.disabled;
            }
        }

        protected override PersistentTriggerable CreatePersistentObject()
        {
            return new PersistentUltimatePlatform(UniqueId, Triggered);
        }

        #endregion protected methods

        //###########################################################

        #region private methods        

        //private void Move(Vector3 startPos, Vector3 endPos, float timeMoving)
        //{
        //    elapsed = timeMoving;
        //    StopAllCoroutines();
        //    StartCoroutine(_Move(initialPosition + startPos, initialPosition + endPos, timeMoving));
        //}

        //private IEnumerator _Move(Vector3 startPos, Vector3 endPos, float timeMoving)
        //{
        //    elapsed = timeMoving - elapsed;
        //    while (elapsed < timeMoving)
        //    {
        //        if (Triggered || finishingMovement)
        //        {
        //            currentState = eUltimatePlatformState.moving;
        //            elapsed += Time.deltaTime;
        //            float t = elapsed / timeMoving;

        //            Vector3 movement = Vector3.Lerp(startPos, endPos, (Mathf.Pow(t, easing) / (Mathf.Pow(t, easing) + Mathf.Pow(1 - t, easing)))) - myTransform.localPosition;
        //            platform.Move(movement);
        //        }
        //        yield return null;
        //    }

        //    myTransform.localPosition = endPos;

        //    currentState = eUltimatePlatformState.waiting;

        //    if (waitTime.Count > currentPoint)
        //        yield return new WaitForSeconds(waitTime[currentPoint]);

        //    if (finishingMovement && currentPoint == 0)
        //    {
        //        finishingMovement = false;
        //    }

        //    currentState = eUltimatePlatformState.newOrder;
        //}

        #endregion private methods

        //###########################################################
    }
} //end of namespace