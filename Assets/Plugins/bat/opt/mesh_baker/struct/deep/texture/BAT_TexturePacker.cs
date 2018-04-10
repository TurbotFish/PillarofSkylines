using bat.util;
using System.Collections.Generic;
using UnityEngine;

namespace bat.opt.bake.util
{

    /// <summary>
    /// Packed Texture for a group of MeshFilters using the same shader and the same uvs.
    /// And the packed texture could be Diffuse ,Normal and so on.
    /// </summary>
    public class BAT_TexturePacker
    {
        //Textures of this group originally used,the list's count<=Mesh Group's Count
        public List<Texture2D> m_textures = new List<Texture2D>();
        //Texture ID corresponding the meshFilters uvs,the list's count==Mesh Group's Count
        public List<int> m_textureIDs = new List<int>();

        //Texture pakced together from  originally used
        public Texture2D m_packedTexture = null;
        //Texture texel regions of original textures in current one
        public Rect[] m_packedTexelRegions = null;

        //Texture pixel regions of original textures in current one
        public Rect[] m_packedPixelRegions = null;

        //are current textures normal map?
        public bool m_isNormal = false;

        /// <summary>
        /// Collect textures correnspoinding the uvs(uv0、uv1 or uv2) of current MeshFilter group.
        /// </summary>
        /// <param name="texture"></param>
        public void AddTexture(Texture texture)
        {
            Texture2D tx = texture as Texture2D;
            int texID = -1;
            if (tx != null)
            {
                //check if the tecture exist
                texID = m_textures.IndexOf(tx);
                if (texID < 0)
                {
                    texID = m_textures.Count;
                    m_textures.Add(tx);
                }
            }
            m_textureIDs.Add(texID);
        }
        public void PackTextures(int maxTextureSize)
        {
            //pack all textures into one
            string s = "--------------packing Texture------------\n";
            foreach (var t in m_textures)
            {
                s += t.name + "\n";
            }
            Debug.Log(s);
            int maxSize = maxTextureSize;
            if (m_textures.Count > 1)
            {
                Texture2D[] toPackTextures = m_textures.ToArray();
                TextureFormat format = toPackTextures[0].format;
                m_packedTexture = new Texture2D(1, 1);
                BAT_BakerBase.Current.MarkAsset(m_packedTexture);
                //m_packedTexture.alphaIsTransparency = true;
                m_packedTexelRegions = m_packedTexture.PackTextures(toPackTextures, 0, maxSize);
                if (m_isNormal)
                {
                    Color[] pixels = BAT_GfxUtil.GetPixels(m_packedTexture);
                    NormalColor(pixels);
                    m_packedTexture.SetPixels(pixels);
                }
                //gen the mipmaps
                OptimizePackedTexture();
                Debug.Log(format+"->"+m_packedTexture.format);
            }
            else if (m_textures.Count == 1)
            {
                m_packedTexture = m_textures[0];
                m_packedTexelRegions = new Rect[1];
                m_packedTexelRegions[0] = new Rect(0, 0, 1, 1);

            }
            //account pixel regions
            m_packedPixelRegions = new Rect[m_packedTexelRegions.Length];
            for (int i = 0; i < m_packedPixelRegions.Length; i++)
            {
                float x = m_packedTexelRegions[i].x * m_packedTexture.width;
                float y = m_packedTexelRegions[i].y * m_packedTexture.height;
                float w = m_packedTexelRegions[i].width * m_packedTexture.width;
                float h = m_packedTexelRegions[i].height * m_packedTexture.height;
                Rect rectI = new Rect(x, y, w, h);
                m_packedPixelRegions[i] = rectI;
            }
        }
        public void RecalculateUVs(List<BAT_BakeUnit> m_BakeUnits, List<Vector2[]> m_uvs )
        {
            //use the table to save calculated result for using future
            Dictionary<int, Vector2[]> uvs_calculated = new Dictionary<int, Vector2[]>();
            //recalculate the uvs
            for (int i = 0; i < m_BakeUnits.Count; i++)
            {
                BAT_BakeUnit_Deep unitI = m_BakeUnits[i] as BAT_BakeUnit_Deep;
                Vector2[] uv = m_uvs[unitI.m_ID];
                int textureID = m_textureIDs[i];
                if (textureID >= m_packedTexelRegions.Length)
                {
                    Debug.LogError("--");
                }
                var rect = m_packedTexelRegions[textureID];
                //make key by diffrent mesh and texture
                int mesh_texture_key = unitI.m_ID * 1024 + textureID;

                Vector2[] uvNew = null;
                uvs_calculated.TryGetValue(mesh_texture_key, out uvNew);
                //if not exsit ,need recalculate the uv
                if (uvNew == null)
                {
                    uvNew = new Vector2[uv.Length];
                    for (int j = 0; j < uv.Length; j++)
                    {
                        uvNew[j].x = rect.xMin + (rect.xMax - rect.xMin) * uv[j].x;
                        uvNew[j].y = rect.yMin + (rect.yMax - rect.yMin) * uv[j].y;
                    }
                    uvs_calculated.Add(mesh_texture_key, uvNew);
                }
                //save the recalculated uv
                unitI.m_uvs = uvNew;
            }
        }
        /// <summary>
        /// pack this group of textures,need to sue the same regions with packedRegions list.
        /// then, it would got the same packed regions with the packedRegions,so it can share
        /// the same uvs with it.
        /// </summary>
        /// <param name="textureSize">the texture size</param>
        /// <param name="packedTexelRegions">need use the same texel regions</param>
        /// <param name="packedPixelRegions">need use the same pixel regions</param>
        public void PackTextures(Vector2 textureSize,Rect[] packedTexelRegions,Rect[] packedPixelRegions)
        {
            //pack all textures into one
            string s = "--------------packing Texture------------\n";
            foreach (var t in m_textures)
            {
                s += t.name + "\n";
            }
            Debug.Log(s);
            if (m_textures.Count > 1)
            {
                TextureFormat format = m_textures[0].format;
                m_packedTexture = new Texture2D((int)textureSize.x, (int)textureSize.y);//,TextureFormat.RGBA32,true
                BAT_BakerBase.Current.MarkAsset(m_packedTexture);
                //m_packedTexture.alphaIsTransparency = true;
                for (int i = 0; i < m_textures.Count; i++)
                {
                    var st = m_textures[i];
                    //TODO:could be optimized...
                    Color[] pixels = BAT_GfxUtil.GetPixels(st);
                    int sw = st.width;
                    int sh = st.height;
                    Rect regionTarget= packedPixelRegions[i];
                    int tw=(int)regionTarget.width;
                    int th=(int)regionTarget.height;
                    int tx=(int)regionTarget.x;
                    int ty=(int)regionTarget.y;
                    Color[] targetPixels = new Color[tw*th];
                    BAT_GfxUtil.ImageResize(pixels,sw,sh,targetPixels,tw,th);
                    if (m_isNormal)
                    {
                        NormalColor(targetPixels);
                    }
                    m_packedTexture.SetPixels(tx, ty, tw, th, targetPixels);
                }
                //gen the mipmaps
                OptimizePackedTexture();
                //juse save it
                m_packedTexelRegions = packedTexelRegions;
                m_packedPixelRegions = packedPixelRegions;
                Debug.Log(format+"->"+m_packedTexture.format);
            }
            else if (m_textures.Count == 1)
            {
                m_packedTexture = m_textures[0];
                m_packedTexelRegions = new Rect[1];
                m_packedTexelRegions[0] = new Rect(0, 0, 1, 1);
                m_packedPixelRegions = new Rect[1];
                m_packedPixelRegions[0] = new Rect(0, 0, m_packedTexture.width, m_packedTexture.height);
            }
        }

        protected void OptimizePackedTexture()
        {
            m_packedTexture.wrapMode = TextureWrapMode.Clamp;
            m_packedTexture.filterMode = FilterMode.Trilinear;
            m_packedTexture.Apply(true);
            m_packedTexture.Compress(true);
        }

        protected void NormalColor(Color[] colors)
        {
#if UNITY_ANDROID || UNITY_IOS
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i].a = colors[i].r;
            }
#endif
        }



    }
}
