using UnityEngine;

public class Cloud : MonoBehaviour {

	public float speed = 1f;
	public float timeToDissipate = 3f;
	float timerToDissipate;
	public float downtime = 10f;
	float timerDowntime;
	public float ratioBeforeCollider = .8f;

	Player player;
	Collider myCollider;

	bool dissipating;

	void Start(){
		myCollider = GetComponent<Collider> ();
	}

	void Update () {
		transform.position += transform.up * speed * Time.deltaTime;
		if (player != null)
			player.transform.position += transform.up * speed * Time.deltaTime;


	}

	public void AddPlayer(Player newPlayer) {
		player = newPlayer;
		dissipating = true;
	}

	public void RemovePlayer() {
		player = null;
	}

}
