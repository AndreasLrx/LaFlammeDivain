using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{   
    
    public float hp = 10;
    
    public void getDamage(float damage)
    {
        hp -= damage;
        // change color to red when hit and return to pink after 0.1 seconds
        GetComponent<SpriteRenderer>().color = Color.red;
        Invoke("ResetColor", 0.1f);
        // destroy the object when hp is 0
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    void ResetColor()
    {
      GetComponent<SpriteRenderer>().color = Color.white;
    }
}
