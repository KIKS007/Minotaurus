using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCloseCombat : EnemyComponent 
{
	void OnTriggerEnter (Collider collider)
	{
		if(collider.gameObject.tag == "Player")
		{
			if(EnemyManager.Instance.debugLog)
				Debug.Log (name + " : player hit!");
		}
	}
}
