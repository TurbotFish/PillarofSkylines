using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using bat.util;
using bat.opt.bake.util;


namespace bat.opt.bake
{
	public class BAT_DeepBaker : BAT_BakerBase
	{
        [SerializeField][Tooltip("all ShadersConfigs saved in array(same with list)")]
        public BAT_ShaderConfig[] m_uvConfigs;
        [System.NonSerialized][Tooltip("quick find table - from shader to BAT_ShadersConfig")]
        private Dictionary<Shader, BAT_ShaderConfig> m_uvConfigsQT = new Dictionary<Shader, BAT_ShaderConfig>();
        
        [SerializeField][Tooltip("show references  ")][HideInInspector]
        public bool m_editor_showReferences = true;
        
        protected override void Start()
        {
            //refresh and generate uvconfigs
            if (m_uvConfigs == null && m_autoBake)
            {
                Refresh(true);
            }
            //int configs and generate quick find table
            if (m_uvConfigs != null)
            {
                foreach (var cfg in m_uvConfigs)
                {
                    cfg.Init();
                    m_uvConfigsQT.Add(cfg.Shader, cfg);
                }
            }

            base.Start();
        }
		#region Bake game objects

        /// <summary>
        /// Bake all game objects under current GameObject,including meshes and materials.
        /// By default, Baking will group the meshes by diffrent shader.
        /// </summary>
        /// <returns>the GameObject created</returns>
        protected override  GameObject Bake()
		{
            //collect all groups
            BAT_GroupCollector_Deep groupsCollector = new BAT_GroupCollector_Deep();
            groupsCollector.Collect(this);
            //create the target game object of merging to
            var allBakedTo = BAT_NodeUtil.CreateChild(m_transform, "__AllBaked");

            int shaderID = 0;
            //get the list of Bake groups ready.
            List<BAT_BakeGroup_Deep> groups = groupsCollector.Groups;
            //merging by groups
            foreach (BAT_BakeGroup_Deep group in groups)
            {
                if (group.Count == 0)
                {
                    continue;
                }
                var childNode = BAT_NodeUtil.CreateChild(allBakedTo, "__shader_" + shaderID);
                group.Bake(childNode, FindConfig(group.m_shader));
                shaderID++;
            }
 
			//clear mesh coponents
            ClearMeshComponents(groups);

            //clear resource not needed
			groupsCollector.Clear();
			Resources.UnloadUnusedAssets();
            //Baked event
			if(OnBaked != null)
			{
				OnBaked();
			}
			return allBakedTo.gameObject;


		}


		#endregion

        public BAT_ShaderConfig FindConfig(Shader shader)
        {
            BAT_ShaderConfig result=null;
            if (m_uvConfigs != null)
            {
                m_uvConfigsQT.TryGetValue(shader, out result);
            }
            return result;
        }


        /// <summary>
        /// Refresh all ShaderConfigs
        /// </summary>
        /// <param name="forceRefresh">force refresh ?</param>
        public void Refresh(bool forceRefresh = false)
        {
            if (m_uvConfigs == null || m_uvConfigs.Length == 0 || forceRefresh)
            {
                BAT_EdtUtil.Undo_RecordObject(this, "refresh");
                //temporary shaders of last validation result.
                Dictionary<Shader, List<Material>> allShaders = new Dictionary<Shader, List<Material>>();
                //all ShadersConfigs saved in list (same with array)
                List<BAT_ShaderConfig> uvConfigList = new List<BAT_ShaderConfig>();
                List<Shader> shaderList = new List<Shader>();
                var meshRenderers = GetComponentsInChildren<MeshRenderer>();

                foreach (var r in meshRenderers)
                {
                    var mts = r.sharedMaterials;
                    foreach (var mt in mts)
                    {
                        if (mt != null && mt.shader != null)
                        {
                            List<Material> mtList = null;
                            allShaders.TryGetValue(mt.shader, out mtList);
                            if (mtList == null)
                            {
                                mtList = new List<Material>();
                                allShaders.Add(mt.shader, mtList);
                            }
                            if (!mtList.Contains(mt))
                            {
                                mtList.Add(mt);
                            }
                        } 
                    }

                }

                //check if exist these shaders
                if (m_uvConfigs != null)
                {
                    for (int i = 0; i < m_uvConfigs.Length; i++)
                    {
                        BAT_ShaderConfig cfgI = m_uvConfigs[i];
                        if (cfgI != null)
                        {
                            var existShader = cfgI.Shader;
                            if (existShader != null)
                            {
                                if (allShaders.ContainsKey(existShader))
                                {
                                    uvConfigList.Add(cfgI);
                                    shaderList.Add(existShader);
                                    cfgI.Init();
                                }
                            }
                        }
                    }
                }
                //add not exist shader configs
                foreach (var newShader in allShaders.Keys)
                {
                    if (!shaderList.Contains(newShader))
                    {
                        BAT_ShaderConfig cfgNew = new BAT_ShaderConfig();
                        cfgNew.Shader = newShader;
                        uvConfigList.Add(cfgNew);
                        shaderList.Add(newShader);
                    }
                }
                //export list
                m_uvConfigs = uvConfigList.ToArray();
                //set materials to config
                foreach (var cfg in m_uvConfigs)
                {
                    cfg.m_mateirals = allShaders[cfg.Shader].ToArray();
                }

                allShaders.Clear();
                uvConfigList.Clear();
            }
        }
       

	}


   
}