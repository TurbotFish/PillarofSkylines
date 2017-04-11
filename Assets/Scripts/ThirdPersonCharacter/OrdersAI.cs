using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Order{
	public float t;
	public Vector2 mvt;
	public bool jump;
	public bool holdRunning;
	public bool echo;
	public bool drift;
	//public float rotation;
}

[System.Serializable]
public class OrderGroup
{
	public List<Order> orders = new List<Order>();
}

public class OrdersAI : MonoBehaviour {

    public bool playOnStart;
    public int orderGroupToPlayOnStart;

	public List<OrderGroup> orderGroups = new List<OrderGroup>();
	private ThirdPersonControllerAI _TPCAI;
	private EchoManager _EM;

	void Start () {
		_TPCAI = GetComponent<ThirdPersonControllerAI>();
		_EM = GetComponent<EchoManager>();
        if (playOnStart)
            ReadGroupOrder(orderGroupToPlayOnStart);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.U))
		{
			_TPCAI.Jump();
			Debug.Log("jump");
		}
	}
	public void ReadGroupOrder(int i)
	{
		StartCoroutine(ReadOrders(i));
	}


	IEnumerator ReadOrders (int o)
	{
		for (int i = 0; i<orderGroups[o].orders.Count;i++)
		{
			Order _order = orderGroups[o].orders[i];
			yield return new WaitForSeconds(_order.t);
			_TPCAI.AImvt = _order.mvt;
			if(_order.jump) _TPCAI.AIjumping = true;
			if(_order.echo) _EM.CreateEcho();
			if(_order.drift) _EM.Drift();
			if(_order.holdRunning) _TPCAI.AIrunning = true;
			if(!_order.holdRunning) _TPCAI.AIrunning = false;
			//yield return null;
		}

	}
}
