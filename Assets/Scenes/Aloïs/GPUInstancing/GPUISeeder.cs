using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

	//public Color grassColor;
	SurfaceTextureHolder surfaceTexHolder;

	Color colorOK = new Color (.3f, .65f, 1f, .5f);
	Color colorRebake = new Color (1f, .6f, .2f, .5f);
	Color colorNeverBaked = new Color (1f, .31f, .31f, .5f);

	[HideInInspector] public Color _paintColor = new Color (.1f, .72f, .03f);
	[HideInInspector] public bool _colorsWerePainted;

	public enum GrowthMode{Mesh, StraightUp};
	public GrowthMode grassRotation = GrowthMode.Mesh;

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
		Quaternion _rotation = transform.rotation;
		Vector3  _vec = transform.up;

		if (grassRotation == GrowthMode.StraightUp) {

			_rotation = transform.position.x < 0f ? Quaternion.Euler (Vector3.back * 90f) : Quaternion.Euler (Vector3.forward * 90f);
			_vec = Vector3.right;
			//Debug.Log (_rotation);
		}

		for (int i = 0; i < _mesh.vertexCount; i++) {
			//Debug.Log (_mesh.colors.Length);
			if(_mesh.colors.Length == 0 || _mesh.colors[i].a == 0)
				transforms.Add(Matrix4x4.TRS (objToWorld.MultiplyPoint (_mesh.vertices [i]), Quaternion.AngleAxis(SuperRandomnessRotation(instanceID, i), _vec) * _rotation, Vector3.one * SuperRandomness(instanceID, _mesh.vertexCount, i)));

		}

		matrix0 = Matrix4x4.TRS (objToWorld.MultiplyPoint (_mesh.vertices [0]), Quaternion.AngleAxis(SuperRandomnessRotation(instanceID, 0), _vec) * _rotation, Vector3.one * SuperRandomness (instanceID, _mesh.vertexCount, 0));
	}

	float SuperRandomness (int _id, int _vertCount, int _vertIndex){
		//float rng = 1f + ((float)_id % 10f) * .1f * .2f + ((float)_vertIndex / (float)_vertCount) * .6f - (.6f * .5f);
		float rng = 1f + ((float)_id % 10f) * .1f * .2f + (((float)_vertIndex % 10f) / 10f) * .6f - (.6f * .5f);
		return rng;
	}

	float SuperRandomnessRotation(int _id, int _vertIndex){
		//returns a value between 0 and 360
		float rng = 360f * (.5f * ((float)_id % 10) * .1f + ((float)_vertIndex % 10f) * .1f * .5f);
		return rng;
	}


	public int AreMatricesUpToDate(){
		//0 : baked
		//1 : rebake needed
		//2 : never baked
		int bakingState = 0;

		if (matrix0 == Matrix4x4.zero) {
			gizmoColor = colorNeverBaked;
			bakingState = 2;
		} else {
			Matrix4x4 _objToWorld = transform.localToWorldMatrix;
			Mesh _mesh = GetComponent<MeshFilter> ().sharedMesh;
			Quaternion _rotation = transform.rotation;

			if (grassRotation == GrowthMode.StraightUp) {
				_rotation = transform.position.x < 0f ? Quaternion.Euler (Vector3.back * 90f) : Quaternion.Euler (Vector3.forward * 90f);
				//Debug.Log (_rotation);
			}

			Matrix4x4 _current0 = Matrix4x4.TRS (_objToWorld.MultiplyPoint (_mesh.vertices [0]), _rotation, Vector3.one * SuperRandomness(instanceID, _mesh.vertexCount, 0));

			gizmoColor = matrix0 == _current0 ? colorOK : colorRebake;
			bakingState = matrix0 == _current0 ? 0 : 1;
		}

		return bakingState;
	}


	public void PaintSurfaceTexture(bool _vertexPainted, Color _grassColor){

		surfaceTexHolder = (SurfaceTextureHolder)Resources.Load ("ScriptableObjects/GrassColorMaps");
		int _mapRes = surfaceTexHolder.mapResolution;

		if (surfaceTexHolder == null) {
			Debug.LogError ("GrassColorMaps scriptable object could not be found at resources/scriptableobjects.");
			return;
		}

		Vector3 v3Holder = Vector3.zero;
		Vector2 v2Holder = Vector2.zero;
		//get worldpos of vertices
		//get vertex color
		//map worldpos to uvpos
		//paint pixels

		Mesh _mesh = GetComponent<MeshFilter> ().sharedMesh;
		Matrix4x4 _objToWorld = transform.localToWorldMatrix;

		if (_vertexPainted) {
			if (_mesh.colors.Length == 0) {
				Debug.LogError ("The mesh you're trying to paint with has no vertex colour.");
			} else {

				for (int i = 0; i < _mesh.vertexCount; i++) {
					if (_mesh.colors [i].a != 1f) {
						v3Holder = _objToWorld.MultiplyPoint (_mesh.vertices [i]);
						v2Holder = new Vector2 (((v3Holder.z + 250f) / 500) * _mapRes, ((v3Holder.y + 250f) / 500) * _mapRes);

						if (transform.position.x < 0f) {
							surfaceTexHolder.westTex.SetPixel ((int)v2Holder.x, (int)v2Holder.y, _mesh.colors [i]);
						} else {
							surfaceTexHolder.eastTex.SetPixel ((int)v2Holder.x, (int)v2Holder.y, _mesh.colors [i]);
						}
					}
				}
				#if UNITY_EDITOR
				if (transform.position.x < 0f) {
					surfaceTexHolder.westTex.Apply ();
					SaveTexture (surfaceTexHolder.westTex, AssetDatabase.GetAssetPath (surfaceTexHolder.westTex));
				} else {
					surfaceTexHolder.eastTex.Apply ();
					SaveTexture (surfaceTexHolder.eastTex, AssetDatabase.GetAssetPath (surfaceTexHolder.eastTex));
				}
				#endif
			}
		} else {
			for (int i = 0; i < _mesh.vertexCount; i++) {
				v3Holder = _objToWorld.MultiplyPoint (_mesh.vertices [i]);
				v2Holder = new Vector2 (((v3Holder.z + 250f) / 500) * _mapRes, ((v3Holder.y + 250f) / 500) * _mapRes);

				if (transform.position.x < 0f) {
					surfaceTexHolder.westTex.SetPixel ((int)v2Holder.x, (int)v2Holder.y, _grassColor);
				} else {
					surfaceTexHolder.eastTex.SetPixel ((int)v2Holder.x, (int)v2Holder.y, _grassColor);
				}
			}
			#if UNITY_EDITOR
			if (transform.position.x < 0f) {
				surfaceTexHolder.westTex.Apply ();
				SaveTexture (surfaceTexHolder.westTex, AssetDatabase.GetAssetPath (surfaceTexHolder.westTex));
			} else {
				surfaceTexHolder.eastTex.Apply ();
				SaveTexture (surfaceTexHolder.eastTex, AssetDatabase.GetAssetPath (surfaceTexHolder.eastTex));
			}
			#endif
		}
	}


	void OnDrawGizmosSelected(){
		Gizmos.color = gizmoColor;

		Gizmos.DrawWireCube (transform.position, Vector3.one * 3f);

		Color _cubeColor = new Color (gizmoColor.r, gizmoColor.g, gizmoColor.b, .2f);
		Gizmos.color = _cubeColor;

		Gizmos.DrawCube (transform.position, Vector3.one * 3f);
	}

	#if UNITY_EDITOR
	void SaveTexture(Texture2D _tex, string _path){
		byte[] bytes = _tex.EncodeToPNG ();
		System.IO.File.WriteAllBytes (_path, bytes);
	}
	#endif
}
