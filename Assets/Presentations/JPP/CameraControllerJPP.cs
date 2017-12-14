using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraControllerJPP : MonoBehaviour {
	public Camera cam;
	public bool lockLookAt;
	public Transform target;
	public List<Transform> positions = new List<Transform>();

	CloudsController cloudController; 
	// Use this for initialization
	void Awake () {
		cloudController = GetComponent<CloudsController> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.A))
			TestTeleportCamera (0);
		if (Input.GetKeyDown (KeyCode.B))
			TestTeleportCamera (1);
		if (Input.GetKeyDown (KeyCode.C))
			TestTeleportCamera (2);
	

	}

	public void TestTeleportCamera(int i)
	{
		DOTween.Restart ("Fade_White");
		cloudController.CloudsDisappear();
		StartCoroutine (TeleportCamera (i, 0.75f));
	}

	IEnumerator TeleportCamera(int pos, float t)
	{
		yield return new WaitForSecondsRealtime (t);
		cam.transform.position = positions [pos].position;
		cam.transform.rotation = positions [pos].rotation;
	}
}
