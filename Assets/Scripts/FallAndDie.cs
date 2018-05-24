using System.Collections;
using UnityEngine;

public class FallAndDie : MonoBehaviour {

    public float waitBeforeFalling = 0.1f;
    public float fallingSpeed = 1f;
    public float fallTime = 1f;

    public Vector3 fallDirection = Vector3.up;

    float currFallSpeed = 0;
    public float gravity = 10;


    Transform my;
    
	void Start () {
        my = transform;
	}

    public void Trigger() {
        StartCoroutine(_FallAndDie());
    }

    IEnumerator _FallAndDie()
    {
        yield return new WaitForSeconds(waitBeforeFalling);

        for (float elapsed = 0; elapsed < fallTime; elapsed+=Time.deltaTime) {

            if (currFallSpeed < fallingSpeed)
                currFallSpeed += gravity * Time.deltaTime;

            my.Translate(fallDirection * currFallSpeed * Time.deltaTime, Space.World);

            yield return null;
        }
        
        Destroy(gameObject);
    }

}
