using UnityEngine;

public class Heatwave : MonoBehaviour {

	public float growSpeed;
	public float fadeSpeed;

	float _startRefraction;
	Material _mat;
    Transform _my, _camera;

	void Start () {
		_mat = GetComponent<MeshRenderer>().material;
		_startRefraction = _mat.GetFloat("_RefractionIntensity");
        _my = transform;
		_camera = GameObject.FindObjectOfType<ThirdPersonCamera> ().transform;
        LookAtCamera();
    }
	
    void LookAtCamera() {
		_my.LookAt(_camera);
        _my.Rotate(90, 0, 0);
    }

	void Update () {
		_my.localScale += Vector3.one*growSpeed/10;
        LookAtCamera();

        _mat.SetFloat("_RefractionIntensity", _mat.GetFloat("_RefractionIntensity")-fadeSpeed/10);
		if (_mat.GetFloat("_RefractionIntensity") <= 0)
		{
			Destroy(gameObject);
		}
	}
}
