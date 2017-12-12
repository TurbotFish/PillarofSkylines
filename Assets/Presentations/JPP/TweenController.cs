using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenController : MonoBehaviour {

	public string nextButton, previousButton;
	public int index;
	public List<string> anims = new List<string>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown (nextButton)  && index < anims.Count) {
			DOTween.Complete (anims [index]);
			index++;
			DOTween.Play (anims [index]);
		}
		if (Input.GetButtonDown (previousButton)&& index>0) {
			DOTween.Rewind (anims [index]);
			index--;
			DOTween.Play (anims [index]);
		}	
	}
}
