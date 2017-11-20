using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesTeleporter : MonoBehaviour {
	
	public List<ParticleSystem> particles;
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.P)) {
			HoldParticles ();
		}
		if (Input.GetKeyUp (KeyCode.P)) {
			ReleaseParticles();
		}	
	}

	public void HoldParticles()
	{
		for (int i = 0; i < particles.Count; i++) {
			var module = particles [i].main;
			module.simulationSpace = ParticleSystemSimulationSpace.Local;
		}
	}

	public void ReleaseParticles()
	{
		for (int i = 0; i < particles.Count; i++) {
			var module = particles [i].main;
			module.simulationSpace = ParticleSystemSimulationSpace.World;
		}
	}

	public void Stop()
	{
		foreach (ParticleSystem ps in particles) {
			ps.Stop ();
		}
	}

	public void Play()
	{
		foreach (ParticleSystem ps in particles) {
			ps.Play ();
		}
	}
}
