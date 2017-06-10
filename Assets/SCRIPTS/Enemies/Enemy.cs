using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class Enemy : MonoBehaviour 
{
	[Header ("Health")]
	[Range (0, 100)]
	public float health = 5;

	[Header ("Speed")]
	public float speed = 10;
	public float lookAtLerp = 0.5f;

	[Header ("Update")]
	public float updateDelay = 0.1f;

	[Header ("Debug")]
	public Transform target;
	public bool debugLog = true;

	private float _initialSpeed;
	private float _initialLookAtLerp;
	private NavMeshAgent _navMeshAgent;
	private Transform _player;

	// Use this for initialization
	void Start () 
	{
		Setup ();
	}

	public void Setup ()
	{
		_player = GameObject.FindGameObjectWithTag ("Player").transform;

		_navMeshAgent = GetComponent<NavMeshAgent> ();
		_initialSpeed = speed;
		_initialLookAtLerp = lookAtLerp;

		_navMeshAgent.speed = speed;

		StartCoroutine (SetDestinationUpdate ());
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void LookAt ()
	{
		
	}

	void OnCollisionEnter (Collision collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			if(debugLog)
				Debug.Log (name + " : player hit!");
		}
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

	public void SetDestination (Transform target, float duration)
	{
		_navMeshAgent.SetDestination (target.position);

		this.target = target;

		StopCoroutine (SetDestinationUpdate ());
		StartCoroutine (SetDestinationUpdate ());

		DOVirtual.DelayedCall (duration, ()=> SetPlayerDestination ());
	}

	public void SetPlayerDestination ()
	{
		if(_player == null)
			_player = GameObject.FindGameObjectWithTag ("Player").transform;

		target = _player;

		StopCoroutine (SetDestinationUpdate ());
		StartCoroutine (SetDestinationUpdate ());

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

	IEnumerator SetDestinationUpdate ()
	{
		if (target == null)
			yield return new WaitUntil (()=> target != null);

		_navMeshAgent.SetDestination (target.position);

		yield return new WaitForSeconds (updateDelay);

		StartCoroutine (SetDestinationUpdate ());
	}
}
