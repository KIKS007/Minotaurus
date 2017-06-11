using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyComponent : MonoBehaviour 
{
	protected NavMeshAgent _navMeshAgent;
	protected Enemy _enemyScript;

	protected virtual void Start () 
	{
		_enemyScript = GetComponent<Enemy> ();
		_navMeshAgent = GetComponent<NavMeshAgent> ();
	}

	protected virtual void Update () 
	{
		
	}
}
