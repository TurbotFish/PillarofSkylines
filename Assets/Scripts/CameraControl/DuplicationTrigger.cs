using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CameraControl
{
    public class DuplicationTrigger : MonoBehaviour
    {
        public bool IsVisible { get; private set; }

        private void OnBecameVisible()
        {
            //Debug.LogErrorFormat("Trigger {0} became visible!", this.name);

            IsVisible = true;
        }

        private void OnBecameInvisible()
        {
            //Debug.LogErrorFormat("Trigger {0} became invisible!", this.name);

            IsVisible = false;
        }
    }
} // end of namespace