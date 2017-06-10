using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public Transform Canon;
    public Ammo Mun1;
    public Ammo Mun2;
    public int ammo1;
    public int ammo2;
    private float _cooldown1 = 0f;
    private float _cooldown2 = 0f;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        if (_cooldown1 > 0f)
        {
            _cooldown1 -= Time.deltaTime;
        }
        else
        {
            if (Input.GetButton("Fire1"))
            {
                if (Mun1 != null && ammo1 > 0)
                {
                    Instantiate(Mun1, Canon.position, transform.rotation);
                    _cooldown1 = Mun1.cooldown;
                    ammo1--;
                }
            }
        }

        if (_cooldown2 > 0f)
        {
            _cooldown2 -= Time.deltaTime;
        }
        else
        {
            if (Input.GetButton("Fire2"))
            {
                if (Mun2 != null && ammo2 > 0)
                {
                    Instantiate(Mun2, Canon.position, transform.rotation);
                    _cooldown2 = Mun2.cooldown;
                    ammo2--;
                }
            }
        }
    }
}
