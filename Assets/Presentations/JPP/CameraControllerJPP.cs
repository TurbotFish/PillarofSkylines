﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Video;
using UnityEngine.UI;

public class CameraControllerJPP : MonoBehaviour {
	public Camera cam;
	public bool lockLookAt;
	public Transform target;
	public List<Transform> positions = new List<Transform>();
	public Transform layer1, layer2;
	public RawImage videoCanvas;
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
		StartCoroutine (TeleportCamera (i, 0.25f));
	}

	public void TeleporteCameraLayerLeftToRight(int i)
	{

		StartCoroutine (LayersLeftToRight (0));
		StartCoroutine (TeleportCamera (i, 1f));
	}

	public void TeleporteCameraLayerLeft(int i)
	{

		StartCoroutine (LayersLeft (0));
		StartCoroutine (TeleportCamera (i, 0.75f));
	}

	public void LayersLeftHop()
	{
		layer2.DOLocalMove (Vector3.zero, 1.3f).SetEase (Ease.OutQuad);
		layer1.DOLocalMove (Vector3.zero, 1.3f).SetEase (Ease.OutQuad).SetDelay(0.2f);
	}
	public void VideoLayerLeftToRight(VideoPlayer v)
	{

		StartCoroutine (LayersLeftToRight (0));
		StartCoroutine (LaunchVideo (v, 1f));
	}

	public void DisableVideoLAyer ()
	{
		videoCanvas.color = new Color (1, 1, 1, 0);
	}

	public void VideoLayerLeft(VideoPlayer v)
	{

		StartCoroutine (LayersLeft (0));
		StartCoroutine (LaunchVideo (v, 0.75f));
	}

	public void StopVideoLayerLeft(VideoPlayer v)
	{

		StartCoroutine (LayersLeft (0));
		StartCoroutine (StopVideo (v, 0.75f));
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

	IEnumerator LaunchVideo(VideoPlayer video, float t)
	{
		yield return new WaitForSecondsRealtime (t);
		videoCanvas.color = new Color (1, 1, 1, 1);
		video.Play ();
	}
	IEnumerator StopVideo(VideoPlayer video, float t)
	{
		yield return new WaitForSecondsRealtime (t);
		videoCanvas.color = new Color (1, 1, 1, 0);
		video.Stop ();
	}

	IEnumerator FadeWhite( float t)
	{
		yield return new WaitForSecondsRealtime (t);
		DOTween.Restart ("Fade_White");
	}
	IEnumerator LayersLeftToRight( float t)
	{
		yield return new WaitForSecondsRealtime (t);
		DOTween.Restart ("layers_lefttoright");
	}
	IEnumerator LayersLeft( float t)
	{
		yield return new WaitForSecondsRealtime (t);
		DOTween.Restart ("layers_left");
	}

	IEnumerator Pan (Vector3 target, float t)
	{
		yield return new WaitForSecondsRealtime (t);
		cam.transform.DOLocalMove (target, 10).SetRelative (true).SetEase (Ease.OutSine);
	}
}
