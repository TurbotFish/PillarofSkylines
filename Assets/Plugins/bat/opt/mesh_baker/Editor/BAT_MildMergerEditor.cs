using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using bat.util;
using UnityEditor;
using bat.opt.bake;


namespace bat.opt.Bake
{

    [CustomEditor(typeof(BAT_MildBaker))]
    public class BAT_MildBakerEditor : Editor
    {
        private BAT_MildBaker m_mildBaker;
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
        ClearMeshSetting.Nothing.ToString(),
        ClearMeshSetting.MeshFilters.ToString(),
        ClearMeshSetting.FiltersAndRenderers.ToString()};
        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                return;
            }
            m_mildBaker = (BAT_MildBaker)target;
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
            var autoBake = EditorGUILayout.Toggle("Auto Bake", m_mildBaker.m_autoBake);
            if (autoBake != m_mildBaker.m_autoBake)
            {
                Undo();
                m_mildBaker.m_autoBake = autoBake;
            }

            //ClearMeshSettings
            int oldSelID = (int)m_mildBaker.m_clearSetting;
            int newSelID = EditorGUILayout.Popup("ClearSetting", oldSelID, m_ClearMeshItems, "MiniToolbarButton");
            if (newSelID != oldSelID)
            {
                Undo();
                m_mildBaker.m_clearSetting = (ClearMeshSetting)newSelID;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
             //endof of head region】


            EditorGUILayout.EndVertical();
            //end of all settings】

            BAT_UIUtil.ResetColor();

        }

        [MenuItem ("Window/Runtime Mesh Baker/Add MildBaker")]
        public static void AddMildBaker()
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
               BAT_EdtUtil.Undo_AddComponent<BAT_MildBaker>(targetGO);
            }
        }

        private void Undo()
        {
            if (m_mildBaker!=null)
            {
                BAT_EdtUtil.Undo_RecordObject(m_mildBaker, "modify property");
            }
        }
     


        
    }
}
