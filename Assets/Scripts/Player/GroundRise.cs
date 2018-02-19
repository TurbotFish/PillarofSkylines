using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundRise : MovingPlatform {

    float height;
    float strength;

    Vector3 playerUp;


    float currHeight = 0f;
    bool finishedMoving = false;

    public void Initialize(float height, float strength, Vector3 position, Vector3 playerUp, Game.Player.CharacterController.CharController player)
    {
        this.height = height;
        this.strength = strength;
        this.playerUp = playerUp;
        transform.position = position;
        currPlayer = player;
    }

    public override void Start ()
    {
        transform.localScale = new Vector3(transform.localScale.x, height, transform.localScale.z);

        base.Start();
    }
	
	void Update ()
    {
        if (!finishedMoving)
        {
            currHeight += strength * Time.deltaTime;
            if (currHeight > height)
            {
                transform.Translate(playerUp * (height - (currHeight - (strength * Time.deltaTime))), Space.World);
                finishedMoving = true;
                print("hey");
                
                //myPlayer.AddExternalVelocity(playerUp * (height - (currHeight - (strength * Time.deltaTime))), true, true);
                if (currPlayer)
                    currPlayer.ImmediateMovement(playerUp * (height - (currHeight - (strength * Time.deltaTime))), true);

            }
            else
            {
                transform.Translate(playerUp * strength * Time.deltaTime, Space.World);
                print("hello");
                
                //myPlayer.AddExternalVelocity(playerUp * strength, true, true);
                if (currPlayer)
                    currPlayer.ImmediateMovement(playerUp * strength * Time.deltaTime, true);
            }
        }
	}
}
