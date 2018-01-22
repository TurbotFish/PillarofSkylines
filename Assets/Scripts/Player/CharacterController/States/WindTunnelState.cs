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

            Vector3 input = charController.InputInfo.leftStickRaw;

            PlayerMovementInfo movementInfo = charController.MovementInfo;

            windTunnelPartList = charController.WindTunnelPartList;

            var partUp = Vector3.zero;
            var wind = Vector3.zero;
            float windAttraction = 0;
            if (windTunnelPartList.Count > 0)
            {
                foreach (var windTunnelPart in windTunnelPartList)
                {
                    partUp = windTunnelPart.MyTransform.up;
                    var partPos = windTunnelPart.MyTransform.position;
                    var center = (partPos + (charController.MyTransform.up * input.z + charController.MyTransform.right * input.x) 
                        * windTunnelPart.windTunnelParent.colliderRadius.Evaluate(windTunnelPart.idInTunnel/windTunnelPartList.Count) * (windTunnelPart.windTunnelParent.transform.lossyScale.x + 1));
                    wind += partUp * windTunnelPart.windStrength + Vector3.ProjectOnPlane(center - movementInfo.position, partUp) * windTunnelPart.tunnelAttraction;
                    windAttraction += windTunnelPart.tunnelAttraction;
                }
                wind /= windTunnelPartList.Count;
                windAttraction /= windTunnelPartList.Count;
            }

            var result = new StateReturnContainer
            {
                CanTurnPlayer = false,
                IgnoreGravity = true,

                PlayerForward = partUp,

                Acceleration = charController.TurnSpaceToLocal(wind),
                TransitionSpeed = 7f
            };


            return result;
        }

        //#############################################################################
    }
}