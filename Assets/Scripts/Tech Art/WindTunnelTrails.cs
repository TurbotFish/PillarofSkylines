﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTunnelTrails : MonoBehaviour {

	public LineRenderer windCurrent;
	public GameObject trailPrefab;
	[Header("specs")]
	public MinMax spawnInterval;
	public MinMax speed;
	public MinMax rotSpeed;
	public MinMax width;
	public MinMax length;
	public MinMax currentWidth;

	[Header("colors")]
	public Gradient opacity;
	public Gradient colors;

	float nextSpawn;
	float lastTime;

	// Use this for initialization
	void Start () {
		nextSpawn = 0;
		lastTime = 0;
	}

	// Update is called once per frame
	void Update () {
		if (Time.time - lastTime > nextSpawn) {
			SpawnTrail ();
		}
	}


	void SpawnTrail()
	{
		GameObject trail = Instantiate (trailPrefab, windCurrent.GetPosition (0), Quaternion.identity) as GameObject;
		ParticlesFollowSpline flw = trail.GetComponent<ParticlesFollowSpline> ();
		flw.lr = windCurrent;
		flw.speed = Random.Range (speed.min, speed.max);
		float sign = (Random.value > 0.5f)?1:-1f;
		flw.rotSpeed = sign * Random.Range (rotSpeed.min, rotSpeed.max);
		TrailRenderer tr = flw.GetComponentInChildren<TrailRenderer> ();
		tr.transform.localPosition = new Vector3 (Random.Range(currentWidth.min,currentWidth.max),0,0);
		tr.widthMultiplier = Random.Range (width.min, width.max);
		tr.time = Random.Range (length.min, length.max);
		nextSpawn = Random.Range (spawnInterval.min, spawnInterval.max);
		lastTime = Time.time;

		/*foreach (GradientAlphaKey key in opacity.alphaKeys) {
			
		}*/
		Color col = colors.Evaluate (Random.value);
		GradientColorKey[] gck = new GradientColorKey[2];
		gck [0].color = col;
		gck [0].time = 0.0f;
		gck [1].color = col;
		gck [1].time = 1.0f;

		Gradient grad = new Gradient();
		grad.SetKeys (gck, opacity.alphaKeys);

		tr.colorGradient = grad;
	}
}
