using UnityEngine;
using System.Collections;

public class EclipseManager : MonoBehaviour {

    [TestButton("Start Eclipse", "StartEclipse", isActiveInEditor = false)]
    [TestButton("Stop Eclipse", "StopEclipse", isActiveInEditor = false)]
    public bool isEclipseActive;

    public float rotationDuration = 1;

    [SerializeField]
    Transform pillar;

    EchoManager echoes;
    Transform player;
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
        needle = FindObjectOfType<Needle>();
        player = FindObjectOfType<ThirdPersonController>()?.transform ?? FindObjectOfType<Player>().transform; //to fix

		Game.Utilities.EventManager.OnEclipseEvent += HandleEventEclipse;
    }
	
    public void StartEclipse() {
        echoes.FreezeAll();
        isEclipseActive = true;
    }

    public void StopEclipse() {
        echoes.UnfreezeAll();
        isEclipseActive = false;
        needle.gameObject.SetActive(true);
    }

	void HandleEventEclipse(bool eclipseOn) {
		if (eclipseOn) {
			StartEclipse ();
		} else {
			StopEclipse ();
		}
	}

}
