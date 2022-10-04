using UnityEngine;

[CreateAssetMenu]
public class TargetFactory : GameObjectFactory
{
    [SerializeField] private Target _prefab;

    public Target Get(float speed, int boardSize, GameTile tile)
    {
        Target instance = CreateGameObjectInstance(_prefab);
        instance.OriginFactory = this;
        instance.Init(speed, boardSize, tile);        
        return instance;
    }

    public void Reclaim(Target target)
    {
        Destroy(target.gameObject);
    }
}
