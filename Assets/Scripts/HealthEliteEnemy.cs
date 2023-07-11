namespace LaFlammeDivain.Assets.Scripts
{
    public class HealthEliteEnemy : EliteEnemy
    {
        public float hpElite = 30.0f;
        private Enemy enemyClass;
        void Awake()
        {
            enemyClass = gameObject.GetComponent<Enemy>();
            enemyClass.hp = hpElite;
        }
    }
}
