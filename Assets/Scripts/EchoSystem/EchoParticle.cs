using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.EchoSystem
{
    public class EchoParticle : MonoBehaviour {

        public Vector3 target;
        public float speed;

	    void Update () {
            transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
	    }
    }
}
