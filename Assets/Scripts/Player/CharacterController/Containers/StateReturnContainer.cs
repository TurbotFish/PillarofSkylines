using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController.Containers
{
    public struct StateReturnContainer
    {
        //acceleration
        Vector3 acceleration;
        public bool AccelerationSet { get; private set; }
        public Vector3 Acceleration { get { return acceleration; } set { acceleration = value; AccelerationSet = true; } }

        //transition speed
        float transitionSpeed;
        public bool TransitionSpeedSet { get; private set; }
        public float TransitionSpeed { get { return transitionSpeed; } set { transitionSpeed = value; TransitionSpeedSet = true; } }

        //max speed
        float maxSpeed;
        public bool MaxSpeedSet { get; private set; }
        public float MaxSpeed { get { return maxSpeed; } set { maxSpeed = value; MaxSpeedSet = true; } }

        //can turn player
        bool canTurnPlayer;
        public bool CanTurnPlayerSet { get; private set; }
        public bool CanTurnPlayer { get { return canTurnPlayer; } set { canTurnPlayer = value; CanTurnPlayerSet = true; } }

        //player forward
        Vector3 playerForward;
        public bool PlayerForwardSet { get; private set; }
        public Vector3 PlayerForward { get { return playerForward; } set { playerForward = value; PlayerForwardSet = true; } }

        //player up
        Vector3 playerUp;
        public bool PlayerUpSet { get; private set; }
        public Vector3 PlayerUp { get { return playerUp; } set { playerUp = value; PlayerUpSet = true; } }

        //gravity
        public bool IgnoreGravity { get; set; }
        
        //gravity multiplier
        float gravityMultiplier;
        public bool GravityMultiplierSet { get; private set; }
        public float GravityMultiplier { get { return gravityMultiplier; } set { gravityMultiplier = value; GravityMultiplierSet = true; } }

        //reset vertical velocity
        public bool resetVerticalVelocity { get; set; }

        //keep vertical movement
        public bool keepVerticalMovement { get; set; }

        //rotation
        float rotation;
        public bool RotationSet { get; private set; }
        public float Rotation { get { return rotation; } set { rotation = value; RotationSet = true; } }
    }
} //end of namespace