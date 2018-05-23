using Game.Model;
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

            spawnPoint.Type = (SpawnPointType)EditorGUILayout.EnumPopup("Type", spawnPoint.Type);

            if(spawnPoint.Type == SpawnPointType.PillarExitIntact || spawnPoint.Type == SpawnPointType.PillarExitDestroyed)
            {
                spawnPoint.Pillar = (PillarId)EditorGUILayout.EnumPopup("Pillar", spawnPoint.Pillar);
            }

            //
            EditorUtility.SetDirty(spawnPoint);
        }
    }
} //end of namespace