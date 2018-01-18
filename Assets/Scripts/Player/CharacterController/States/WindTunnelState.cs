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

            windTunnelPartList = charController.WindTunnelPartList;

            var wind = Vector3.zero;
            Debug.Log("num parts : " + windTunnelPartList.Count);
            if (windTunnelPartList.Count > 0)
            {
                foreach (var windTunnelPart in windTunnelPartList)
                {
                    var partUp = windTunnelPart.MyTransform.up;
                    var partPos = windTunnelPart.MyTransform.position;

                    wind += partUp * windTunnelPart.windStrength + Vector3.ProjectOnPlane(partPos - movementInfo.position, partUp) * windTunnelPart.tunnelAttraction;
                    Debug.Log("added wind : " + wind);
                }
                wind /= windTunnelPartList.Count;
            }

            var result = new StateReturnContainer
            {
                CanTurnPlayer = false,
                IgnoreGravity = true,

                Acceleration = charController.TurnSpaceToLocal(wind),
                TransitionSpeed = 7f
            };

            Debug.Log("wind velocity : " + result.Acceleration);

            return result;
        }

        //#############################################################################
    }
}