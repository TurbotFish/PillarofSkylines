//using Game.Model;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Game.World
//{
//    [CreateAssetMenu(menuName = "ScriptableObjects/PillarData", fileName = "PillarData")]
//    public class PillarData : ScriptableObject
//    {
//        //#####################################################

//        #region pillar entry prices

//        [SerializeField]
//        [HideInInspector]
//        List<int> pillarEntryPriceList = new List<int>();

//#if UNITY_EDITOR
//        public List<int> PillarEntryPriceList { get { return pillarEntryPriceList; } set { pillarEntryPriceList = value; } }
//#endif

//        public int GetPillarEntryPrice(ePillarId pillarId)
//        {
//            if (pillarEntryPriceList.Count > (int)pillarId)
//            {
//                return pillarEntryPriceList[(int)pillarId];
//            }
//            else
//            {
//                return 0;
//            }
//        }

//        #endregion pillar entry prices

//        //#####################################################

//        #region pillar ability groups

//        [SerializeField]
//        [HideInInspector]
//        List<eAbilityGroup> pillarAbilityGroups = new List<eAbilityGroup>();

//#if UNITY_EDITOR
//        public List<eAbilityGroup> PillarAbilityGroups { get { return pillarAbilityGroups; } set { pillarAbilityGroups = value; } }
//#endif

//        public eAbilityGroup GetPillarAbilityGroup(ePillarId pillarId)
//        {
//            if (pillarAbilityGroups.Count > (int)pillarId)
//            {
//                return pillarAbilityGroups[(int)pillarId];
//            }
//            else
//            {
//                return 0;
//            }
//        }

//        #endregion pillar ability groups

//        //#####################################################
//    }
//}