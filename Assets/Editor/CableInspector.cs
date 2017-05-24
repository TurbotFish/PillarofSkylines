using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cable))]
public class CableInspector : Editor {

	private Cable spline;
	private Transform handleTransform;
	private Quaternion handleRotation;

	private const int lineSteps = 10;
	private const float directionScale = 0.5f;

	private bool showDebug = true;
	private int precision = 20;

	public override void OnInspectorGUI () {
		//DrawDefaultInspector();
		spline = target as Cable;
		showDebug = EditorGUILayout.Toggle("Show Debug", showDebug);

		EditorGUI.BeginChangeCheck ();
		precision = EditorGUILayout.IntField("Cable Precision",precision);
		if (EditorGUI.EndChangeCheck ()) {
			spline.SetPrecision (precision);
		}

		if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount) {
			DrawSelectedPointInspector();
		}
		if (GUILayout.Button("Extend Cable")) {
			Undo.RecordObject(spline, "Extend Cable");
			spline.AddCurve();
			EditorUtility.SetDirty(spline);
		}

		if (GUI.changed)
		{
			Undo.RecordObject(spline, "Change Show Debug");
			EditorUtility.SetDirty (spline);
		}
	}

	private void DrawSelectedPointInspector() {
		GUILayout.Label("Selected Point");
		EditorGUI.BeginChangeCheck();
		Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedIndex));
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(spline, "Move Point");
			EditorUtility.SetDirty(spline);
			spline.SetControlPoint(selectedIndex, point);
		}
		EditorGUI.BeginChangeCheck();
		BezierControlPointMode mode = (BezierControlPointMode)
			EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(spline, "Change Point Mode");
			spline.SetControlPointMode(selectedIndex, mode);
			EditorUtility.SetDirty(spline);
		}
	}


	private void OnSceneGUI ()
	{
		spline = target as Cable;
		handleTransform = spline.transform;
		handleRotation = Tools.pivotRotation == PivotRotation.Local?handleTransform.rotation:Quaternion.identity;

		if (showDebug == true)
		{
			Vector3 p0 = ShowPoint(0);
			for (int i = 1; i < spline.ControlPointCount; i += 3) {
				Vector3 p1 = ShowPoint(i);
				Vector3 p2 = ShowPoint(i + 1);
				Vector3 p3 = ShowPoint(i + 2);

				Handles.color = Color.gray;
				Handles.DrawLine(p0, p1);
				Handles.DrawLine(p2, p3);

				Handles.DrawBezier(p0, p3, p1, p2, Color.black, null, 2f);
				p0 = p3;
			}

			ShowDirections();
			if (GUI.changed || spline.transform.hasChanged) {
				spline.UpdateLR ();
			}
		}

	}


	private const float handleSize = 0.04f;
	private const float pickSize = 0.06f;
	private int selectedIndex = -1;
	private static Color[] modeColors = {
		Color.white,
		Color.yellow,
		Color.cyan
	};

	private Vector3 ShowPoint (int index) {
		Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
		float size = HandleUtility.GetHandleSize(point);
		Handles.color = modeColors[(int)spline.GetControlPointMode(index)];
		if (Handles.Button(point, handleRotation, size*handleSize,  size*pickSize, Handles.DotCap)) {
			selectedIndex = index;
			Repaint();
		}
		if (selectedIndex == index) {
			EditorGUI.BeginChangeCheck();
			point = Handles.DoPositionHandle(point, handleRotation);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(spline, "Move Point");
				EditorUtility.SetDirty(spline);
				spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
			}
		}
		return point;
	}



	private const int stepsPerCurve = 10;

	private void ShowDirections ()
	{
		Handles.color = Color.green;
		Vector3 point = spline.GetPoint(0f);
		Handles.DrawLine(point, point+spline.GetDirection(0f)*directionScale);
		int steps = stepsPerCurve * spline.CurveCount;
		for(int i = 1; i <= steps; i++)
		{
			point = spline.GetPoint(i/(float)steps);
			Handles.DrawLine(point,point+spline.GetDirection(i/(float)steps)*directionScale);
		}

	}

}
