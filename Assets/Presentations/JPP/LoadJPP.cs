using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadJPP : MonoBehaviour {

	public bool loadScene;
	//Game.World.New.WorldController worldController;
	//public Game.World.ChunkSystem.WorldController WorldController { get { return this.worldController; } }
	Camera mainCam;
	// Use this for initialization
	void Awake () {
		//this.worldController = FindObjectOfType<Game.World.New.WorldController>();
		if (loadScene)
			SceneManager.LoadScene ("JPP", LoadSceneMode.Additive);


	}

	void Start()
	{
		StartCoroutine (InitializeWC (0));

		
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
		//	SceneManager.UnloadSceneAsync ("JPP");
			//SceneManager.UnloadSceneAsync("
		//	SceneManager.LoadScene("JPP", LoadSceneMode.Additive);
		//	StartCoroutine (InitializeWC (0.5f));
		}
	}


	IEnumerator InitializeWC(float t)
	{
		yield return new WaitForSecondsRealtime (t);
		mainCam = Camera.main;

        Debug.LogError("Not working anymore! Patrick");
		//worldController.InitializeWorldController(mainCam.transform, mainCam.transform);
	}
}
