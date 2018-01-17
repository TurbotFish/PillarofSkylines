using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ParticlesArea))]
public class ParticleAreaEditor : Editor {

	public override void OnInspectorGUI()
	{
        ParticlesArea sim = (ParticlesArea)target;

       /* if (SystemInfo.graphicsShaderLevel < 50)
        {
            EditorGUILayout.HelpBox("Cocuy needs DX11 enabled.", MessageType.Error);
        }
        else*/
        {
            base.OnInspectorGUI();

            float fMaxQuality = 2048.0f;
            float fMinQuality = 128.0f;
            sim.Resolution = (int)EditorGUILayout.Slider("Area Resolution", sim.Resolution, fMinQuality, fMaxQuality);

            float disp_min = 0.9f;
            float disp_max = 1.0f;
            float range = disp_max - disp_min;
            float density_dissipation = Mathf.Clamp(sim.Dissipation, disp_min, disp_max);
            density_dissipation = (density_dissipation - disp_min) / range;
            density_dissipation = EditorGUILayout.Slider("Particle Life", density_dissipation, 0.0f, 1.0f);
            sim.Dissipation = density_dissipation * range + disp_min;
        }

		EditorUtility.SetDirty(sim);
	}
}
