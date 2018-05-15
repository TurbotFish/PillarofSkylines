using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CameraControl
{
    public class DuplicationPlane : MonoBehaviour
    {
        public bool IsVisible { get; private set; }

        private void OnBecameVisible()
        {
            //Debug.LogError("Became visible!");

            IsVisible = true;
        }

        private void OnBecameInvisible()
        {
            //Debug.LogError("Became invisible!");

            IsVisible = false;
        }
    }
} // end of namespace