using UnityEngine;

public class PlantPhysics : MonoBehaviour {

	public Transform goal, arc;
	Vector3 _startPos;
	public LineRenderer lr;
	public float minHeight, maxHeight;
	public Color startColorA, startColorB;
	public Color endColorA, endColorB;
	public float returnSpeed = 1;
	Vector3 _force;

	public float displaceAngle;
	public Vector3 wind;

	// Use this for initialization
	void Start () {
		RandomSeed ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		displaceAngle = Vector3.Angle (arc.position - transform.position, goal.position - transform.position);
		if (displaceAngle > 0) {
			
		}
		goal.position += _force;
	}

	void RandomSeed()
	{
		float _random;
		_random = Random.Range (0f, 1f);
		lr.startColor = Color.Lerp (startColorA, startColorB, _random);
		_random = Random.Range (0f, 1f);
		lr.endColor = Color.Lerp (endColorA, endColorB, _random);

		float _height = Random.Range (minHeight, maxHeight);
		goal.position = transform.position + transform.up * _height;
		_startPos = goal.position;
		arc.position = Vector3.Lerp (transform.position, goal.position, 0.5f);
	}
}
