using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/*[System.Serializable]
public class Slide
{
	[HideInInspector]
	public string name;
	public GameObject slide;
	public int order;
	public List<GameObject> anims = new List<GameObject>();
}*/

public class PresentationManagerV2 : MonoBehaviour {

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
	
	// Update is called once per frame
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
		
	}

	void PreviousAnimation()
	{
		
	}

	void SetSlidesInOrder()
	{
		foreach(Slide slide in slides)
		{
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

	[ExecuteInEditMode]
	void OnValidate()
	{
		foreach (Slide slide in slides)
		{
			slide.name = "SLIDE " + slide.order.ToString();
		}
	}
}
