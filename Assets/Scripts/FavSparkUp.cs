using System.Collections;
using UnityEngine;

public class FavSparkUp : MonoBehaviour {

    ParticleSystem ps;

	void Start () {
        ps = GetComponent<ParticleSystem>();
        StartCoroutine(Coroutine());
	}
	
    IEnumerator Coroutine() {

        var em = ps.emission;
        em.rate = new ParticleSystem.MinMaxCurve(0);

        while (em.rate.constant < 50) {
            em.rate = new ParticleSystem.MinMaxCurve(em.rate.constant + 0.5f);
            yield return new WaitForSeconds(0.01f);
        }
        ps.Stop();
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

}
