using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // Entity move speed
    public float speed = 10;
    // Range of the entity attack
    public float range = 5;
    // Speed of the attack. The higher it is, the faster the entity can attack. 
    public float attackSpeed = 1;
    // Moving speed of the shot sent (wisps, projectiles...)
    public float shotSpeed = 1;

    // Player/AI specific
    public float damage = 3;
    // Enemies specific
    public float health = 20;
}
