using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarMarkFX : MonoBehaviour {

	public Animator eyeAnim;
	public float timeBeforeChange;
	public Material crystalOff;
	public List<MeshRenderer> crystalsTransforming = new List<MeshRenderer>();
	public List<MeshRenderer> crystalsImmediate = new List<MeshRenderer>();

	public List<ParticleSystemRenderer> crystalParticles = new List<ParticleSystemRenderer>();


	public void GetMark()
	{
		eyeAnim.SetBool ("marked", true);
		StartCoroutine (EndAnimation ());
	}
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.H)) {
			GetMark ();
		}	
	}

	IEnumerator EndAnimation()
	{
		yield return new WaitForSecondsRealtime (timeBeforeChange);
		foreach (MeshRenderer ms in crystalsImmediate) {
			ms.material = crystalOff;
		}
		for (int i = 0; i < 400; i++) {
			yield return new WaitForSeconds (0.01f);
			foreach (MeshRenderer ms in crystalsTransforming) {
				Material mat = ms.material;
				mat.SetFloat ("_Transition", mat.GetFloat ("_Transition") - 0.1f);
			}
		}
		foreach (ParticleSystemRenderer psr in crystalParticles) {
			psr.material = crystalOff;
		}
	}

}
