using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpeedEffector : MonoBehaviour {

    public float speed;
    public float timeIn;
    public float timeStay;
    public float timeOut;

    private Vector3 _scale;

    private List<Enemy> enemyList = new List<Enemy>();

    void Start()
    {
        _scale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(_scale, timeIn).OnComplete(()=> 
            DOVirtual.DelayedCall(timeStay, ()=>
                transform.DOScale(0f, timeOut).OnComplete(()=> 
                    Destroy(gameObject)
                )
            )
        );
    }

	void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Enemy")
        {
            enemyList.Add(col.GetComponent<Enemy>());
            enemyList[enemyList.Count - 1].SetSpeed(speed);
        }
    }
    
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Enemy")
        {
            Enemy temp = col.GetComponent<Enemy>();
            enemyList.Remove(temp);
            temp.ResetSpeed();


        }
    }
}
