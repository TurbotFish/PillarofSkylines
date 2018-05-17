using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
    public class SubSceneJob
    {
        public RegionBase Region { get; private set; }
        public SubSceneVariant SubSceneVariant { get; private set; }
        public SubSceneLayer SubSceneLayer { get; private set; }
        public SubSceneJobType JobType { get; private set; }

        public SubSceneJobState CurrentState = SubSceneJobState.Pending;

        public SubSceneJob(RegionBase region, SubSceneVariant subSceneVariant, SubSceneLayer subSceneLayer, SubSceneJobType jobType)
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