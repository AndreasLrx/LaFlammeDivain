public class HealthElite : Elite
{
    protected override void Awake()
    {
        base.Awake();
        enemyClass.health *= 2;
    }
}

