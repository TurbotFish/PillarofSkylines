using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesFollowSpline : MonoBehaviour {

	public LineRenderer lr;
	public float speed;
	public float rotSpeed;
	int _currentPos;
	float _counterPos;
	float _counterRot;

	// Use this for initialization
	void Start () {
		_currentPos = 0;
	}
	
	// Update is called once per frame
	void Update () {

		if (_currentPos != -1) {
			_counterPos += (speed * Time.deltaTime)/Vector3.Distance(lr.GetPosition(_currentPos),lr.GetPosition(_currentPos+1));
			transform.position = Vector3.Lerp (lr.GetPosition(_currentPos), lr.GetPosition(_currentPos + 1), _counterPos);
			if (_counterPos >= 1) {
				_counterPos = 0;
				_currentPos++;
				if (_currentPos + 1 >= lr.positionCount-1) {
					_currentPos = -1;
					Destroy (gameObject, 1f);
				}
			}
			if (_counterPos < lr.positionCount-3)
				transform.LookAt (Vector3.Lerp(lr.GetPosition (_currentPos + 1),lr.GetPosition (_currentPos + 2),_counterPos));
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,transform.localEulerAngles.y, _counterRot + 10*rotSpeed * Time.deltaTime);
			_counterRot = transform.localEulerAngles.z;


		}

	}
}
