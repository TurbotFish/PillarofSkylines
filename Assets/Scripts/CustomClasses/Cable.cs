using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class Cable : BezierSpline {

	private int precision = 20;
	private LineRenderer lr;

	public void SetPrecision(int f)
	{
		precision = f;
	}

	public void UpdateLR()
	{
		lr = GetComponent<LineRenderer> ();
		Vector3[] lrPoints = new Vector3[precision];

		for (int i = 0; i < precision; i++) {
			lrPoints [i] = GetPoint ((float)i / (float)Mathf.Clamp((precision-1),0,precision));
		}

		lr.positionCount = lrPoints.Length;
		lr.SetPositions (lrPoints);
	}
}
