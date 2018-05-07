using System.Collections.Generic;
/// <summary>
/// A group of MeshFilter using the same m_sharedMaterial(come from sharedMaterial).
/// Game objects using the same sharedMaterial will be placed into the same group.
/// </summary>
using UnityEngine;
namespace bat.opt.bake.util
{
    public class BAT_BakeGroup_Mild:BAT_BakeGroup
    {
        //main meterial
        public Material m_sharedMaterial;
    }
}