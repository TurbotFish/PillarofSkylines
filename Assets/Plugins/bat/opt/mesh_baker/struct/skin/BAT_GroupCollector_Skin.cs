using bat.util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace bat.opt.bake.util
{

    /// <summary>
    /// SkinnedMesh baking group colloctor.
    /// Save diffrent of MeshFilters in table,they are grouped by dirrent material.
    /// Game objects using the same material and at the same hierarchy will be placed into the same group. 
    /// For the SkinnedMesh baking,only bake the meshes.(it dose not support multi-material baking)
    /// </summary>
    public class BAT_GroupCollector_Skin : BAT_Collecor<String, BAT_BakeGroup_Skin>
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
            List<SkinnedMeshRenderer> skinnedMeshRenderers = BAT_NodeUtil.ListAllInChildren<SkinnedMeshRenderer>(baker.transform);
            for (int i = 0; i < skinnedMeshRenderers.Count; i++)
            {
                SkinnedMeshRenderer meshRendererI = skinnedMeshRenderers[i];
                if (meshRendererI == null || meshRendererI.sharedMesh == null)
                {
                    continue;
                }
                var material = meshRendererI.sharedMaterial;
                if (material == null)
                {
                    continue;
                }
                //grab the hierarchy
                Animator animator = null;
                Animation animation = null;
                String hierarchy = getHierarchy(meshRendererI,out animator,out animation);
                if (hierarchy == null)
                {
                    continue;
                }
                //group by hierarchy
                BAT_BakeGroup_Skin group = null;
                if (m_groupTables.ContainsKey(hierarchy))
                {
                    group = m_groupTables[hierarchy];
                }
                else
                {
                    group = new BAT_BakeGroup_Skin();
                    group.m_hierarchy = hierarchy;
                    group.SetAnimHandler(animator, animation);
                    m_groupTables.Add(hierarchy, group);
                }
                //place into the group
                group.m_BakeUnits.Add(new BAT_BakeUnit_Skin().SetRenderer(meshRendererI));
            }
        }

        /// <summary>
        /// From the SkinnedMeshRenderer node,search the parents,if exist Animation or Aniamtor ,
        /// it should be its Anim-parent ,then calculate the relative path as the Hierarchy.
        /// If the Anim-parent dose not exist, the Hierarchy is null.
        /// </summary>
        /// <param name="meshRendererI"> SkinnedMeshRenderer object</param>
        /// <returns></returns>
        private string getHierarchy(SkinnedMeshRenderer meshRendererI, out Animator animator, out Animation animation)
        {
            String path = null;
            animation = GetComponentInParent<Animation>(meshRendererI.transform);
            String path_aniamtion = null;
            if (animation != null)
            {
                path_aniamtion = GetRelPath(meshRendererI.transform, animation.transform);
            }
            String path_animator = null;
            animator = GetComponentInParent<Animator>(meshRendererI.transform);
            if (animator != null)
            {
                path_animator = GetRelPath(meshRendererI.transform, animator.transform);
            }
            if (path_animator != null && path_aniamtion != null)
            {
                path = path_animator.Length > path_aniamtion.Length ? path_aniamtion : path_animator;
            }
            else
            {
                path = path_animator != null ? path_animator : path_aniamtion;
            }
            return path;
        }
        /// <summary>
        /// Get the ralative path duaring two gameObject,
        /// for example : A->B->C ,C is child and A is parent's parent,
        /// returns "A/B"
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns>relative path</returns>
        private static string GetRelPath(Transform child, Transform parent)
        {
            if (child.parent == null)
            {
                return "";
            }
            string name = child.parent.name;
            if (child.parent != parent)
            {
                name = GetRelPath(child.parent, parent) + "/" + name; 
            }
            return name;
        }

        private T GetComponentInParent<T>(Transform trans) where T : Component
        {
            if (trans != null)
            {
                var p = trans.parent;
                while (p != null)
                {
                    var t = p.GetComponent<T>();
                    if (t != null)
                    {
                        return t;
                    }
                    p = p.parent;
                }
            }
            return null;
        }
  

    }
 
}
