using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesTeleporter : MonoBehaviour {

	Vector3 velocity;
	//public Transform target;
	//Vector3 _startPosition;
	//Vector3 _startEuler;
	public List<ParticleSystem> particles;

	// Use this for initialization
	void Awake () {
		//_startEuler = transform.localEulerAngles;
		//_startPosition = transform.localPosition;
	}
	
	// Update is called once per frame
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


	public void SetVelocity(Vector3 v)
	{
		velocity = v;
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
