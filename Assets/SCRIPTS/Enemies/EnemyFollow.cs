using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyFollow : EnemyComponent 
{
	[MinMaxSliderAttribute (0, 100)]
	public Vector2 followRange;

	protected override void Start ()
	{
		base.Start ();

		if(_enemyScript.distanceFromPlayer > followRange.x && _enemyScript.distanceFromPlayer < followRange.y)
			_enemyScript.SetPlayerDestination ();
	}

	protected override void Update ()
	{
		base.Update ();

		if (_enemyScript.enemyState != EnemyState.Normal)
			return;

		if(_enemyScript.distanceFromPlayer > followRange.x && _enemyScript.distanceFromPlayer < followRange.y)
		{
			_enemyScript.LookAt (_enemyScript._player);

			if (!_enemyScript.IsMoving ())
				_enemyScript.SetPlayerDestination ();
		}

		else if(_enemyScript.IsMoving ())
			_enemyScript.StopFollow ();
			
	}
}
