using UnityEngine;

namespace Game.CameraControl
{
    public class CameraController : MonoBehaviour
    {
        public Camera Camera { get; private set; }
        public PoS_Camera PoS_Camera { get; private set; }
        public EchoSystem.EchoCameraEffect EchoCameraEffect { get; private set; }
        public Eclipse EclipseEffect { get; private set; }
        public GPUIDisplayManager GPUIDisplayManager { get; private set; }

        public void InitializeCameraController(GameControl.IGameController gameController)
        {
            Camera = GetComponent<Camera>();
            PoS_Camera = GetComponent<PoS_Camera>();
            EchoCameraEffect = GetComponent<EchoSystem.EchoCameraEffect>();
            EclipseEffect = GetComponent<Eclipse>();
            GPUIDisplayManager = GetComponentInChildren<GPUIDisplayManager>();
            
            PoS_Camera.Initialize(gameController);
            GPUIDisplayManager.Initialize(gameController);
        }
    }
} // end of namespace