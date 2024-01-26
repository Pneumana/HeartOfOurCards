using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericBody : MonoBehaviour
{
    public int health;
    public int maxHealth;

    public int shield;
    public int thorns;

    //any % increases should use CeilToInt() so it all stays as ints

    //replace gameobject with netID
    public void TakeDamage(int damageRecieved, GenericBody damageSource)
    {
        var damageToTake = damageRecieved;
        var shieldThisTurn = shield;
        //shield damage block
        while (shieldThisTurn > 0 && damageToTake > 0) 
        {
            shieldThisTurn--;
            damageToTake--;
        }
        health-=damageToTake;

    }
    
}
