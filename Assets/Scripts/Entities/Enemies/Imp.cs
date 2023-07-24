using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imp : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        onTakeDamage += OnTakeDamage;
    }

    protected override void UpdateTarget()
    {
        if ((AICompanion.instance.transform.position - transform.position).sqrMagnitude < (PlayerController.Instance.transform.position - transform.position).sqrMagnitude)
            target = AICompanion.instance.gameObject;
        else
            target = PlayerController.Instance.gameObject;
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
