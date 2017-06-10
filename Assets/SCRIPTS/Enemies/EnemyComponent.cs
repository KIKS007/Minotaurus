using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyComponent : MonoBehaviour 
{
	protected NavMeshAgent _navMeshAgent;

	protected virtual void Start () 
	{
		_navMeshAgent = GetComponent<NavMeshAgent> ();
	}
	
	protected virtual void Update () 
	{
		
	}
}
