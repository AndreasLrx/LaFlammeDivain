using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWisp : Wisp
{
    public float damageBoost = 0.3f;

    [SerializeField] private AudioSource WispAttackSound;

    protected override void Awake()
    {
        base.Awake();
        onActivate += OnActivate;
        onDeath += OnDetach;
        onAttach += OnAttach;
    }

    private IEnumerator OnActivate()
    {
        SetTarget((Vector2)owner.transform.position + owner.aimedDirection * range);
        while (MoveTowardsTarget() && !Attack())
            yield return null;
    }

    private IEnumerator OnDetach()
    {
        trailRenderer.time = detachedTrailDuration;
        owner.entity.damage -= damageBoost;
        yield break;
    }

    private IEnumerator OnAttach()
    {
        owner.entity.damage += damageBoost;
        StartCoroutine(SmoothlyChangeTrailDuration(attachedTrailDuration));
        yield break;
    }

    public bool Attack()
    {
        List<Collider2D> colliders = new();
        WispAttackSound.Play();
        gameObject.GetComponent<CircleCollider2D>().GetContacts(colliders);
        foreach (Collider2D other in colliders)
            switch (other.tag)
            {
                case "Enemy":
                    other.GetComponent<Enemy>().TakeDamage(damage);
                    return true;
                case "Wall":
                    return true;
                default:
                    break;
            }
        return false;
    }
}
