using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SimultaneousTween : MonoBehaviour {

	public void TweenByID ()
	{
		string _id = GetComponent<DOTweenAnimation> ().id;
		if (_id != null) {
			DOTween.Play (_id);
		}
	}

	public void TweenByIDRewind()
	{
		string _id = GetComponent<DOTweenAnimation> ().id;
		if (_id != null) {
			DOTween.Rewind(_id);
		}
	}
}
