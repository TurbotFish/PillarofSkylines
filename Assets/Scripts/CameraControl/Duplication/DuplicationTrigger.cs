using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CameraControl
{
    public class DuplicationTrigger : MonoBehaviour
    {
        //###############################################################

        // -- CONSTANTS

        [SerializeField] private DuplicationAxis _DuplicationAxis;

        //###############################################################

        // -- ATTRIBUTES

        public bool IsVisible { get; private set; }

        private DuplicationCameraManager DuplicationCameraManager;

        //###############################################################

        // -- INITIALIZATION

        public void Initialize(DuplicationCameraManager duplication_camera_manager)
        {
            DuplicationCameraManager = duplication_camera_manager;
        }

        //###############################################################

        // -- INQUIRIES

        public DuplicationAxis DuplicationAxis { get { return _DuplicationAxis; } }

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