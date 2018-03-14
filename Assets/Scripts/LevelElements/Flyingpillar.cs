using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyingpillar : Interactible

{
    [SerializeField] public MovingPlatform movingPillar;
    [SerializeField] public float damp = 0.2f;

    bool here;
    Vector3 targetPos;
    Transform player, move;

    void Start ()
    {
        move = movingPillar.transform;
        targetPos = move.position;

    }

    public override void EnterTrigger(Transform player)
    {
        here = true;
        this.player = player;
    }

    public override void ExitTrigger(Transform player)
    {
        here = false;
    }


    void Update () {
        if (here)
        {
            targetPos.y = player.position.y;
            move.position = Vector3.Lerp(move.position, targetPos, Time.deltaTime / damp);
        }
	}

}
