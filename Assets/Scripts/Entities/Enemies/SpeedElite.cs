public class SpeedElite : Elite
{

    protected override void Awake()
    {
        base.Awake();
        if (enemyClass.speed < 1)
            enemyClass.speed *= 0.125f;
        else
            enemyClass.speed *= 1.25f;
    }
}
