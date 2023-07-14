using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    public float hp = 10;
    public GameObject target;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        agent.SetDestination(target.transform.position);
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        OnTakeDamage();
        // destroy the object when hp is 0
        if (hp <= 0)
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
