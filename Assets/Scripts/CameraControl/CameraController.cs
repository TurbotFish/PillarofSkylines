using UnityEngine;

namespace Game.CameraControl
{
    public class CameraController : MonoBehaviour
    {
        public PoS_Camera PoS_Camera { get; private set; }
        public EchoSystem.EchoCameraEffect EchoCameraEffect { get; private set; }
        public Eclipse EclipseEffect { get; private set; }
        public GPUIDisplayManager GPUIDisplayManager { get; private set; }

        public void InitializeCameraController(GameControl.IGameControllerBase gameController)
        {
            PoS_Camera = GetComponent<PoS_Camera>();
            EchoCameraEffect = GetComponent<EchoSystem.EchoCameraEffect>();
            EclipseEffect = GetComponent<Eclipse>();
            GPUIDisplayManager = GetComponentInChildren<GPUIDisplayManager>();
            
            //this.PoS_Camera.InitializePoS_Camera();
            //this.EchoCameraEffect.InitializeEchoCameraEffect();
        }
    }
} //end of namespace