using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController.Containers
{
    public struct PlayerMovementInfo
    {
        public Vector3 position;
        public Vector3 forward;
        public Vector3 up;
        public Vector3 side;

        public Vector3 velocity;
    }
}