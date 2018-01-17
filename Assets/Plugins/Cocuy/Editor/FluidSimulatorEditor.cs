using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FluidSimulator))]
public class FluidSimulatorEditor : Editor {

	static bool m_showAdvanced = false;

	public static bool Foldout(bool foldout, GUIContent content, bool toggleOnLabelClick, GUIStyle style)
	{
		Rect position = GUILayoutUtility.GetRect(40f, 40f, 16f, 16f, style);
		return EditorGUI.Foldout(position, foldout, content, toggleOnLabelClick, style);
	}

	public static bool Foldout(bool foldout, string content, bool toggleOnLabelClick, GUIStyle style)
	{
		return Foldout(foldout, new GUIContent(content), toggleOnLabelClick, style);
	}

	public override void OnInspectorGUI()
	{
        FluidSimulator sim = (FluidSimulator)target;
        base.OnInspectorGUI();


        string[] options = { "High", "Mid", "Low" };
        int quality = 0;
        if (sim.Resolution == 256)
        {
            quality = 1;
        }
        else if (sim.Resolution == 128)
        {
            quality = 2;
        }
        quality = EditorGUILayout.Popup("Quality", quality, options);
        switch (quality)
        {
            case 0:
                sim.Resolution = 512;
                break;
            case 1:
                sim.Resolution = 256;
                break;
            default:
                sim.Resolution = 128;
                break;
        }

        sim.Vorticity = EditorGUILayout.Slider("Vorticity", sim.Vorticity, 0.0f, 50.0f);
        sim.Viscosity = EditorGUILayout.Slider("Viscosity", sim.Viscosity * 10.0f, 0.0f, 1.0f) / 10.0f;

        m_showAdvanced = Foldout(m_showAdvanced, "Advanced", true, EditorStyles.foldout);
        if (m_showAdvanced)
        {
            EditorGUI.indentLevel++;
            sim.m_cacheVelocity = EditorGUILayout.Toggle("Cache Velocity", sim.m_cacheVelocity);
            sim.Iterations = EditorGUILayout.IntSlider("Simulation Quality", sim.Iterations, 0, 100);
            sim.Speed = EditorGUILayout.Slider("Simulation Speed", sim.Speed, 0.0f, 1000.0f);

            float disp_min = 0.9f;
            float disp_max = 1.0f;
            float range = disp_max - disp_min;
            float vel_min = 0.97f;
            float vel_max = 1.0f;
            range = vel_max - vel_min;
            float velocity_dissipation = Mathf.Clamp(sim.VelocityDissipation, vel_min, vel_max);
            velocity_dissipation = (velocity_dissipation - vel_min) / range;
            velocity_dissipation = EditorGUILayout.Slider("Velocity Dissipation", velocity_dissipation, 0.0f, 1.0f);
            sim.VelocityDissipation = velocity_dissipation * range + vel_min;
            EditorGUI.indentLevel--;
        }
        EditorUtility.SetDirty(sim);
	}
}
