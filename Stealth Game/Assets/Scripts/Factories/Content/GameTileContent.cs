using UnityEngine;

public class GameTileContent : MonoBehaviour
{
    [SerializeField] private GameTileContentType _type;
    
    public GameTileContentFactory OriginFactory { get; set; }
    
    public GameTileContentType Type => _type;

    public void Recycle()
    {
        OriginFactory.Reclaim(this);
    }
}

public enum GameTileContentType
{
    Empty,
    Destination,
    Obstacle,
    TargetSpawnPoint,
    EnemySpawnPoint
}
