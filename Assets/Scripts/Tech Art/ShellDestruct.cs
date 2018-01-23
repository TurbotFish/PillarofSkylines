using UnityEngine;

public class ShellDestruct : MonoBehaviour {

	bool destroyed;
	public GameObject model;
	public ParticleSystem particles;

	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag ("Player") && !destroyed) {
			destroyed = true;
			model.SetActive (false);
			particles.Play ();
			Destroy (this, 5);
		}
	}
}
