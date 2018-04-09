
using bat.opt.bake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRuntimeWork : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        //not support multi-material baking
        //gameObject.AddComponent<BAT_MildBaker>();
        gameObject.AddComponent<BAT_DeepBaker>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
