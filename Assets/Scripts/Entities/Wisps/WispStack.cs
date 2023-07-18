using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispStack : MovingObject
{
    private List<Wisp> wisps = new();
    private Type _wispsType;

    public Type wispsType { get { return _wispsType; } }

    public WispStack Setup(Wisp wisp)
    {
        transform.position = wisp.transform.position;
        wisps.Clear();
        _wispsType = wisp.GetType();
        gameObject.name = "WispStack (" + _wispsType + ")";
        return this;
    }

    public bool IsWispTypeCompatible(Wisp wisp)
    {
        return wisp.GetType() == wispsType;
    }

    public void AddWisp(Wisp wisp)
    {
        if (!IsWispTypeCompatible(wisp))
            throw new Exception("Invalid wisp type");
        wisps.Add(wisp);
        wisp.transform.SetParent(transform);
        wisp.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        // Update size
    }

    public Wisp GetFirstActivable()
    {
        foreach (Wisp wisp in wisps)
            if (wisp.IsActivable())
                return wisp;
        return null;
    }

    public bool ActivateSingle()
    {
        Wisp wisp = GetFirstActivable();
        if (wisp == null)
            return false;
        wisp.owner.StartCoroutine(wisp.Activate());
        return true;
    }

    public bool ActivateStack()
    {
        List<Wisp> activableWisps = new();

        foreach (Wisp wisp in wisps)
            if (wisp.IsActivable())
                activableWisps.Add(wisp);

        for (int i = 0; i < activableWisps.Count; i++)
            activableWisps[i].owner.StartCoroutine(activableWisps[i].Activate(i, activableWisps.Count));
        return activableWisps.Count > 0;
    }

    public bool IsActivable()
    {
        foreach (Wisp wisp in wisps)
            if (wisp.IsActivable())
                return true;
        return false;
    }

    public void DetachWisp(Wisp wisp)
    {
        wisps.Remove(wisp);
        wisp.transform.SetParent(null);
    }

    public Wisp PopWisp()
    {
        Wisp wisp = wisps[0];

        DetachWisp(wisp);
        return wisp;
    }

    public int WispsCount()
    {
        return wisps.Count;
    }

    public bool AbsorbDamage(bool absorbed)
    {
        foreach (Wisp wisp in wisps)
            absorbed = (wisp.onDamage?.Invoke(absorbed) == true) | absorbed;
        return absorbed;
    }

    protected override float GetSpeed()
    {
        if (wisps.Count == 0)
            return 1;
        return wisps[0].speed;
    }
}
