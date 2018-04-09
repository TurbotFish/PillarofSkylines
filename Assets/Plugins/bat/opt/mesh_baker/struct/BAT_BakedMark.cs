using System.Collections.Generic;
using UnityEngine;
namespace bat.opt.bake.util
{
    public class BAT_BakedMark
    {
        private static Dictionary<int, Mesh> M_bakedMeshes = new Dictionary<int, Mesh>();
        public static Mesh GetBakedMesh(List<Mesh> meshGroup)
        {
            int mark=GenMark(meshGroup);
            Mesh mesh = null;
            M_bakedMeshes.TryGetValue(mark, out mesh);
            return mesh;
        }

        public static void SetBakedMesh(List<Mesh> meshGroup, Mesh baked)
        {
            int mark = GenMark(meshGroup);
            if (!M_bakedMeshes.ContainsKey(mark))
            {
                M_bakedMeshes.Add(mark, baked);
            }
        }

        public static int GenMark(List<Mesh> meshGroup)
        {
            int mark = 0;
            foreach (var m in meshGroup)
            {
                mark += m.vertexCount;
            }
            return mark;
        }
        /// <summary>
        /// Clear cached meshes when you need to release resource
        /// </summary>
        public static void Clear()
        {
            if (M_bakedMeshes != null && M_bakedMeshes.Count > 0)
            {
                List<Mesh> list=new List<Mesh>();
                list.AddRange(M_bakedMeshes.Values);
                for (int i = 0; i < list.Count;i++ )
                {
                    if (list[i] != null)
                    {
                        GameObject.DestroyImmediate(list[i]);
                    }
                }
                M_bakedMeshes.Clear();
                Resources.UnloadUnusedAssets();
            }

        }

    }
}
