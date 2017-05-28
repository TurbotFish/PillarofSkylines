using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProtoSwitch : MonoBehaviour {


	public bool inPrez = true;


	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.P))
		{
			if(inPrez)
			{
				SceneManager.LoadSceneAsync("grrTests");
			}	
			if(!inPrez)
			{
				SceneManager.LoadSceneAsync("PresentationJury");
			}	
		}
	}
}
