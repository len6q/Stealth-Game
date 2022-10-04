
public abstract class EnemyFactory : GameObjectFactory
{
    public Enemy Get(EnemyType type, float speed, GameTile tile)
    {
        EnemyConfig config = GetConfig(type);
        Enemy instance = CreateGameObjectInstance(config.Prefab);
        instance.OriginFactory = this;
        instance.Init(
            config.PatrolDistance,
            config.PatrolPoints,
            config.ViewRadius,
            config.ViewAngle,
            config.Acceleration,
            speed,
            tile
            );
        
        return instance;
    }

    public void Reclaim(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    protected abstract EnemyConfig GetConfig(EnemyType type);
}
