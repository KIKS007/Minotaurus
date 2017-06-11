using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shield : MonoBehaviour {

    public int life;
    public int damage;
    public float firerate;

    private List<Enemy> _enemyList = new List<Enemy>();
    private int _life;
    private Vector3 _scale;
    private Renderer _rend;
    private float _firerate;

	// Use this for initialization
	void Start () {
        _life = life;
        _firerate = firerate;
        _scale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(_scale, 0.5f);
        _rend = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {

        _firerate -= Time.deltaTime;
        if (_firerate <= 0f)
        {
            foreach (Enemy e in _enemyList)
            {
                e.Hit(damage);
            }
            _firerate = firerate;
        }
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Enemy")
        {
            _enemyList.Add(col.GetComponent<Enemy>());
        }

        if (col.tag == "Enemy Ammo")
        {
            Destroy(col.gameObject);
            //_life -= ammo.damage;
            _life -= 5;
            float colorValue = ((float)_life * _rend.material.color.r) /(float)life ;
            _rend.material.color = new Color(colorValue, colorValue, colorValue, 1f);
            if(_life <= 0)
            {
                transform.DOScaleY(0, 0.2f).OnComplete(() => Destroy(gameObject));
            }
        }
    }
    void OnTriggerExit(Collider col)
    {
        if(col.tag == "Enemy")
        {
            Enemy temp = col.GetComponent<Enemy>();
            _enemyList.Remove(temp);
        }
    }
}
