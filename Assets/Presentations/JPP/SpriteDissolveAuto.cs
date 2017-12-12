using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDissolveAuto : MonoBehaviour {


	public MinMax dissolveAmount;
	public float speed;
	bool dissolve;
	bool appear;
	float counter;
	float startAmount;
	Material mat;

	void Awake()
	{
		if (GetComponent<Renderer> ().material != null) {
			mat = GetComponent<Renderer> ().material;
		} else {
			Debug.Log ("woops, something is wrong");
		}
	}
	void Update()
	{
		if (dissolve) {
			if (appear) {
				counter += Time.deltaTime*speed;
				mat.SetFloat ("_DissolveAmount", Mathf.Lerp (startAmount, dissolveAmount.min, counter));
			} else {
				counter += Time.deltaTime * speed;
				mat.SetFloat ("_DissolveAmount",Mathf.Lerp (startAmount, dissolveAmount.max, counter));
			}
		}
	}

	public void DissolveAppear()
	{
		appear = true;
		startAmount = mat.GetFloat ("_DissolveAmount");
		//counter = (startAmount - dissolveAmount.min) / (dissolveAmount.max - dissolveAmount.min);
		counter = 0;
		dissolve = true;
	}
	public void DissolveDisappear()
	{
		appear = false;
		startAmount = mat.GetFloat ("_DissolveAmount");
		//counter = (startAmount - dissolveAmount.min) / (dissolveAmount.max - dissolveAmount.min);
		counter = 0;
		dissolve = true;
	}
}
