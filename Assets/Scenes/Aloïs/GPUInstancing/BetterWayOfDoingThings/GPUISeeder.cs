using UnityEngine;
using System.Collections.Generic;

public class GPUISeeder : MonoBehaviour {

	[SerializeField, HideInInspector]
	List<Matrix4x4> transforms = new List<Matrix4x4> ();

	public Mesh meshToDraw;
	public Material materialToDraw;

	void OnBecameVisible(){
		//Debug.Log ("VISIBLE");
		GPUIDisplayManager.displayManager.AddStuffToDraw (transforms, transform.GetInstanceID());
	}

	void OnBecameInvisible(){
		//Debug.Log ("INVISIBLE");
		GPUIDisplayManager.displayManager.RemoveStuffToDraw (transforms, transform.GetInstanceID());
	}

	public void BakeGPUIData(){
		transforms.Clear ();
		Mesh _mesh = GetComponent<MeshFilter> ().sharedMesh;
		Matrix4x4 objToWorld = transform.localToWorldMatrix;

		for (int i = 0; i < _mesh.vertexCount; i++) {
			transforms.Add(Matrix4x4.TRS (objToWorld.MultiplyPoint (_mesh.vertices [i]), transform.rotation, Vector3.one));
		}

	}

	public void TestTest(){
		Debug.Log (transforms [0]);
	}
}
