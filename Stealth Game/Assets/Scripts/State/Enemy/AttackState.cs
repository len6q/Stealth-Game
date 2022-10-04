using UnityEngine.AI;
using UnityEngine;

public class AttackState : EnemyBaseState
{
    private readonly float _acceleration;    

    private const int STATE = 1;
    private const string STATE_NAME = "State";    

    public AttackState(IEnemyStateSwitcher stateSwitcher, NavMeshAgent enemy, FieldOfView fieldOfView, Animator animator, float acceleration)
        : base(stateSwitcher, enemy, fieldOfView, animator)
    {
        _acceleration = acceleration;
    }

    public override void Move(Target target)
    {        
        if (target != null)
        {            
            _enemy.SetDestination(target.transform.position);
        }
        else
        {
            _stateSwitcher.SwitchState<PatrolState>();
        }
    }

    public override void Start()
    {
        _fieldOfView.SetDetectionColor();
        _enemy.speed *= _acceleration;

        _animator.SetInteger(STATE_NAME, STATE);
        _animator.speed = 1f * _acceleration;
    }

    public override void Stop()
    {
        _fieldOfView.SetBaseColor();
        _enemy.speed /= _acceleration;

        _animator.speed = 1f / _acceleration;
    }
}
