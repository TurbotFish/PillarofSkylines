using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraTransition : MonoBehaviour {

	public Transform targetCamera;
	public Transform mainCameraParent;
	public SpriteRenderer backgroundAttenuation;
	public float transitionTime = 3;
	private Vector3 _cameraBasePosition;
	private Vector3 _cameraBaseEuler;

	void Start () {
		_cameraBasePosition = mainCameraParent.position;
		_cameraBaseEuler = mainCameraParent.rotation.eulerAngles + new Vector3(0,4,0);
	}
	/*void Update()
	{
		_cameraBasePosition = transform.position;
		_cameraBaseEuler = transform.rotation.eulerAngles;
	}*/
	
	public void Transition(bool _dir)
	{
		if (_dir) {
			transform.DOMove (targetCamera.position, transitionTime).SetEase (Ease.OutSine);
			transform.DORotate (targetCamera.rotation.eulerAngles, transitionTime).SetEase (Ease.OutSine);
			backgroundAttenuation.DOFade (0, transitionTime).SetEase (Ease.InSine);
		} else {
			transform.position = targetCamera.position;
			transform.rotation = targetCamera.rotation;
			transform.DOMove (_cameraBasePosition, transitionTime).SetEase (Ease.OutSine);
			transform.DORotate(_cameraBaseEuler,transitionTime).SetEase (Ease.OutSine);
			backgroundAttenuation.DOFade (0.274f, transitionTime).SetEase (Ease.InSine);
		}
	}
}
