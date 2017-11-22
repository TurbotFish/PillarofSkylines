using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class Limb : BezierSpline {

	public int precision = 20;
	public float curvature = 2;
	public float exagerationFactor = 2;
	public bool invert;
	bool scaleInvert;
	//public float maxLength = 5;
	private LineRenderer lr;
	public Transform endPoint;
	public Transform root;
	public Transform arcPoint;

	void LateUpdate()
	{
		UpdateBezierControlPoints ();
		UpdateLR ();
	}

	//public void SetPrecision(int f)
	//{
	//	precision = f;
	//}

	public void UpdateLR()
	{
		lr = GetComponent<LineRenderer> ();
		Vector3[] lrPoints = new Vector3[precision];

		for (int i = 0; i < precision; i++) {
			lrPoints [i] = GetPoint ((float)i / (float)Mathf.Clamp((precision-1),0,precision));
		}

		lr.positionCount = lrPoints.Length;
		lr.SetPositions (lrPoints);

		//SetControlPoint (0, arcPoint.position);
	}

	public void UpdateBezierControlPoints()
	{
		if (endPoint != null) {
			Vector3 start, end, arc;
			start = transform.InverseTransformPoint (transform.position);
			end = transform.InverseTransformPoint (endPoint.position);

			SetControlPoint (0, start);
			SetControlPoint (3, end);

			if (root.localScale.x > 0)
				scaleInvert = false;
			else
				scaleInvert = true;

			float _curvature = curvature * Mathf.Abs(root.localScale.x);
			if (invert)
				_curvature = -_curvature;
			if (scaleInvert)
				_curvature = -_curvature;

			//float curvatureFactor = 1 - Mathf.Clamp01((endPoint.position - transform.position).magnitude*exagerationFactor/Mathf.Abs(_curvature));
			//arc = Vector3.Lerp (endPoint.position, transform.position, 0.5f) + GetPerpendicularVector(endPoint.position-transform.position)*_curvature*curvatureFactor;
			arc = arcPoint.position;

			SetControlPoint (1, transform.InverseTransformPoint(Vector3.Lerp(arc,transform.position,0.5f)));
			SetControlPoint (2, transform.InverseTransformPoint(Vector3.Lerp(arc,endPoint.position,0.5f)));
		}
	}


	Vector3 GetPerpendicularVector(Vector3 v)
	{
		v.Normalize ();
		return new Vector3 (v.y, -v.x, v.z);
	}
}
