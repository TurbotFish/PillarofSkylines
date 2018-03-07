using UnityEngine;
using Game.LevelElements;
using Game.GameControl;
using System.Collections;

[RequireComponent(typeof(MovingPlatform))]
public class CustomDoor : TriggerableObject {

	public AnimationCurve activationCurve;
	public float activationLength = 2f;
	public AnimationCurve deactivationCurve;
	public float deactivationLength = 2f;

	public Vector3 offsetWhenActivated;
	Vector3 positionOn, positionOff;

	MovingPlatform platform;
	float timer;
	float percent;
	Vector3 vectorToMove;
	float moveAmount;
	bool isActivated;
	Vector3 moveVector;

	#region public methods

	public override void Initialize(IGameControllerBase gameController, bool isCopy)
	{
		base.Initialize(gameController, isCopy);

		//myTransform = transform;
		platform = GetComponent<MovingPlatform>();
		positionOff = transform.position;
		positionOn = transform.position + transform.localToWorldMatrix.MultiplyPoint (offsetWhenActivated);
		vectorToMove = positionOn - positionOff;

//		if (Triggered)
//		{
//			localPositionWhenOpen = my.localPosition;
//			elapsed = 0;
//		}
//		else
//		{
//			localPositionWhenClosed = my.localPosition;
//			elapsed = timeToMove;
//		}
	}

	#endregion public methods


	#region protected methods

	protected override void Activate()
	{
		Debug.LogFormat("Door \"{0}\": Activate called!", name);

		isActivated = true;
	}

	protected override void Deactivate()
	{
		Debug.LogFormat("Door \"{0}\": Deactivate called!", name);
	}

	#endregion protected methods


	void Update(){
		if (isActivated)
			Move ();
	}

	void Move(){

		if (timer < activationLength) {
			timer += Time.deltaTime;
			percent = Mathf.Clamp01 (timer / activationLength);
			moveAmount = activationCurve.Evaluate (percent);

		} else {
			timer += Time.deltaTime;

			percent = Mathf.Clamp01 ((timer - activationLength) / deactivationLength);
			moveAmount = deactivationCurve.Evaluate (percent);
		}

		moveVector = (positionOff + vectorToMove * moveAmount) - transform.position;
		platform.Move (moveVector);

		if (timer >= activationLength + deactivationLength) {
			timer = 0;
			isActivated = false;
		}
	}
}
