using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCloseCombat : EnemyComponent 
{
	public GameObject cacCollider;
	public float cacCooldown = 1f;

	protected override void Start ()
	{
		base.Start ();
	}

	void OnTriggerEnter (Collider collider)
	{
		if(collider.gameObject.tag == "Player")
		{
			if(EnemyManager.Instance.debugLog)
				Debug.Log (name + " : player hit!");

			cacCollider.SetActive (false);

			StartCoroutine (Cooldown ());
		}
	}

	IEnumerator Cooldown ()
	{
		yield return new WaitForSeconds (cacCooldown);

		cacCollider.SetActive (true);
	}
}
