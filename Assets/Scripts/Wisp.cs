using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Wisp : MonoBehaviour
{
    public GameObject playerObject;
    public Rigidbody2D rb2D;           //The Rigidbody2D component attached to this object.
    public Color color;
    public Color disabledColor;

    public float orbitSpeed = 100;
    public float orbitDistance = 2;

    // The cooldown time, in seconds
    public float cooldownTime = 1;
    // The time remaining before the wisp can be activated
    private float currentCooldown = 0;

    // Start is called before the first frame update
    void Start()
    {  
        ResetColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCooldown > float.Epsilon) {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown < 0) {
                currentCooldown = 0;
                ResetColor();
            }
        }
    }

    void ResetColor() {
        if (IsActivable())
            GetComponent<SpriteRenderer>().color = color;
        else
            GetComponent<SpriteRenderer>().color = disabledColor;
    }

    public bool IsActivable() {
        return currentCooldown < float.Epsilon;
    }

    public bool Activate() {
        if (IsActivable() && OnActivate()) {
            currentCooldown = cooldownTime;
            return true;
        }
        return false;
    }

    protected abstract bool OnActivate();
}
