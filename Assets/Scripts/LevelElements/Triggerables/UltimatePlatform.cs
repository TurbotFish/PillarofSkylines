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


        [HideInInspector]
        public List<Vector3> waypoints;
        [Header("Step By Step Movement")]
        public List<float> waitTime;

        public List<float> timeToMove;
        public float easing = 1;
        public float initialWaitTime;
        public bool looping = true;
        public bool finishMovement = true;

        [HideInInspector]
        public int currentPoint = 0;
        eUltimatePlatformState currentState = eUltimatePlatformState.newOrder;
        Transform my;
        bool goingForward = true;
        bool finishingMovement;
        MovingPlatform platform;
        Vector3 initialPosition;
        float elapsed;
        bool starting = true;
        bool inCoroutine = false;

        [Space(50f)]
        public bool debug;

		[Header("Sound")]
		public bool playsSoundOnStart;
		public AudioClip startClip;
		public float minDistanceStart = 10f;
		public float maxDistanceStart = 50f;
		[Range(0,2)] public float volumeStart = 1f;
		public float clipDurationStart = 0f;
		public bool addRandomisationStart = false;

		GameController gController;

        //###########################################################

        #region public methods

        public override void Initialize(GameController gameController)
        {
            if (IsInitialized)
                return;

            base.Initialize(gameController);

            //(PersistentDataObject as PersistentStepByStepTriggerable).State = currentState;
            //Debug.Log("I am " + name);
            my = transform;
            platform = GetComponent<MovingPlatform>();

            waypoints.Add(Vector3.zero);

			gController = gameController;

            foreach (Transform child in transform.GetChild(0))
            {
                //Debug.Log("adding : " + child.name);
                waypoints.Add(my.localRotation * new Vector3(child.localPosition.x * my.lossyScale.x, child.localPosition.y * my.lossyScale.y, child.localPosition.z * my.lossyScale.z));
            }

            //Debug.Log("waypoints " + waypoints.Count);

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
            //Debug.LogFormat("Platform \"{0}\": Activate called!", name);
            
        }

        protected override void Deactivate()
        {
            //Debug.LogFormat("Platform \"{0}\": Deactivate called!", name);
            if (finishMovement)
            {
                finishingMovement = true;
            }
            else
            {

                currentState = eUltimatePlatformState.disabled;
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
            if (!IsInitialized)
                return;


            if (currentState == eUltimatePlatformState.newOrder && (Triggered || finishingMovement))
            {

                Move(waypoints[currentPoint % (waypoints.Count)], waypoints[(currentPoint + (goingForward ? 1 : -1)) % (waypoints.Count)], timeToMove[currentPoint]);

                //print("moving order : " + waypoints[currentPoint % (waypoints.Count)] + " to " + waypoints[(currentPoint + (goingForward? 1:-1)) % (waypoints.Count)]);
                
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
                currentState = eUltimatePlatformState.moving;
            }
            if (currentState == eUltimatePlatformState.disabled && !inCoroutine)
            {
                currentState = eUltimatePlatformState.newOrder;
            }
        }

        private void Move(Vector3 startPos, Vector3 endPos, float timeMoving)
        {
            elapsed = timeMoving;
            StopAllCoroutines();
            StartCoroutine(_Move(initialPosition + startPos, initialPosition + endPos, timeMoving));

			if (playsSoundOnStart) {
				Vector3 _playerToObject = transform.position - gController.PlayerController.transform.position;
				if(Vector3.Dot(_playerToObject, _playerToObject) < 150f)
					SoundifierOfTheWorld.PlaySoundAtLocation (startClip, transform, maxDistanceStart, volumeStart, minDistanceStart, clipDurationStart, addRandomisationStart);
				
			}
        }

        private IEnumerator _Move(Vector3 startPos, Vector3 endPos, float timeMoving)
        {
            inCoroutine = true;
            elapsed = timeMoving - elapsed;

			currentState = eUltimatePlatformState.waiting;

			if (waitTime.Count > currentPoint) {
				//Debug.Log ("GAGA   " +waitTime [currentPoint]);
				yield return new WaitForSeconds (waitTime [currentPoint] + (starting ? initialWaitTime : 0));
                starting = false;
			}

            while (elapsed < timeMoving)
            {
                if (Triggered || finishingMovement)
                {
                    currentState = eUltimatePlatformState.moving;
                    elapsed += Time.deltaTime;
                    float t = elapsed / timeMoving;

                    if (debug)
                        Debug.Log("hey " + t);
                        
                    Vector3 movement = Vector3.Lerp(startPos, endPos, (Mathf.Pow(t, easing) / (Mathf.Pow(t, easing) + (float.IsNaN(Mathf.Pow(1 - t, easing))?0: Mathf.Pow(1 - t, easing))))) - my.localPosition;
                    platform.Move(movement);
                }
                yield return null;
            }

            my.localPosition = endPos;

           

            if (finishingMovement && currentPoint == 0)
            {
                finishingMovement = false;
            }

            if (debug)
                Debug.Log("out");
            inCoroutine = false;
            currentState = eUltimatePlatformState.newOrder;
        }

        #endregion private methods
        
        //###########################################################
    }


    public enum eUltimatePlatformState
    {
        moving,
        newOrder,
        waiting,
        disabled
    }
} //end of namespace