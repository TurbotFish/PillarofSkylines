using UnityEngine;

public class QuickRandomRotation : MonoBehaviour {

	void Start(){
		transform.LookAt (transform.position + Vector3.right);
		transform.localRotation = transform.localRotation * Quaternion.Euler (new Vector3 (0, Random.Range (0f, 360f), 0));

	}
}
