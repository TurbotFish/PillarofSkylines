using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBuilder : MonoBehaviour {

	public float floorHeight;
	public List<GameObject> stories = new List<GameObject>();
	List<GameObject> oldStories = new List<GameObject>();
	public GameObject roof;
	GameObject oldRoof;

	
	// Update is called once per frame
	public void UpdateBuilding () {
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
}
