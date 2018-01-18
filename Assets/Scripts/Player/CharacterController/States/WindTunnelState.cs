using System.Collections;
using System.Collections.Generic;
using Game.Player.CharacterController.Containers;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class WindTunnelState : IState
    {
        //#############################################################################

        public ePlayerState StateId { get { return ePlayerState.windTunnel; } }

        CharController charController;
        StateMachine stateMachine;

        List<WindTunnelPart> windTunnelPartList = new List<WindTunnelPart>();

        //#############################################################################

        public WindTunnelState(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
        }

        //#############################################################################

        public void Enter()
        {
            Debug.Log("Enter State: Wind Tunnel");
        }

        public void Exit()
        {
            Debug.Log("Exit State: Wind Tunnel");
        }

        //#############################################################################

        public void HandleInput()
        {
            if (charController.WindTunnelPartList.Count == 0)
            {
                var state = new AirState(charController, stateMachine, AirState.eAirStateMode.fall);

                stateMachine.ChangeState(state);
            }
        }

        public StateReturnContainer Update(float dt)
        {
            PlayerMovementInfo movementInfo = charController.MovementInfo;

            var wind = Vector3.zero;

            if (windTunnelPartList.Count > 0)
            {
                foreach (var windTunnelPart in windTunnelPartList)
                {
                    var partUp = windTunnelPart.MyTransform.up;
                    var partPos = windTunnelPart.MyTransform.position;

                    wind += partUp * windTunnelPart.windStrength + Vector3.ProjectOnPlane(partPos - movementInfo.position, partUp) * windTunnelPart.tunnelAttraction;
                }
                wind /= windTunnelPartList.Count;
            }

            var result = new StateReturnContainer
            {
                CanTurnPlayer = true,
                IgnoreGravity = true,

                Acceleration = wind,
                TransitionSpeed = 0.7f
            };

            return result;
        }

        //#############################################################################
    }
}