using UnityEngine;

public class Cloud : MonoBehaviour {

	[Header("Parameters")]
	public float speed = 1f;
	public float timeToDestination = 5f;
	float timerToDestination;
	public bool endless = false;

	[Header("Dissipation")]
	public float timeToDissipate = 3f;
	float timerToDissipate;
	public float downtime = 10f;
	float timerDowntime;
	public float ratioBeforeCollider = .8f;

	CS_Cloud myCloudCS;
	float initialCloudFade;

	Game.Player.CharacterController.Character player;
	Collider myCollider;

	bool dissipating;
	bool dissipated;
	bool reappearing;

	void Start(){
		myCollider = GetComponent<Collider> ();
		if (!endless) {
			timerToDestination = timeToDestination;
		}
		//assign Cloud_CS to cloud gameObject
		myCloudCS = transform.GetChild(0).transform.GetComponentInChildren<CS_Cloud> ();
		initialCloudFade = myCloudCS.Fading;
	}

	void Update () {

		#region movement
		transform.position += Vector3.up * speed * Time.deltaTime;
		//move the player too if he's on the cloud
		if (player != null)
			player.transform.position += Vector3.up * speed * Time.deltaTime;


		timerToDestination -= Time.deltaTime;
		if (timerToDestination < 0f && !endless) {
			speed = -speed;
			timerToDestination = timeToDestination;

		}
		#endregion movement

		#region dissipation cycle
		if (dissipating) {
			timerToDissipate -= Time.deltaTime;
			//myRenderer.material.color = new Color (.2f, .2f, .2f, (timerToDissipate / timeToDissipate)+.1f);
			myCloudCS.Fading = (timerToDissipate / timeToDissipate) * initialCloudFade;
			if (timerToDissipate <= (1 - ratioBeforeCollider) * timeToDissipate) {
				myCollider.enabled = false;
			}
			if (timerToDissipate <= 0) {
				//myRenderer.material.color = new Color (.2f, .2f, .2f, .1f);
				myCloudCS.Fading = 0;
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
			//myRenderer.material.color = new Color (1f, 1f, 1f, ((1-timerToDissipate)/ timeToDissipate)+.1f);
			myCloudCS.Fading = ((timeToDissipate-timerToDissipate)/ timeToDissipate) * initialCloudFade;
			if (timerToDissipate <= 0) {
				//myRenderer.material.color = new Color (1f, 1f, 1f, 1f);
				myCloudCS.Fading = initialCloudFade;
				reappearing = false;
				myCollider.enabled = true;
			}
		}
		#endregion dissipation cycle
	}

	public void AddPlayer(Game.Player.CharacterController.Character newPlayer) {
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
