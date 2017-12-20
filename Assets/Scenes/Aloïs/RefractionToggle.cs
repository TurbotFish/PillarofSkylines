using UnityEngine;

public class RefractionToggle : MonoBehaviour {

	public GameObject mainCam;
	public GameObject player;
	Material mat;

	//JPP special
	public Material plantMat;

	void Start(){
		mat = player.GetComponent<Renderer> ().material;

		//JPP special
		mat.SetShaderPassEnabled ("Always", true);

		plantMat.EnableKeyword ("_WIND_ROT_Y");
	}

	void OnTriggerEnter(Collider camCollider){
		mat.SetShaderPassEnabled ("Always", true);
		//Debug.Log ("INSIDE");
	}

	void OnTriggerExit(Collider camCollider){
		mat.SetShaderPassEnabled ("Always", false);
		//Debug.Log ("OUTSIDE");
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube (transform.position, Vector3.one * 3);
	}



}
