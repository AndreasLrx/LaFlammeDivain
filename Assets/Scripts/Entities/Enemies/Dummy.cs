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

    private void OnTakeDamage()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        Invoke("ResetColor", 0.1f);
    }

    void ResetColor()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
