using UnityEngine;
using System.Collections;

public class EclipseManager : MonoBehaviour {

    [TestButton("Start Eclipse", "StartEclipse", isActiveInEditor = false)]
    [TestButton("Stop Eclipse", "StopEclipse", isActiveInEditor = false)]
    public bool isEclipseActive;

	public float rotationDuration = 1;
	public Vector3 regularGravity;
	public Vector3 eclipseGravity;

    [SerializeField]
    Transform pillar;

    EchoManager echoes;
    Player player;
    Needle needle;

    #region Singleton
    public static EclipseManager instance;
    void Awake() {
        if (!instance) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this)
            Destroy(gameObject);
    }
    #endregion

    void Start () {
        echoes = EchoManager.instance;
        player = FindObjectOfType<Player>(); //to fix

		Game.Utilities.EventManager.OnEclipseEvent += HandleEventEclipse;
    }
	
    public void StartEclipse() {
        echoes.FreezeAll();
        isEclipseActive = true;
    }

    public void StopEclipse() {
        echoes.UnfreezeAll();
        isEclipseActive = false;
    }

	void HandleEventEclipse(object sender, Game.Utilities.EventManager.OnEclipseEventArgs args) {
		if (args.EclipseOn) {
			StartEclipse ();
		} else {
			StopEclipse ();
		}
		StartCoroutine ("ChangeGravity", args.EclipseOn);
	}

	IEnumerator ChangeGravity(bool eclipseOn){
		float gravityTimer = 0;
		while (gravityTimer < rotationDuration) {
			if (eclipseOn) {
				player.ChangeGravityDirection (Vector3.Lerp (regularGravity, eclipseGravity, gravityTimer / rotationDuration));
			} else {
				player.ChangeGravityDirection (Vector3.Lerp (eclipseGravity, regularGravity, gravityTimer / rotationDuration));
			}
			gravityTimer += Time.deltaTime;
			yield return null;
		}
	}
}
