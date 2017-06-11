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

	[Header ("Debug")]
	public Transform target;
	public Vector3 targetPos;
	public float distanceFromPlayer;

	[HideInInspector]
	public float _initialSpeed;
	private NavMeshAgent _navMeshAgent;
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

	public bool IsFollowingTransform ()
	{
		if (target != null)
			return true;
		else
			return false;
	}

	public bool IsFollowingVector ()
	{
		if (targetPos != Vector3.zero)
			return true;
		else
			return false;
	}

	void FixedUpdate () 
	{

	}

	public void LookAt (Transform target, float lerpValue = -1)
	{
		if (target == null)
			return;

		if (enemyState == EnemyState.Stunned)
			return;

		Vector3 targetPosition = target.position;

		float lerp = lerpValue != -1 ? lerpValue : lookAtLerp;

		Quaternion rotation = new Quaternion ();

		if(lookAtHead)
		{
			rotation = Quaternion.LookRotation (targetPosition - _head.position);
			_head.rotation = Quaternion.Lerp (_head.rotation, rotation, lerp);
		}

		targetPosition.y = transform.position.y; 
		rotation = Quaternion.LookRotation (targetPosition - transform.position);
		transform.rotation = Quaternion.Lerp (transform.rotation, rotation, lerp);
	}

	public void LookAt (Vector3 target, float lerpValue = -1)
	{
		if (enemyState == EnemyState.Stunned)
			return;

		float lerp = lerpValue != -1 ? lerpValue : lookAtLerp;

		Quaternion rotation = new Quaternion ();

		if(lookAtHead)
		{
			rotation = Quaternion.LookRotation (target - _head.position);
			_head.rotation = Quaternion.Lerp (_head.rotation, rotation, lerp);
		}

		target.y = transform.position.y; 
		rotation = Quaternion.LookRotation (target - transform.position);
		transform.rotation = Quaternion.Lerp (transform.rotation, rotation, lerp);
	}

	void DistanceFromTarget ()
	{
		if (target == null && targetPos == Vector3.zero)
			_distanceFromTarget = -1;
		else
		{
			if(target != null)
				_distanceFromTarget = Vector3.Distance (transform.position, target.position);
			else 
				_distanceFromTarget = Vector3.Distance (transform.position, targetPos);
		}

		distanceFromPlayer = Vector3.Distance (transform.position, EnemyManager.Instance._player.position);
	}

	void OnCollisionEnter (Collision collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			if(EnemyManager.Instance.debugLog)
				Debug.Log (name + " : player hit!");
		}

		if(collision.gameObject.tag == "Wall" || (EnemyManager.Instance.wallLayer.value & 1<<collision.gameObject.layer) == 1 <<collision.gameObject.layer)
		{
			if(EnemyManager.Instance.debugLog)
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

		_rigidbody.AddForce (direction * force, forcemode);

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

		if(EnemyManager.Instance.debugLog)
			Debug.Log (name + " : -" + damage);

		if(health <= 0)
			Death ();
	}

	public void Death ()
	{
		if(EnemyManager.Instance.debugLog)
			Debug.Log (name + " : dead!");

		if (OnDeath != null)
			OnDeath ();

		Destroy (gameObject);
	}

	public void SetDestinationTemporarily (Transform target, float duration)
	{
		_navMeshAgent.SetDestination (target.position);

		_navMeshAgent.enabled = true;

		targetPos = Vector3.zero;
		this.target = target;

		StopCoroutine (SetFollowUpdate ());
		StartCoroutine (SetFollowUpdate ());

		DOVirtual.DelayedCall (duration, ()=> SetPlayerDestination ());
	}

	public void SetDestination (Vector3 target)
	{
		_navMeshAgent.SetDestination (target);

		_navMeshAgent.enabled = true;

		targetPos = target;
		this.target = null;

		StopCoroutine (SetFollowUpdate ());
		StartCoroutine (SetFollowUpdate ());
	}

	public void SetDestination (Transform target)
	{
		_navMeshAgent.SetDestination (target.position);

		_navMeshAgent.enabled = true;

		this.target = target;
		targetPos = Vector3.zero;

		StopCoroutine (SetFollowUpdate ());
		StartCoroutine (SetFollowUpdate ());
	}

	public void SetPlayerDestination ()
	{
		_navMeshAgent.enabled = true;

		target = EnemyManager.Instance._player;
		targetPos = Vector3.zero;

		StopCoroutine (SetFollowUpdate ());
		StartCoroutine (SetFollowUpdate ());

		_navMeshAgent.SetDestination (EnemyManager.Instance._player.position);
	}

	public void StopFollow ()
	{
		StopCoroutine (SetFollowUpdate ());

		_navMeshAgent.ResetPath ();
		target = null;
		targetPos = Vector3.zero;
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

		yield return new WaitForSeconds (EnemyManager.Instance.updateTime);

		StartCoroutine (SetFollowUpdate ());
	}
}
