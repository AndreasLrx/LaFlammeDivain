using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWisp : Wisp
{
    protected override bool OnActivate()
    {
        GetComponent<SpriteRenderer>().color = Color.blue;
        Invoke("ResetColor", 0.1f);
        return true;
    }
}
