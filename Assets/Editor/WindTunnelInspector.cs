using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WindTunnel))]
public class WindTunnelInspector : Editor {

	private WindTunnel tunnel;
	private Transform handleTransform;
	private Quaternion handleRotation;

	private const int lineSteps = 10;
	private const float directionScale = 0.5f;

	private bool showDebug = true;

	public override void OnInspectorGUI () {
		DrawDefaultInspector();

		tunnel = target as WindTunnel;
		/*
		precision = tunnel.GetPrecision();
		colliderPrecision = tunnel.GetColliderPrecision();
		colliderRadius = tunnel.GetColliderRadius();
		windStrength = tunnel.GetWindStrength();
		showDebug = EditorGUILayout.Toggle("Show Debug", showDebug);

		EditorGUI.BeginChangeCheck ();
		precision = EditorGUILayout.PropertyField("Renderer Precision", precision);
		if (EditorGUI.EndChangeCheck ()) {
			tunnel.SetPrecision (precision);
		}

		EditorGUI.BeginChangeCheck ();
		colliderPrecision = EditorGUILayout.IntField("Collider Precision", colliderPrecision);
		if (EditorGUI.EndChangeCheck ()) {
			tunnel.SetColliderPrecision (colliderPrecision);
		}

		EditorGUI.BeginChangeCheck ();
		colliderRadius = EditorGUILayout.FloatField("Collider Radius", colliderRadius);
		if (EditorGUI.EndChangeCheck ()) {
			tunnel.SetColliderRadius(colliderRadius);
		}

		EditorGUI.BeginChangeCheck ();
		windStrength = EditorGUILayout.FloatField("Wind Strength", windStrength);
		if (EditorGUI.EndChangeCheck ()) {
			tunnel.SetWindStrength(windStrength);
		}*/


		if (selectedIndex >= 0 && selectedIndex < tunnel.ControlPointCount) {
			DrawSelectedPointInspector();
		}
		if (GUILayout.Button("Extend Tunnel")) {
			Undo.RecordObject(tunnel, "Extend Tunnel");
			tunnel.AddCurve();
			EditorUtility.SetDirty(tunnel);
		}

		if (GUILayout.Button("Update Wind Colliders")) {
			Undo.RecordObject(tunnel, "Update Wind Colliders");
			tunnel.UpdateColliders();
			EditorUtility.SetDirty(tunnel);
		}

		if (GUI.changed)
		{
			Undo.RecordObject(tunnel, "Change Show Debug");
			EditorUtility.SetDirty (tunnel);
		}
	}

	private void DrawSelectedPointInspector() {
		GUILayout.Label("Selected Point");
		EditorGUI.BeginChangeCheck();
		Vector3 point = EditorGUILayout.Vector3Field("Position", tunnel.GetControlPoint(selectedIndex));
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(tunnel, "Move Point");
			EditorUtility.SetDirty(tunnel);
			tunnel.SetControlPoint(selectedIndex, point);
		}
		EditorGUI.BeginChangeCheck();
		BezierControlPointMode mode = (BezierControlPointMode)
			EditorGUILayout.EnumPopup("Mode", tunnel.GetControlPointMode(selectedIndex));
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(tunnel, "Change Point Mode");
			tunnel.SetControlPointMode(selectedIndex, mode);
			EditorUtility.SetDirty(tunnel);
		}
	}


	private void OnSceneGUI ()
	{
		tunnel = target as WindTunnel;
		handleTransform = tunnel.transform;
		handleRotation = Tools.pivotRotation == PivotRotation.Local?handleTransform.rotation:Quaternion.identity;

		if (showDebug == true)
		{
			Vector3 p0 = ShowPoint(0);
			for (int i = 1; i < tunnel.ControlPointCount; i += 3) {
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
			if (GUI.changed || tunnel.transform.hasChanged) {
				tunnel.UpdateLR ();
			}
		}

	}


	private const float handleSize = 0.04f;
	private const float pickSize = 0.06f;
	private int selectedIndex = -1;
	private static Color[] modeColors = {
		Color.cyan,
		Color.yellow,
		Color.red
	};

	private Vector3 ShowPoint (int index) {
		Vector3 point = handleTransform.TransformPoint(tunnel.GetControlPoint(index));
		float size = HandleUtility.GetHandleSize(point);
		Handles.color = modeColors[(int)tunnel.GetControlPointMode(index)];
		if (Handles.Button(point, handleRotation, size*handleSize,  size*pickSize, Handles.DotHandleCap)) {
			selectedIndex = index;
			Repaint();
		}
		if (selectedIndex == index) {
			EditorGUI.BeginChangeCheck();
			point = Handles.DoPositionHandle(point, handleRotation);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(tunnel, "Move Point");
				EditorUtility.SetDirty(tunnel);
				tunnel.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
			}
		}
		return point;
	}



	private const int stepsPerCurve = 10;

	private void ShowDirections ()
	{
		Handles.color = Color.green;
		Vector3 point = tunnel.GetPoint(0f);
		Handles.DrawLine(point, point+tunnel.GetDirection(0f)*directionScale);
		int steps = stepsPerCurve * tunnel.CurveCount;
		for(int i = 1; i <= steps; i++)
		{
			point = tunnel.GetPoint(i/(float)steps);
			Handles.DrawLine(point,point+tunnel.GetDirection(i/(float)steps)*directionScale);
		}

	}

}
