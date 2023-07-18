using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonWisp : Wisp
{
    EdgeCollider2D edgeCollider2D;
    static List<EdgeCollider2D> unusedColliders = new List<EdgeCollider2D>();

    public float detachedTrailWidth;
    public float attachedTrailWidth;
    public float poisonCoolDown = 0.5f;

    private float poisonCurrentCoolDown = 0.0f;

    protected override void Awake()
    {
        base.Awake();
        edgeCollider2D = GetValidCollider();
        onActivate += OnActivate;
        onDetach += OnDetach;
        onAttach += OnAttach;
    }

    protected override void Update()
    {
        SetColliderPointFromTrail(trailRenderer, edgeCollider2D);
        if (poisonCurrentCoolDown >= float.Epsilon)
            poisonCurrentCoolDown -= Time.deltaTime;
        Attack();

        base.Update();
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

    private IEnumerator OnActivate()
    {
        SetTarget((Vector2)owner.transform.position + owner.aimedDirection * range);
        while (MoveTowardsTarget() && !Attack())
            yield return null;
    }

    private IEnumerator OnDetach()
    {
        StartCoroutine(SmoothlyChangeTrailDuration(detachedTrailDuration));
        StartCoroutine(SmoothlyChangeTrailWidth(detachedTrailWidth));
        yield break;
    }

    private IEnumerator OnAttach()
    {
        // Wait for the trail to disapear to validate the attach 
        // (quickfix for trail disappearing when wisp was attached back to the player)
        StartCoroutine(SmoothlyChangeTrailDuration(attachedTrailDuration, true));
        StartCoroutine(SmoothlyChangeTrailWidth(attachedTrailWidth, true));
        yield break;
    }

    private void OnDestroy()
    {
        if (edgeCollider2D != null)
        {
            edgeCollider2D.enabled = false;
            unusedColliders.Add(edgeCollider2D);
        }
    }

    public bool Attack()
    {
        List<Collider2D> colliders = new();
        edgeCollider2D.GetContacts(colliders);
        foreach (Collider2D other in colliders)
        {
            switch (other.tag)
            {
                case "Enemy":
                    if (poisonCurrentCoolDown > float.Epsilon)
                        break;
                    other.GetComponent<Enemy>().TakeDamage(owner.entity.damage * trailRenderer.startWidth);
                    poisonCurrentCoolDown = poisonCoolDown;
                    return false;
                case "Wall":
                    return true;
                default:
                    break;
            }
        }
        return false;
    }
}
