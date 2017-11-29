using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesFollowSpline : MonoBehaviour {

	public LineRenderer lr;
	public float speed;
	public float rotSpeed;
	public int _currentPos;
	public int _maxPos;
	float _counterPos;
	float _counterRot;
	bool move;
	// Use this for initialization
	void Start () {
		//_currentPos = 0;
		move = true;
	}
		
	// Update is called once per frame
	void LateUpdate () {

		if (move) {
			_counterPos += (speed * Time.deltaTime)/Vector3.Distance(lr.GetPosition(_currentPos),lr.GetPosition(_currentPos+1));
			transform.position = Vector3.Lerp (lr.GetPosition(_currentPos), lr.GetPosition(_currentPos + 1), Mathf.Clamp01(_counterPos));
			if (_counterPos >= 1) {
				_counterPos = 0;
				_currentPos++;
				if (_currentPos + 1 >= _maxPos/*lr.positionCount-1*/) {
					move = false;
					Destroy (gameObject, 1f);
				}
			}
			if (_counterPos < lr.positionCount-3 && move)
				transform.LookAt (Vector3.Lerp(lr.GetPosition (_currentPos + 1),lr.GetPosition (_currentPos + 2),_counterPos));
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,transform.localEulerAngles.y, _counterRot + 10*rotSpeed * Time.deltaTime);
			_counterRot = transform.localEulerAngles.z;


		}

	}
}
