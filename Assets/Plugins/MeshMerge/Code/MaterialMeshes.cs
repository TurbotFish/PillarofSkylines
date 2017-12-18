using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.ootii.MeshMerge
{
    /// <summary>
    /// Container for holding mesh filters that are tied to a 
    /// specific material.
    /// </summary>
    public class MaterialMeshes
    {
        public Material Material;

        public List<MeshFilter> MeshFilters = new List<MeshFilter>();

        public List<Mesh> Meshes = new List<Mesh>();

        public List<Matrix4x4> Transform = new List<Matrix4x4>();
    }
}
