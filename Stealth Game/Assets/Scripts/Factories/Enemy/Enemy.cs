using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Enemy : GameBehavior, IEnemyStateSwitcher
{
    [SerializeField] private FieldOfView _fieldOfView;
    [SerializeField] private NavMeshAgent _enemy;
    [SerializeField] private Animator _animator;        

    private float _distance;
    private int _patrolPoints;
    private float _acceleration;

    private Target _target;    

    private Vector3[] _points;

    private EnemyBaseState _currentState;
    private List<EnemyBaseState> _allStates;

    public EnemyFactory OriginFactory { get; set; }

    private void OnDestroy()
    {        
        Game.OnCatched -= FindTarget;
    }

    private void FindTarget(Target target)
    {
        _target = target;
    }

    public void Init(float distance, int patrolPoints, float viewRadius, float viewAngle, float acceleration, float speed, GameTile tile)
    {
        Game.OnCatched += FindTarget;

        _distance = distance;
        _patrolPoints = patrolPoints;
        _acceleration = acceleration;
        _enemy.speed = speed;
        
        transform.localPosition = tile.transform.localPosition;

        _fieldOfView.Init(viewRadius, viewAngle);

        SetPatrolPoints();

        _allStates = new List<EnemyBaseState>()
        {
            new PatrolState(this, _enemy, _fieldOfView, _animator, _points),
            new AttackState(this, _enemy, _fieldOfView, _animator, _acceleration)
        };

        _currentState = _allStates[0];
        _currentState.Start();
    }

    private void SetPatrolPoints()
    {        
        _points = new Vector3[_patrolPoints];

        for(int i = 0; i < _patrolPoints; i++)
        {           
            Vector3 point = Random.onUnitSphere * _distance;
            _points[i] = i % 2 == 0 ? point : -point;            
        }                
    }

    public void SwitchState<T>() where T : EnemyBaseState
    {
        var state = _allStates.FirstOrDefault(s => s is T);
        _currentState.Stop();
        state.Start();
        _currentState = state;        
    }

    public override void GameUpdate()
    {
        if (_fieldOfView.IsTargetTracked(ref _target) == false)
        {
            _target = _fieldOfView.AcquireTarget();
        }

        _currentState.Move(_target);
    }

    public override void Recycle()
    {
        OriginFactory.Reclaim(this);
    }
}
