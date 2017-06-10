using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class EnemyTest : MonoBehaviour 
{
	public Transform target;

	private NavMeshAgent agent;

	[ButtonAttribute]
	public void MoveToTarget ()
	{
		agent.SetDestination (target.position);
	}

	// Use this for initialization
	void Start () 
	{
		agent = GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
