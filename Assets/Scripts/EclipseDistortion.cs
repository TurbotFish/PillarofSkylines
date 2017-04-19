using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EclipseDistortion : MonoBehaviour {

	public float posVariationMax;
	public float rotVariationMax;
	public float effectRange;
	public float compensation;

	Transform _player;
	bool _eclipse;
	Vector3 _eclipseRot, _eclipsePos, _baseRot, _basePos;

	void Start () {
		_player = GameObject.FindGameObjectWithTag("Player").transform;
		_eclipse = false;
		_basePos = transform.position;
		_baseRot = transform.root.eulerAngles;
		_eclipsePos = _basePos + new Vector3(Random.Range(-posVariationMax,posVariationMax), Random.Range(-posVariationMax,posVariationMax), Random.Range(-posVariationMax,posVariationMax));
		_eclipseRot = _baseRot + new Vector3(Random.Range(-rotVariationMax,rotVariationMax), Random.Range(-rotVariationMax,rotVariationMax), Random.Range(-rotVariationMax,rotVariationMax));
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.N))
		{
			if (!_eclipse)
				StartDistortion();
			else
				StopDistortion();
		}

		if (_eclipse)
		{
			ReactiveDistortion();
		}

	}

	void StartDistortion()
	{
		transform.position = _eclipsePos;
		transform.rotation = Quaternion.Euler(_eclipseRot);
		_eclipse = true;
	}
	void StopDistortion()
	{
		transform.position = _basePos;
		transform.rotation = Quaternion.Euler(_baseRot);
		_eclipse = false;
	}
	void ReactiveDistortion()
	{
		float _distanceToPlayer = Vector3.Distance(transform.position, _player.position);
		float _ratio = _distanceToPlayer/effectRange - compensation;
		transform.position = Vector3.Lerp(_basePos,_eclipsePos, _ratio);
		transform.rotation = Quaternion.Euler(Vector3.Lerp(_baseRot, _eclipseRot, _ratio));
	}
}
