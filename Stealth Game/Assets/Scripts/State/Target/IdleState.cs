using UnityEngine;

public class IdleState : TargetBaseState
{
    private readonly float _waitingTime = 0.5f;
    private readonly int _step = 1;

    private float _currentTime = 0f;

    private const int STATE = 0;
    private const string STATE_NAME = "State";

    public IdleState(ITargetStateSwitcher stateSwitcher, KeyboardInput inputSystem, Animator animator, float speed, Rect allowedArea, Target target)
        : base(stateSwitcher, inputSystem, animator, speed, allowedArea, target)
    {

    }

    public override void Move()
    {
        if(_inputSystem.IsMove(_speed, _allowedArea))
        {
            _stateSwitcher.SwitchState<MoveState>();
        }
        else
        {
            _currentTime += Time.deltaTime;

            if(_currentTime > _waitingTime)
            {
                Game.MakeNoise(-_step);
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
