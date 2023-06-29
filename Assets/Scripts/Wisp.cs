using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public abstract class Wisp : MovingObject
{
    public GameObject playerObject;

    private Color _color;
    public Color color
    {
        get { return _color; }
        set
        {
            _color = value;
            gameObject.GetComponent<Light2D>().color = _color;
        }
    }
    public Color disabledColor;


    // The cooldown time, in seconds
    public float cooldownTime = 1;
    // The time remaining before the wisp can be activated
    private float currentCooldown = 0;

    protected GameObject owningWispsGroup = null;

    // Start is called before the first frame update
    void Start()
    {
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

    protected Player Player()
    {
        return playerObject.GetComponent<Player>();
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
        owningWispsGroup = gameObject.transform.parent.gameObject;
        owningWispsGroup.GetComponent<WispsGroup>().DetachWisp(gameObject);
        yield return StartCoroutine(OnDetach());
    }

    private IEnumerator Attach()
    {
        owningWispsGroup.GetComponent<WispsGroup>().AddWisp(gameObject);
        owningWispsGroup = null;
        yield return StartCoroutine(OnAttach());
    }

    public IEnumerator Activate()
    {
        if (IsActivable())
        {
            // Activate the cooldown
            currentCooldown = cooldownTime;
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

    protected abstract IEnumerator OnAttach();

    protected IEnumerator ReturnToPlayer()
    {
        while (MoveTowardsTarget((Vector2)playerObject.transform.position - ((Vector2)(playerObject.transform.position - transform.position)).normalized * owningWispsGroup.GetComponent<WispsGroup>().orbitDistance))
            yield return null;
    }
}
