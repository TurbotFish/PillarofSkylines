using UnityEngine;

public class RefractionToggle : MonoBehaviour {

	public GameObject mainCam;
	public GameObject player;
	Material mat;

	void Start(){
		mat = player.GetComponent<Renderer> ().material;


	}

	void OnTriggerEnter(Collider camCollider){
		mat.SetShaderPassEnabled ("Always", true);
		Debug.Log ("INSIDE");
	}

	void OnTriggerExit(Collider camCollider){
		mat.SetShaderPassEnabled ("Always", false);
		Debug.Log ("OUTSIDE");
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube (transform.position, Vector3.one * 3);
	}



}
