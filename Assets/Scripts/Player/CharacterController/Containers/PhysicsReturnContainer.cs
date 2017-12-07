using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController.Containers
{
    public struct PhysicsReturnContainer
    {
        //final velocity
        Vector3 finalVelocity;
        public bool FinalVelocitySet { get; private set; }
        public Vector3 FinalVelocity { get { return finalVelocity; } set { finalVelocity = value; FinalVelocitySet = true; } }
    }
}