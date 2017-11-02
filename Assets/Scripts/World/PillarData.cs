using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
    [CreateAssetMenu(menuName = "ScriptableObjects/PillarData", fileName = "PillarData")]
    public class PillarData : ScriptableObject
    {
        //#####################################################

        #region pillar entry prices

        [SerializeField]
        [HideInInspector]
        List<int> pillarEntryPriceList = new List<int>();
#if UNITY_EDITOR
        public List<int> PillarEntryPriceList { get { return this.pillarEntryPriceList; } set { this.pillarEntryPriceList = value; } }
#endif

        public int GetPillarEntryPrice(ePillarId pillarId)
        {
            if (this.pillarEntryPriceList.Count > (int)pillarId)
            {
                return this.pillarEntryPriceList[(int)pillarId];
            }
            else
            {
                return 0;
            }
        }

        #endregion pillar entry prices

        //#####################################################
    }
}