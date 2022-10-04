using UnityEngine;

[CreateAssetMenu]
public class GeneralEnemyFactory : EnemyFactory
{
    [SerializeField] private EnemyConfig _smallEnemy;
    [SerializeField] private EnemyConfig _mediumEnemy;
    [SerializeField] private EnemyConfig _largeEnemy;

    protected override EnemyConfig GetConfig(EnemyType type)
    {
        switch (type) 
        {
            case EnemyType.Small: return _smallEnemy;
            case EnemyType.Medium: return _mediumEnemy;
            case EnemyType.Large: return _largeEnemy;
        }

        return null;
    }
}
