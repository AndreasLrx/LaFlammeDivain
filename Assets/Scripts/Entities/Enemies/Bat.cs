using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bat : Enemy
{
    public float newDirectionCooldown = 0.5f;
    public float cooldownRange = 0.2f;
    public float stepLength = 3f;
    private float currentDirCooldown = 0;

    protected override void Awake()
    {
        base.Awake();
        customMove = true;
    }

    protected override void Update()
    {
        base.Update();

        currentDirCooldown -= Time.deltaTime;
        if (currentDirCooldown <= float.Epsilon || agent.remainingDistance < float.Epsilon)
        {
            UpdateDirection();
            currentDirCooldown = newDirectionCooldown + Random.Range(-cooldownRange, cooldownRange);
        }
    }

    private void UpdateDirection()
    {
        Vector2 targetDirection = (target.transform.position - transform.position).normalized;
        Vector2 newDirection = Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)) * targetDirection * stepLength + transform.position;
        NavMesh.SamplePosition(newDirection, out var navHit, stepLength, -1);

        agent.SetDestination(navHit.position);
    }
}
