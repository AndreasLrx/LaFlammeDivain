using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseWisp : Wisp
{
    protected override void Awake()
    {
        base.Awake();
        onActivate += OnActivate;
    }

    private void Start()
    {
        owner.hitNumber += 1;
    }

    private IEnumerator OnActivate()
    {
        if (owner.hitNumber == 0)
            owner.hitNumber += 1;
        yield break;
    }
}
