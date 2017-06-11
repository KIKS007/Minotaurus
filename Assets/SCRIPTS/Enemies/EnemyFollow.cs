using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyFollow : EnemyComponent 
{
	[Header ("Follow")]
	[MinMaxSliderAttribute (0, 100)]
	public Vector2 followRange;
	[MinMaxSliderAttribute (-10f, 10f)]
	public Vector2 followMaxRangeRandom;
	[MinMaxSliderAttribute (0, 50)]
	public Vector2 followSpeedRange = new Vector2 (8, 12);

	[Header ("Follow Player Direction")]
	public float followPlayerDirectionThreshold = 10f;
	[MinMaxSliderAttribute (-10f, 10f)]
	public Vector2 playerDirectionFactorLimits;
	public float playerDirectionFactor;

	private Transform _playerDirection;
	private Vector3 _playerPreviousPosition;

	protected override void Start ()
	{
		base.Start ();

		_playerDirection = new GameObject ().transform;
		_playerDirection.SetParent (transform);
		_playerDirection.name = "Player Direction";

		followRange.y += Random.Range (followMaxRangeRandom.x, followMaxRangeRandom.y);

		playerDirectionFactor = Random.Range (playerDirectionFactorLimits.x, playerDirectionFactorLimits.y);

		_enemyScript.speed = Random.Range (followSpeedRange.x, followSpeedRange.y);
		_enemyScript._initialSpeed = _enemyScript.speed;

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
			_enemyScript.LookAt (EnemyManager.Instance._player);

			if(_enemyScript.distanceFromPlayer < followPlayerDirectionThreshold)
			{
				if (!_enemyScript.IsMoving () || _enemyScript.target != EnemyManager.Instance._player)
					_enemyScript.SetPlayerDestination ();
			}

			else
			{
				SetPlayerDirection ();

				if (!_enemyScript.IsMoving () || _enemyScript.target != _playerDirection)
					_enemyScript.SetDestination (_playerDirection);
			}
		}

		else if(_enemyScript.IsMoving ())
			_enemyScript.StopFollow ();
			
	}

	void SetPlayerDirection ()
	{
		//		_playerDirection.position = _playerRigidbody.position + _playerRigidbody.velocity.normalized;

		_playerDirection.position = EnemyManager.Instance._player.position + (EnemyManager.Instance._player.position - _playerPreviousPosition).normalized * playerDirectionFactor;

		_playerPreviousPosition = EnemyManager.Instance._player.position;
	}
}
