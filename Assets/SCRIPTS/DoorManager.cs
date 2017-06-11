using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoorManager : MonoBehaviour {

    private float posYDrink;
    public GameObject door;
    public float timebeforedown = 2;

	// Use this for initialization
	void Start () {
        posYDrink = transform.position.y;
	}
	
	// Update is called once per frame  
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            door.transform.DOMoveY(posYDrink + transform.localScale.y + 0.5f, 1, false);
        }
    }

    void OnTriggerExit(Collider other)
    {
       if(other.tag == "Player")
        {
            DOVirtual.DelayedCall(timebeforedown, () => door.transform.DOMoveY(posYDrink, 1, false));
        }
    }
}
