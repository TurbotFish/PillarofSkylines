using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utilities
{
    public static class PillarMath
    {
        public static List<Vector3> GetBoxColliderCorners(BoxCollider collider)
        {
            var result = new List<Vector3>
            {
                collider.center + new Vector3(collider.size.x, -collider.size.y, collider.size.z) * 0.5f,
                collider.center + new Vector3(-collider.size.x, -collider.size.y, collider.size.z) * 0.5f,
                collider.center + new Vector3(collider.size.x, -collider.size.y, -collider.size.z) * 0.5f,
                collider.center + new Vector3(-collider.size.x, -collider.size.y, -collider.size.z) * 0.5f,

                collider.center + new Vector3(collider.size.x, collider.size.y, collider.size.z) * 0.5f,
                collider.center + new Vector3(-collider.size.x, collider.size.y, collider.size.z) * 0.5f,
                collider.center + new Vector3(collider.size.x, collider.size.y, -collider.size.z) * 0.5f,
                collider.center + new Vector3(-collider.size.x, collider.size.y, -collider.size.z) * 0.5f
            };

            return result;
        }
    }
}