using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Vector2 PointerDirection { get; set; }

    private void Update()
    {
        Vector2 direction = PointerDirection;
        transform.right = direction;
        Vector2 scale = transform.localScale;
        if (direction.x < 0)
            scale.y = -1;
        else
            scale.y = 1;

        transform.localScale = scale;
    }

    public void Attack()
    {
        List<Collider2D> colliders = new();
        gameObject.GetComponentInChildren<BoxCollider2D>().GetContacts(colliders);

        foreach (Collider2D other in colliders)
            if (other.tag == "Enemy")
                other.GetComponent<Enemy>().TakeDamage(PlayerController.Instance.entity.damage);
    }
}
