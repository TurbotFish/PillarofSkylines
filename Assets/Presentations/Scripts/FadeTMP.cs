using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class FadeTMP : MonoBehaviour {

	private float _delay;
	private TextMeshPro _tmp;
	public List<TextMeshPro> _tmps = new List<TextMeshPro>();

	void Awake ()
	{
		_tmp = GetComponent<TextMeshPro>();
		_tmp.enabled = false;
		_delay = GetComponent<DOTweenAnimation>().delay;
	}

	void Update()
	{
		/*if(Input.GetKeyDown(KeyCode.A))
		{
			Debug.Log ("hello");
			TextMeshPro _tmp = GetComponent<TextMeshPro>();
			Debug.Log (_tmp.name);
			_tmp.DOFade(0,3).SetDelay(0);
		}*/
	}

	public void FadeFromZero (float _t)
	{
		_tmp.enabled = true;
		_tmp.DOFade(0,_t).SetEase(Ease.OutQuad).SetDelay(_delay).From();
	}
		
	public void FadeToZero (float _t)
	{
		_tmp.enabled = true;
		_tmp.DOFade(0,_t).SetEase(Ease.OutQuad).SetDelay(_delay);
	}

	public void DisableTMP()
	{
		_tmp.enabled = false;
	}

	public void GroupFadeToZero (float _t)
	{
		foreach (TextMeshPro t in _tmps) {
			t.enabled = true;
			t.DOFade(0,_t).SetEase(Ease.OutQuad).SetDelay(_delay);
		}
	}
}
