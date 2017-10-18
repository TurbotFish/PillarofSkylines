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
	Renderer myRenderer;

	bool dissipating;
	bool dissipated;
	bool reappearing;

	void Start(){
		myCollider = GetComponent<Collider> ();
		myRenderer = GetComponent<Renderer> ();
	}

	void Update () {
		transform.position += transform.up * speed * Time.deltaTime;
		if (player != null)
			player.transform.position += transform.up * speed * Time.deltaTime;

		if (dissipating) {
			timerToDissipate -= Time.deltaTime;
			myRenderer.material.color = new Color (.2f, .2f, .2f, (timerToDissipate / timeToDissipate)+.1f);
			if (timerToDissipate <= (1 - ratioBeforeCollider) * timeToDissipate) {
				myCollider.enabled = false;
			}
			if (timerToDissipate <= 0) {
				myRenderer.material.color = new Color (.2f, .2f, .2f, .1f);
				dissipating = false;
				dissipated = true;
				timerDowntime = downtime;
			}
		}

		if (dissipated) {
			timerDowntime -= Time.deltaTime;
			if (timerDowntime <= 0) {
				dissipated = false;
				reappearing = true;
				timerToDissipate = timeToDissipate;
			}
		}

		if (reappearing) {
			timerToDissipate -= Time.deltaTime;
			myRenderer.material.color = new Color (1f, 1f, 1f, ((1-timerToDissipate)/ timeToDissipate)+.1f);
			if (timerToDissipate <= 0) {
				myRenderer.material.color = new Color (1f, 1f, 1f, 1f);
				reappearing = false;
				myCollider.enabled = true;
			}
		}
	}

	public void AddPlayer(Player newPlayer) {
		player = newPlayer;
		if (!dissipating) {
			dissipating = true;
			timerToDissipate = timeToDissipate;
		}
	}

	public void RemovePlayer() {
		player = null;
	}

}
