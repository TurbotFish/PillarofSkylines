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
		if (lockLookAt && target != null) {
			cam.transform.LookAt (target);
		}
	

	}

	public void TeleportCameraCloudsSide(int i)
	{
		cloudController.CloudsAppearFromSide (3);
		StartCoroutine (FadeWhite (1.2f));
		StartCoroutine (TeleportCamera (i, 2f));
	}

	public void TeleportCameraCloudsDisappear(int i)
	{
		cloudController.CloudsDisappear (1);
		StartCoroutine (FadeWhite (0f));
		StartCoroutine (TeleportCamera (i, 0.75f));
	}

	public void TeleportCamera(int i)
	{
		
		StartCoroutine (FadeWhite (0));
		StartCoroutine (TeleportCamera (i, 0.75f));
	}

	public void SetTarget(Transform t)
	{
		target = t;
		lockLookAt = true;
	}
	public void RemoveTarget()
	{
		target = null;
		lockLookAt = false;
	}

	IEnumerator TeleportCamera(int pos, float t)
	{
		yield return new WaitForSecondsRealtime (t);
		cam.transform.position = positions [pos].position;
		cam.transform.rotation = positions [pos].rotation;
	}
	IEnumerator FadeWhite( float t)
	{
		yield return new WaitForSecondsRealtime (t);
		DOTween.Restart ("Fade_White");
	}

}
