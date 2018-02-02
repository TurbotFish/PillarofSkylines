using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAndForthMovement : MovingPlatform {

    public float waitTime;
    public float speed;
    public float slowingRatio;
    public Vector3 movement;

    float currWaitTime;
    Vector3 initialPosition;
    float movementProgression;
    Vector3 startOfFramePosition;

    eMovingState currState = eMovingState.forth;

    void Start()
    {
        movementProgression = 0.01f;
        currWaitTime = waitTime;
        initialPosition = transform.position;
        gameObject.tag = "MovingPlatform";
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
                    currWaitTime = waitTime;
                    currState = eMovingState.forth;
                }
                break;
            case eMovingState.waitingAfterForth:
                currWaitTime -= Time.deltaTime;
                if (currWaitTime < 0f)
                {
                    currWaitTime = waitTime;
                    currState = eMovingState.back;
                }
                break;
            default:
                break;
        }

        startOfFramePosition = transform.position;
        transform.position = initialPosition + movement * movementProgression;

        if (currPlayer != null)
        {
            currPlayer.ImmediateMovement(transform.position - startOfFramePosition, true);
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
