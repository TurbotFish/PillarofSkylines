using UnityEngine;
using System.Collections;
using Game.GameControl;

namespace Game.LevelElements
{
    [RequireComponent(typeof(BoxCollider))]
    public class RacingTrigger : Trigger
    {
        //###########################################################

        [SerializeField]
        Transform racer;

        [SerializeField]
        Transform target;

        [Space]

        [SerializeField]
        float timeToReachTarget = 5;

        [SerializeField]
        float timeActive = 1;

        [SerializeField]
        BezierSpline spline;

        bool racing;

        //###########################################################

        #region editor methods

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (!racer)
            {
                racer = transform;
            }

            if (!target && Targets[0])
            {
                target = Targets[0].transform;
            }
        }

#endif

        #endregion editor methods

        //###########################################################

        #region monobehaviour methods

        private void OnTriggerEnter(Collider other)
        {
            if (target && !racing)
            {
                StartCoroutine(Race());
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        #region public methods

        public override void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            base.Initialize(gameController, isCopy);

            GetComponent<BoxCollider>().isTrigger = true;
        }

        #endregion public methods

        //###########################################################

        #region private methods

        private IEnumerator Race()
        {
            Vector3 startPosition = racer.position;
            racing = true;

            for (float elapsed = 0; elapsed < timeToReachTarget; elapsed += Time.deltaTime)
            {
                float t = elapsed / timeToReachTarget;
                if (spline)
                {
                    racer.position = spline.GetPoint(t);
                    print("POSITION " + t + " ON SPLINE " + spline.GetPoint(t));
                }
                else
                    racer.position = Vector3.Lerp(startPosition, target.position, t);
                yield return null;
            }

            SetTriggerState(true);

            yield return new WaitForSeconds(timeActive);

            SetTriggerState(false);
            racer.position = startPosition;
            racing = false;
        }

        #endregion private methods

        //###########################################################
    }
} //end of namespace