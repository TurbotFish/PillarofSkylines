using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBuilder : MonoBehaviour {

	public float floorHeight;
	public List<GameObject> stories = new List<GameObject>();
	List<GameObject> oldStories = new List<GameObject>();
	public GameObject roof;
	GameObject oldRoof;

	public List<GameObject> randomStories = new List<GameObject>();
	public List<GameObject> randomRooves = new List<GameObject>();

	
	// Update is called once per frame
	public void CreateBuilding () {
		foreach (GameObject go in oldStories) {
			DestroyImmediate (go);
		}
		DestroyImmediate (oldRoof);
		oldStories.Clear();
		for (int i = 0; i < stories.Count; i++) {
			GameObject bat = Instantiate (stories[i], this.transform.position, Quaternion.identity, this.transform) as GameObject;
			bat.transform.localPosition = new Vector3 (0, i * floorHeight,0);
			bat.transform.rotation = transform.rotation;
			oldStories.Add (bat);
		}
		GameObject batRoof = Instantiate (roof, this.transform.position, Quaternion.identity, this.transform) as GameObject;
		batRoof.transform.localPosition = new Vector3(0,(stories.Count - 1)*floorHeight,0);
		batRoof.transform.rotation = transform.rotation;
		oldRoof = batRoof;
	}

	public void RandomBuilding () {
		foreach (GameObject go in oldStories) {
			DestroyImmediate (go);
		}
		DestroyImmediate (oldRoof);
		oldStories.Clear();
		for (int i = 0; i < stories.Count; i++) {
			int randStory = Random.Range (0, randomStories.Count);
			GameObject bat = Instantiate (randomStories[randStory], this.transform.position, Quaternion.identity, this.transform) as GameObject;
			bat.transform.localPosition = new Vector3 (0, i * floorHeight,0);
			bat.transform.rotation = transform.rotation;
			float p = Random.Range (0, 100);
			if (p > 50) {
				bat.transform.localEulerAngles += new Vector3 (0, 180, 0);
			}
			oldStories.Add (bat);
		}
		int randRoof = Random.Range (0, randomRooves.Count);
		GameObject batRoof = Instantiate (randomRooves[randRoof], this.transform.position, Quaternion.identity, this.transform) as GameObject;
		batRoof.transform.localPosition = new Vector3(0,(stories.Count - 1)*floorHeight,0);
		batRoof.transform.rotation = transform.rotation;
		float pbis = Random.Range (0, 100);
		if (pbis > 50) {
			batRoof.transform.localEulerAngles += new Vector3 (0, 180, 0);
		}
		oldRoof = batRoof;
	}

	public void ClearBuilding()
	{
		foreach (Transform child in transform) {
			if (child.CompareTag("Building"))
			{
				DestroyImmediate (child.gameObject);
			}
		}
		foreach (Transform child in transform) {
			if (child.CompareTag("Building"))
			{
				DestroyImmediate (child.gameObject);
			}
		}		
		foreach (Transform child in transform) {
			if (child.CompareTag("Building"))
			{
				DestroyImmediate (child.gameObject);
			}
		}

	}
}
