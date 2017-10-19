using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTrailRenderer : MonoBehaviour {

	public int maxPos;
	public float vertexDistance;

	[Header("Wobbler")]
	public float speed = 1;
	public float amplitude = 0.2f;
	public float offsetSpeed = 1;

	List<Vector3> _positions = new List<Vector3> ();

	Vector3 _spawner;
	Vector3 _lastSpawnPosition;

	LineRenderer _LR;
	Material _mat;

	// Use this for initialization
	void Start () {
		_LR = GetComponent<LineRenderer> ();
		_mat = _LR.material;
		_lastSpawnPosition = transform.position;
		//_LR.SetPositions
	}
	
	// Update is called once per frame
	void LateUpdate () {
		OffsetMaterial ();

		if (GetPositionDifference () > vertexDistance) {
			Spawn ();
			Debug.Log ("OK");

		}
	}

	void Spawn()
	{
		_spawner = transform.position;
		_spawner += new Vector3 (Mathf.Sin (Time.time * speed) * amplitude, 0, 0);
		_positions.Add (_spawner);
		_lastSpawnPosition = _spawner;

		if (_LR.positionCount > maxPos) {
			ClearLastPosition ();

		}

		SetLRPositions ();
	}

	void ClearLastPosition()
	{
		_positions.RemoveAt (0);
		//SetLRPositions ();
	}

	void SetLRPositions()
	{
		Vector3[] _positionsArray = new Vector3[_positions.Count];
		for (int i = 0; i < _positions.Count; i++) 
		{
			_positionsArray [i] = _positions [i];
		}
		_LR.positionCount = _positionsArray.Length;
		_LR.SetPositions (_positionsArray);
		Debug.Log (_positionsArray.Length);

	}

	float GetPositionDifference()
	{
		float _dist;
		_dist = Vector3.Distance (_lastSpawnPosition, transform.position);
		return _dist;
	}

	void OffsetMaterial()
	{
		_mat.mainTextureOffset += new Vector2 (offsetSpeed * Time.deltaTime, 0);
	}
}
