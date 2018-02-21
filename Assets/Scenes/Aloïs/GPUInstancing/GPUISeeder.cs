using UnityEngine;
using System.Collections.Generic;

public class GPUISeeder : MonoBehaviour {

	[SerializeField, HideInInspector]
	List<Matrix4x4> transforms = new List<Matrix4x4> ();

	//public Mesh meshToDraw;
	//public Material materialToDraw;

	GPUIDisplayManager manager;
	int instanceID;

	Color gizmoColor = Color.clear;

	[SerializeField, HideInInspector]
	Matrix4x4 matrix0;

	public Color grassColor;

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

			if(_mesh.colors[i].a == 0)
				transforms.Add(Matrix4x4.TRS (objToWorld.MultiplyPoint (_mesh.vertices [i]), transform.rotation, Vector3.one * SuperRandomness(instanceID, _mesh.vertexCount, i)));

		}
		matrix0 = Matrix4x4.TRS (objToWorld.MultiplyPoint (_mesh.vertices [0]), transform.rotation, Vector3.one * SuperRandomness (instanceID, _mesh.vertexCount, 0));
	}

	float SuperRandomness (int _id, int _vertCount, int _vertIndex){
		//float rng = 1f + ((float)_id % 10f) * .1f * .2f + ((float)_vertIndex / (float)_vertCount) * .6f - (.6f * .5f);
		float rng = 1f + ((float)_id % 10f) * .1f * .2f + (((float)_vertIndex % 10f) / 10f) * .6f - (.6f * .5f);
		return rng;
	}


	public Color AreMatricesUpToDate(){
		Color colorOK = new Color (.3f, .65f, 1f, .5f);
		Color colorRebake = new Color (1f, .6f, .2f, .5f);
		Color colorNeverBaked = new Color (1f, .31f, .31f, .5f);


		if (matrix0 == Matrix4x4.zero) {
			gizmoColor = colorNeverBaked;
		} else {
			Matrix4x4 _objToWorld = transform.localToWorldMatrix;
			Mesh _mesh = GetComponent<MeshFilter> ().sharedMesh;

			Matrix4x4 _current0 = Matrix4x4.TRS (_objToWorld.MultiplyPoint (_mesh.vertices [0]), transform.rotation, Vector3.one * SuperRandomness(instanceID, _mesh.vertexCount, 0));

			gizmoColor = matrix0 == _current0 ? colorOK : colorRebake;
		}

		return gizmoColor;
	}


	public void PaintSurfaceTexture(){
		//SurfaceTextureHolder.eastTex = Texture2D.blackTexture;
		Debug.Log ("I'M PAINTING");
	}


	void OnDrawGizmosSelected(){
		Gizmos.color = gizmoColor;

		Gizmos.DrawWireCube (transform.position, Vector3.one * 3f);

		Color _cubeColor = new Color (gizmoColor.r, gizmoColor.g, gizmoColor.b, .2f);
		Gizmos.color = _cubeColor;

		Gizmos.DrawCube (transform.position, Vector3.one * 3f);
	}
}
