using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VelocityManipulator))]
public class VelocityManipulatorEditor : Editor
{
    bool m_debug = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        VelocityManipulator emitter = (VelocityManipulator)target;

        if (emitter.m_fluid == null)
        {
            EditorGUILayout.HelpBox("Fluid not defined", MessageType.Error);
        }

        emitter.m_fluidVelocitySpeed = EditorGUILayout.Slider("Speed", emitter.m_fluidVelocitySpeed, 0.0f, 50.0f);

        // Direction
        string[] direction_options = { "Global Rotation", "Movement Direction"};
        int direction = emitter.m_velocityFromMovement ? 1 : 0;
        direction = EditorGUILayout.Popup("Direction", direction, direction_options);
        emitter.m_velocityFromMovement = (direction == 1) ? true : false;

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
