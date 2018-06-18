using System.Collections;
using Game.GameControl;
using UnityEngine;

namespace Game.LevelElements
{
    [RequireComponent(typeof(MovingPlatform))]
    public class Door : TriggerableObject
    {
        //###########################################################

        // -- CONSTANTS

        [Header("Door")]
        public Vector3 offsetWhenOpen;

        public float timeToMove = 1;
        public float comingBackMultiplicator = 1;

        [Header("Sound")]
        public bool playsSoundOnStart;
        public AudioClip startClip;
        public float minDistanceStart = 10f;
        public float maxDistanceStart = 50f;
        [Range(0, 2)] public float volumeStart = 1f;
        public float clipDurationStart = 0f;
        public bool addRandomisationStart = false;
        public bool playsSoundOnEnd;
        public AudioClip endClip;
        public float minDistanceEnd = 10f;
        public float maxDistanceEnd = 50f;
        [Range(0, 2)] public float volumeEnd = 1f;
        public float clipDurationEnd = 0f;
        public bool addRandomisationEnd = false;

        //###########################################################

        // -- ATTRIBUTES

        private Vector3 LocalPositionWhenOpen;
        private Vector3 LocalPositionWhenClosed;

        private Transform MyTransform; //use the property "MyTransform" instead of this! The property is guranteed to be initialized!
        private MovingPlatform MovingPlatform;
        private float Elapsed;
        private bool ComingBack = false;

        //###########################################################

        // -- INITIALIZATION

        public override void Initialize(GameController gameController)
        {
            base.Initialize(gameController);

            MovingPlatform = GetComponent<MovingPlatform>();

            if (Triggered)
            {
                LocalPositionWhenOpen = Transform.localPosition;
                LocalPositionWhenClosed = LocalPositionWhenOpen - offsetWhenOpen;
                Elapsed = 0;
            }
            else
            {
                LocalPositionWhenClosed = Transform.localPosition;
                LocalPositionWhenOpen = LocalPositionWhenClosed + offsetWhenOpen;
                Elapsed = timeToMove;
            }
        }

        //###########################################################

        // -- INQUIRIES

        public Transform Transform
        {
            get
            {
                if (MyTransform == null)
                {
                    MyTransform = transform;
                }

                return MyTransform;
            }
        }

        //###########################################################

        // -- OPERATIONS

        #region protected methods

        protected override void Activate()
        {
            //Debug.LogFormat("Door \"{0}\": Activate called!", name);
            ComingBack = false;
            //LocalPositionWhenOpen = LocalPositionWhenClosed + offsetWhenOpen;
            Move(LocalPositionWhenClosed, LocalPositionWhenOpen);
        }

        protected override void Deactivate()
        {
            //Debug.LogFormat("Door \"{0}\": Deactivate called!", name);
            ComingBack = true;
            //LocalPositionWhenClosed = LocalPositionWhenOpen - offsetWhenOpen;
            Move(LocalPositionWhenOpen, LocalPositionWhenClosed);
        }

        #endregion protected methods

        //###########################################################

        #region private methods

        private void Move(Vector3 startPos, Vector3 endPos)
        {
            StopAllCoroutines();
            StartCoroutine(_Move(startPos, endPos));

            if (playsSoundOnStart)
            {
                SoundifierOfTheWorld.PlaySoundAtLocation(startClip, transform, maxDistanceStart, volumeStart, minDistanceStart, clipDurationStart, addRandomisationStart);
            }
        }

        private IEnumerator _Move(Vector3 startPos, Vector3 endPos)
        {
            //Debug.LogErrorFormat("Door {0}: _Move: startPos={1} endPos={2}", this.name, startPos, endPos);

            for (Elapsed = timeToMove - Elapsed; Elapsed < timeToMove; Elapsed += Time.deltaTime * (ComingBack ? comingBackMultiplicator : 1))
            {
                float t = Elapsed / timeToMove;

                Vector3 movement = Vector3.Lerp(startPos, endPos, t) - Transform.localPosition;
                MovingPlatform.Move(movement);
                yield return null;
            }

            //Debug.LogErrorFormat("Door {0}: _Move: startPos={1} endPos={2}", this.name, startPos, endPos);
            Transform.localPosition = endPos;

            if (playsSoundOnEnd)
            {
                SoundifierOfTheWorld.PlaySoundAtLocation(endClip, transform, maxDistanceEnd, volumeEnd, minDistanceEnd, clipDurationEnd, addRandomisationEnd);
            }
        }

        #endregion private methods

        //###########################################################
    }
} //end of namespace