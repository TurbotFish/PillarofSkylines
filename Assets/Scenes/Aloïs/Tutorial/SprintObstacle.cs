using UnityEngine;

public class SprintObstacle : MonoBehaviour {

	public float cycleLength = 5f;
	public AnimationCurve cycleCurve;
	public float cycleDistance = 5f;
	float cycleTimer;
	float cyclePercent;
	Vector3 frameMovement;
	Vector3 initialPos;

	void Start(){
		initialPos = transform.position;
	}


	void Update(){
		DoCycleObstacle ();
	}

	void DoCycleObstacle(){
		cycleTimer += Time.deltaTime;
		cyclePercent = cycleTimer / cycleLength;
		transform.position = initialPos + transform.forward * cycleCurve.Evaluate(cyclePercent) * cycleDistance;
		//transform.Translate (frameMovement, Space.World);
		if (cyclePercent >= 1f) {
			cycleTimer = 0;
		}
	}
}
