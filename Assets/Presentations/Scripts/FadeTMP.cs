using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class FadeTMP : MonoBehaviour {

	float _delay;
	float speed = 0.5f;
	private TextMeshPro _tmp;
	public List<TextMeshPro> _tmps = new List<TextMeshPro>();

	void Awake ()
	{
		_tmp = GetComponent<TextMeshPro>();
		_tmp.enabled = false;
		//if(GetComponent<DOTweenAnimation>() != null)
		_delay = GetComponent<DOTweenAnimation>().delay;
	}


	void Update()
	{

	}

	public void FadeFromZero (float _t)
	{
		_tmp.enabled = true;
		_tmp.alpha = 1;
		_tmp.DOFade(0,speed).SetEase(Ease.OutQuad).SetDelay(_delay+_t).From();
	}
		
	public void FadeToZero (float _t)
	{
		_tmp.enabled = true;
		_tmp.DOFade(0,speed).SetEase(Ease.OutQuad).SetDelay(_delay+_t);
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
