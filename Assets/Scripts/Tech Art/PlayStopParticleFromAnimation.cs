using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayStopParticleFromAnimation : MonoBehaviour {

	public List<ParticleSystem> particles = new List<ParticleSystem>();

	public void PlayParticleSystem (int i) {
		particles[i].Play ();
	}
	public void StopParticleSystem (int i) {
		particles[i].Stop ();
	}
}
