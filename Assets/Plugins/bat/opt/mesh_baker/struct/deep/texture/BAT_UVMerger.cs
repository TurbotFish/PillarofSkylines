using bat.util;
using System.Collections.Generic;
using UnityEngine;

namespace bat.opt.bake.util
{

    /// <summary>
    /// Store the uvs and and  a list of TexturePacker corresponding current uvs. 
    /// uvs could be uv0 , uv1 or uv2.
    /// </summary>
    public class BAT_UVBaker
    {
        //TexturePakckers for these uvs
        public List<BAT_TexturePacker> m_texturePakcers = new List<BAT_TexturePacker>();
        //All calculated new uvs for this group.the list's count==Group's Count
        public List<Vector2[]> m_uvsRecalculated = new List<Vector2[]>();
        /// <summary>
        /// get the TexturePacker by id.
        /// </summary>
        /// <param name="id">id of TexturePacker</param>
        /// <returns></returns>
        public BAT_TexturePacker GetTexturePakcer(int id)
        {
            if (id < 0)
            {
                return null;
            }
            int countNeed = id + 1;
            if (m_texturePakcers.Count < countNeed)
            {
                for (int i = m_texturePakcers.Count; i < countNeed; i++)
                {
                    BAT_TexturePacker pt = new BAT_TexturePacker();
                    m_texturePakcers.Add(pt);
                }
            }
            return m_texturePakcers[id];
        }

        public void Bake(BAT_UVConfig config,List<BAT_BakeUnit> m_BakeUnits,Material material)
        {
            int maxTextureSize = config.m_maxTextureSize;
            //find the BAT_TexturePacker whick has the max texture count
            int maxTexCountID = -1;
            int maxTexCount=0;
            BAT_TexturePacker maxTexPacker = null;
            for (int i = 0; i < m_texturePakcers.Count; i++)
            {
                //fin the id of max texture count 
                var countI=m_texturePakcers[i].m_textures.Count;
                if ( countI> maxTexCount)
                {
                    maxTexCountID = i;
                    maxTexCount = countI;
                    maxTexPacker = m_texturePakcers[maxTexCountID];
                }
                //set the isNormal value to texturePakcer
                m_texturePakcers[i].m_isNormal = config.m_uvTxtures[i].m_isNormal;
            }

            //pack the the max texture count BAT_TexturePacker and recalculate the uvs by it
            if(maxTexPacker!=null)
            {
                maxTexPacker.PackTextures(maxTextureSize);
                maxTexPacker.RecalculateUVs(m_BakeUnits,m_uvsRecalculated);
                 //set texture into the material
                material.SetTexture(config.m_uvTxtureShaderIDs[maxTexCountID], maxTexPacker.m_packedTexture);
            }
            //pack all other texture pakcers,it must share the uvs.so need got the same pack regions.
            if (maxTexPacker != null)
            {
                for (int i = 0; i < m_texturePakcers.Count; i++)
                {
                    if (maxTexCountID == i)
                    {
                        continue;
                    }
                    var tx = maxTexPacker.m_packedTexture;
                    var size = new Vector2(tx.width, tx.height);
                    var texelRegion = maxTexPacker.m_packedTexelRegions;
                    var pixelRegion = maxTexPacker.m_packedPixelRegions;
                    m_texturePakcers[i].PackTextures(size, texelRegion, pixelRegion);
                    //set texture into the material
                    material.SetTexture(config.m_uvTxtureShaderIDs[i], m_texturePakcers[i].m_packedTexture);
                }
            }

        }
    }
}
