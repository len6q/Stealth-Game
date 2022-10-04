using UnityEngine;
using UnityEngine.AI;

public class PatrolState : EnemyBaseState
{    
    private readonly Vector3[] _points;
   
    private float _distanceToChangePoint = 0.1f;
    private int _currentPoint = 0;

    private const int STATE = 1;
    private const string STATE_NAME = "State";    

    public PatrolState(IEnemyStateSwitcher stateSwitcher, NavMeshAgent enemy,FieldOfView fieldOfView, Animator animator, Vector3[] points)
        : base(stateSwitcher, enemy, fieldOfView, animator)
    {
        _points = points;              
    }

    public override void Move(Target target)
    {        
        if(target != null)
        {            
            _stateSwitcher.SwitchState<AttackState>();
        }
        else if (_enemy.remainingDistance < _distanceToChangePoint)
        {
            if (_currentPoint == _points.Length)
            {
                _currentPoint = 0;
            }
            _enemy.SetDestination(_points[_currentPoint]);

            _currentPoint++;
        }
    }

    public override void Start()
    {
        _fieldOfView.SetBaseColor();
        _animator.SetInteger(STATE_NAME, STATE);        
    }

    public override void Stop()
    {
        _fieldOfView.SetDetectionColor();        
    }
}
