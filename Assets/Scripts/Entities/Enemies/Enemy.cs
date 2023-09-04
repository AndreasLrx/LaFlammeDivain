using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : Entity
{
    public float targetUpdateCooldown = 2;
    public GameObject target = null;
    private NavMeshAgent _agent;
    private Animator animator;
    public NavMeshAgent agent { get { return _agent; } }
    public bool dealsContactDamage = true;
    protected bool customMove = false;

    public delegate void OnTakeDamage();
    public AsyncEventsProcessor.AsyncEvent onDeath = null;
    public OnTakeDamage onTakeDamage = null;
    private AsyncEventsProcessor eventsProcessor;
    private float currentTargetUpdateCooldown = 0f;
    private float attackCooldown = 0f;

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        eventsProcessor = gameObject.AddComponent<AsyncEventsProcessor>();
    }

    protected virtual void Update()
    {
        // Target update
        currentTargetUpdateCooldown -= Time.deltaTime;
        if (currentTargetUpdateCooldown <= float.Epsilon)
        {
            UpdateTarget();
            currentTargetUpdateCooldown = targetUpdateCooldown;
        }

        // Attack
        attackCooldown = Mathf.Max(attackCooldown - Time.deltaTime, 0);
        if (attackCooldown <= 0 && Attack())
            attackCooldown = 1 / attackSpeed;

        // Destination
        agent.speed = speed;
        if (!customMove && target != null)
            agent.SetDestination(target.transform.position);

        // Update animation
        if (animator != null)
        {
            Vector2 vel = agent.velocity.normalized;
            animator.SetFloat("DirectionX", vel.x);
            animator.SetFloat("DirectionY", vel.y);
        }
    }

    public virtual void InitTarget()
    {
        if (Random.Range(0, 2) == 0)
            target = PlayerController.Instance.gameObject;
        else
            target = AICompanion.Instance.gameObject;
    }

    protected virtual void UpdateTarget() { }
    protected virtual bool Attack() { return false; }

    protected virtual bool CanTakeDamage() { return true; }

    public void TakeDamage(float damage)
    {
        if (!CanTakeDamage())
            return;
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
        if (!dealsContactDamage)
            return;
        if (other.tag == "Player")
            other.GetComponent<Player>().TakeDamage();
    }
}
