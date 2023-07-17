using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEliteEnemy : EliteEnemy
{
    protected TrailRenderer trailRenderer;
    EdgeCollider2D edgeCollider2D;
    static List<EdgeCollider2D> unusedColliders = new List<EdgeCollider2D>();

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
        if (unusedColliders.Count > 0)
        {
            validCollider = unusedColliders[0];
            validCollider.enabled = true;
            unusedColliders.RemoveAt(0);
        }
        else
        {
            validCollider = new GameObject("TrailCollider", typeof(EdgeCollider2D)).GetComponentInParent<EdgeCollider2D>();
            validCollider.GetComponent<EdgeCollider2D>().isTrigger = true;
        }
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
            unusedColliders.Add(edgeCollider2D);
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

