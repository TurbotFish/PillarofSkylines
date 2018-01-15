using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController
{
    public class WallRunState {




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
    }
}