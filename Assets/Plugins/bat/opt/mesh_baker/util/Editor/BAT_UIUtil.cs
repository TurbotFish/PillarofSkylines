using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace bat.util
{
    public class BAT_UIUtil
    {
        public static GUIContent insertContent = new GUIContent("+", "duplicate this point");
        public static GUIContent deleteContent = new GUIContent("-", "delete this point");
        public static GUIContent pointContent = GUIContent.none;
        public static GUILayoutOption buttonWidth20 = GUILayout.MaxWidth(20f);
        public static GUILayoutOption buttonWidth50 = GUILayout.MaxWidth(50f);
        public static bool EditList<T>(List<T> list,Func<T,T> drawT,Action<List<T>,int> inseart,Action<List<T>,int> delete)
        {
            GUILayout.Space(5);
            int arraySize = list.Count;
            int deleteID = -1;
            int inseartID = -1;
            bool changed = false;
            if (arraySize > 0)
            {
                for (int i = 0; i < arraySize; i++)
                {
                    var pI = list[i];

                    EditorGUILayout.BeginHorizontal();
                    list[i] = drawT(pI);
                    
                    if (GUILayout.Button(insertContent, EditorStyles.miniButtonLeft, buttonWidth20))
                    {
                        inseartID = i;
                    }
                    if (GUILayout.Button(deleteContent, EditorStyles.miniButtonRight, buttonWidth20))
                    {
                        deleteID = i;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(insertContent, EditorStyles.miniButton, buttonWidth20))
                {
                    inseartID = 0;
                }
                EditorGUILayout.EndHorizontal();
            }
          

            if (deleteID >= 0 && arraySize >= 1)
            {
                delete(list,deleteID);
                changed = true;
            }
            if (inseartID >= 0)
            {
                inseart(list,inseartID);
                changed = true;
            }
            GUILayout.Space(5);

            return changed;
        }

        public static void BeginVerticalAsTextArea(Color color, float space = 2)
        {
            GUI.backgroundColor = color;
            EditorGUILayout.BeginVertical("AS TextArea", GUILayout.ExpandWidth(true));
            if (space > 0)
            {
                GUILayout.Space(space);
            }
        }

        public static void EndVerticalAsTextArea(float space = 2)
        {
            if (space > 0)
            {
                GUILayout.Space(space);
            }
            EditorGUILayout.EndVertical();
        }



        public static void BeginHorizontalAsTextArea(Color color, float space = 2)
        {
            GUI.backgroundColor = color;
            if (space > 0)
            {
                GUILayout.Space(space);
            }
            EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));

        }

        public static void EndHorizontalAsTextArea(float space = 2)
        {
            EditorGUILayout.EndHorizontal();
            if (space > 0)
            {
                GUILayout.Space(space);
            }
        }

        public static void RememberColor()
        {
            M_ContentColor = GUI.contentColor;
            M_BackgroundColor = GUI.backgroundColor;
        }

        public static void ResetContentColor()
        {
            GUI.contentColor = M_ContentColor;
        }

        public static void ResetColor()
        {
            GUI.contentColor = M_ContentColor;
            GUI.backgroundColor = M_BackgroundColor;
        }

        public static bool ShowWarning(string content)
        {
             return EditorUtility.DisplayDialog("Warning", content, "OK"); 
        }

        public static Color Color_BlueGreen = new Color(0.7f, 1, 1);
        public static Color Color_MildBlue = new Color(0.6f, 0.6f, 0.8f);//BAT_GfxUtil.IntToColor(0x323238ff);
        public static Color Color_DeepBlue = new Color(0.4f, 0.4f, 0.8f);//BAT_GfxUtil.IntToColor(0x323238ff);
        public static Color Color_LightGray = new Color(0.75f, 0.75f, 0.75f);
        public static Color Color_DarkGray = new Color(0.35f, 0.35f, 0.35f);
        public static Color Color_LogoBg = BAT_GfxUtil.IntToColor(0x222238ff);// 222238ff 0x1eaeb4ff
        private static Color M_ContentColor;
        private static Color M_BackgroundColor;


        public static GUIStyle GUIStyle_OL_Toggle=new GUIStyle("OL Toggle");
        public static GUIStyle GUIStyle_BoldToggle=new GUIStyle("BoldToggle");
        
    }
}
