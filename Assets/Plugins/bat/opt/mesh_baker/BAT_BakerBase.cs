using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using bat.util;
using bat.opt.bake.util;

namespace bat.opt.bake
{
    public enum ClearMeshSetting
    {
        Nothing,
        MeshFilters,
        FiltersAndRenderers,
    }

    public enum ClearSkinnedMeshSetting
    {
        Nothing,
        SkinnedRenderers,
    }


	public abstract class BAT_BakerBase : MonoBehaviour
	{
		[SerializeField][Tooltip("Auto combine if not Baked.")]
		public bool m_autoBake=true;
		[SerializeField][Tooltip("Remove originial MeshFilters and MeshRenderers after Baked")]
		public ClearMeshSetting m_clearSetting=ClearMeshSetting.FiltersAndRenderers;
		[System.NonSerialized][Tooltip("Events on Baked")]
		public Action OnBaked;
        [System.NonSerialized][Tooltip("has Baked or not.")]
		protected bool m_hasBaked=false;

        protected Transform m_transform;

        public static BAT_BakerBase Current
        {
            get;
            set;
        }

        protected virtual void Awake()
        {
            m_transform =transform;
        }
        public static int MaxBakingVertex
        {
            get
            {
                return 65535;
            }
        }

        protected virtual void Start()
        {
            if (!m_hasBaked && m_autoBake)
            {
                StartBake();
                m_hasBaked = true;
            }
        }
        /// <summary>
        /// Start Bake
        /// </summary>
        public void StartBake()
        {
            Current = this;
            Bake();
        }

		/// <summary>
        /// Bake all game objects under current GameObject,including meshes and materials.
        /// By default, baking will group the meshes by diffrent material(ShareMaterial).
		/// </summary>
        protected abstract GameObject Bake();

        /// <summary>
        /// clear mesh coponents in original game objects by diffrent deep
        /// </summary>
        /// <param name="groups">groups holding the old elelemts which have been Baked</param>
        protected virtual void ClearMeshComponents<T>(List<T> groups) where T:BAT_BakeGroup
        {
            if (m_clearSetting <= ClearMeshSetting.Nothing)
            {
                return;
            }
            foreach (var group in groups)
            {
                if (group.Count == 0)
                {
                    continue;
                }
                ClearMeshComponents(group);
            }
        }
        protected virtual void ClearMeshComponents<T>(T group) where T:BAT_BakeGroup
        {
            for (int i = 0; i < group.Count; i++)
            {
                if (group[i] == null)
                {
                    continue;
                }
                if (!(group[i] is BAT_BakeUnit_Mesh)) //FIXME: think about the condition of skinned mesh
                {
                    continue;
                }
                MeshFilter mfI = ((BAT_BakeUnit_Mesh)group[i]).MeshFilter;
                if (mfI == null)
                {
                    continue;
                }
                var goI = mfI.gameObject;

                if (goI != null)
                {
                    if (m_clearSetting >= ClearMeshSetting.FiltersAndRenderers)
                    {
                        MeshRenderer mrI = mfI.GetComponent<MeshRenderer>();
                        if (mrI != null)
                        {
                            GameObject.DestroyImmediate(mrI);
                        }
                    }
                    GameObject.DestroyImmediate(mfI);
                }

            }
        }
        #region assets releasing
        /// <summary>
        /// m_assetsCreated are those assets created in baking progress,you need to
        /// destroy them directly when you does't need the them,because them may not be
        /// destroyed successfully by destroying the created GameObject.
        /// </summary>
        private List<UnityEngine.Object> m_assetsCreated = new List<UnityEngine.Object>();

        /// <summary>
        /// mark the asset for releasing
        /// </summary>
        /// <param name="_asset"> asset to release</param>
        public void MarkAsset(UnityEngine.Object _asset)
        {
            m_assetsCreated.Add(_asset);
        }

        protected void OnDestroy()
        {
            if (m_assetsCreated != null && m_assetsCreated.Count > 0)
            {
                for (int i = 0; i < m_assetsCreated.Count; i++)
                {
                    if (m_assetsCreated[i] != null)
                    {
                        GameObject.DestroyImmediate(m_assetsCreated[i]);
                    }
                }
                m_assetsCreated.Clear();
                Resources.UnloadUnusedAssets();
            }
        }
        #endregion

    }


}