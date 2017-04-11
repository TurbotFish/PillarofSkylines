using UnityEngine;

public class BreakEchoParticles : MonoBehaviour {

    ParticleSystem ps;
    Transform pool;

    void Awake() {
        ps = GetComponentInChildren<ParticleSystem>();
        EchoManager echoManager = FindObjectOfType<EchoManager>();
        pool = echoManager.pool;
    }
	
    void OnEnable() {
        ps.Play();
        Invoke("DestroyWhenOver", ps.main.duration);
    }

    void DestroyWhenOver() {
        gameObject.SetActive(false);
        transform.parent = pool;
    }

}
