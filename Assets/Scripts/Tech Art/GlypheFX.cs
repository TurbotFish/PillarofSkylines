using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GlypheFX : MonoBehaviour {

	[Header("General")]
	public float timeToActivate;
	public MeshRenderer glypheOn;

	[Header("Crystals")]
	public Animator anim;
	public Material crystalOn, crystalOff;
	public List<MeshRenderer> crystalRenderers = new List<MeshRenderer>();


	[Header("Light")]
	public Light lightGlyph;
	public float intensityOn,intensityOff;

	[Header("Particles")]
	public List<ParticleSystem> particlesOn = new List<ParticleSystem>();
	public ParticleSystem particlesIdleOn;
	public List<ParticleSystem> particlesOff = new List<ParticleSystem>();

	public void On()
	{
		anim.SetBool ("activated",true);
		lightGlyph.DOIntensity (intensityOn, timeToActivate).SetEase (Ease.OutSine);
		glypheOn.material.DOFloat (0, "_DissolveAmount", timeToActivate).SetEase (Ease.OutSine);
		foreach (MeshRenderer mr in crystalRenderers) {
			mr.material = crystalOn;
		}
		foreach (ParticleSystem ps in particlesOn) {
			ps.Play ();
		}
		particlesIdleOn.Play ();
	}

	public void Off()
	{
		anim.SetBool ("activated",false);
		lightGlyph.DOIntensity (intensityOff, timeToActivate).SetEase (Ease.OutSine);
		glypheOn.material.DOFloat (1, "_DissolveAmount", timeToActivate).SetEase (Ease.OutSine);
		foreach (MeshRenderer mr in crystalRenderers) {
			mr.material = crystalOff;
		}
		foreach (ParticleSystem ps in particlesOff) {
			ps.Play ();
		}
		particlesIdleOn.Stop ();
	}

	
	///*
	void Update () {
		if (Input.GetKeyDown (KeyCode.H)) {
			On ();
		}
		if (Input.GetKeyDown (KeyCode.J)) {
			Off ();
		}
	}
	//*/
}
