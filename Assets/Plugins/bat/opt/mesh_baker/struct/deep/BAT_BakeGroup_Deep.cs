using bat.util;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace bat.opt.bake.util
{
    /// <summary>
    /// A group of MeshFilters using the same shader(come from sharedMaterial's shader).
    /// Game objects using the same shader will be placed into the same group. 
    /// For the deep merging,Bake the meshes, material and textures.
    /// </summary>
    public class BAT_BakeGroup_Deep:BAT_BakeGroup
    {
        //main meterial,for copying the other params except main texture
        public Material m_mainMaterial;
        //shader
        public Shader m_shader;
        //All Meshes of current group,the key(int value) is a hash id formed by mesh's hushcode and subMeshID,
        //it means that one BAT_SubMesh contains one submesh's information
        public Dictionary<int, BAT_SubMesh> m_subMeshes = new Dictionary<int, BAT_SubMesh>();
        //UV0 of all Meshes
        public BAT_UVBaker m_meshesUV0 = new BAT_UVBaker();
        //UV1 of all Meshes
        public BAT_UVBaker m_meshesUV1 = new BAT_UVBaker();
        //UV2 of all Meshes
        public BAT_UVBaker m_meshesUV2 = new BAT_UVBaker();
        //Bake config of current group
        private BAT_ShaderConfig m_shaderConfig;
        public BAT_BakeGroup_Deep()
        {
        }
        public void SetBakeConfig(BAT_ShaderConfig BakeConfig)
        {
            m_shaderConfig = BakeConfig;
        }
        public void CollectMeshFilter(MeshFilter meshFilter, Material material,int subMeshID)
        {
            //make new unit
            BAT_BakeUnit_Deep bakeUnit = new BAT_BakeUnit_Deep();
            bakeUnit.m_ID = m_BakeUnits.Count;
            m_BakeUnits.Add(bakeUnit);
            //main material
            if (m_mainMaterial == null)
            {
				m_mainMaterial = (Material)GameObject.Instantiate(material);
                BAT_BakerBase.Current.MarkAsset(m_mainMaterial);
            }
            //set new unit's MeshFilter
            bakeUnit.SetMeshFilter(meshFilter);
            //collect meshes
            var mesh = meshFilter.sharedMesh;
            if (mesh != null)
            {
                //gen the hashID
                int hashID = mesh.GetHashCode()*10 + subMeshID;
                bakeUnit.m_hashID = hashID;
                //check if the unit exist
                BAT_SubMesh subMeshUnit=null;
                m_subMeshes.TryGetValue(hashID, out subMeshUnit);
                if (subMeshUnit==null)
                {
                    //create new submesh
                    subMeshUnit = new BAT_SubMesh();
                    subMeshUnit.m_hashID = hashID;
                    subMeshUnit.m_mesh = mesh;
                    subMeshUnit.m_subMeshID = subMeshID;

                    m_subMeshes.Add(hashID, subMeshUnit);

                    //add uvs
                    if (m_shaderConfig.m_uvConfigs[0].UVTextureNum > 0)
                    {
                        m_meshesUV0.m_uvsRecalculated.Add(mesh.uv);
                    }
                    if (m_shaderConfig.m_uvConfigs[1].UVTextureNum > 0)
                    {
                        m_meshesUV1.m_uvsRecalculated.Add(mesh.uv2);
                    }
					#if UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_7 || UNITY_5_9 || UNITY_6 || UNITY_7
                    if (m_shaderConfig.m_uvConfigs[2].UVTextureNum > 0)
                    {
                        m_meshesUV2.m_uvsRecalculated.Add(mesh.uv3);
                    }
					#endif

#if UNITY_EDITOR
                    //UvCheck(meshFilter);
#endif
                }

            }
            //collect textures and set texture id of current mesh
            CollectTexture(m_meshesUV0, m_shaderConfig.m_uvConfigs[0], material);
            CollectTexture(m_meshesUV1, m_shaderConfig.m_uvConfigs[1], material);
            CollectTexture(m_meshesUV2, m_shaderConfig.m_uvConfigs[2], material);
        }

        /// <summary>
        /// Scan the material by texture names of uvConfig,and save the result to 
        /// corresponding BAT_TexturePacker in BAT_TexturePackerGroup.
        /// </summary>
        /// <param name="packerGroup">the TexturePackerGroup for same uvs</param>
        /// <param name="uvConfig">the UVConfig for same uvs</param>
        /// <param name="material">target material</param>
        private void CollectTexture(BAT_UVBaker packerGroup, BAT_UVConfig uvConfig,Material material)
        {
           int[] scanIDs =  uvConfig.m_uvTxtureShaderIDs;
           for (int i = 0; i < scanIDs.Length; i++)
           {
              var texture =   material.GetTexture(scanIDs[i]);
              packerGroup.GetTexturePakcer(i).AddTexture(texture);
           }
        }
 

        private void UvCheck(MeshFilter meshFilter)
        {
            Mesh mesh = meshFilter.sharedMesh;
            Vector2[] uv = mesh.uv;
            bool uvError = false;
            foreach (var uvI in uv)
            {
                if (uvI.x < 0 || uvI.x > 1 || uvI.y < 0 || uvI.y > 1)
                {
                    uvError = true;
                    break;
                }
            }
            if (uvError)
            {
                Debug.LogWarning("UV out of bounds in mesh:" + meshFilter.name);
            }
        }

        /// <summary>
        /// Bake all meshes and materials of current gruop to target GameObject.
        /// </summary>
        /// <param name="_root">Bake root.</param>
        /// <param name="shaderConfig">shader config</param>
        public void Bake(Transform _root, BAT_ShaderConfig shaderConfig)
        {
            //Bake all uvs
            m_meshesUV0.Bake(shaderConfig.m_uvConfigs[0],m_BakeUnits,m_mainMaterial);
            //【not supported now】
            m_meshesUV1.Bake(shaderConfig.m_uvConfigs[1], m_BakeUnits, m_mainMaterial);
            m_meshesUV2.Bake(shaderConfig.m_uvConfigs[2], m_BakeUnits, m_mainMaterial);

            //Bake mesh of current group,if mesh vertexCount>=64k,
            //it would be seperated into several children
            int beginID = 0;
            int vertexCount = 0;
            int meshBakeID = 0;
            int currentID = 0;
            while (beginID < m_BakeUnits.Count)
            {
                BAT_BakeUnit_Deep unitI = null;
                int vertexCountI = 0;
                if (currentID >= 0 && currentID < m_BakeUnits.Count)
                {
                    unitI = m_BakeUnits[currentID] as BAT_BakeUnit_Deep;
                    BAT_SubMesh subMeshInfor=m_subMeshes[unitI.m_hashID];
                    vertexCountI = subMeshInfor.m_mesh.vertexCount;
                }
                //whether exceed the vertextCount
                bool exceedVC = (vertexCount + vertexCountI >=BAT_BakerBase.MaxBakingVertex);
                //the end of group
                bool endOfGroup = currentID >= m_BakeUnits.Count - 1;
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
                    GameObject target = BAT_NodeUtil.CreateChild(_root, _root.name + "_mesh_" + meshBakeID).gameObject;

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
                            count = m_BakeUnits.Count - beginID;
                            beginIDNext = currentID + 1;
                            vertexCount = 0;
                        }
                    }
                    //start merging
                    Mesh toBakedMesh = new Mesh();
                    BAT_BakerBase.Current.MarkAsset(toBakedMesh);
                    CombineInstance[] combineInsts = new CombineInstance[count];
                    List<Vector2> combinedUVs = new List<Vector2>();
                    //collect all meshes of this group
                    for (int i = 0; i < count; i++)
                    {
                        if (beginID + i >= m_BakeUnits.Count)
                        {
                            Debug.Log("-_#!");
                        }
                        BAT_BakeUnit_Deep bakeUnitI = m_BakeUnits[beginID + i]as BAT_BakeUnit_Deep;
                        BAT_SubMesh subMeshInf = m_subMeshes[bakeUnitI.m_hashID];
                        combineInsts[i].mesh = subMeshInf.m_mesh;
                        combineInsts[i].transform = bakeUnitI.MeshFilter.transform.localToWorldMatrix;
                        combineInsts[i].subMeshIndex = subMeshInf.m_subMeshID;
                        if (bakeUnitI.m_uvs != null)
                        {
                            combinedUVs.AddRange(bakeUnitI.m_uvs);
                        }
                    }

                    try
                    {
                        toBakedMesh.CombineMeshes(combineInsts, true, true);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("Error occured in merging " + count + " items \n " + e.Message);
                    }
                    
                    //combine the uvs
                    var combinedUVArray = combinedUVs.ToArray();
                    toBakedMesh.uv = combinedUVArray;

                    //save the result of baking
                    MeshFilter meshFilterRoot = target.GetComponent<MeshFilter>();
                    if (meshFilterRoot == null)
                    {
                        meshFilterRoot = target.AddComponent<MeshFilter>();
                    }
                    meshFilterRoot.sharedMesh = toBakedMesh;

                    Renderer meshRender = target.GetComponent<Renderer>();
                    if (meshRender == null)
                    {
                        meshRender = target.AddComponent<MeshRenderer>();
                    }
                    meshRender.sharedMaterial = m_mainMaterial;

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
