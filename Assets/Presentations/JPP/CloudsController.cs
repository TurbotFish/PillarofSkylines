using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsController : MonoBehaviour {

	public List<CS_Cloud> clouds = new List<CS_Cloud>();
	public List<Vector3> outerPositions = new List<Vector3>();
	List<Vector3> defaultPositions = new List<Vector3>();
	List<Vector3> startPositions = new List<Vector3>();
	public float fadeSpeed;
	public float moveSpeed;
	public bool disappear;
	float counterA, counterB;

	[Range(0,1)]
	public float fadeAmount = 0.6f;
	float defaultFadeAmount;
	// Use this for initialization
	void Awake () {
		counterA = 1;
		counterB = 1;
		defaultFadeAmount = fadeAmount;
		for (int i = 0; i < clouds.Count; i++) {
			startPositions.Add(clouds [i].transform.localPosition);
			defaultPositions.Add(clouds [i].transform.localPosition);
		}
	}
	
	// Update is called once per frame
	void Update () {
		foreach (CS_Cloud cloud in clouds) {
			cloud.Fading = fadeAmount;
		}
		if (disappear) {
			counterA += Time.deltaTime*fadeSpeed;
			counterB += Time.deltaTime*moveSpeed;
			fadeAmount = Mathf.Lerp (fadeAmount, 0, counterA);
			for (int i = 0; i < clouds.Count; i++) {
				clouds [i].transform.localPosition = Vector3.Lerp (startPositions[i], outerPositions [i], counterB);
			}
		} else {
			counterA += Time.deltaTime*fadeSpeed;
			counterB += Time.deltaTime*moveSpeed;
			fadeAmount = Mathf.Lerp (0, defaultFadeAmount, counterA);
			for (int i = 0; i < clouds.Count; i++) {
				clouds [i].transform.localPosition = Vector3.Lerp (startPositions[i], defaultPositions [i], counterB);
			}
		}
	}

	public void CloudsDisappear ()
	{
		counterA = 0;
		counterB = 0;
		disappear = true;
		for (int i = 0; i < clouds.Count; i++) {
			startPositions[i] = clouds [i].transform.localPosition;
		}
	}
	public void CloudsAppear ()
	{
		counterA = 0;
		counterB = 0;
		disappear = false;
		for (int i = 0; i < clouds.Count; i++) {
			startPositions[i] = clouds [i].transform.localPosition;
		}
	}

}
