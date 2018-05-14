using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
    public class SubSceneJob
    {
        public RegionBase Region { get; private set; }
        public eSubSceneVariant SubSceneVariant { get; private set; }
        public eSubSceneLayer SubSceneLayer { get; private set; }
        public eSubSceneJobType JobType { get; private set; }

        public eSubSceneJobState CurrentState = eSubSceneJobState.Pending;

        public SubSceneJob(RegionBase region, eSubSceneVariant subSceneVariant, eSubSceneLayer subSceneLayer, eSubSceneJobType jobType)
        {
            Region = region;
            SubSceneVariant = subSceneVariant;
            SubSceneLayer = subSceneLayer;
            JobType = jobType;
        }

        public float GetPriority()
        {
            return Region.PlayerDistance + Region.GetJobIndex(this);
        }
    }
} //end of namespace