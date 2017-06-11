using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyFollow : EnemyComponent 
{
	[MinMaxSliderAttribute (0, 100)]
	public Vector2 followRange;

	protected override void Update ()
	{
		base.Update ();

		if(_enemyScript.distanceFromPlayer > followRange.x && _enemyScript.distanceFromPlayer < followRange.y)
		{
			_enemyScript.LookAt (_enemyScript._player);

			if (!_navMeshAgent.enabled)
				_enemyScript.SetPlayerDestination ();
		}
		else if(_navMeshAgent.enabled)
			_enemyScript.StopFollow ();
			
	}
}
