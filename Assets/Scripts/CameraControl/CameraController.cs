using Game.EchoSystem;
using Game.GameControl;
using UnityEngine;

namespace Game.CameraControl
{
    public class CameraController : MonoBehaviour
    {
        //########################################################################

        // -- ATTRIBUTES

        public PoS_Camera PoS_Camera { get; private set; }
        public EchoCameraEffect EchoCameraEffect { get; private set; }
        public Eclipse EclipseEffect { get; private set; }
        public GPUIDisplayManager GPUIDisplayManager { get; private set; }

        //########################################################################

        // -- INITIALIZATION

        public void InitializeCameraController(GameController gameController)
        {
            PoS_Camera = GetComponent<PoS_Camera>();
            EchoCameraEffect = GetComponent<EchoCameraEffect>();
            EclipseEffect = GetComponent<Eclipse>();
            GPUIDisplayManager = GetComponentInChildren<GPUIDisplayManager>();
            
            PoS_Camera.Initialize(gameController);
            GPUIDisplayManager.Initialize(gameController);
        }

        //########################################################################

        // -- OPERATIONS

        public void HandleInput()
        {
            PoS_Camera.HandleInput();
        }
    }
} // end of namespace