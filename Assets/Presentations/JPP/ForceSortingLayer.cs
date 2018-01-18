using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceSortingLayer : MonoBehaviour {

	public string newSortingLayer;
	// Use this for initialization
	void Start () {
		GetComponent<MeshRenderer> ().sortingLayerName = newSortingLayer;
		GetComponent<MeshRenderer> ().sortingOrder = 0;

	}

	void Update()
	{
		if (GetComponent<MeshRenderer> ().sortingLayerName != newSortingLayer)
			GetComponent<MeshRenderer> ().sortingLayerName = newSortingLayer;
		if (Input.GetKeyDown(KeyCode.Space))
			Debug.Log(GetComponent<MeshRenderer>().sortingLayerName + " : " +GetComponent<MeshRenderer>().sortingLayerID);
	}
	

}
