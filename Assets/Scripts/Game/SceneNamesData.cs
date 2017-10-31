﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/SceneNamesData", fileName = "SceneNamesData")]
    public class SceneNamesData : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        Object openWorldScene;

        public string GetOpenWorldSceneName()
        {
            return this.openWorldScene.name;
        }

#if UNITY_EDITOR
        public Object OpenWorldScene { get { return this.openWorldScene; } set { this.openWorldScene = value; } }
#endif

        [SerializeField]
        [HideInInspector]
        List<Object> pillarScenes = new List<Object>();

        public string GetPillarSceneName(World.ePillarId pillarId)
        {
            if (this.pillarScenes.Count > (int)pillarId && this.pillarScenes[(int)pillarId] != null)
            {
                return this.pillarScenes[(int)pillarId].name;
            }
            else
            {
                return string.Empty;
            }
        }

#if UNITY_EDITOR
        public List<Object> PillarScenes { get { return this.pillarScenes; } set { this.pillarScenes = value; } }

        public void SetPillarSceneName(World.ePillarId pillarId, Object scene)
        {
            if (!Application.isPlaying)
            {
                if (this.pillarScenes.Count > (int)pillarId)
                {
                    this.pillarScenes[(int)pillarId] = scene;
                }
                else
                {
                    while (this.pillarScenes.Count <= (int)pillarId)
                    {
                        this.pillarScenes.Add(null);
                    }

                    this.pillarScenes[(int)pillarId] = scene;
                }
            }
        }
#endif
    }
} //end of namespace