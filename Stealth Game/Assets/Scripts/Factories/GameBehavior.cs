using UnityEngine;

public abstract class GameBehavior : MonoBehaviour
{
    public abstract void GameUpdate();

    public abstract void Recycle();
}
