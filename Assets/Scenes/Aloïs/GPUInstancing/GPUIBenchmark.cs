using UnityEngine;

public class GPUIBenchmark : MonoBehaviour {

	public int instances;
	public GameObject objToInstantiate;
	public Transform parent;

	void Start () {
		for (int i = 0; i < instances; i++) {
			Instantiate (objToInstantiate, parent);
		}
	}
}
