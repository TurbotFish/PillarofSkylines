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

        //collision info
        CharacControllerRecu.CollisionInfo collisionInfo;
        public bool CollisionInfoSet { get; private set; }
        public CharacControllerRecu.CollisionInfo CollisionInfo { get { return collisionInfo; } set { collisionInfo = value; CollisionInfoSet = true; } }
    }
}