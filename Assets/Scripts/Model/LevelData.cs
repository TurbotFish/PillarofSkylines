using Game.World;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Model
{
    [CreateAssetMenu(menuName = "ScriptableObjects/LevelData", fileName = "LevelData")]
    public class LevelData : ScriptableObject
    {
        //###############################################################

        // -- ATTRIBUTES

        [SerializeField, HideInInspector] public Object OpenWorldSceneObject;
        [SerializeField, HideInInspector] public string OpenWorldSceneName;

        [SerializeField, HideInInspector] public List<Object> PillarSceneObjectList = new List<Object>();
        [SerializeField, HideInInspector] public List<string> PillarSceneNameList = new List<string>();

        [SerializeField, HideInInspector] public List<int> PillarSceneActivationPriceList = new List<int>();
        [SerializeField, HideInInspector] public List<AbilityType> PillarRewardAbilityList = new List<AbilityType>();

        //###############################################################

        // -- INQUIRIES

        public string GetPillarSceneName(PillarId pillar_id)
        {
            return PillarSceneNameList[(int)pillar_id];
        }

        public int GetPillarSceneActivationCost(PillarId pillar_id)
        {
            return PillarSceneActivationPriceList[(int)pillar_id];
        }

        public AbilityType GetPillarRewardAbility(PillarId pillar_id)
        {
            return PillarRewardAbilityList[(int)pillar_id];
        }

        //###############################################################

        // -- OPERATIONS

        //public void SetNames()
        //{
        //    if (OpenWorldSceneObject == null)
        //    {
        //        OpenWorldSceneName = string.Empty;
        //    }
        //    else
        //    {
        //        OpenWorldSceneName = OpenWorldSceneObject.name;
        //    }

        //    PillarSceneNameList.Clear();
        //    foreach (var pillar_scene in PillarSceneObjectList)
        //    {
        //        if (pillar_scene == null)
        //        {
        //            PillarSceneNameList.Add(string.Empty);
        //        }
        //        else
        //        {
        //            PillarSceneNameList.Add(pillar_scene.name);
        //        }
        //    }
        //}
    }
} // end of namespace