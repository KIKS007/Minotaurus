using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class EnemyShooting : EnemyComponent 
{
	[Header ("Shoot")]
	public GameObject projectile;
	[MinMaxSliderAttribute (0, 100)]
	public Vector2 shootingRange;
	[MinMaxSliderAttribute (0, 2)]
	public Vector2 shootingDelay;
	public float shootingCooldown;
	public float shootingLookAtLerp = 0.8f;
	public float projectileSpawnDistance = 1f;

	private bool _canShoot = true;
	private float _initialLookAtLerp;

	protected override void Start ()
	{
		base.Start ();

		_initialLookAtLerp = _enemyScript.lookAtLerp;
	}

	protected override void Update ()
	{
		base.Update ();

		if(_enemyScript.distanceFromPlayer > shootingRange.x && _enemyScript.distanceFromPlayer < shootingRange.y)
		{
			_enemyScript.LookAt (_enemyScript._player);

			_enemyScript.lookAtLerp = shootingLookAtLerp;

			if(_canShoot)
			{
				_canShoot = false;

				if (shootingDelay.y > 0)
					DOVirtual.DelayedCall (Random.Range (shootingDelay.x, shootingDelay.y), () => Shoot ());
				else
					Shoot ();
			}
		}
		else
			_enemyScript.lookAtLerp = _initialLookAtLerp;
	}

	void Shoot ()
	{
		Vector3 position = transform.position + transform.forward * projectileSpawnDistance;
		Quaternion rotation = transform.rotation;

		if(_enemyScript.lookAtHead)
			rotation = _enemyScript._head.transform.rotation;

		Instantiate (projectile, position, rotation);

		DOVirtual.DelayedCall (shootingCooldown, ()=> _canShoot = true);	
	}
}
