using System.Collections;
using System.Collections.Generic;
using Game.Player.CharacterController.Containers;
using Game.Player.CharacterController.EnterArgs;
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

            Utilities.EventManager.WindTunnelPartEnteredEvent += OnWindTunnelPartEnteredEventHandler;
            Utilities.EventManager.WindTunnelExitedEvent += OnWindTunnelPartExitedEventHandler;
        }

        //#############################################################################

        public void Enter(IEnterArgs enterArgs)
        {
            Debug.Log("Enter State: Wind Tunnel");
        }

        public void Exit()
        {
            Debug.Log("Exit State: Wind Tunnel");
        }

        //#############################################################################

        public void HandleInput(PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {

        }

        public StateReturnContainer Update(float dt, PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
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

        void OnWindTunnelPartEnteredEventHandler(object sender, Utilities.EventManager.WindTunnelPartEnteredEventArgs args)
        {
            if (!windTunnelPartList.Contains(args.WindTunnelPart))
            {
                windTunnelPartList.Add(args.WindTunnelPart);
            }
        }

        void OnWindTunnelPartExitedEventHandler(object sender, Utilities.EventManager.WindTunnelPartExitedEventArgs args)
        {
            windTunnelPartList.Remove(args.WindTunnelPart);

            if (windTunnelPartList.Count == 0)
            {
                stateMachine.ChangeState(new FallEnterArgs(StateId));
            }
        }

        //#############################################################################
    }
}