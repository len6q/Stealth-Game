using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBaseState 
{
    protected readonly IEnemyStateSwitcher _stateSwitcher;
    protected readonly NavMeshAgent _enemy;    
    protected readonly FieldOfView _fieldOfView;
    protected readonly Animator _animator;

    protected EnemyBaseState(IEnemyStateSwitcher stateSwitcher, NavMeshAgent enemy, FieldOfView fieldOfView, Animator animator)
    {
        _stateSwitcher = stateSwitcher;
        _enemy = enemy;
        _fieldOfView = fieldOfView;
        _animator = animator;
    }

    public abstract void Move(Target target);

    public abstract void Start();

    public abstract void Stop();
}
