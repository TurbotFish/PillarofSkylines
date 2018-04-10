using System;
using System.Collections.Generic;
using UnityEngine;

namespace bat.opt.bake.util
{
    public abstract class BAT_Collecor<KeyType,GroupType> where GroupType : BAT_BakeGroup
    {
        public bool m_isSkinnedMesh=false;
        /// <summary>
        /// all groups
        /// </summary>
        protected Dictionary<KeyType, GroupType> m_groupTables = new Dictionary<KeyType, GroupType>();
        public abstract void Collect(BAT_BakerBase baker);

        public List<GroupType> Groups
        {
            get
            {
                List<GroupType> groups = new List<GroupType>();
                foreach (var g in m_groupTables.Values)
                {
                    groups.Add(g);
                }
                return groups;
            }
        }
        public void Clear()
        {
            m_groupTables.Clear();
        }
    }
}
