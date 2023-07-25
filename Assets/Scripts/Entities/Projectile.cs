using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MovingObject
{
    private float speed;

    public void Launch(Vector2 direction, float speed, float distance)
    {
        Vector2 target = (Vector2)transform.position + direction * distance;

        SetTarget(target);
        this.speed = speed;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // If the projectile isn't moving anymore it mean it reached its target and must be destroyed
        if (!IsMoving())
            Destroy(gameObject);
    }

    protected override float GetSpeed()
    {
        return speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
            other.GetComponent<Player>().TakeDamage();
        Destroy(gameObject);
    }
}
