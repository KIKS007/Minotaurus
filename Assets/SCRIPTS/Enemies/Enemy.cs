using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Sirenix.OdinInspector;

public enum EnemyState { Normal, Stunned, Stuck, Pushed, Dead };

public class Enemy : MonoBehaviour 
{
	public EnemyState enemyState = EnemyState.Normal;
	public LayerMask wallLayer;

	[Header ("Health")]
	[Range (0, 100)]
	public float health = 5;

	[Header ("Speed")]
	public float speed = 10;

	[Header ("LookAt")]
	public float lookAtLerp = 0.5f;

	[Header ("Follow")]
	public float followUpdateDelay = 0.1f;

	[Header ("Debug")]
	public Transform target;
	public float distanceFromTarget;
	public bool debugLog = true;

	private float _initialSpeed;
	private float _initialLookAtLerp;
	private NavMeshAgent _navMeshAgent;
	private Transform _player;
	private Rigidbody _rigidbody;

	// Use this for initialization
	void Start () 
	{
		Setup ();
	}

	public void Setup ()
	{
		enemyState = EnemyState.Normal;

		_player = GameObject.FindGameObjectWithTag ("Player").transform;

		_rigidbody = GetComponent<Rigidbody> ();
		_navMeshAgent = GetComponent<NavMeshAgent> ();
		_initialSpeed = speed;
		_initialLookAtLerp = lookAtLerp;

		_navMeshAgent.speed = speed;

		StartCoroutine (SetFollowUpdate ());
	}
	
	void Update () 
	{
		LookAt ();

		DistanceFromTarget ();
	}

	void FixedUpdate () 
	{

	}

	void LookAt ()
	{
		if (target == null)
			return;

		if (enemyState == EnemyState.Stunned)
			return;


		Vector3 targetPosition = target.position;
		targetPosition.y = transform.position.y;

		Quaternion rotation = Quaternion.LookRotation (targetPosition - transform.position);

		transform.rotation = Quaternion.Lerp (transform.rotation, rotation, lookAtLerp);
	}

	void DistanceFromTarget ()
	{
		if (target == null)
			distanceFromTarget = -1;
		else
			distanceFromTarget = Vector3.Distance (transform.position, target.position);
	}

	void OnCollisionEnter (Collision collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			if(debugLog)
				Debug.Log (name + " : player hit!");
		}

		if(collision.gameObject.tag == "Wall" || (wallLayer.value & 1<<collision.gameObject.layer) == 1 <<collision.gameObject.layer)
		{
			if(debugLog)
				Debug.Log (name + " : wall hit!");
		}
	}

	public void Push (Vector3 direction, float force, ForceMode forcemode)
	{
		enemyState = EnemyState.Pushed;

		Stuck (false);

		_rigidbody.AddForce (direction* force, forcemode);

		StartCoroutine (WaitZeroVelocity (()=> Unstuck ()));
	}

	public void Push (Vector3 direction, float force, ForceMode forcemode, float duration)
	{
		enemyState = EnemyState.Pushed;

		_navMeshAgent.enabled = false;

		_rigidbody.isKinematic = false;

		_rigidbody.AddForce (direction* force, forcemode);

		DOVirtual.DelayedCall (duration, Unstuck);
	}

	public void Stun (float duration)
	{
		enemyState = EnemyState.Stunned;

		Stuck (false);

		DOVirtual.DelayedCall (duration, Unstuck);
	}

	public void Stuck (bool changeState = true)
	{
		if(changeState)
			enemyState = EnemyState.Stuck;

		_navMeshAgent.enabled = false;
		_rigidbody.isKinematic = false;
	}

	public void Unstuck ()
	{
		enemyState = EnemyState.Normal;

		_rigidbody.isKinematic = true;
		_navMeshAgent.enabled = true;
	}

	public IEnumerator WaitZeroVelocity (System.Action action)
	{
		yield return new WaitUntil (() => _rigidbody.velocity.magnitude < 1.5f);

		action ();
	}

	public void Hit (int damage)
	{
		health -= damage;

		if(debugLog)
			Debug.Log (name + " : -" + damage);

		if(health <= 0)
			Death ();
	}

	public void Death ()
	{
		if(debugLog)
			Debug.Log (name + " : dead!");

		Destroy (gameObject);
	}

	public void SetDestinationTemporarily (Transform target, float duration)
	{
		_navMeshAgent.SetDestination (target.position);

		this.target = target;

		StopCoroutine (SetFollowUpdate ());
		StartCoroutine (SetFollowUpdate ());

		DOVirtual.DelayedCall (duration, ()=> SetPlayerDestination ());
	}

	[ButtonAttribute("Follow Player")]
	public void SetPlayerDestination ()
	{
		if(_player == null)
			_player = GameObject.FindGameObjectWithTag ("Player").transform;

		target = _player;

		StopCoroutine (SetFollowUpdate ());
		StartCoroutine (SetFollowUpdate ());

		_navMeshAgent.SetDestination (_player.position);
	}

	public void SetSpeed (float speedValue)
	{
		speed = speedValue;

		_navMeshAgent.speed = speed;
	}

	public void SetSpeed (float speedValue, float angularSpeedValue)
	{
		speed = speedValue;
		lookAtLerp = angularSpeedValue;

		_navMeshAgent.speed = speed;
	}

	public void ResetSpeed ()
	{
		speed = _initialSpeed;
		lookAtLerp = _initialLookAtLerp;
	}

	IEnumerator SetFollowUpdate ()
	{
		if (target == null)
			yield return new WaitUntil (()=> target != null);

		if(_navMeshAgent.enabled)
		{
			if(distanceFromTarget > _navMeshAgent.stoppingDistance)
				_navMeshAgent.SetDestination (target.position);
		}

		yield return new WaitForSeconds (followUpdateDelay);

		StartCoroutine (SetFollowUpdate ());
	}
}
