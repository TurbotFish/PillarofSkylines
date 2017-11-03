﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class WindTunnel : BezierSpline {

	public AnimationCurve windStrength;
	public AnimationCurve tunnelAttraction;
	public int precision = 20;
	public int colliderPrecision = 3;
	public AnimationCurve colliderRadius;
	private LineRenderer lr;
	private WindTunnelPart partPrefab;
	private WindTunnelPart currentPart;
	private GameObject[] children = new GameObject[0];


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

	public void UpdateColliders() {
		partPrefab = Resources.Load<WindTunnelPart>("Prefabs/WindTunnelPart");
		Vector3 position = Vector3.zero;
		Vector3 nextPosition = Vector3.zero;

		Debug.Log("Destroying " + transform.childCount + " objects, instancing " + (colliderPrecision-1));


		children = new GameObject[transform.childCount];
		for (int i = 0; i < transform.childCount; i++)
		{
			children[i] = transform.GetChild(i).gameObject;
		}
		foreach (GameObject go in children)
		{
			GameObject.DestroyImmediate(go.gameObject);
		}


		for (int i = 0; i < colliderPrecision-1; i++)
		{
			position = GetPoint((float)i / (float)Mathf.Clamp((colliderPrecision - 1), 0, colliderPrecision));
			nextPosition = GetPoint((float)(i+1) / (float)Mathf.Clamp((colliderPrecision - 1), 0, colliderPrecision));
			currentPart = Instantiate<WindTunnelPart>(partPrefab, position + (nextPosition - position), Quaternion.LookRotation(nextPosition - position), transform);
			currentPart.transform.Rotate(90f, 0f, 0f);
			currentPart.GetComponent<CapsuleCollider>().radius = colliderRadius.Evaluate((float)i/colliderPrecision);
			currentPart.GetComponent<CapsuleCollider>().height = (nextPosition - position).magnitude/2;
			currentPart.windStrength = windStrength.Evaluate((float)i/colliderPrecision);
			currentPart.tunnelAttraction = tunnelAttraction.Evaluate((float)i/colliderPrecision);
			currentPart.idInTunnel = i;
		}
	}
}