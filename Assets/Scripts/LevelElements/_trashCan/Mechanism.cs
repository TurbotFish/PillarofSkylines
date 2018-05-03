//using UnityEngine;
//using System.Collections;

//namespace Game.LevelElements
//{
//    public class Mechanism : MonoBehaviour
//    {
//        [Header("Movement")]
//        public Vector3 movement;
//        public float timeToCompleteMovement = 1;
//        public AnimationCurve movementCurve;

//        [Header("Rotation")]
//        public Vector3 rotation;
//        public float timeToCompleteRotation = 1;
//        public AnimationCurve rotationCurve;

//        bool state = false;
//        Transform my;
//        Vector3 startPos, endPos, startRot, endRot;

//        private void Awake()
//        {
//            my = transform;
//            startPos = my.localPosition;
//            endPos = startPos + movement;
//            startRot = my.localEulerAngles;
//            endRot = my.localEulerAngles + rotation;
//        }

//        public void Trigger(bool newState)
//        {
//            StopAllCoroutines();

//            state = newState;
//            if (movement.sqrMagnitude != 0)
//                StartCoroutine(Move());
//            if (rotation.sqrMagnitude != 0)
//                StartCoroutine(Rotate());
//        }

//        IEnumerator Move()
//        {
//            Vector3 startPos = state ? this.startPos : this.endPos;
//            Vector3 endPos = state ? this.endPos : this.startPos;

//            float startValue = InverseLerp(startPos, endPos, my.localPosition) * timeToCompleteMovement;

//            for (float elapsed = startValue; elapsed < timeToCompleteMovement; elapsed += Time.deltaTime)
//            {
//                float t = elapsed / timeToCompleteMovement;
//                if (movementCurve.length > 0)
//                    t = movementCurve.Evaluate(t);
//                my.localPosition = Vector3.Lerp(startPos, endPos, t);
//                yield return null;
//            }
//            my.localPosition = endPos;
//        }

//        IEnumerator Rotate()
//        {
//            Vector3 startRot = state ? this.startRot : this.endRot;
//            Vector3 endRot = state ? this.endRot : this.startRot;

//            float startValue = InverseLerp(startRot, endRot, my.localEulerAngles) * timeToCompleteRotation;

//            for (float elapsed = startValue; elapsed < timeToCompleteRotation; elapsed += Time.deltaTime)
//            {
//                float t = elapsed / timeToCompleteRotation;
//                if (rotationCurve.length > 0)
//                    t = rotationCurve.Evaluate(t);
//                my.localEulerAngles = Vector3.Lerp(startRot, endRot, t);
//                yield return null;
//            }
//            my.localEulerAngles = endRot;
//        }

//        public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
//        {
//            Vector3 AB = b - a;
//            Vector3 AV = value - a;
//            return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
//        }
//    }
//} //end of namespace