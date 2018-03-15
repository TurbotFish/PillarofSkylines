using UnityEngine;

public class AutoMovePlatform : MovingPlatform {

    Transform my;
    Vector3 startPos;
    
    [SerializeField] float speed = 1;
    [SerializeField] Vector3 movement;

    void Awake () {
        my = transform;
        startPos = my.position;
	}
	
	void Update () {
        Vector3 pos = Mathf.Sin(Time.time * speed) * movement + startPos;
        Move(pos - my.position);
    }
}
