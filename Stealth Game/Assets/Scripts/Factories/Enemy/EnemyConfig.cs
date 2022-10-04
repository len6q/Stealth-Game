using UnityEngine;
using System;

[Serializable]
public class EnemyConfig 
{
    public Enemy Prefab;

    [Range(4f, 8f)] public float PatrolDistance;
    [Range(2, 6)] public int PatrolPoints;
    [Range(3f, 10f)] public float ViewRadius;
    [Range(50f, 360f)] public float ViewAngle;
    [Range(1f, 3f)] public float Acceleration;
}

public enum EnemyType
{
    Small,
    Medium,
    Large
}
