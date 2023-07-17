using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public abstract class Wisp : MovingObject
{
    public Entity owner;
    protected TrailRenderer trailRenderer;

    public float detachedTrailDuration;
    public float attachedTrailDuration;
    public float trailChangeDuration = 0.5f;

    [SerializeField] private Color _color;
    public Color color
    {
        get { return _color; }
        set
        {
            _color = value;
            gameObject.GetComponent<Light2D>().color = _color;
            trailRenderer.material.color = _color;
        }
    }
    public Color disabledColor;


    public float rangeMultiplier = 1;
    public float range { get { return owner.range * rangeMultiplier; } }
    public float speedMultiplier = 3;
    public float speed { get { return owner.shotSpeed * speedMultiplier; } }
    public float damageMultiplier = 1.0f;
    public float damage { get { return owner.damage * damageMultiplier; } }


    // The cooldown time, in seconds
    public float cooldownTime = 1;
    // The time remaining before the wisp can be activated
    private float currentCooldown = 0;

    public WispsGroup owningWispsGroup = null;

    // Start is called before the first frame update
    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        // Triggers light/trail color update
        color = color;
        ResetColor();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (currentCooldown > float.Epsilon)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown < 0)
            {
                currentCooldown = 0;
                ResetColor();
            }
        }
    }

    protected override float GetSpeed()
    {
        return speed;
    }

    bool IsDetached()
    {
        return owningWispsGroup != null;
    }

    protected void ResetColor()
    {
        // Wisp is in detached mode
        if (IsDetached())
            return;
        if (IsActivable())
            GetComponent<SpriteRenderer>().color = color;
        else
            GetComponent<SpriteRenderer>().color = disabledColor;
    }

    public bool IsActivable()
    {
        return currentCooldown < float.Epsilon && !IsDetached();
    }

    private IEnumerator Detach()
    {
        owningWispsGroup = gameObject.transform.GetComponentInParent<Player>().GetWisps();
        owningWispsGroup.DetachWisp(this);
        yield return StartCoroutine(OnDetach());
    }

    private IEnumerator Attach()
    {
        owningWispsGroup.AddWisp(this);
        owningWispsGroup = null;
        yield return StartCoroutine(OnAttach());
    }

    public IEnumerator Activate()
    {
        if (IsActivable())
        {
            // Activate the cooldown
            currentCooldown = cooldownTime / owner.attackSpeed;
            // Detach the wisp from the group
            yield return StartCoroutine(Detach());
            // Call the effective activation effect
            yield return StartCoroutine(OnActivate());
            // Return the wisp to the player position
            yield return StartCoroutine(ReturnToPlayer());
            // Attach back the wisp to the group
            yield return StartCoroutine(Attach());
            // Reset the color
            ResetColor();
        }
    }

    protected abstract IEnumerator OnActivate();

    protected abstract IEnumerator OnDetach();

    public abstract IEnumerator OnAttach();

    protected IEnumerator ReturnToPlayer()
    {
        while (MoveTowardsTarget((Vector2)owner.transform.position - ((Vector2)(owner.transform.position - transform.position)).normalized * owningWispsGroup.GetComponent<WispsGroup>().orbitDistance))
            yield return null;
    }

    private IEnumerator SmoothlyChangeTrailParameter(float newValue, Func<float> getter, Func<float, float> setter, bool waitCurrentDuration = false)
    {
        if (trailRenderer == null)
            yield break;
        if (waitCurrentDuration)
            yield return new WaitForSeconds(trailRenderer.time);
        float delta = (getter() - newValue) / trailChangeDuration;
        while ((delta > 0 && getter() > newValue) || (delta < 0 && getter() < newValue))
        {
            setter(getter() - delta * Time.deltaTime);
            yield return null;
        }
        setter(newValue);
    }

    protected IEnumerator SmoothlyChangeTrailDuration(float newDuration, bool waitCurrentDuration = false)
    {
        return SmoothlyChangeTrailParameter(newDuration, () => trailRenderer.time, (float newValue) => trailRenderer.time = newValue, waitCurrentDuration);
    }

    protected IEnumerator SmoothlyChangeTrailWidth(float newWidth, bool waitCurrentDuration = false)
    {
        return SmoothlyChangeTrailParameter(newWidth, () => trailRenderer.startWidth, (float newValue) => trailRenderer.startWidth = newValue, waitCurrentDuration);
    }

}
