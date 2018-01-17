using System.Collections;
using System.Collections.Generic;
using Game.Player.CharacterController.Containers;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class WallRunState : IState
    {
        //#############################################################################

        public ePlayerState StateId { get { return ePlayerState.wallRun; } }

        CharController charController;
        StateMachine stateMachine;
        CharData.WallRunData wallRunData;

        //#############################################################################

        public WallRunState(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            wallRunData = charController.CharData.WallRun;
        }

        //#############################################################################

        public void Enter()
        {
            Debug.Log("Enter State: Wall Run");

            //if the player should not be able to walldrift he starts falling again
            if (!CheckCanEnterWallRun(charController))
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine));
            }
        }

        public void Exit()
        {
            Debug.Log("Exit State: Wall Run");
        }

        //#############################################################################

        public void HandleInput()
        {
        }

        public StateReturnContainer Update(float dt)
        {
            var result = new StateReturnContainer()
            {

            };

            return result;
        }

        //#############################################################################

        /// <summary>
        /// Checks whether a horizontal wall run can be started.
        /// </summary>
        /// <returns></returns>
        public static bool CheckCanEnterWallRun(CharController charController)
        {
            bool isAbilityActive = charController.PlayerModel.CheckAbilityActive(eAbilityType.WallRun);

            bool isTouchingWall = charController.CollisionInfo.side;

            bool isJumping = charController.MovementInfo.velocity.y > 0;

            bool directionOK = CheckWallRunPlayerForward(charController, -0.95f);

            bool stickOK = CheckWallRunStick(charController);

            return (isAbilityActive && isTouchingWall && isJumping && directionOK && stickOK);
        }

        /// <summary>
        /// Checks if the player is turned towards the wall.
        /// </summary>
        public static bool CheckWallRunPlayerForward(CharController charController, float minDotProduct = -1)
        {
            float dotProduct = Vector3.Dot(charController.MyTransform.forward, charController.CollisionInfo.currentWallNormal);
            return (minDotProduct <= dotProduct && dotProduct <= charController.PlayerModel.AbilityData.WallRun.General.DirectionTrigger);
        }

        /// <summary>
        /// Checks whether the player is holding the left stick in position to activate or stay in wall run mode.
        /// </summary>
        public static bool CheckWallRunStick(CharController charController)
        {
            var stick = charController.InputInfo.leftStickToCamera;
            float dotproduct = Vector3.Dot(stick, charController.MyTransform.forward);
            return (dotproduct >= charController.PlayerModel.AbilityData.WallRun.General.StickTrigger && !charController.InputInfo.leftStickAtZero);
        }

        //#############################################################################
    }
}