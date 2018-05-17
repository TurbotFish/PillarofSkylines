using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CameraControl
{
    public class DuplicationTrigger : MonoBehaviour
    {
        public bool IsVisible { get; private set; }

        //public bool isVisible;
        //public bool rendererVisible;

        //private Renderer myRenderer;

        //private void Start()
        //{
        //    myRenderer = GetComponent<Renderer>();
        //}

        private void OnBecameVisible()
        {
            //Debug.LogErrorFormat("Trigger {0} became visible!", this.name);

            IsVisible = true;
            //isVisible = true;
        }

        private void OnBecameInvisible()
        {
            //Debug.LogErrorFormat("Trigger {0} became invisible!", this.name);

            IsVisible = false;
            //isVisible = false;
        }

        //private void Update()
        //{
        //    rendererVisible = myRenderer.isVisible;
        //}
    }
} // end of namespace