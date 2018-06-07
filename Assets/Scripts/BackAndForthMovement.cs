using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MovingPlatform))]
public class BackAndForthMovement : MonoBehaviour {

    public float waitTimeBack;
	public float waitTimeForth;
    public float speed;
    public float slowingRatio;
    public Vector3 movement;
	public float initialWaitTime;

    MovingPlatform platform;
    float currWaitTime;
    float movementProgression;
    Vector3 startOfFramePosition;
    Vector3 posLastFrame;


	eMovingState currState = eMovingState.waitingAfterBack;

    void Start()
    {
        if (!GetComponent<MovingPlatform>())
        {
            gameObject.AddComponent<MovingPlatform>();
        }
        platform = GetComponent<MovingPlatform>();
        movementProgression = 0.01f;
		currWaitTime = waitTimeForth + initialWaitTime;
        posLastFrame = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currState)
        {
            case eMovingState.forth:
                movementProgression += speed * Time.deltaTime * Mathf.Clamp01(movementProgression/slowingRatio) * Mathf.Clamp01((1 - movementProgression) / slowingRatio);
                if (movementProgression >= 0.99f)
                {
                    movementProgression = 0.99f;
                    currState = eMovingState.waitingAfterForth;
                }
                break;
            case eMovingState.back:
                movementProgression -= speed * Time.deltaTime * Mathf.Clamp01(movementProgression / slowingRatio) * Mathf.Clamp01((1-movementProgression)/slowingRatio);
                if (movementProgression <= 0.01f)
                {
                    movementProgression = 0.01f;
                    currState = eMovingState.waitingAfterBack;
                }
                break;
            case eMovingState.waitingAfterBack:
                currWaitTime -= Time.deltaTime;
                if (currWaitTime < 0f)
                {
					currWaitTime = waitTimeBack;
                    currState = eMovingState.forth;
                }
                break;
            case eMovingState.waitingAfterForth:
                currWaitTime -= Time.deltaTime;
                if (currWaitTime < 0f)
                {
					currWaitTime = waitTimeForth;
                    currState = eMovingState.back;
                }
                break;
            default:
                break;
        }

        /*
        startOfFramePosition = transform.localPosition;
        transform.localPosition = initialPosition + movement * movementProgression;
        */

        if (platform != null)
        {
            platform.Move((movement * movementProgression) - posLastFrame, true);
            posLastFrame = movement * movementProgression;
        }
    }

    public enum eMovingState
    {
        forth,
        back,
        waitingAfterForth,
        waitingAfterBack
    }


}
