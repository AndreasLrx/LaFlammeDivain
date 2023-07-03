using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWisp : Wisp
{
    public float range = 5;
    public float wispDamage = 1.0f;

    void Start()
    {
        color = new Color(
            Random.Range(0.4f, 1f),
            Random.Range(0.4f, 1f),
            Random.Range(0.4f, 1f)
        );
        ResetColor();
    }

    protected override IEnumerator OnActivate()
    {
        SetTarget((Vector2)playerObject.transform.position + Player().AimedDirection() * range);
        while (MoveTowardsTarget() && !Attack(wispDamage))
            yield return null;
    }

    protected override IEnumerator OnDetach()
    {
        yield break;
    }

    protected override IEnumerator OnAttach()
    {
        yield break;
    }

    public bool Attack(float wispDamage)
    {
        List<Collider2D> colliders = new();
        gameObject.GetComponent<CircleCollider2D>().GetContacts(colliders);
        foreach (Collider2D other in colliders)
            switch (other.tag)
            {
                case "Enemy":
                    other.GetComponent<Dummy>().getDamage(wispDamage);
                    return true;
                case "Wall":
                    return true;
                default:
                    break;
            }
        return false;
    }
}
