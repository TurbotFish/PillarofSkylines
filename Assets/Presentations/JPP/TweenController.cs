using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Video;
public class TweenController : MonoBehaviour {

	public string nextButton, previousButton;
	public VideoPlayer vPlayer;

	public int index;
	public List<string> anims = new List<string>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		/*
		if (Input.GetKeyDown (KeyCode.P) ) {
			if (!vPlayer.isPlaying) {
				vPlayer.targetCameraAlpha = 1;
				vPlayer.Play ();
			} else {
				vPlayer.targetCameraAlpha = 1;
				vPlayer.Stop ();
			}
		} 
		if (Input.GetKeyDown (KeyCode.Q) ) {
			vPlayer.targetCameraAlpha = 0;
			vPlayer.Stop ();

		} */
			
		if (Input.GetButtonDown (nextButton)  && index < anims.Count) {
			DOTween.Complete (anims [index]);
			index++;
			DOTween.Restart (anims [index]);
		}
		if (Input.GetButtonDown (previousButton)&& index>0) {
			DOTween.Rewind (anims [index]);
			index--;
			DOTween.Restart (anims [index]);
		}	
	}

}
