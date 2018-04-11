using bat.util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace bat.opt.bake.util
{

    /// <summary>
    /// Mild baking group colloctor.
    /// Save diffrent of MeshFilters in table,they are grouped by dirrent material.
    /// Game objects using the same material will be placed into the same group. 
    /// For the mild baking,only bake the meshes.(it dose not support multi-material baking)
    /// </summary>
    public class BAT_GroupCollector_Mild : BAT_Collecor<Material,BAT_BakeGroup_Mild>
    {

        /// <summary>
        /// group MeshFilters by main material(sharedMaterial).
        /// if exsit two MeshFilter,using the same main meterial(sharedMaterial),
        /// but the diffrent sharedMaterials,it would be recognized as the same meterials ,
        /// use the first marked sharedMaterials.
        /// </summary>
        /// <param name="meshFilters">MeshFilter list</param>
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
                var materials = meshRendererI.sharedMaterials;
                if (materials == null)
                {
                    continue;
                }
                var mesh = meshFilterI.sharedMesh;
                int subMeshCount = mesh.subMeshCount;
                for (int j = 0; j < subMeshCount; j++)
                {
                    if (j >= materials.Length)
                    {
                        continue;
                    }
                    var material = materials[j];
                    //grop by main material
                    BAT_BakeGroup_Mild group = null;
                    if (m_groupTables.ContainsKey(material))
                    {
                        group = m_groupTables[material];
                    }
                    else
                    {
                        group = new BAT_BakeGroup_Mild();
                        group.m_sharedMaterial = material;
  
                        m_groupTables.Add(material, group);
                    }
                    var bakeUnit = new BAT_BakeUnit_Mild();
                    bakeUnit.SetMeshFilter(meshFilterI);
                    bakeUnit.m_subMeshID = j;
                    //place into the group
                    group.m_BakeUnits.Add(bakeUnit);
                }
   

            }
        }


    }
 
}
