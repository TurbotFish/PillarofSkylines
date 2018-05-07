using System.Collections.Generic;
/// <summary>
/// A group of MeshFilters using the same thing,base class
/// </summary>
using UnityEngine;
namespace bat.opt.bake.util
{
    public class BAT_BakeGroup
    {
        //MeshFilters using the same main material
        public List<BAT_BakeUnit> m_BakeUnits = new List<BAT_BakeUnit>();

        public int Count
        {
            get
            {
                return m_BakeUnits.Count;
            }
        }
        public BAT_BakeUnit this[int id]
        {
            get
            {
                if (id >= 0 && id < m_BakeUnits.Count)
                {
                    return m_BakeUnits[id];
                }
                return null;
            }
        }
    }
}