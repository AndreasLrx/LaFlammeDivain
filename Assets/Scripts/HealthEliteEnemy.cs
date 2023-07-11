namespace LaFlammeDivain.Assets.Scripts
{
    public class HealthEliteEnemy : EliteEnemy
    {
        public float hpElite = 30.0f;

        protected override void Awake()
        {
            base.Awake();

            enemyClass.hp = hpElite;
        }
    }
}
