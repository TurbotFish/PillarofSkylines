using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTunnelGraph : MonoBehaviour {

	public bool updateBonesOnStart;
	public Transform rootBone;
	public Transform[] bones;
	public BezierSpline spline;
	// Use this for initialization
	void Start () {
		if (updateBonesOnStart) {
			bones [0] = rootBone;
			for (int i = 1; i < bones.Length; i++) {
				bones [i] = bones [i - 1].GetChild (0);
			}
		}

		float stepSpline = 1f/bones.Length;

		for (int i = 0; i < bones.Length; i++) {
			bones [i].position = spline.GetPoint (i * stepSpline);
			if (i < bones.Length - 1) {
				bones [i].LookAt (spline.GetPoint ((i+1) * stepSpline));
				bones [i].Rotate (new Vector3 (0, 90, 0));
			} else {
				bones [i].rotation = bones [i - 1].rotation;
			}
		}
	}
	

}
