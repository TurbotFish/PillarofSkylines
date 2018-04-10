using bat.util;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// A group of MeshFilter using the same m_sharedMaterial(come from sharedMaterial).
/// Game objects using the same hierarchy will be placed into the same group.
/// </summary>
using UnityEngine;
namespace bat.opt.bake.util
{
    public class BAT_BakeGroup_Skin:BAT_BakeGroup
    {
        //the hierarchy,the relative path from Anim-Parent
        public string m_hierarchy;

        //the animator
        private Animator m_animator;

        //the animation
        private Animation m_animation;

        private bool m_isPlaying;

        //the anim parent
        public Transform m_animParent;

        //the rootBone

        private Transform m_topRootBone;

        //save the units grouped by material in table
        private Dictionary<Material, BAT_BakeSubGroup_Skin> m_SubGroupTable = new Dictionary<Material, BAT_BakeSubGroup_Skin>();
        //save the units grouped by material in list
        private List<BAT_BakeSubGroup_Skin> m_SubGroupList = new List<BAT_BakeSubGroup_Skin>();
        public void generateSubGroups()
        {
            m_SubGroupTable.Clear();
            //group by material
            foreach (var unit in m_BakeUnits)
            {
                var unitS = (BAT_BakeUnit_Skin)unit;
                var material = unitS.m_material;
                BAT_BakeSubGroup_Skin subGroup = null;
                if (m_SubGroupTable.ContainsKey(material))
                {
                    subGroup = m_SubGroupTable[material];
                }
                else
                {
                    subGroup = new BAT_BakeSubGroup_Skin();
                    m_SubGroupTable.Add(material, subGroup);
                    subGroup.m_sharedMaterial = material;
                }
                subGroup.m_BakeUnits.Add(unit);
            }
            //fill into the list
            m_SubGroupList.Clear();
            foreach (var sub in m_SubGroupTable.Values)
            {
                m_SubGroupList.Add(sub);
            }
        }

        public Transform calculateTopRootBone()
        {
            Transform topRootBone = null;
            int depthOfTop=1000;
            foreach (var unit in m_BakeUnits)
            {
                var unitS = (BAT_BakeUnit_Skin)unit;
                var smr = unitS.SkinnedMeshRenderer;
                Transform rootBone = smr.rootBone;
                int depthI = BAT_NodeUtil.GameObjectDepth(rootBone);
                if (topRootBone == null || depthI < depthOfTop)
                {
                    topRootBone = rootBone;
                    depthOfTop = depthI;
                }
            }
            m_topRootBone = topRootBone;
            return m_topRootBone;
        }

        public Transform TopRootBone
        {
            get
            {
                return m_topRootBone;
            }
        }

        public Dictionary<Material, BAT_BakeSubGroup_Skin> SubGroupTable
        {
            get
            {
                return m_SubGroupTable;
            }
        }

        public List<BAT_BakeSubGroup_Skin> SubGroupList
        {
            get
            {
                return m_SubGroupList;
            }
        }
        public void SetAnimHandler(Animator animator, Animation animation)
        {
            m_animator = animator;
            m_animation = animation;
            if (m_animator != null)
            {
                m_isPlaying = m_animator.enabled;
                m_animParent = m_animator.transform;
            }
            else if (m_animation!=null)
            {
                m_isPlaying = m_animation.isPlaying;
                m_animParent = m_animation.transform;
            }
        }
        public IEnumerator OnBaked()
        {
            if (!m_isPlaying)
            {
                yield break;
            }
            //We need to restart the animation or animator

            //Debug.LogWarning("Frame:" + Time.frameCount);
            //AnimatorCullingMode mode = AnimatorCullingMode.AlwaysAnimate;
            //if (m_animator != null)
            //{
            //    mode = m_animator.cullingMode;
            //    m_animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            //}
            //else if (m_animation != null)
            //{
            //    m_animation.Play();
            //}

            //yield return null;
            //if (m_isPlaying)
            //{
            //    Debug.LogWarning("Frame:"+Time.frameCount);
            //    if (m_animator != null)
            //    {
            //        m_animator.cullingMode = mode;
            //    }
            //    else if (m_animation != null)
            //    {
            //        m_animation.Play();
            //    }
            //}
        }


    }
    public class BAT_BakeSubGroup_Skin : BAT_BakeGroup
    {
        //meterial
        public Material m_sharedMaterial;
        //meterials
        //public Material[] m_sharedMaterials;
    }
}