using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class EnemyManager : MonoBehaviour 
{
	[Header ("Update Time")]
	public float updateTime = 0.1f;

	[Header ("Layers")]
	public LayerMask wallLayer;

	[Header ("Debug")]
	public bool debugLog = true;

	public static EnemyManager Instance;
	[HideInInspector]
	public Transform _playerDirection;
	[HideInInspector]
	public Transform _player;
	[HideInInspector]
	public Rigidbody _playerRigidbody;

	void Awake ()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy (gameObject);

		DontDestroyOnLoad (this);
	}

	void Start ()
	{
		_player = GameObject.FindGameObjectWithTag ("Player").transform;
		_playerRigidbody = _player.GetComponent<Rigidbody> ();
	}
}
