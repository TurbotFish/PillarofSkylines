using bat.util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace bat.opt.bake.util
{
    public class BAT_BakeUnit_Deep : BAT_BakeUnit_Mesh
    {
        //self id in baking group
        public int m_ID = -1;
        //SubMesh's hash id in mesh subMesh group
        public int m_hashID = -1;
        //uv of mesh after recalculated
        public Vector2[] m_uvs;
    }
}
