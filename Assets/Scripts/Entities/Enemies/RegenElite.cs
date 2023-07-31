using UnityEngine;

public class RegenElite : Elite
{
    private float maxHealth;
    private float regenCooldown = 4;
    private float currentRegenCooldown = 0;

    protected override void Awake()
    {
        base.Awake();
        maxHealth = enemyClass.health;
    }

    private void Update()
    {
        if (enemyClass.health < maxHealth)
        {
            currentRegenCooldown -= Time.deltaTime;
            if (currentRegenCooldown <= float.Epsilon)
            {
                enemyClass.health += 1;
                currentRegenCooldown = regenCooldown;
                if (enemyClass.health > maxHealth)
                    enemyClass.health = maxHealth;
            }
        }

    }
}

