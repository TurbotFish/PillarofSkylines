using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadJPP : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		SceneManager.LoadScene ("JPP", LoadSceneMode.Additive);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			SceneManager.UnloadSceneAsync ("JPP");
			SceneManager.LoadScene("JPP", LoadSceneMode.Additive);
		}
	}
}
