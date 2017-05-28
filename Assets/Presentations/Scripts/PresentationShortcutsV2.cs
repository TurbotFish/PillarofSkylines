using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PresentationShortcutsV2 : MonoBehaviour {


	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.Tab))
		{
			string _scene = SceneManager.GetActiveScene().name;
			SceneManager.LoadSceneAsync(_scene);
		}
	}
}
