using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using bat.util;
using bat.opt.bake.util;

namespace bat.opt.bake
{
	public class BAT_SkinnedMeshBaker : BAT_BakerBase
    {
		[SerializeField][Tooltip("Remove originial ClearSkinnedMeshSetting after Baked")]
        public ClearSkinnedMeshSetting m_clearMeshSetting = ClearSkinnedMeshSetting.SkinnedRenderers;
        /// <summary>
        /// Bake all game objects under current GameObject,including meshes and materials.
        /// By default, baking will group the meshes by diffrent material and diffrent hierarchy
        /// </summary>
        protected override GameObject Bake()
        {
            //collect all groups
            BAT_GroupCollector_Skin BakeTable = new BAT_GroupCollector_Skin();
            BakeTable.Collect(this);

            int BakedID = 0;
            List<BAT_BakeGroup_Skin> groups = BakeTable.Groups;
            //merging by groups
            foreach (BAT_BakeGroup_Skin group in groups)
            {
                if (group.Count <= 0)
                {
                    continue;
                }
                BakeGroup(group);

                //clear mesh coponents
                ClearMeshComponents(group);

                BakedID++;
            }

            //clear resource not needed
            BakeTable.Clear();
            Resources.UnloadUnusedAssets();
            //Baked event
            foreach (BAT_BakeGroup_Skin group in groups)
            {
                StartCoroutine(group.OnBaked());
            }
            if (OnBaked != null)
            {
                OnBaked();
            }
            return gameObject;
        }

        private Vector3 m_localScale, m_localPosition;
        private Quaternion m_localRotation;
        private Transform m_parent;
        private void standardTrasnform(Transform _transform)
        {
            m_localScale = _transform.localScale;
            m_localRotation = _transform.localRotation;
            m_localPosition = _transform.localPosition;
            m_parent = _transform.parent;

            _transform.SetParent(null);
            _transform.localScale = Vector3.one;
            _transform.localRotation = Quaternion.identity;
            _transform.localPosition = Vector3.zero;
        }
        private void recoverTransform(Transform _transform)
        {
            _transform.SetParent(m_parent);
            _transform.localScale = m_localScale;
            _transform.localRotation = m_localRotation;
            _transform.localPosition = m_localPosition;
        }
        protected void BakeGroup(BAT_BakeGroup_Skin group)
        {
            group.generateSubGroups();
            var subGroupList = group.SubGroupList;
            group.calculateTopRootBone();
            for (int i = 0; i < subGroupList.Count; i++)
            {
                BakeSubGroup(subGroupList[i], group.m_animParent, group.TopRootBone, i);
            }

        }
        private static Mesh BakedMesh_Static;
        protected void BakeSubGroup(BAT_BakeSubGroup_Skin subGroup, Transform _root, Transform _topRootBone, int subID)
        {
            //Bake mesh of current group,if mesh vertexCount>=64k,
            //it would be seperated into several children
            int beginID = 0;
            int vertexCount = 0;
            int meshBakeID = 0;
            int currentID = 0;
            while (beginID < subGroup.Count)
            {
                BAT_BakeUnit_Skin bbs = (BAT_BakeUnit_Skin)subGroup[beginID];
                SkinnedMeshRenderer smr_org = bbs.SkinnedMeshRenderer;
                Mesh mesh_i = smr_org.sharedMesh;

                int vertexCountI = mesh_i.vertexCount;
                //whether exceed the vertextCount
                bool exceedVC = (vertexCount + vertexCountI >= MaxBakingVertex);
                //the end of group
                bool endOfGroup = currentID >= subGroup.Count - 1;
                //need Bake now? 
                bool needBake = false;
                if (exceedVC)
                {
                    needBake = true;
                }
                else if (endOfGroup)
                {
                    needBake = true;
                }
                //need merging now
                if (needBake)
                {
                    //create new child node
                    var childNode = BAT_NodeUtil.CreateChild(_root, "__Baked_" + subID+"_" + meshBakeID).gameObject;
                    int count;
                    int beginIDNext;
                    //one mesh's vertexCount exceed the max,Bake one
                    if (currentID == beginID)
                    {
                        count = 1;
                        beginIDNext = currentID + 1;
                        vertexCount = 0;
                    }
                    else
                    {
                        //exceed ,Bake [beginID,currentID)
                        if (exceedVC)
                        {
                            count = currentID - beginID;
                            beginIDNext = currentID;
                            vertexCount = vertexCountI;
                        }
                        else //end of group ,and not exceed,Bake [beginID,currentID]
                        {
                            count = subGroup.Count - beginID;
                            beginIDNext = currentID + 1;
                            vertexCount = 0;
                        }
                    }
                    //reset the root state
                    standardTrasnform(_root);


                    //collect all information of this group
                    CombineInstance[] combineInsts = new CombineInstance[count];
                    List<Mesh> meshList = new List<Mesh>();
                    List<Transform> bonesCombineList = new List<Transform>();


                    List<SkinnedMeshRenderer> allBakedSkinnedMeshes = new List<SkinnedMeshRenderer>();
                    for (int i = 0; i < count; i++)
                    {
                        BAT_BakeUnit_Skin BakeUnitI = subGroup[beginID + i] as BAT_BakeUnit_Skin;
                        SkinnedMeshRenderer smrI = BakeUnitI.SkinnedMeshRenderer;
                        allBakedSkinnedMeshes.Add(smrI);
                        Mesh meshI = smrI.sharedMesh;
                        meshList.Add(meshI);
                        combineInsts[i].mesh = meshI;
                        combineInsts[i].transform = smrI.transform.localToWorldMatrix;
                        //remeber the bones array
                        Transform[] bonesI = smrI.bones;
                        foreach (var bone in bonesI)
                        {
                            bonesCombineList.Add(bone);
                        }
                    }

                    //check if this mesh group has baked before
                    Mesh BakedMesh = BAT_BakedMark.GetBakedMesh(meshList);

                    //start merging
                    if (BakedMesh == null)
                    {
                        BakedMesh = new Mesh();
                        try
                        {
                            BakedMesh.CombineMeshes(combineInsts, true, true);
                            BAT_BakedMark.SetBakedMesh(meshList, BakedMesh);
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError("Error occured in merging " + count + " items \n " + e.Message);
                        }
                    }
  

                    SkinnedMeshRenderer mr_Baked = childNode.AddComponent<SkinnedMeshRenderer>();
                    if (BakedMesh_Static == null)
                    {
                        BakedMesh_Static = BakedMesh;
                    }
                    mr_Baked.sharedMesh = BakedMesh;
                    mr_Baked.sharedMaterial = subGroup.m_sharedMaterial;
                    var bonesNew = bonesCombineList.ToArray();
                    mr_Baked.bones = bonesNew;

                    mr_Baked.rootBone = _topRootBone;
                    //mr_Baked.localBounds = BakedMesh.bounds;


                    //mr_Baked.sharedMaterials = subGroup.m_sharedMaterials;
                    //set same layer with current 
                    childNode.layer = gameObject.layer;

                    //clear the baked elements

                    if (m_clearMeshSetting == ClearSkinnedMeshSetting.SkinnedRenderers)
                    {
                        int bakedCount = allBakedSkinnedMeshes.Count;
                        for (int i = 0; i < bakedCount; i++)
                        {
                            GameObject.DestroyImmediate(allBakedSkinnedMeshes[i]);
                        }
                        allBakedSkinnedMeshes.Clear();
                    }
                    //recover the root state
                    recoverTransform(_root);
                    //ready for next
                    beginID = beginIDNext;
                    meshBakeID++;
                }
                else //not need Bake, add the vertexCount
                {
                    vertexCount += vertexCountI;
                }
                currentID++;
            }

        }
        

	}


   
}