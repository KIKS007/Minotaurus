using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

    public GameObject Head;
    public int damage;
    public float firerate;
    public float duration;

    private List<Enemy> _enemyList = new List<Enemy>();
    private float _time;
    private Enemy closer;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, duration);
	}
	
	// Update is called once per frame
	void Update () {
        if(closer != null)
            Head.transform.LookAt(closer.transform.position);

        _time -= Time.deltaTime;
        if(_time <= 0f)
        {
            if (_enemyList.Count > 0)
            {
                float distMin = 1000f;
                foreach (Enemy e in _enemyList)
                {
                    if (e == null)
                        return;
                    float dist = Vector3.Distance(transform.position, e.transform.position);
                    if (distMin == 1000f || distMin > dist)
                    {
                        distMin = dist;
                        closer = e;
                    }
                }
                Shoot();
            }
            _time = firerate;
        }
	}

    void Shoot()
    {
        closer.Hit(damage);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Enemy")
        {
            _enemyList.Add(col.GetComponent<Enemy>());
        }

    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Enemy")
        {
            Enemy temp = col.GetComponent<Enemy>();
            _enemyList.Remove(temp);
            if (temp == closer)
                closer = null;
        }
    }
}
