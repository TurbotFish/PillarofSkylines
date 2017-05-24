using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveDistant : MonoBehaviour {

	public void SetActiveGO (GameObject _go) {
		_go.SetActive(true);
	}

	public void SetInactiveGO(GameObject _go)
	{
		_go.SetActive(false);
	}
}
