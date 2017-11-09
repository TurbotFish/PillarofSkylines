using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CameraControl
{
    public class CameraController : MonoBehaviour
    {
        public PoS_Camera PoS_Camera { get; private set; }
        public EchoSystem.EchoCameraEffect EchoCameraEffect { get; private set; }

        public void InitializeCameraController(GameControl.IGameControllerBase gameController)
        {
            this.PoS_Camera = GetComponent<PoS_Camera>();
            this.EchoCameraEffect = GetComponent<EchoSystem.EchoCameraEffect>();

            //this.PoS_Camera.InitializePoS_Camera();
            //this.EchoCameraEffect.InitializeEchoCameraEffect();
        }
    }
} //end of namespace