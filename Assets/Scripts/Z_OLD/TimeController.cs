//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Game.GameControl
//{
//    public class TimeController : MonoBehaviour
//    {
//        //###########################################################

//        // -- ATTRIBUTES

//        [SerializeField] float transitionTimeSlowDown = 0.2f;
//        [SerializeField] float transitionTimeSpeedUp = 0.2f;
//        [SerializeField] float timescaleMenuOpen = 0.1f;

//        bool isSlowingDownTime = false;
//        bool isSpeedingUpTime = false;

//        //###########################################################

//        // -- INITIALIZATION

//        void Start()
//        {
//            Utilities.EventManager.OnMenuSwitchedEvent += OnMenuOpenedEventHandler;
//        }

//        private void OnDestroy()
//        {
//            Utilities.EventManager.OnMenuSwitchedEvent -= OnMenuOpenedEventHandler;
//        }

//        //###########################################################

//        // -- OPERATIONS

//        void OnValidate()
//        {
//            if (this.timescaleMenuOpen <= 0.001f)
//            {
//                this.timescaleMenuOpen = 0.001f;
//            }
//        }

//        void OnMenuOpenedEventHandler(object sender, Utilities.EventManager.OnMenuSwitchedEventArgs args)
//        {
//            //slow down time
//            if (args.NewUiState != UI.MenuType.HUD && args.PreviousUiState == UI.MenuType.HUD)
//            {
//                if (this.isSlowingDownTime)
//                {
//                    return;
//                }

//                if (this.isSpeedingUpTime)
//                {
//                    StopAllCoroutines();
//                    this.isSpeedingUpTime = false;
//                }

//                StartCoroutine(ChangeTimescaleRoutine(this.timescaleMenuOpen, this.transitionTimeSlowDown));
//            }
//            //speed up time
//            else if (args.NewUiState == UI.MenuType.HUD && args.PreviousUiState != UI.MenuType.HUD)
//            {
//                if (this.isSpeedingUpTime)
//                {
//                    return;
//                }

//                if (this.isSlowingDownTime)
//                {
//                    StopAllCoroutines();
//                    this.isSlowingDownTime = false;
//                }

//                StartCoroutine(ChangeTimescaleRoutine(1f, this.transitionTimeSpeedUp));
//            }
//        }

//        IEnumerator ChangeTimescaleRoutine(float targetValue, float duration)
//        {
//            float initialValue = Time.timeScale;

//            if (targetValue < initialValue)
//            {
//                this.isSlowingDownTime = true;
//            }
//            else if (targetValue > initialValue)
//            {
//                this.isSpeedingUpTime = true;
//            }
//            else
//            {
//                yield break;
//            }

//            float changePerSecond = (targetValue - initialValue) / duration;

//            while (Time.timeScale != targetValue)
//            {
//                float newTimeScale = Time.timeScale + (Time.deltaTime * changePerSecond);

//                if (this.isSlowingDownTime && newTimeScale <= targetValue)
//                {
//                    newTimeScale = targetValue;
//                }
//                else if (this.isSpeedingUpTime && newTimeScale >= targetValue)
//                {
//                    newTimeScale = targetValue;
//                }

//                Time.timeScale = newTimeScale;

//                yield return null;
//            }

//            if (targetValue < initialValue)
//            {
//                this.isSlowingDownTime = false;
//            }
//            else if (targetValue > initialValue)
//            {
//                this.isSpeedingUpTime = false;
//            }
//        }
//    }
//} // end of namespace