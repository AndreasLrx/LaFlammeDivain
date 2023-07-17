using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    private struct Target
    {
        public Vector2 position;
        public bool local;

        public Target(Vector2 position, bool local = false)
        {
            this.position = position;
            this.local = local;
        }
    }

    private List<Target> targets;
    private Entity entity;

    protected virtual void Awake()
    {
        targets = new();
        entity = GetComponent<Entity>();
    }

    protected virtual void Update()
    {
        MoveTowardsTarget();
    }

    protected virtual float GetSpeed()
    {
        return entity.speed;
    }

    public bool IsMoving()
    {
        return targets.Count > 0;
    }

    /// Returns the current target
    // Warning: Will raise Out of range error if no target is left
    private Target GetTarget()
    {
        return targets[0];
    }

    // Delete all the targets and set a new one
    public void SetTarget(Vector2 target, bool local = false)
    {
        targets.Clear();
        targets.Add(new Target(target, local));
    }

    // Add a new target in the list
    public void AddTarget(Vector2 target, bool local = false)
    {
        targets.Add(new Target(target, local));
    }

    // Delete the current target if any
    public void PopCurrentTarget()
    {
        if (IsMoving())
            targets.RemoveAt(0);
    }

    // Move toward the current target if any.
    // Return true if the object position changed.
    public bool MoveTowardsTarget()
    {
        // Be sure to have a target
        if (!IsMoving())
            return false;

        Target target = GetTarget();
        float sqrRemainingDistance = ((Vector2)(target.local ? transform.localPosition : transform.position) - target.position).sqrMagnitude;

        // We reached this target
        if (sqrRemainingDistance < float.Epsilon)
        {
            // Pop this target and try to move to the next one (if any)
            PopCurrentTarget();
            return MoveTowardsTarget();
        }

        // Move towards the target
        if (target.local)
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, target.position, GetSpeed() * Time.deltaTime);
        else
            transform.position = Vector3.MoveTowards(transform.position, target.position, GetSpeed() * Time.deltaTime);
        return true;
    }

    public bool MoveTowardsTarget(Vector2 target, bool local = false)
    {
        SetTarget(target, local);
        return MoveTowardsTarget();
    }

    protected IEnumerator MoveToTarget()
    {
        while (MoveTowardsTarget())
            yield return null;
    }

    protected IEnumerator MoveToTarget(Vector2 target, bool local = false)
    {
        SetTarget(target, local);
        while (MoveTowardsTarget())
            yield return null;
    }
}
