using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyProjectile : MonoBehaviour 
{
	public int damage;
	public float speed = 5f;
	public float heightOffset = 0;
	public float gravity = 0;

	private Rigidbody _rigidbody;
	[HideInInspector]
	public Enemy _enemyScript;

	void Start ()
	{
		Transform player = GameObject.FindGameObjectWithTag ("Player").transform;

		transform.Rotate (-Vector3.right * heightOffset);

		_rigidbody = GetComponent<Rigidbody> ();

		_rigidbody.AddForce (transform.forward * speed, ForceMode.VelocityChange);
	}

	void FixedUpdate ()
	{
		_rigidbody.AddForce (Vector3.down * gravity, ForceMode.Acceleration);
	}

	void OnTriggerEnter (Collider collider)
	{
		if (collider.isTrigger)
			return;

		if(collider.gameObject.tag == "Player")
		{
			if(EnemyManager.Instance.debugLog)
				Debug.Log ("Player hit!");

			DOTween.Kill ("Projectile" + gameObject.GetInstanceID ());
			Destroy (gameObject);
		}

		if(collider.gameObject.tag == "Wall" || (EnemyManager.Instance.wallLayer.value & 1<<collider.gameObject.layer) == 1 <<collider.gameObject.layer)
		{
			_rigidbody.isKinematic = true;
			_rigidbody.velocity = Vector3.zero;

			DOVirtual.DelayedCall (2f, ()=> 
				{ 
					if(gameObject)
						Destroy (gameObject);
					
				}).SetId ("Projectile" + gameObject.GetInstanceID ());
		}
	}
}
