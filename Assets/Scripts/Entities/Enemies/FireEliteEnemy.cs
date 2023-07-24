using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEliteEnemy : EliteEnemy
{
    protected TrailRenderer trailRenderer;
    EdgeCollider2D edgeCollider2D;

    public float fireDuration = 2;

    protected void Update()
    {
        SetColliderPointFromTrail(trailRenderer, edgeCollider2D);
        Attack();
    }

    protected override void Awake()
    {
        base.Awake();

        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.time = fireDuration;
        edgeCollider2D = GetValidCollider();
    }

    EdgeCollider2D GetValidCollider()
    {
        EdgeCollider2D validCollider;
        validCollider = new GameObject("TrailCollider", typeof(EdgeCollider2D)).GetComponentInParent<EdgeCollider2D>();
        validCollider.GetComponent<EdgeCollider2D>().isTrigger = true;
        validCollider.transform.SetParent(transform);

        return validCollider;
    }

    void SetColliderPointFromTrail(TrailRenderer trail, EdgeCollider2D collider)
    {
        List<Vector2> points = new List<Vector2>();
        if (trailRenderer.positionCount == 0)
            points.Add(trail.transform.position);
        else
            for (int position = 0; position < trail.positionCount; position++)
                points.Add(trail.GetPosition(position));
        collider.points = points.ToArray();
    }

    private void OnDestroy()
    {
        if (edgeCollider2D != null)
        {
            edgeCollider2D.enabled = false;
        }
    }

    public void Attack()
    {
        List<Collider2D> colliders = new();
        edgeCollider2D.GetContacts(colliders);
        foreach (Collider2D other in colliders)
        {
            if (other.tag == "Player")
                other.GetComponent<Player>().TakeDamage();
        }
    }
}

