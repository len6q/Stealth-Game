using UnityEngine;

public class MoveState : TargetBaseState
{    
    private readonly float _waitingTime = 1f;
    private readonly int _step = 3;

    private float _currentTime = 0f;

    private const int STATE = 1;
    private const string STATE_NAME = "State";

    public MoveState(ITargetStateSwitcher stateSwitcher, KeyboardInput inputSystem, Animator animator, float speed, Rect allowedArea, Target target)
        : base(stateSwitcher, inputSystem, animator, speed, allowedArea, target)
    {
        
    } 

    public override void Move()
    {
        if (_inputSystem.IsMove(_speed, _allowedArea) == false) 
        {
            _stateSwitcher.SwitchState<IdleState>();
        }
        else
        {
            _currentTime += Time.deltaTime;

            if(_currentTime > _waitingTime)
            {
                Game.MakeNoise(_step);
                _currentTime -= _waitingTime;
            }
        }
    }

    public override void Start()
    {
        _animator.SetInteger(STATE_NAME, STATE);
        _target.OnDefeated += ChangeState;
    }

    private void ChangeState()
    {
        _stateSwitcher.SwitchState<DefeatState>();
    }

    public override void Stop()
    {
        _target.OnDefeated -= ChangeState;
    }
}
