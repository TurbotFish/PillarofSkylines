using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MouseManipulator))]
public class MouseManipulatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MouseManipulator emitter = (MouseManipulator)target;
        if (emitter.m_fluid == null)
        {
            EditorGUILayout.HelpBox("Fluid simulator not defined", MessageType.Error);
        }

        if (emitter.m_particlesArea == null)
        {
            EditorGUILayout.HelpBox("Particles Area not defined", MessageType.Error);
        }

        emitter.m_particlesStrength = EditorGUILayout.Slider("Particles Strength", emitter.m_particlesStrength / 1000f, 0.0f, 10.0f) * 1000f;
        emitter.m_particlesRadius = EditorGUILayout.Slider("Particles Radius", emitter.m_particlesRadius, 0.0f, 5.0f);

        emitter.m_velocityStrength = EditorGUILayout.Slider("Velocity Strength", emitter.m_velocityStrength, 0.0f, 10.0f);
        emitter.m_velocityRadius = EditorGUILayout.Slider("Velocity Radius", emitter.m_velocityRadius, 0.0f, 5.0f);

        EditorUtility.SetDirty(emitter);
    }
}
