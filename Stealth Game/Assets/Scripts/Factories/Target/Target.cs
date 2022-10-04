using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Target : GameBehavior, ITargetStateSwitcher
{    
    [SerializeField] private KeyboardInput _inputSystem;
    [SerializeField] private Animator _animator;

    public TargetFactory OriginFactory { get; set; }

    private float _speed;
    private Rect _allowedArea;

    private TargetBaseState _currentState;
    private List<TargetBaseState> _allStates;

    private bool _isEnabled;
    private float _currentTime;

    public event Action OnDefeated;
    public event Action OnWinner;    

    private void OnTriggerEnter(Collider other)
    {        
        if(other.TryGetComponent(out Enemy enemy) && _isEnabled)
        {
            OnDefeated?.Invoke();            
        }
        else if(other.TryGetComponent(out Destination destination) && _isEnabled)
        {
            OnWinner?.Invoke();
        }
    }

    public void Init(float speed, int boardSize, GameTile tile)
    {        
        _speed = speed;

        float xCoordinate = boardSize % 2 == 0 ? -boardSize / 2 + 0.5f : -boardSize / 2; 
        _allowedArea = new Rect(xCoordinate, xCoordinate, boardSize - 1f, boardSize - 1f);                       

        transform.localPosition = tile.transform.localPosition;

        _allStates = new List<TargetBaseState>()
        {
            new IdleState(this, _inputSystem, _animator, _speed, _allowedArea, this),
            new MoveState(this, _inputSystem, _animator, _speed, _allowedArea, this),            
            new DefeatState(this, _inputSystem, _animator, _speed, _allowedArea, this),            
        };
        _currentState = _allStates[0];
        _currentState.Start();        
    }

    public void SwitchState<T>() where T : TargetBaseState
    {
        var state = _allStates.FirstOrDefault(s => s is T);
        _currentState.Stop();
        state.Start();
        _currentState = state;
    }

    public override void GameUpdate()
    {
        if(_isEnabled == false)
        {
            _currentTime += Time.deltaTime;

            if(_currentTime > 1f)
            {
                _isEnabled = true;
            }
        }
        
        _currentState.Move();               
    }

    public override void Recycle()
    {
        OriginFactory.Reclaim(this);
    }
}
