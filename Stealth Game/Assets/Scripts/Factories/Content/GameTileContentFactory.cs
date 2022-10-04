using UnityEngine;

[CreateAssetMenu]
public class GameTileContentFactory : GameObjectFactory
{
    [SerializeField] private GameTileContent _emptyPrefab;
    [SerializeField] private GameTileContent _destinationPrefab;    
    [SerializeField] private GameTileContent _obstaclePrefab;
    [SerializeField] private GameTileContent _targetSpawnPointPrefab;
    [SerializeField] private GameTileContent _enemySpawnPointPrefab;

    public void Reclaim(GameTileContent content)
    {
        Destroy(content.gameObject);
    }

    private GameTileContent Get(GameTileContent prefab)
    {
        GameTileContent instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;        
        return instance;
    }

    public GameTileContent Get(GameTileContentType type)
    {
        switch (type)
        {
            case GameTileContentType.Empty: return Get(_emptyPrefab);
            case GameTileContentType.Destination: return Get(_destinationPrefab);
            case GameTileContentType.Obstacle: return Get(_obstaclePrefab);
            case GameTileContentType.TargetSpawnPoint: return Get(_targetSpawnPointPrefab);
            case GameTileContentType.EnemySpawnPoint: return Get(_enemySpawnPointPrefab);
        }
        return null;
    }
}
