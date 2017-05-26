using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class FadeTMP : MonoBehaviour {

	private float _delay;

	void Awake ()
	{
		_delay = GetComponent<DOTweenAnimation>().delay;
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			TextMeshPro _tmp = GetComponent<TextMeshPro>();
			_tmp.DOFade(0,3).SetDelay(4);
		}
	}

	public void FadeFromZero (float _t)
	{
		TextMeshPro _tmp = GetComponent<TextMeshPro>();
		_tmp.DOFade(0,_t).SetEase(Ease.OutQuad).SetDelay(_delay).From();
	}

	public void FadeToZero (float _t)
	{
		TextMeshPro _tmp = GetComponent<TextMeshPro>();
		_tmp.DOFade(0,_t).SetEase(Ease.OutQuad).SetDelay(_delay);
	}
}
