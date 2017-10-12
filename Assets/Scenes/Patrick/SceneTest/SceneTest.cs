using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTest : MonoBehaviour
{
	bool isLoadingScene2 = false;
	bool isScene2Loaded = false;

	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad (this.gameObject);
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.G) && !this.isScene2Loaded && !this.isLoadingScene2) {
			Debug.Log ("'G' down!");
			this.isLoadingScene2 = true;
			StartCoroutine (LoadScene2Routine ());
		} else if (Input.GetKeyDown (KeyCode.H) && this.isScene2Loaded) {
			StartCoroutine (ActivateScene1Routine ());
		}
	}

	IEnumerator LoadScene2Routine ()
	{
		yield return null;

		Debug.Log ("started loading scene 2!");
		var ao = SceneManager.LoadSceneAsync ("SceneTestScene2", LoadSceneMode.Additive);
		ao.allowSceneActivation = false;

		while (!ao.isDone) {

			if (ao.progress == 0.9f) {
				Debug.Log ("activating scene 2!");
				ao.allowSceneActivation = true;

				this.isScene2Loaded = true;
				this.isLoadingScene2 = false;

				yield return null;

				//this makes scene2 active
				SceneManager.SetActiveScene (SceneManager.GetSceneByName ("SceneTestScene2"));

				yield return null;

				foreach (var go in SceneManager.GetSceneByName ("SceneTestScene1").GetRootGameObjects ()) {
					go.SetActive (false);
				}

				break;
			}
		}
	}

	IEnumerator ActivateScene1Routine ()
	{
		yield return null;

		foreach (var go in SceneManager.GetSceneByName ("SceneTestScene2").GetRootGameObjects ()) {
			go.SetActive (false);
		}

		yield return null;

		foreach (var go in SceneManager.GetSceneByName ("SceneTestScene1").GetRootGameObjects ()) {
			go.SetActive (true);
		}

		yield return null;

		SceneManager.SetActiveScene (SceneManager.GetSceneByName ("SceneTestScene1"));

		yield return null;

		SceneManager.UnloadSceneAsync ("SceneTestScene2");

		yield return null;

		this.isScene2Loaded = false;
	}
}
