using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
    public class SubSceneJob
    {
        public RegionBase Region { get; private set; }
        public eSubSceneMode SubSceneMode { get; private set; }
        public eSubSceneLayer SubSceneLayer { get; private set; }
        public eSubSceneJobType JobType { get; private set; }
        public Action<SubSceneJob, bool> Callback { get; private set; }

        public SubSceneJob(RegionBase region, eSubSceneMode subSceneMode, eSubSceneLayer subSceneLayer, eSubSceneJobType jobType, Action<SubSceneJob, bool> callback)
        {
            Region = region;
            SubSceneMode = subSceneMode;
            SubSceneLayer = subSceneLayer;
            JobType = jobType;
            Callback = callback;
        }

        public float Priority { get { return Region.CameraDistance; } }
    }
} //end of namespace