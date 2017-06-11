using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotDrinkManager : MonoBehaviour {

    public GameObject drink;
    public Transform posDrink;

    public int nbmin_drink = 2;
    public int nbmax_drink = 5;

    public void UseHotDrink()
    {
        int nb_drink = Random.Range(nbmin_drink, nbmax_drink + 1);
        for (int i = 0; i < nb_drink; i++)
        {
            Instantiate(drink, posDrink.position, drink.transform.rotation);
        }
    }
}
