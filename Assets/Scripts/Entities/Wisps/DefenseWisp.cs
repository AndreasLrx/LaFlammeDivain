using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseWisp : Wisp
{
    private bool loaded;

    protected override void Awake()
    {
        base.Awake();
        loaded = true;
        onActivate += OnActivate;
        _onDamage += OnDamage;
    }

    private IEnumerator OnActivate()
    {
        loaded = true;
        yield break;
    }

    private new bool OnDamage(bool absorbed)
    {
        // Damage has not been absorbed and the wisp is loaded
        if (!absorbed && loaded)
        {
            loaded = false;
            return true;
        }
        return false;
    }
}
