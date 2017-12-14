using System;
using System.Linq.Expressions;
using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    [CustomEditor(typeof(PostProcessingBehaviour))]
    public class PostProcessingBehaviourEditor : Editor
    {
        SerializedProperty m_ProfileIn, m_ProfileOut;

        public void OnEnable()
        {
            m_ProfileIn = FindSetting((PostProcessingBehaviour x) => x.profileInside);
			m_ProfileOut = FindSetting((PostProcessingBehaviour x) => x.profileOutside);
		}

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_ProfileIn);
			EditorGUILayout.PropertyField(m_ProfileOut);

			serializedObject.ApplyModifiedProperties();
        }

        SerializedProperty FindSetting<T, TValue>(Expression<Func<T, TValue>> expr)
        {
            return serializedObject.FindProperty(ReflectionUtils.GetFieldPath(expr));
        }
    }
}
