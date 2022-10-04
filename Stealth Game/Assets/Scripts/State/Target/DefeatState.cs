using UnityEngine;

public class DefeatState : TargetBaseState
{
    private const int STATE = 2;
    private const string STATE_NAME = "State";

    public DefeatState(ITargetStateSwitcher stateSwitcher, KeyboardInput inputSystem, Animator animator, float speed, Rect allowedArea, Target target)
        : base(stateSwitcher, inputSystem, animator, speed, allowedArea, target)
    {

    }

    public override void Move()
    {
        return;
    }

    public override void Start()
    {
        _animator.SetInteger(STATE_NAME, STATE);
    }

    public override void Stop()
    {
        
    }
}
