using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProtoSwitch : MonoBehaviour {


	public SceneField _scene;

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.P))
		{
			SceneManager.LoadSceneAsync (_scene);	
		}
	}
}
