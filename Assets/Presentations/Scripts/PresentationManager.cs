using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

[System.Serializable]
public class Slide
{
	[HideInInspector]
	public string name;
	public GameObject slide;
	public int order;
	public List<GameObject> animList = new List<GameObject>();
}

public class PresentationManager : MonoBehaviour {

	[Range(0,2)]
	public float timeScale;
	public string nextButton, previousButton;
	public int currentSlide, currentAnim;
	[Space(10)]
	public List<Slide> slides = new List<Slide>();

	private List<Slide> _slidesInOrder = new List<Slide>();


	void Start ()
	{
		SetSlidesInOrder();
	}


	void Update () {
		Time.timeScale = timeScale;

		if (Input.GetButtonDown(nextButton))
		{
			Debug.Log ("next");
			NextAnimation();
		}
		else if (Input.GetButtonDown(previousButton))
		{
			Debug.Log("previous");
			PreviousAnimation();
		}
	}


	void NextAnimation()
	{
		if (currentSlide < _slidesInOrder.Count-1 || (currentSlide == _slidesInOrder.Count-1 && currentAnim < _slidesInOrder[currentSlide].animList.Count-1))
		{
			currentAnim ++;
			if (currentAnim > _slidesInOrder[currentSlide].animList.Count-1)
			{
				MoveSlideAway();
				currentSlide++;
				_slidesInOrder[currentSlide].slide.transform.DOComplete();
				_slidesInOrder[currentSlide].slide.SetActive(true);
				_slidesInOrder[currentSlide].slide.GetComponent<DOTweenAnimation>().DORestart();
				currentAnim = -1;
			}
			else
			{
				ReadAnimation(currentSlide,currentAnim);
			}
		}
	}


	void PreviousAnimation()
	{
		if (currentAnim > -1)
		{
			GameObject _anim =_slidesInOrder[currentSlide].animList[currentAnim];
			if (_anim.GetComponent<DOTweenAnimation> () != null) {
				_anim.GetComponent<DOTweenAnimation> ().DORewind ();
			} /*else {
				DOTweenAnimation[] _childTweens = _anim.GetComponentsInChildren<DOTweenAnimation> ();
				foreach (DOTweenAnimation tween in _childTweens)
					tween.DORewind ();
			}*/

			if (!_anim.name.Contains("_parent"))
				_anim.SetActive(false);
			currentAnim --;
		}
		else if (currentAnim < 0 && currentSlide > 0)
		{
			MoveSlideBack(true);
			currentSlide--;
			currentAnim = _slidesInOrder[currentSlide].animList.Count-1;
			MoveSlideBack(false);
		}
		//ReadAnimation(currentSlide,currentAnim);
	}
		

	void ReadAnimation (int slideNumber, int animNumber)
	{
		if (_slidesInOrder[slideNumber].slide.activeSelf == false)
		{
			_slidesInOrder[slideNumber].slide.SetActive(true);
		}
		if (_slidesInOrder[slideNumber].animList.Count!=0)
		{
			GameObject anim = _slidesInOrder[slideNumber].animList[animNumber];
			anim.SetActive(true);
			if (anim.GetComponent<DOTweenAnimation> () != null) {
				anim.GetComponent<DOTweenAnimation>().DORewind();
				anim.GetComponent<DOTweenAnimation>().DOPlay();
			}
		}
	}
		

	void SetSlidesInOrder()
	{
		foreach(Slide slide in slides)
		{
			foreach(GameObject anim in slide.animList)
			{
				if (!anim.name.Contains("_parent"))
					anim.SetActive(false);
			}
			if (slide.order >= 0)
				_slidesInOrder.Add(null);
		}

		for (int i = 0; i< _slidesInOrder.Count; i++)
		{
			for (int j = 0; j<slides.Count;j++)
			{
				if (slides[j].order == i)
				{
					if (_slidesInOrder[i] != null)
						Debug.Log ("WARNING : multiple slides are marked with the same order, please set things right");
					_slidesInOrder[i] = slides[j];
				}
			}
		}
	}

	void MoveSlideAway()
	{
		GameObject _slide = _slidesInOrder[currentSlide].slide;
		Debug.Log (_slide.transform.right);
		_slide.transform.DOLocalMove(_slide.transform.position + _slide.transform.right*-150, 2 , false).SetEase(Ease.OutQuint).OnComplete(()=>DisableSlide(_slide));
	}

	void MoveSlideBack(bool _disable)
	{
		GameObject _slide = _slidesInOrder[currentSlide].slide;
		_slide.transform.DOComplete();
		_slide.SetActive(true);
		if (!_disable)
			_slide.transform.DOLocalMove(new Vector3(-39.7f,17.4f,40.6f), 2 , false).SetEase(Ease.OutQuint);
		else
			_slide.transform.DOLocalMove(_slide.transform.position + _slide.transform.right*150, 2 , false).SetEase(Ease.OutQuint).OnComplete(()=>DisableSlide(_slide));
	}

	void DisableSlide(GameObject slide)
	{
		slide.SetActive(false);
	}

	//Renaming the slides in the inspector list for easier reading + clamping the target value of the fade animation if needed
	[ExecuteInEditMode]
	void OnValidate()
	{
		foreach (Slide slide in slides)
		{
			slide.name = "SLIDE " + slide.order.ToString();
		}
	}
}
