using UnityEngine;

public class GrassGPU : MonoBehaviour {

	public GameObject obj;
	public Material mat;
	Matrix4x4[] transforms;
	public Mesh meshToDraw;
	public float scaleFactor = 1f;

	int batchSize = 1023;

	void Start(){
		Mesh _mesh = obj.GetComponent<MeshFilter> ().mesh;

		if (_mesh == null) {
			Debug.LogError ("need a mesh with vertices");
			return;
		}

		transforms = new Matrix4x4[_mesh.vertexCount];
		Matrix4x4 _objMatrix = obj.transform.localToWorldMatrix;
		Quaternion _objRotation = obj.transform.rotation;
		Vector3 _objScale = Vector3.one * scaleFactor;




		for (int i = 0; i < _mesh.vertexCount; i++) {
			transforms [i] = Matrix4x4.TRS (_objMatrix.MultiplyPoint(_mesh.vertices [i]), _objRotation, _objScale);

		}

		batchSize = _mesh.vertexCount <= batchSize ? _mesh.vertexCount : batchSize;
	}

	void Update(){
		DrawAllTheMeshes ();
	}



	void DrawAllTheMeshes(){
		Graphics.DrawMeshInstanced (meshToDraw, 0, mat, transforms, batchSize, null, UnityEngine.Rendering.ShadowCastingMode.Off, true);
	}

}
