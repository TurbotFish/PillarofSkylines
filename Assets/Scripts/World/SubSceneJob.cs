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
        public eSubSceneType SubSceneType { get; private set; }
        public eSubSceneJobType JobType { get; private set; }
        public Action<SubSceneJob> Callback { get; private set; }

        public Transform SubSceneRoot { get; set; }
        public bool IsJobSuccessful { get; set; }

        public SubSceneJob(RegionBase region, eSubSceneMode subSceneMode, eSubSceneType subSceneType, eSubSceneJobType jobType, Action<SubSceneJob> callback)
        {
            Region = region;
            SubSceneMode = subSceneMode;
            SubSceneType = subSceneType;
            JobType = jobType;
            Callback = callback;
        }
    }
}