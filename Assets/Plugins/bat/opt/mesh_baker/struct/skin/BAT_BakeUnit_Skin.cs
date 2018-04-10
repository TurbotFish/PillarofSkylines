using bat.util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace bat.opt.bake.util
{
    public class BAT_BakeUnit_Skin : BAT_BakeUnit
    {
        public Material m_material;
        private SkinnedMeshRenderer m_skinnedMeshRenderer;
        public SkinnedMeshRenderer SkinnedMeshRenderer
        {
            get
            {
                return m_skinnedMeshRenderer;
            }
        }

        public BAT_BakeUnit SetRenderer(SkinnedMeshRenderer renderer)
        {
            m_skinnedMeshRenderer = renderer;
            m_material = renderer.sharedMaterial;
            return this;
        }
    }
}
