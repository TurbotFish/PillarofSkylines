﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class GlyphFX_Once : GlyphFX {

	public Animator anim;
	public Animator smallCrystalsAnim;

	public ParticleSystem psIdle;

	public float waitTime;
	public List<ParticleSystem> particleSystems = new List<ParticleSystem>();

	[Header ("Meshes & Materials")]
	public Material crystalOn;
	public List<MeshRenderer> crystals = new List<MeshRenderer>();
	public Material lineOn;
	public float waitTimeBetween;
	public List<MeshRenderer> lines = new List<MeshRenderer>();

	[Header("Light")]
	public Light lightGlyph;
	public float intensityOn;
	public float lightDelay;

	[Header("Colliders")]
	public GameObject colOn;
	public GameObject colOff;

	bool alreadyActivated = false;

	public override void GlyphOn(){
		if (alreadyActivated)
			return;

		alreadyActivated = true;
		anim.SetBool ("activated", true);
		smallCrystalsAnim.SetBool ("activated", true);

		lightGlyph.DOIntensity (intensityOn, 0.2f).SetDelay (lightDelay).SetEase (Ease.OutSine); 
		psIdle.Stop ();
		StartCoroutine (Sequence ());
		colOn.SetActive (true);
		colOff.SetActive (false);
	}


	IEnumerator Sequence () {

		yield return new WaitForSeconds (waitTime);
		foreach (ParticleSystem ps in particleSystems) {
			ps.Play ();
		}
		foreach (MeshRenderer mr in crystals) {
			mr.material = crystalOn;
		}

		for (int i = 0; i < lines.Count; i++) {
			yield return new WaitForSeconds (i * waitTimeBetween);
			if (lines[i] != null)
				lines[i].material = lineOn;
		}
	}
}
