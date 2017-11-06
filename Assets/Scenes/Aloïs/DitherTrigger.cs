using UnityEngine;
using System.Collections.Generic;

public class DitherTrigger : MonoBehaviour {

	Material mat;

	[Range(0,5)]
	public float rayLength = 2.5f;

	RaycastHit[] hits;
	List<Material> mats = new List<Material>();

	void Update(){

		GivePixelsBack ();

		ClearList ();

		CheckForObstruction ();
	}

	void CheckForObstruction(){
		Debug.DrawRay (transform.position, transform.forward * rayLength, Color.red);
		hits = Physics.RaycastAll (transform.position, transform.forward, rayLength);

		for (int i = 0; i < hits.Length; i++) {
			mat = hits[i].transform.GetComponent<Renderer> ().material;
			mat.SetShaderPassEnabled ("Always", false);
			Debug.Log ("hit " + hits [i].transform.name);
			mat.EnableKeyword ("_DITHER_OBSTRUCTION");
			mat.SetFloat ("_DistFromCam", hits[i].distance);
			mats.Add (mat);
		}
	}

	void GivePixelsBack(){
		if (mats.Count == 0)
			return;
		
		for (int i = 0; i < mats.Count; i++) {
			mats [i].DisableKeyword ("_DITHER_OBSTRUCTION");
			mat.SetShaderPassEnabled ("Always", true);
		}
	}

	void ClearList(){
		if (mats.Count == 0)
			return;
		
		mats.Clear ();
	}
}
