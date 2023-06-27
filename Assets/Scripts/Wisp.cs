using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Wisp : MonoBehaviour
{
    public GameObject playerObject;
    public Rigidbody2D rb2D;           //The Rigidbody2D component attached to this object.
    public Color color;
    public Color disabledColor;
    public float moveSpeed = 20;


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
    void Update()
    {
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

    void ResetColor()
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
            Vector2 returnPosition = playerObject.transform.position;
            returnPosition -= ((Vector2)(playerObject.transform.position - transform.position)).normalized * owningWispsGroup.GetComponent<WispsGroup>().orbitDistance;
            yield return StartCoroutine(SmoothMovement(returnPosition));
            // Attach back the wisp to the group
            yield return StartCoroutine(Attach());
            // Reset the color
            ResetColor();
        }
    }

    protected abstract IEnumerator OnActivate();

    protected abstract IEnumerator OnDetach();

    protected abstract IEnumerator OnAttach();

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
        //Square magnitude is used instead of magnitude because it's computationally cheaper.
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > float.Epsilon)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(transform.position, end, moveSpeed * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            transform.position = newPostion;

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }
    }
}
