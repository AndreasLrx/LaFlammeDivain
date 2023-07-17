using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : Entity
{
    public GameObject target;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        agent.speed = speed;
        agent.SetDestination(target.transform.position);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        OnTakeDamage();
        // destroy the object when hp is 0
        if (health <= 0)
            StartCoroutine(Death());
    }

    protected virtual void OnTakeDamage()
    {

    }

    private IEnumerator Death()
    {
        yield return StartCoroutine(OnDeath());
        Destroy(gameObject);
    }

    protected virtual IEnumerator OnDeath()
    {
        yield break;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
            other.GetComponent<Player>().TakeDamage();
    }
}
