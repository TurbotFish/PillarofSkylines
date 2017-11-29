using System.Collections;
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
	public MinMax posTravel;

	[Header("colors")]
	public Gradient opacity;
	public Gradient colors;

	[Header("material")]
	public float scrollSpeed;
	Material mat;

	float nextSpawn;
	float lastTime;

	// Use this for initialization
	void Start () {
		nextSpawn = 0;
		lastTime = 0;
		mat = windCurrent.material;

	}

	// Update is called once per frame
	void FixedUpdate () {
		if (Time.time - lastTime > nextSpawn) {
			SpawnTrail ();
		}

		mat.mainTextureOffset += new Vector2 (scrollSpeed, 0);
	}


	void SpawnTrail()
	{
		GameObject trail = Instantiate (trailPrefab, windCurrent.GetPosition (0), Quaternion.identity) as GameObject;
		ParticlesFollowSpline flw = trail.GetComponent<ParticlesFollowSpline> ();
		flw._currentPos = Random.Range (0, windCurrent.positionCount - 3);
		flw._maxPos = Mathf.Clamp (Mathf.RoundToInt(flw._currentPos + Random.Range (posTravel.min, posTravel.max)), 1, windCurrent.positionCount - 1);
		flw.lr = windCurrent;
		flw.speed = Random.Range (speed.min, speed.max);
		float sign = (Random.value > 0.5f)?1:-1f;
		flw.rotSpeed = sign * Random.Range (rotSpeed.min, rotSpeed.max);
		TrailRenderer tr = flw.GetComponentInChildren<TrailRenderer> ();
		tr.transform.localPosition = new Vector3 (Random.Range(windCurrent.widthMultiplier + currentWidth.min,windCurrent.widthMultiplier + currentWidth.max),0,0);
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
