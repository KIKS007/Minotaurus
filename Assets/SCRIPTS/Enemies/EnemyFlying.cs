using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class EnemyFlying : EnemyComponent 
{
	public enum Avoidance { None, Up, Down }

	[Header ("Avoidance")]
	public Avoidance avoidance = Avoidance.None;
	public float rayDistance = 3f;
	public float avoidanceSpeed = 1;
	public float avoidanceLerp = 0.1f;

	[Header ("Prefered Height")]
	[MinMaxSliderAttribute (0, 10)]
	public Vector2 preferedHeightLimits;
	public float preferedHeightDelay = 1f;
	public float floatingHeight = 1f;
	public float floatingHeightSpeed = 1f;

	private float _preferedHeight;
	private Tween _floatingTween;

	protected override void Start ()
	{
		base.Start ();

		_preferedHeight = Random.Range (preferedHeightLimits.x, preferedHeightLimits.y);

		_floatingTween = DOTween.To (()=> _preferedHeight, x=> _preferedHeight =x, _preferedHeight + floatingHeight, floatingHeightSpeed).SetEase (Ease.OutQuad).SetLoops (-1, LoopType.Yoyo).SetSpeedBased ();

		_enemyScript.OnStuck += () => _floatingTween.Pause ();
		_enemyScript.OnUntuck += () => _floatingTween.Play ();
	}

	protected override void Update ()
	{
		base.Update ();

		if (_enemyScript.enemyState != EnemyState.Normal)
			return;

		Avoid ();

		if (avoidance == Avoidance.None)
			PreferedHeight ();
	}

	void PreferedHeight ()
	{
		_navMeshAgent.baseOffset = Mathf.Lerp (_navMeshAgent.baseOffset, _preferedHeight, avoidanceLerp * 0.1f);
	}

	void Avoid ()
	{
		Vector3 up = transform.forward;
		Vector3 down = up;
		up.y = 1f;
		down.y = -1f;

		RaycastHit hit1 = new RaycastHit ();
		Physics.Raycast (transform.position, up, out hit1, rayDistance, EnemyManager.Instance.wallLayer, QueryTriggerInteraction.Ignore);
		RaycastHit hit2 = new RaycastHit ();
		Physics.Raycast (transform.position, down, out hit2, rayDistance, EnemyManager.Instance.wallLayer, QueryTriggerInteraction.Ignore);

		Debug.DrawRay (transform.position, up.normalized * rayDistance, Color.blue);
		Debug.DrawRay (transform.position, down.normalized * rayDistance, Color.red);

		if(hit1.collider != null && hit2.collider == null || hit1.collider != null && hit1.distance < hit2.distance)
		{
			avoidance = Avoidance.Down;
			DOTween.Kill ("WaitPreferedHeight" + gameObject.GetInstanceID ());
			_navMeshAgent.baseOffset = Mathf.Lerp (_navMeshAgent.baseOffset, _navMeshAgent.baseOffset - avoidanceSpeed, avoidanceLerp);
		}

		else if(hit2.collider != null && hit1.collider == null || hit2.collider != null && hit1.distance > hit2.distance)
		{
			avoidance = Avoidance.Up;
			DOTween.Kill ("WaitPreferedHeight" + gameObject.GetInstanceID ());
			_navMeshAgent.baseOffset = Mathf.Lerp (_navMeshAgent.baseOffset, _navMeshAgent.baseOffset + avoidanceSpeed, avoidanceLerp);
		}
		
		if (hit1.collider == null && hit2.collider == null)
		{
			if (avoidance != Avoidance.None)
			{
				DOVirtual.DelayedCall (preferedHeightDelay, ()=> avoidance = Avoidance.None).SetId ("WaitPreferedHeight" + gameObject.GetInstanceID ());
			}
		}
	}
}
