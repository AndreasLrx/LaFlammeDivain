using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombWisp : Wisp
{
    public LayerMask explosionMask;
    public float explosionDamage = 10.0f;
    public float explosionRadius = 5.0f;
    private CircleCollider2D explosionCollider;
    private bool hasExploded;
    [SerializeField] private AudioSource WispAttackSound;


    protected override void Awake()
    {
        base.Awake();
        onActivate += OnActivate;
        explosionCollider = new GameObject("ExplosionCollider", typeof(CircleCollider2D)).GetComponent<CircleCollider2D>();
        explosionCollider.transform.SetParent(transform);
        explosionCollider.transform.localPosition = Vector3.zero;
        explosionCollider.isTrigger = true;
        explosionCollider.radius = explosionRadius;
        explosionCollider.contactCaptureLayers = explosionMask;
    }


    private IEnumerator OnActivate()
    {
        hasExploded = false;
        SetTarget((Vector2)owner.transform.position + owner.aimedDirection * range);
        while (MoveTowardsTarget() && !hasExploded)
            yield return null;
        if (!hasExploded)
            Explode();
        hasExploded = true;
        yield break;
    }

    public void Explode()
    {
        List<Collider2D> colliders = new();
        WispAttackSound.Play();
        hasExploded = true;

        explosionCollider.GetContacts(colliders);

        foreach (Collider2D other in colliders)
        {
            switch (other.tag)
            {
                case "Enemy":
                    other.GetComponent<Enemy>().TakeDamage(explosionDamage);
                    break;
                case "Player":
                    other.GetComponent<Player>().TakeDamage();
                    break;
                default:
                    break;
            }
        }
    }
}
