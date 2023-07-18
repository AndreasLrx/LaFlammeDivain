using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        onTakeDamage += OnTakeDamage;
    }

    // Declared as new because Enemy has a delegate type named "OnTakeDamage" which is in conflict with this private method
    private new void OnTakeDamage()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        Invoke("ResetColor", 0.1f);
    }

    void ResetColor()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
