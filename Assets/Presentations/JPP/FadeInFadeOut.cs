using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeInFadeOut : MonoBehaviour {

	public Image fadePanel;


	public void FadeOut(Color color, float speed)
	{
		fadePanel.color = color;
		fadePanel.DOFade (0, speed);
	}
	public void FadeIn (Color color, float speed) {
		fadePanel.color = color;
		fadePanel.DOFade (1, speed);
	}

}
