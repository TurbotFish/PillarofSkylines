using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.GameControl
{
    [CustomEditor(typeof(SpawnPoint))]
    public class SpawnPointEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //
            base.OnInspectorGUI();

            //
            var spawnPoint = target as SpawnPoint;

            spawnPoint.Type = (eSpawnPointType)EditorGUILayout.EnumPopup("Type", spawnPoint.Type);

            if(spawnPoint.Type == eSpawnPointType.PillarExitIntact || spawnPoint.Type == eSpawnPointType.PillarExitDestroyed)
            {
                spawnPoint.Pillar = (ePillarId)EditorGUILayout.EnumPopup("Pillar", spawnPoint.Pillar);
            }

            //
            EditorUtility.SetDirty(spawnPoint);
        }
    }
} //end of namespace