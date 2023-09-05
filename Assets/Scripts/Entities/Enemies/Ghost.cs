using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Enemy
{
    public LayerMask layerMask;
    public float moveTransparency = 0.5f;

    private SpriteRenderer spriteRenderer;
    private Color baseColor;
    private Color moveColor;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;
        moveColor = new Color(baseColor.r, baseColor.g, baseColor.b, moveTransparency);
        spriteRenderer.color = moveColor;
    }

    protected override void UpdateTarget()
    {
        if ((AICompanion.Instance.transform.position - transform.position).sqrMagnitude < (PlayerController.Instance.transform.position - transform.position).sqrMagnitude)
            target = AICompanion.Instance.gameObject;
        else
            target = PlayerController.Instance.gameObject;
    }

    protected override bool CanTakeDamage()
    {
        return customMove;
    }

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
        if (customMove)
        {
            spriteRenderer.color = moveColor;
            customMove = false;
            agent.isStopped = false;
        }
        return false;
    }

    private void Fire(Vector2 direction)
    {
        Instantiate(PrefabManager.Instance.ectoplasmProjectile, transform.position, Quaternion.identity).GetComponent<Projectile>().Launch(direction, shotSpeed, range);
        if (!customMove)
        {
            spriteRenderer.color = baseColor;
            customMove = true;
            agent.isStopped = true;
        }
    }
}
