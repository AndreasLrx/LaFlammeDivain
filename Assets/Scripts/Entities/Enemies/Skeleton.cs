using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy
{
    public LayerMask layerMask;
    public int healthPerBone = 1;

    protected override bool Attack()
    {
        // In range
        if (agent.remainingDistance < range)
        {

            // Not in line view
            Vector2 direction = (agent.destination - transform.position).normalized;
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, direction, range, layerMask);

            if (hit2D.collider != null && hit2D.collider.tag == "Player")
            {
                Fire(direction);
                return true;
            }
        }
        customMove = false;
        agent.isStopped = false;
        return false;
    }

    protected override void UpdateTarget()
    {
        if ((AICompanion.Instance.transform.position - transform.position).sqrMagnitude < (PlayerController.Instance.transform.position - transform.position).sqrMagnitude)
            target = AICompanion.Instance.gameObject;
        else
            target = PlayerController.Instance.gameObject;
    }

    private void Fire(Vector2 direction)
    {
        Instantiate(PrefabManager.Instance.boneProjectile, transform.position, Quaternion.identity).GetComponent<Projectile>().Launch(direction, shotSpeed, range);
        customMove = true;
        agent.isStopped = true;
        TakeDamage(healthPerBone);
    }
}
