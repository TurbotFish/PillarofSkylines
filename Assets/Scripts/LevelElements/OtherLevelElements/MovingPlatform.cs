using Game.Player.CharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    [HideInInspector]
    public CharController currPlayer;
    public bool active = true;

	[HideInInspector]
	public Vector3 impactPoint;

	protected virtual void Start () {
        if (active)
        {
            if (transform.tag == "Untagged")
                transform.tag = "MovingPlatform";
            TestChildren(transform);
        }
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

    public void Move(Vector3 movement, bool worldSpace = false)
    {
        transform.localPosition += movement;
        if(currPlayer!= null)
            currPlayer.ImmediateMovement((worldSpace ? Quaternion.identity :(Quaternion.AngleAxis(Vector3.Angle(Vector3.up, transform.up), (Vector3.Cross(Vector3.up, transform.up) != Vector3.zero ? Vector3.Cross(Vector3.up, transform.up) : Vector3.forward)))) * movement, true, false);
    }

	virtual public void AddPlayer(CharController player, Vector3 playerImpactPoint) {
		currPlayer = player;
		impactPoint = playerImpactPoint;
	}

	virtual public void RemovePlayer() {
		currPlayer = null;
	}
}
