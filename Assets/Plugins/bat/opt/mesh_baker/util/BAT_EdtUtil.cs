using System;
using System.Collections.Generic;
using UnityEngine;

namespace bat.util
{
    public class BAT_EdtUtil
    {
        public static void Undo_RecordObject(UnityEngine.Object _object,string name)
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(_object,name);
#endif
        }
        public static void Undo_RecordObjects(UnityEngine.Object[] _object, string name)
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObjects(_object, name);
#endif
        }

        public static void Undo_AddComponent<T>(GameObject _object) where T : Component
        {
#if UNITY_EDITOR
            UnityEditor.Undo.AddComponent<T>(_object);
#endif
        }


    }
}
