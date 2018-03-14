//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using UnityEditor;
//using UnityEngine;

//namespace Game.World.ChunkSystem
//{
//    [CustomEditor(typeof(SubChunkController))]
//    public class SubChunkControllerEditor : Editor
//    {
//        SubChunkController self;
//        SerializedProperty layerProperty;

//        internal void OnEnable()
//        {
//            self = target as SubChunkController;
//            layerProperty = serializedObject.FindProperty("layer");
//        }

//        public override void OnInspectorGUI()
//        {
//            //
//            base.OnInspectorGUI();

//            //
//            layerProperty.enumValueIndex = EditorGUILayout.Popup("Layer", (int)self.Layer, GetEnumDescriptions());

//            //
//            serializedObject.ApplyModifiedProperties();
//            EditorUtility.SetDirty(self);
//        }

//        string[] GetEnumDescriptions()
//        {
//            var result = new List<string>();

//            var layerValues = Enum.GetValues(typeof(eSubChunkLayer)).Cast<eSubChunkLayer>();

//            foreach (var layer in layerValues)
//            {
//                var da = (DescriptionAttribute[])(layer.GetType().GetField(layer.ToString())).GetCustomAttributes(typeof(DescriptionAttribute), false);
//                result.Add(da.Length > 0 ? da[0].Description : layer.ToString());
//            }

//            return result.ToArray();
//        }
//    }
//}