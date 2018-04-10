using System;
using UnityEngine;


namespace bat.util
{
    public static class QuaternionUtil
    {
        public static void RotateByLocalEuler(Transform _transform,Vector3  eulerAnglesPlus)
        {
            Vector3  eulerAngles = _transform.localEulerAngles;
            eulerAngles = eulerAngles + eulerAnglesPlus;
            _transform.localEulerAngles=eulerAngles;
        }
        public static void SetLocalEuler(Transform _transform, Vector3 eulerAngles)
        {
            var _localRotation = _transform.localRotation;
            _localRotation.eulerAngles = eulerAngles;
            _transform.localRotation = _localRotation;
        }
    }
}
