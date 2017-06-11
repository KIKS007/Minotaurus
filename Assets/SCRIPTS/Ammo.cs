using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour {

    public float force;
    public int damage;
    public GameObject child;
    [Header("Continuous Damage")]
    public float duration;
    public float rate;
    [Header("Weapon Related")]
    public float cooldown;
    public float recoil;
    [Header("Enemy Related")]
    public float knockback;
    
    private float _timer = -1f;
    private Rigidbody _rb;
    private bool _hitting;
    private Enemy daddy;

	// Use this for initialization
	void Start () {
        _rb = GetComponent<Rigidbody>();
        _rb.AddForce(transform.up * force);
	}
	
	// Update is called once per frame
	void Update () {
        if (_hitting)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                daddy.Hit(damage);
                _timer = 1f;
            }
        }
	}

    void OnCollisionEnter(Collision col)
    {
        if (child != null && col.transform.tag != "Ammo")
        {
            Instantiate(child, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else
        {
            if (damage != 0f && col.transform.tag == "Enemy")
            {
                col.transform.GetComponent<Enemy>().Push(_rb.velocity.normalized, knockback, ForceMode.Impulse);
                if (duration != 0f)
                {
                    transform.GetComponent<Collider>().enabled = false;
                    _rb.isKinematic = true;
                    transform.parent = col.transform;
                    daddy = transform.parent.GetComponent<Enemy>();
                    _hitting = true;
                    Destroy(gameObject, duration);
                }
                else
                {
                    col.transform.GetComponent<Enemy>().Hit(damage);
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject, 2f);
            }
        }
    }
}
