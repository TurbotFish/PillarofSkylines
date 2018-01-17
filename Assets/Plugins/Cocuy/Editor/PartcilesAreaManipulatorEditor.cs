using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ParticlesAreaManipulator))]
public class ParticlesAreaManipulatorEditor : Editor
{
    bool m_debug = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ParticlesAreaManipulator emitter = (ParticlesAreaManipulator)target;
        if (emitter.m_particlesArea == null)
        {
            EditorGUILayout.HelpBox("Fluid particles not defined", MessageType.Error);
        }

        // Strength
        emitter.m_strength = EditorGUILayout.Slider("Strength", emitter.m_strength / 1000f, 0.0f, 10.0f) * 1000f;

        // Scale
        emitter.m_useScaleAsSize = !EditorGUILayout.Toggle("Set Size Manually", !emitter.m_useScaleAsSize);
        if (emitter.m_useScaleAsSize)
        {
            EditorGUILayout.HelpBox(" Using global scale as size", MessageType.None);
        }
        else
        {
            ++EditorGUI.indentLevel;
            emitter.m_radius = EditorGUILayout.Slider("Radius", emitter.m_radius, 0.0f, 5.0f);
            --EditorGUI.indentLevel;
        }

        // Debug
        m_debug = EditorGUILayout.Foldout(m_debug, "Debug");
        if (m_debug)
        {
            ++EditorGUI.indentLevel;
            emitter.m_showGizmo = EditorGUILayout.Toggle("Draw Gizmo", emitter.m_showGizmo);
            --EditorGUI.indentLevel;
        }

        EditorUtility.SetDirty(emitter);
    }
}
