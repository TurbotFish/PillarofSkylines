using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController.Containers
{
    public struct StateReturnContainer
    {
        //desired velocity
        Vector3 desiredVelocity;
        public bool DesiredVelocitySet { get; private set; }
        public Vector3 DesiredVelocity { get { return desiredVelocity; } set { desiredVelocity = value; DesiredVelocitySet = true; } }

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
    }
} //end of namespace