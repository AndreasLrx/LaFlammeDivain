using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWisp : Wisp
{
    public float range = 5;

    protected override IEnumerator OnActivate()
    {
        return SmoothMovement((Vector2)playerObject.transform.position + Player().AimedDirection() * range);
    }

    protected override IEnumerator OnDetach()
    {
        yield break;
    }

    protected override IEnumerator OnAttach()
    {
        yield break;
    }
}
