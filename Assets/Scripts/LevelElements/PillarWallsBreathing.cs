using UnityEngine;

public class PillarWallsBreathing : MonoBehaviour {

    [Header("Animation")]
    [SerializeField] float speed = 1;
    [SerializeField] float intensity = 0.2f;
    [SerializeField] Vector3 scale = Vector3.one;
    Vector3 one = Vector3.one;

    Transform my;

    void Start() {
        my = transform;
    }

    void Update() {
        my.localScale = Mathf.Sin(Time.time * speed) * intensity * one + scale;
	}
}
