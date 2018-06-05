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

		[Header("Sound")]
		public bool playsSoundOnStart;
		public AudioClip startClip;
		public float minDistanceStart = 10f;
		public float maxDistanceStart = 50f;
		[Range(0,2)] public float volumeStart = 1f;
		public float clipDurationStart = 0f;
		public bool addRandomisationStart = false;
		public bool playsSoundOnEnd;
		public AudioClip endClip;
		public float minDistanceEnd = 10f;
		public float maxDistanceEnd = 50f;
		[Range(0,2)] public float volumeEnd = 1f;
		public float clipDurationEnd = 0f;
		public bool addRandomisationEnd = false;
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

			if(playsSoundOnStart)
				SoundifierOfTheWorld.PlaySoundAtLocation (startClip, transform, maxDistanceStart, volumeStart, minDistanceStart, clipDurationStart, addRandomisationStart);
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

			if (playsSoundOnEnd) 
				SoundifierOfTheWorld.PlaySoundAtLocation (endClip, transform, maxDistanceEnd, volumeEnd, minDistanceEnd, clipDurationEnd, addRandomisationEnd);
			
        }

        #endregion private methods

        //###########################################################
    }
} //end of namespace