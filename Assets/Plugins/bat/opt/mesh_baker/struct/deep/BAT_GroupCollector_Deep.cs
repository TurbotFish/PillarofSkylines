using bat.util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace bat.opt.bake.util
{

    /// <summary>
    /// Deep baking group colloctor.
    /// Save diffrent of MeshFilters in table,they are grouped by dirrent shader.
    /// Game objects using the same shader will be placed into the same group. 
    /// For the deep merging,Bake the meshes, material and textures.
    /// </summary>
    public class BAT_GroupCollector_Deep : BAT_Collecor<Shader,BAT_BakeGroup_Deep>
    {

        /// <summary>
        /// Collect MeshFilters by diffrent shader in all MeshFilters of target Children 
        /// </summary>
        /// <param name="deepBaker">target deepBaker</param>
        public override void Collect(BAT_BakerBase baker)
        {
            //all the MeshFilter
            List<MeshFilter> meshFilters = BAT_NodeUtil.ListAllInChildren<MeshFilter>(baker.transform);
            for (int i = 0; i < meshFilters.Count; i++)
            {
                MeshFilter meshFilterI = meshFilters[i];
                if (meshFilterI == null || meshFilterI.mesh == null)
                {
                    continue;
                }
                //check if exist MeshRenderer on the MeshFilter
                MeshRenderer meshRendererI = meshFilterI.GetComponent<MeshRenderer>();
                if (meshRendererI == null)
                {
                    continue;
                }
                Material[] materials = meshRendererI.sharedMaterials;
                for (int subID = 0; subID < materials.Length; subID++)
                {
                    var material = materials[subID];
                    if (material == null)
                    {
                        continue;
                    }
                    Shader shader_i = material.shader;
                    if (material == null)
                    {
                        continue;
                    }
                    //grop by main material
                    BAT_BakeGroup_Deep group = null;
                    if (m_groupTables.ContainsKey(shader_i))
                    {
                        group = m_groupTables[shader_i];
                    }
                    else
                    {
                        group = new BAT_BakeGroup_Deep();
                        group.m_shader = shader_i;
                        group.SetBakeConfig(((BAT_DeepBaker)baker).FindConfig(shader_i));
                        m_groupTables.Add(shader_i, group);
                    }
                    //place into the group
                    group.CollectMeshFilter(meshFilterI, material, subID);
                }
                
            }
        }


    }
  


   
}
