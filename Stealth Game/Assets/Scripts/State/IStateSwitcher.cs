
public interface IEnemyStateSwitcher
{
    public void SwitchState<T>() where T : EnemyBaseState;       
}

public interface ITargetStateSwitcher
{
    public void SwitchState<T>() where T : TargetBaseState;
}
