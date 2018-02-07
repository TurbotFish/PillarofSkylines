using Game.Player.CharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

	public CharController currPlayer;

	[HideInInspector]
	public Vector3 impactPoint;

	void Start () {
		transform.tag = "MovingPlatform";
        TestChildren(transform);
	}

    void TestChildren(Transform trans)
    {
        foreach (Transform children in trans)
        {
            if (children.GetComponent<Collider>() != null)
            {
                if (children.tag == "Untagged")
                    children.tag = "MovingPlatform";
            }
            TestChildren(children);
        }
    }

    public void Move(Vector3 movement)
    {
        transform.position += movement;
        if(currPlayer!= null)
            currPlayer.ImmediateMovement(movement, false);
    }

	virtual public void AddPlayer(CharController player, Vector3 playerImpactPoint) {
		currPlayer = player;
		impactPoint = playerImpactPoint;
	}

	virtual public void RemovePlayer() {
		currPlayer = null;
	}
}
