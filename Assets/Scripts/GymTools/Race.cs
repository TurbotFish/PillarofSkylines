using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Race : MonoBehaviour {

	Transform racer;
	bool racing;
	public float speed;

	void Start () {
		racer = transform.GetChild(0);
	}

	public void OnTriggerEnter(Collider c){
		StartRace();
	}

	void StartRace(){
		racing = true;
		racer.position = transform.position;
	}

	void Update(){
		if (racing) 
			racer.position += new Vector3(0, 0, speed * Time.deltaTime);

		if (racer.localPosition.z > 180f) {
			StopRace();
		}
	}

	void StopRace(){
		racing = false;
		racer.position = transform.position;
	}
}