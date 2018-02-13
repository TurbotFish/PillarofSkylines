using UnityEngine;
using System.Collections.Generic;

public class GPUISeeder : MonoBehaviour {

	[SerializeField, HideInInspector]
	List<Matrix4x4> transforms = new List<Matrix4x4> ();

	public Mesh meshToDraw;
	public Material materialToDraw;

	GPUIDisplayManager manager;
	int instanceID;

	void Start(){
		manager = GPUIDisplayManager.displayManager;
		instanceID = transform.GetInstanceID ();
	}

	void OnBecameVisible(){
		if(manager != null)
			GPUIDisplayManager.displayManager.AddStuffToDraw (transforms, instanceID);
	}

	void OnBecameInvisible(){
		if(manager != null)
			GPUIDisplayManager.displayManager.RemoveStuffToDraw (transforms, instanceID);
	}

	public void BakeGPUIData(){
		transforms.Clear ();
		Mesh _mesh = GetComponent<MeshFilter> ().sharedMesh;
		Matrix4x4 objToWorld = transform.localToWorldMatrix;

		for (int i = 0; i < _mesh.vertexCount; i++) {
			transforms.Add(Matrix4x4.TRS (objToWorld.MultiplyPoint (_mesh.vertices [i]), transform.rotation, Vector3.one));
		}

	}
}
