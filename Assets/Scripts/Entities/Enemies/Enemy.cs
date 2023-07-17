using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : Entity
{
    public GameObject target;
    private NavMeshAgent agent;

    public delegate void OnTakeDamage();
    public AsyncEventsProcessor.AsyncEvent onDeath = null;
    public OnTakeDamage onTakeDamage = null;
    private AsyncEventsProcessor eventsProcessor;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        eventsProcessor = gameObject.AddComponent<AsyncEventsProcessor>();
    }

    void Update()
    {
        agent.speed = speed;
        agent.SetDestination(target.transform.position);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        onTakeDamage?.Invoke();
        // destroy the object when hp is 0
        if (health <= 0)
            StartCoroutine(Death());
    }

    private IEnumerator Death()
    {
        yield return StartCoroutine(eventsProcessor.StartAsyncEvents(onDeath));
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
            other.GetComponent<Player>().TakeDamage();
    }
}
