using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class EnemyManager : MonoBehaviour 
{
	public LayerMask wallLayer;

	public static EnemyManager Instance;

	void Awake ()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy (gameObject);

		DontDestroyOnLoad (this);
	}
}
