using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Sirenix.OdinInspector;

public enum EnemyState { Normal, Stunned, Stuck, Pushed, Dead };
public delegate void EventHandler ();

public class Enemy : MonoBehaviour 
{
	public EnemyState enemyState = EnemyState.Normal;

	[Header ("Health")]
	public float health = 5;

	[Header ("Speed")]
	public float speed = 10;

	[Header ("LookAt")]
	public float lookAtLerp = 0.5f;
	public bool lookAtHead = false;

	[Header ("Follow")]
	public float followUpdateDelay = 0.1f;

	[Header ("Debug")]
	public Transform target;
	public float distanceFromPlayer;
	public bool debugLog = true;

	private float _initialSpeed;
	private NavMeshAgent _navMeshAgent;
	[HideInInspector]
	public Transform _player;
	private Rigidbody _rigidbody;
	[HideInInspector]
	public Transform _head;
	[HideInInspector]
	public float _distanceFromTarget;

	public event EventHandler OnStuck;
	public event EventHandler OnUntuck;
	public event EventHandler OnStun;
	public event EventHandler OnPush;
	public event EventHandler OnHit;
	public event EventHandler OnDeath;

	/*public float force;
	[Button]
	public void PushDebug ()
	{
		Push (-transform.forward, force, ForceMode.Impulse);
	}*/

	// Use this for initialization
	void Start () 
	{
		Setup ();
	}

	public void Setup ()
	{
		enemyState = EnemyState.Normal;

		_player = GameObject.FindGameObjectWithTag ("Player").transform;

		if(lookAtHead)
			_head = transform.GetChild (0);

		_rigidbody = GetComponent<Rigidbody> ();
		_navMeshAgent = GetComponent<NavMeshAgent> ();
		_initialSpeed = speed;

		_navMeshAgent.speed = speed;

		StartCoroutine (SetFollowUpdate ());
	}
	
	void Update () 
	{
		DistanceFromTarget ();
	}

	public bool IsMoving ()
	{
		if (_navMeshAgent.enabled && _navMeshAgent.hasPath)
			return true;
		else
			return false;
	}

	void FixedUpdate () 
	{

	}

	public void LookAt ()
	{
		if (target == null)
			return;

		if (enemyState == EnemyState.Stunned)
			return;

		Vector3 targetPosition = target.position;

		Quaternion rotation = Quaternion.LookRotation (targetPosition - _head.position);

		if(lookAtHead)
			_head.rotation = Quaternion.Lerp (_head.rotation, rotation, lookAtLerp);

		targetPosition.y = transform.position.y; 
		rotation = Quaternion.LookRotation (targetPosition - transform.position);
		transform.rotation = Quaternion.Lerp (transform.rotation, rotation, lookAtLerp);
	}

	public void LookAt (Transform target, float lerpValue = -1)
	{
		if (target == null)
			return;

		if (enemyState == EnemyState.Stunned)
			return;

		Vector3 targetPosition = target.position;

		Quaternion rotation = Quaternion.LookRotation (targetPosition - _head.position);

		float lerp = lerpValue != -1 ? lerpValue : lookAtLerp;

		if(lookAtHead)
			_head.rotation = Quaternion.Lerp (_head.rotation, rotation, lerp);

		targetPosition.y = transform.position.y; 
		rotation = Quaternion.LookRotation (targetPosition - transform.position);
		transform.rotation = Quaternion.Lerp (transform.rotation, rotation, lerp);
	}

	public void LookAt (Vector3 target, float lerpValue = -1)
	{
		if (enemyState == EnemyState.Stunned)
			return;

		Quaternion rotation = Quaternion.LookRotation (target - _head.position);

		float lerp = lerpValue != -1 ? lerpValue : lookAtLerp;

		if(lookAtHead)
			_head.rotation = Quaternion.Lerp (_head.rotation, rotation, lerp);

		target.y = transform.position.y; 
		rotation = Quaternion.LookRotation (target - transform.position);
		transform.rotation = Quaternion.Lerp (transform.rotation, rotation, lerp);
	}

	void DistanceFromTarget ()
	{
		if (target == null)
			_distanceFromTarget = -1;
		else
			_distanceFromTarget = Vector3.Distance (transform.position, target.position);

		distanceFromPlayer = Vector3.Distance (transform.position, _player.position);
	}

	void OnCollisionEnter (Collision collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			if(debugLog)
				Debug.Log (name + " : player hit!");
		}

		if(collision.gameObject.tag == "Wall" || (EnemyManager.Instance.wallLayer.value & 1<<collision.gameObject.layer) == 1 <<collision.gameObject.layer)
		{
			if(debugLog)
				Debug.Log (name + " : wall hit!");
		}
	}

	public void Push (Vector3 direction, float force, ForceMode forcemode)
	{
		enemyState = EnemyState.Pushed;

		if (OnPush != null)
			OnPush ();

		Stuck (false);

		_rigidbody.AddForce (direction* force, forcemode);

		StartCoroutine (WaitZeroVelocity (()=> Unstuck ()));
	}

	public void Push (Vector3 direction, float force, ForceMode forcemode, float duration)
	{
		enemyState = EnemyState.Pushed;

		if (OnPush != null)
			OnPush ();

		Stuck (false);

		_rigidbody.AddForce (direction* force, forcemode);

		DOVirtual.DelayedCall (duration, Unstuck);
	}

	public void Stun (float duration)
	{
		enemyState = EnemyState.Stunned;

		if (OnStun != null)
			OnStun ();

		Stuck (false);

		DOVirtual.DelayedCall (duration, Unstuck);
	}

	public void Stuck (bool changeState = true)
	{
		if(changeState)
			enemyState = EnemyState.Stuck;

		_navMeshAgent.enabled = false;
		_rigidbody.isKinematic = false;

		if (OnStuck != null)
			OnStuck ();
	}

	public void Unstuck ()
	{
		enemyState = EnemyState.Normal;

		_rigidbody.velocity = Vector3.zero;
		_rigidbody.isKinematic = true;
		_navMeshAgent.enabled = true;
		
		if (OnUntuck != null)
			OnUntuck ();
	}

	public IEnumerator WaitZeroVelocity (System.Action action)
	{
		yield return new WaitForSeconds (0.2f);

		yield return new WaitUntil (() => _rigidbody.velocity.magnitude < 1.5f);

		action ();
	}

	public void Hit (int damage)
	{
		health -= damage;

		if (OnHit != null)
			OnHit ();

		if(debugLog)
			Debug.Log (name + " : -" + damage);

		if(health <= 0)
			Death ();
	}

	public void Death ()
	{
		if(debugLog)
			Debug.Log (name + " : dead!");

		if (OnDeath != null)
			OnDeath ();

		Destroy (gameObject);
	}

	public void SetDestinationTemporarily (Transform target, float duration)
	{
		_navMeshAgent.SetDestination (target.position);

		_navMeshAgent.enabled = true;

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

		_navMeshAgent.enabled = true;

		target = _player;

		StopCoroutine (SetFollowUpdate ());
		StartCoroutine (SetFollowUpdate ());

		_navMeshAgent.SetDestination (_player.position);
	}

	public void StopFollow ()
	{
		StopCoroutine (SetFollowUpdate ());

		_navMeshAgent.ResetPath ();
		target = null;
	}

	public void SetSpeed (float speedValue)
	{
		speed = speedValue;

		_navMeshAgent.speed = speed;
	}

	public void ResetSpeed ()
	{
		speed = _initialSpeed;
	}

	IEnumerator SetFollowUpdate ()
	{
		if (target == null)
			yield return new WaitUntil (()=> target != null);

		if(_navMeshAgent.enabled)
		{
			if(_distanceFromTarget > _navMeshAgent.stoppingDistance && _navMeshAgent.enabled)
				_navMeshAgent.SetDestination (target.position);
		}

		yield return new WaitForSeconds (followUpdateDelay);

		StartCoroutine (SetFollowUpdate ());
	}
}
