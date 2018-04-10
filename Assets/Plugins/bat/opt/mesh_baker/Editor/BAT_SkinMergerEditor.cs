using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using bat.util;
using UnityEditor;
using bat.opt.bake;


namespace bat.opt.Bake
{

    [CustomEditor(typeof(BAT_SkinnedMeshBaker))]
    public class BAT_SkinMergerEditor : Editor
    {
        private BAT_SkinnedMeshBaker m_skinBaker;
        private Texture m_logo;
        void OnEnable()
        {
            if (Application.isPlaying)
            {
                return;
            }
            m_logo = Resources.Load<Texture>("bat_logo");
        }
        private string[] m_ClearMeshItems = new string[] { 
        ClearSkinnedMeshSetting.Nothing.ToString(),
        ClearSkinnedMeshSetting.SkinnedRenderers.ToString()};
        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                return;
            }
            m_skinBaker = (BAT_SkinnedMeshBaker)target;
            BAT_UIUtil.RememberColor();

            EditorGUILayout.Separator();

            //【begin of all settings
            GUI.backgroundColor = Color.white;
            EditorGUILayout.BeginVertical("ProgressBarBack");

            //【begin of head region

            EditorGUILayout.BeginVertical("ProgressBarBack");
            
            //logo
            GUI.backgroundColor = BAT_UIUtil.Color_LogoBg;
            EditorGUILayout.BeginHorizontal("HelpBox");
            GUILayout.FlexibleSpace();
            GUILayout.Label(new GUIContent(m_logo));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            //end of logo

            GUI.backgroundColor =  BAT_UIUtil.Color_LightGray;
             EditorGUILayout.Space();
            //【begin of setting title
            EditorGUILayout.BeginHorizontal();
            GUI.contentColor = BAT_UIUtil.Color_LightGray;
            GUILayout.Label("Settings", "GUIEditor.BreadcrumbLeft");

            EditorGUILayout.Space();

            EditorGUILayout.EndHorizontal();
            //endof of setting title】

            //set global definition
            BAT_UIUtil.ResetColor();
            EditorGUILayout.BeginVertical();
            var autoBake = EditorGUILayout.Toggle("Auto Bake", m_skinBaker.m_autoBake);
            if (autoBake != m_skinBaker.m_autoBake)
            {
                Undo();
                m_skinBaker.m_autoBake = autoBake;
            }

            //ClearMeshSettings
            int oldSelID = (int)m_skinBaker.m_clearMeshSetting;
            int newSelID = EditorGUILayout.Popup("ClearSetting", oldSelID, m_ClearMeshItems, "MiniToolbarButton");
            if (newSelID != oldSelID)
            {
                Undo();
                m_skinBaker.m_clearMeshSetting = (ClearSkinnedMeshSetting)newSelID;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
             //endof of head region】


            EditorGUILayout.EndVertical();
            //end of all settings】

            BAT_UIUtil.ResetColor();

        }

        [MenuItem ("Window/Runtime Mesh Baker/Add SkinnedMeshBaker")]
        public static void AddSkinnedMeshBaker()
        {
            var targetGO = UnityEditor.Selection.activeGameObject as GameObject;
            if(targetGO!=null)
            {
               BAT_BakerBase childItem =   targetGO.GetComponentInChildren<BAT_BakerBase>();
               if (childItem != null)
               {
                   BAT_UIUtil.ShowWarning("You don't need two Bakers in the same tree,because it exist in "+childItem.name);
                   return;
               }
               BAT_BakerBase parentItem = targetGO.GetComponentInParent<BAT_BakerBase>();
               if (parentItem != null)
               {
                   BAT_UIUtil.ShowWarning("You don't need two Bakers in the same tree,because it exist in "+parentItem.name);
                   return;
               }
               BAT_EdtUtil.Undo_AddComponent<BAT_SkinnedMeshBaker>(targetGO);
            }
        }

        private void Undo()
        {
            if (m_skinBaker!=null)
            {
                BAT_EdtUtil.Undo_RecordObject(m_skinBaker, "modify property");
            }
        }
     


        
    }
}
