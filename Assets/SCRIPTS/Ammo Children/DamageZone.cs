using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DamageZone : MonoBehaviour {

	public int damage;
    public float firerate;
    public float timeIn;
    public float timeStay;
    public float timeOut;

    private Vector3 _scale;
    private List<Enemy> enemyList = new List<Enemy>();

    void Start()
    {
        _scale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(_scale, timeIn).OnComplete(() =>
            DOVirtual.DelayedCall(timeStay, () =>
                transform.DOScale(0f, timeOut).OnComplete(() =>
                    Destroy(gameObject)
                )
            )
        );
        StartCoroutine(HitThemAll());
    }

    IEnumerator HitThemAll()
    {
        yield return new WaitForSeconds(firerate);

        foreach(Enemy e in enemyList)
        {
            e.Hit(damage);
        }
        StartCoroutine(HitThemAll());
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Enemy")
        {
            enemyList.Add(col.GetComponent<Enemy>());
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Enemy")
        {
            Enemy temp = col.GetComponent<Enemy>();
            enemyList.Remove(temp);
        }
    }
}
