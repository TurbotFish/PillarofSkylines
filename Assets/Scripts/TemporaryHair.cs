using UnityEngine;

public class TemporaryHair : MonoBehaviour {

    new LineRenderer renderer;
    [SerializeField] Vector3 endPosition = new Vector3(0, -0.6f, 0.2f);
    Transform my;

    private void Start()
    {
        my = transform;
        renderer = GetComponent<LineRenderer>();
    }

    void Update () {
        Vector3 pos = my.InverseTransformPoint(my.parent.position + endPosition);
        renderer.SetPosition(1, pos);
	}
}
