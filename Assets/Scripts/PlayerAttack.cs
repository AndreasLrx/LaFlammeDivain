using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float meleeSpeed;
    [SerializeField] private float meleeDamage;

    private void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.tag == "Enemy")
        {   
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Attack(other);
            }
        }
    }
    private void Attack(Collider2D other)
    {
        other.GetComponent<Dummy>().getDamage(meleeDamage);
    }
}
