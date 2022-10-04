
using UnityEngine;

public abstract class TargetBaseState
{
    protected readonly ITargetStateSwitcher _stateSwitcher;
    protected readonly KeyboardInput _inputSystem;
    protected readonly Animator _animator;
    protected readonly float _speed;
    protected readonly Rect _allowedArea;                 
    protected readonly Target _target;                 

    protected TargetBaseState(ITargetStateSwitcher stateSwitcher, KeyboardInput inputSystem, Animator animator, float speed, Rect allowedArea, Target target)
    {
        _stateSwitcher = stateSwitcher;
        _inputSystem = inputSystem;
        _animator = animator;
        _speed = speed;
        _allowedArea = allowedArea;
        _target = target;
    }

    public abstract void Move();

    public abstract void Start();

    public abstract void Stop();
}
