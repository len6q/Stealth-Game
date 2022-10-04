using UnityEngine.AI;
using UnityEngine;
using System.Collections.Generic;
using System;

public class Game : MonoBehaviour
{
    [SerializeField] private NavMeshSurface _navMesh;
    [SerializeField, Range(6, 16)] private int _boardSize;
    [SerializeField] private GameBoard _board;
    [SerializeField] private GameTileContentFactory _contentFactory;
	[SerializeField] private GeneralEnemyFactory _generalEnemyFactory;
    [SerializeField] private TargetFactory _targetFactory;
    [SerializeField, Range(1, 10)] private int _enemyCount;    
    [SerializeField, Range(2f, 8f)] private float _gameSpeed;
    [SerializeField] private DefenderHud _defenderHud;
    [SerializeField, Range(8, 20)] private int _maxNoiseIndicator = 10;    

    private int _currentNoiseIndicator;
    private int CurrentNoiseIndicator
    {
        get => _currentNoiseIndicator;
        set
        {
            _currentNoiseIndicator = value;
            if (_currentNoiseIndicator >= 0)
            {                
                _defenderHud.SetNoiseIndicator(_currentNoiseIndicator);
            }
            else if(_currentNoiseIndicator < 0)
            {
                _currentNoiseIndicator = 0;
            }
            
            if(_currentNoiseIndicator >= _maxNoiseIndicator)
            {
                _currentNoiseIndicator = _maxNoiseIndicator;
                OnCatched?.Invoke(_target);
            }         
        }
    }
    private static Game _instance;

    public static event Action<Target> OnCatched;

    private Queue<EnemyType> _enemyTypes = new Queue<EnemyType>();
    private List<Enemy> _allEnemies = new List<Enemy>();
    private Target _target;
    
    private bool _isStartGame;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }        
    }

    private void Start()
    {
        _board.Init(_boardSize, _contentFactory, _enemyCount);
        SpawnTarget();
        SpawnEnemies();
        _defenderHud.Init(_maxNoiseIndicator);        
    }

    private void Update()
    {
        foreach (var enemy in _allEnemies)
        {
            enemy.GameUpdate();
        }
        _target.GameUpdate();
    }

    private void LateUpdate()
    {
        if(_isStartGame == false)
        {
            _navMesh.BuildNavMesh();
            _isStartGame = true;
        }
    }

    private void OnDestroy()
    {
        _target.OnDefeated -= DefeatedGame;
        _target.OnWinner -= WinnerGame;
    }

    private void SpawnEnemies()
    {
        _enemyTypes.Enqueue(EnemyType.Small);
        _enemyTypes.Enqueue(EnemyType.Medium);
        _enemyTypes.Enqueue(EnemyType.Large);

        for(int i = 0; i < _enemyCount; i++)
        {
            GameTile tile = _board.GetEnemySpawnPoint();
            Enemy enemy = _generalEnemyFactory.Get(GetEnemyType(), _gameSpeed, tile);
            _allEnemies.Add(enemy);
        }        
    }

    private void SpawnTarget()
    {
        GameTile tile = _board.GetTargetSpawnPoint();
        _target = _targetFactory.Get(_gameSpeed, _boardSize, tile);
        _target.OnDefeated += DefeatedGame;
        _target.OnWinner += WinnerGame;
    }

    private void DefeatedGame()
    {
        _defenderHud.ShowPanel(true);
        
        foreach(var enemy in _allEnemies)
        {
            enemy.Recycle();           
        }
        _allEnemies.Clear();
    }

    private void WinnerGame()
    {
        _defenderHud.ShowPanel(false);

        foreach (var enemy in _allEnemies)
        {
            enemy.Recycle();
        }
        _allEnemies.Clear();
    }

    private EnemyType GetEnemyType()
    {
        EnemyType type = _enemyTypes.Dequeue();
        _enemyTypes.Enqueue(type);
        return type;
    }

    public static void MakeNoise(int noiseIndicator)
    {
        _instance.CurrentNoiseIndicator += noiseIndicator;
    }
}
