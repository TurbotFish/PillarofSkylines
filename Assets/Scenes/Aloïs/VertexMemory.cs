using UnityEngine;

public class VertexMemory : MonoBehaviour {

	float maxBend = 60f;
	float currentBend;
	float recoveryTime = 2f;
	float timer = 0f;

	Material mat;
	bool doBend;

	public Transform player;


	void Start(){
		mat = GetComponent<Renderer> ().material;
	}

	void Update(){

		Vector3 obj2Player = player.position - transform.position;
		float sqrDist = obj2Player.sqrMagnitude;
		float bendPercent = 1f - Mathf.Clamp01 ((sqrDist - .2f) / (.5f - .2f));


		if (bendPercent == 0) {
			doBend = true;
			timer = 0;
			currentBend = maxBend;
		}

		if (doBend) {

			timer += Time.deltaTime;
			currentBend = Mathf.Lerp (maxBend, 0f, timer);
			mat.SetFloat ("_BendMemory", currentBend);

			if (timer > recoveryTime) {
				doBend = false;
			}
		}
	}
}
