using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using bat.util;

namespace bat.opt.bake.util
{
    [Serializable]
    public class BAT_ShaderConfig
    {
        [SerializeField][Tooltip("Shared shader for Bake group")]
        private Shader m_shader;

        [SerializeField][Tooltip("Materials for Bake group")]
        public Material[] m_mateirals;

        [SerializeField][Tooltip("uv configs,uv0,uv1 adn uv2")]
        public BAT_UVConfig[] m_uvConfigs = new BAT_UVConfig[3];

        [SerializeField][Tooltip("show materials in editor")][HideInInspector]
        public bool m_editor_showMaterials = false;

        [SerializeField][Tooltip("uv id selected in editor ")][HideInInspector]
        public int m_editor_uvSel = 0;

        public BAT_ShaderConfig()
        {
            Init();
        }

        public void Init()
        {
            if (m_uvConfigs == null)
            {
                m_uvConfigs = new BAT_UVConfig[3];
            }
            for (int i = 0; i < m_uvConfigs.Length; i++)
            {
                if (m_uvConfigs[i] == null)
                {
                    m_uvConfigs[i] = new BAT_UVConfig();
                }
                m_uvConfigs[i].Init();
            }
        }

        public Shader Shader
        {
            set
            {
                if (m_shader != value)
                {
                    m_shader = value;
                    Refresh();
                }
            }
            get
            {
                return m_shader;
            }
        }

        public void Refresh()
        {
            //set all the texture properties to uv0 by default
            if(m_uvConfigs!=null && m_uvConfigs.Length>0)
            {
                m_uvConfigs[0].Refresh(Shader);
                //other uvs init
                for (int i = 1; i < m_uvConfigs.Length; i++)
                {
                    m_uvConfigs[i].Init();
                }
            }
        }


    }
}