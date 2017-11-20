using UnityEngine;

//[ExecuteInEditMode]
public class VertexOffset : MonoBehaviour {

	public GameObject _go;

	Material _mat;

	void Start(){
		_mat = _go.GetComponent<Renderer> ().material;
	}

	void Update(){
		Vector4 _vec = new Vector4(transform.position.x, transform.position.y, transform.position.z, 0);
		_mat.SetVector ("_PlayerPos", _vec);

		//Shader.SetGlobalVector("_PlayerPos", _vec);
	}


}
