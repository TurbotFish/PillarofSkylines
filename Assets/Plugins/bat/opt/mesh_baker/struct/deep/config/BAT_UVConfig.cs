using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using bat.util;

namespace bat.opt.bake.util
{
    [Serializable]
    public class BAT_UVConfig
    {
        [SerializeField][Tooltip("Max texture size allowed")]
        public int m_maxTextureSize = 1024;
        [SerializeField][Tooltip("Shader texture names for this uv")]
        public BAT_TextureProperty[] m_uvTxtures;
        [SerializeField][Tooltip("Shader texture ids for this uv")]
        public int[] m_uvTxtureShaderIDs;

        public void Init()
        {
            int count=0;
            if (m_uvTxtures != null)
            {
                count = m_uvTxtures.Length;
            }
            m_uvTxtureShaderIDs = new int[count];
            for (int i = 0; i < m_uvTxtureShaderIDs.Length; i++)
            {
                int id = Shader.PropertyToID(m_uvTxtures[i].m_textureName);
                m_uvTxtureShaderIDs[i]=id;
            }
        }

        public void Refresh(Shader shader)
        {
#if UNITY_EDITOR
            m_uvTxtures = BAT_TextureProperty.ListTextures(shader).ToArray();
#endif
            Init();
        }

        public int UVTextureNum
        {
            get
            {
                return m_uvTxtureShaderIDs.Length;
            }
        }
        public void setMaxTureSize(int size)
        {
            if (size <= 32)
            {
                size = 32;
            }
            else
            {
                double pow = Math.Log(size, 2);
                int powInt = (int)Math.Ceiling(pow);
                size = (int)Math.Pow(2, powInt);
            }
            m_maxTextureSize = size;
        }
    }
    [Serializable]
    public class BAT_TextureProperty
    {
        [SerializeField][Tooltip("Texture Name")]
        public string m_textureName;
        [SerializeField][Tooltip("Is Normal?")]
        public bool m_isNormal;
        public BAT_TextureProperty()
        {
        }
        public BAT_TextureProperty(string textureName,bool isNormal)
        {
            m_textureName = textureName;
            m_isNormal = isNormal;
        }
#if UNITY_EDITOR
        /// <summary>
        /// check the shader and try to list all the texture properties.
        /// </summary>
        /// <param name="shader">shader</param>
        /// <returns>Texture properties</returns>
        public static List<BAT_TextureProperty> ListTextures(Shader shader)
        {
            List<BAT_TextureProperty> list = new List<BAT_TextureProperty>();
            int propertyCount = UnityEditor.ShaderUtil.GetPropertyCount(shader);
            for (int i = 0; i < propertyCount; i++)
            {
                if (UnityEditor.ShaderUtil.GetPropertyType(shader, i) == UnityEditor.ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    string propertyName = UnityEditor.ShaderUtil.GetPropertyName(shader, i);
                    BAT_TextureProperty tp = new BAT_TextureProperty();
                    tp.m_textureName = propertyName;
                    string pLow = propertyName.ToLower();
                    tp.m_isNormal = pLow.Contains("normal") || pLow.Contains("bump");
                    list.Add(tp);
                }
            }
            return list;
        }
#endif
    }
}