using System;
using System.Collections.Generic;
using UnityEngine;

namespace bat.opt.bake.util
{
    public class BAT_BakeUnit_Mesh : BAT_BakeUnit
    {
        //MeshFilter of original
        private MeshFilter m_meshFilter;

        public MeshFilter MeshFilter
        {
            get
            {
                return m_meshFilter;
            }
        }
        public BAT_BakeUnit SetMeshFilter(MeshFilter meshFilter)
        {
            m_meshFilter = meshFilter;
            return this;
        }
    }
}
