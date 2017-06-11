using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buzzer : MonoBehaviour {

    public float duration;

    void Start()
    {
        Destroy(gameObject, duration);
    }

	void OnTriggerEnter (Collider col) {
		if(col.tag == "Enemy")
        {
            col.GetComponent<Enemy>().SetDestination(transform, duration);
        }
	}
}
