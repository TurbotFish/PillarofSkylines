using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
	public class PlayerModel : MonoBehaviour
	{

		public int Favours{ get; set; }

		//###########################################################

		#region monobehaviour methods

		// Use this for initialization
		void Start ()
		{
		
		}
	
		// Update is called once per frame
		void Update ()
		{
		
		}

		#endregion monobehaviour methods

		//###########################################################
		//###########################################################

		bool CheckAbilityActive (eAbility ability)
		{
			return true;
		}

		//###########################################################
	}
}
//end of namespace

