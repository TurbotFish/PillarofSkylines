using UnityEngine;

public class VertexOffset : MonoBehaviour {

	public GameObject _go;

	Material _mat;

	void Start(){
		_mat = _go.GetComponent<Renderer> ().material;
	}

	void Update(){
		_mat.SetVector ("_PlayerPos", new Vector4 (transform.position.x, transform.position.y, transform.position.z, 0));
	}


}
