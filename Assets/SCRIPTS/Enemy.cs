using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Hit(int damage)
    {
        Debug.Log(damage);
    }

    public void SetSpeed(float speed)
    {

    }

    public void ResetSpeed()
    {

    }

    public void SetDestination(Transform t, float d)
    {

    }
}
