using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Blocker))]
public class BlockerEditor : Editor
{
    bool m_debug = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Blocker obstacle = (Blocker)target;
        if (obstacle.m_fluid == null)
        {
            EditorGUILayout.HelpBox("Fluid not defined", MessageType.Error);
        }

        // Scale
        obstacle.m_useScaleAsSize = !EditorGUILayout.Toggle("Set Size Manually", !obstacle.m_useScaleAsSize);
        if (obstacle.m_useScaleAsSize)
        {
            EditorGUILayout.HelpBox(" Using global scale as size", MessageType.None);
        }
        else
        {
            ++EditorGUI.indentLevel;
            obstacle.m_radius = EditorGUILayout.Slider("Radius", obstacle.m_radius, 0.0f, 5.0f);
            --EditorGUI.indentLevel;
        }
        
        // Debug
        m_debug = EditorGUILayout.Foldout(m_debug, "Debug");
        if (m_debug)
        {
            ++EditorGUI.indentLevel;
            obstacle.m_showGizmo = EditorGUILayout.Toggle("Draw Gizmo", obstacle.m_showGizmo);
            --EditorGUI.indentLevel;
        }
        EditorUtility.SetDirty(obstacle);
    }
}