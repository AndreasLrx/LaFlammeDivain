using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombWisp : Wisp
{
    public float explosionDamage = 10.0f;
    public float explosionRadius = 5.0f;
    private CircleCollider2D circleCollider;
    private float defaultRadius = 0.0f;

    protected override void Awake()
    {
        base.Awake();
        onActivate += OnActivate;
    }

    private void Start()
    {
        gameObject.GetComponent<CircleCollider2D>();
        circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.excludeLayers = LayerMask.GetMask("BlockingLayer");
        circleCollider.isTrigger = true;
    }

    private IEnumerator OnActivate()
    {
        SetTarget((Vector2)owner.transform.position + owner.aimedDirection * range);
        while (MoveTowardsTarget() && !Attack())
            yield return null;
    }

    public bool Attack()
    {
        List<Collider2D> colliders = new();
        gameObject.GetComponent<CircleCollider2D>().GetContacts(colliders);
        foreach (Collider2D other in colliders)
        {
            switch (other.tag)
            {
                case "Enemy":
                    circleCollider.radius = explosionRadius;
                    return true;
                case "Wall":
                    circleCollider.radius = explosionRadius;
                    return true;
                default:
                    break;
            }
        }
        return false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<Enemy>().TakeDamage(explosionDamage);
            circleCollider.radius = defaultRadius;
        }
        if (other.tag == "Player")
        {
            other.GetComponent<Player>().TakeDamage();
            circleCollider.radius = defaultRadius;
        }
        if (other.tag == "Wall")
            circleCollider.radius = defaultRadius;
    }
}
