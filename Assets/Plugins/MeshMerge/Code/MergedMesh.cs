using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.ootii.MeshMerge
{
    /// <summary>
    /// Container to store the mesh filter IDs that are used
    /// to create this mesh.
    /// </summary>
    public class MergedMesh : MonoBehaviour
    {
        /// <summary>
        /// IDs of the objects that were merged into this object
        /// </summary>
        public int[] MeshFilterIDs = null;
    }
}
