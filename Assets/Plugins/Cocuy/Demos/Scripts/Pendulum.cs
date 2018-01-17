using UnityEngine;
using System.Collections;

public class Pendulum : MonoBehaviour {

	public Transform pivot;
	public float speed = 0.5f;
	private float startAngle = 90.0f;
	private float endAngle = -90.0f;
	private float fTimer = 0.0f;
	private Vector3 v3T = Vector3.zero;

	void Update()
	{

		float f = (Mathf.Sin(fTimer * speed - Mathf.PI / 2.0f) + 1.0f) / 2.0f;
		v3T.Set(0.0f, 0.0f, Mathf.Lerp(startAngle, endAngle, f));
		pivot.eulerAngles = v3T;
		fTimer += Time.deltaTime;
	}
}
