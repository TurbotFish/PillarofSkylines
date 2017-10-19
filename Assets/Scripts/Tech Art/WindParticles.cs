using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindParticles : MonoBehaviour {

	public Vector3 velocity;
	public float maxVelocity;
	public List<ParticleSystem> particles;
	public List<Vector3> thresholds;

	// Use this for initialization
	void Awake () {

	}

	void Update()
	{
		float _velmag = velocity.magnitude;
		float _ratio = _velmag / maxVelocity;

		for (int i = 0; i < thresholds.Count; i++) {
			if (i < particles.Count) {

				if (_ratio > thresholds [i].x) {
					if(!particles[i].isPlaying)
					{
						particles [i].Play ();
						//Debug.Log ("play : " + i);
					}
		
				} else if (particles[i].isPlaying) {
					particles [i].Stop ();
					//Debug.Log ("stop : " + i);
				}
			}
		}

		for (int i = 0; i < thresholds.Count; i++) {
			if (i < particles.Count) {
				//var emissionModule = particles [i].emission;
				//emissionModule.rateOverTime = Mathf.RoundToInt(Mathf.SmoothStep (thresholds [i].y, thresholds [i].z, _ratio));
				var velocityModule = particles [i].main;
				velocityModule.simulationSpeed = Mathf.SmoothStep (thresholds [i].y, thresholds [i].z, _ratio);
			}
		}



	}

	void LateUpdate () {

		transform.LookAt(transform.position + velocity.normalized*10);
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
