using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public enum Direction {from, to}

[System.Serializable]
public class CustomAnimation
{
	public bool fade = false;
	public Direction direction = Direction.from;
	public float targetValue = 0;
	public float duration = 1; 
	public DOTweenAnimation tween;
	public TextMeshPro tmp;
}
