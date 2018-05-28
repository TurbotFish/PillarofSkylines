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

	public float executionDelay;

	MovingPlatform platform;
	float timer;
	float percent;
	Vector3 vectorToMove;
	float moveAmount;
	bool isActivated;
	Vector3 moveVector;
	float delayTimer;

	public bool dirtyOverrideMaterialSwap;

	[ConditionalHideAttribute("dirtyOverrideMaterialSwap", false)]
	public Material matOn;

	[ConditionalHideAttribute("dirtyOverrideMaterialSwap", false)]
	public Material matOff;

	[ConditionalHideAttribute("dirtyOverrideMaterialSwap", false)]
	public Renderer _renderer;

	#region public methods

	public override void Initialize(GameController gameController)
	{
		base.Initialize(gameController);

		//myTransform = transform;
		platform = GetComponent<MovingPlatform>();
		positionOff = transform.localPosition;
		//positionOn = transform.position + transform.localToWorldMatrix.MultiplyVector (offsetWhenActivated);
		positionOn = transform.localPosition + transform.forward * offsetWhenActivated.z + transform.right * offsetWhenActivated.x + transform.up * offsetWhenActivated.y;


		vectorToMove = positionOn - positionOff;

		if(dirtyOverrideMaterialSwap)
			_renderer.material = matOff;

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
		if(dirtyOverrideMaterialSwap)
			_renderer.material = matOn;
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
		
		if (delayTimer < executionDelay) {
			delayTimer += Time.deltaTime;
			return;
		}
			

		if (timer < activationLength) {
			timer += Time.deltaTime;
			percent = Mathf.Clamp01 (timer / activationLength);
			moveAmount = activationCurve.Evaluate (percent);

		} else {
			timer += Time.deltaTime;

			percent = Mathf.Clamp01 ((timer - activationLength) / deactivationLength);
			moveAmount = deactivationCurve.Evaluate (percent);
		}

		moveVector = (positionOff + vectorToMove * moveAmount) - transform.localPosition;
		platform.Move (moveVector);

		Debug.Log ("move amount : " + moveVector);

		if (timer >= activationLength + deactivationLength) {
			timer = 0;
			isActivated = false;
			delayTimer = 0;
			if(dirtyOverrideMaterialSwap)
				_renderer.material = matOff;
		}
	}
}
