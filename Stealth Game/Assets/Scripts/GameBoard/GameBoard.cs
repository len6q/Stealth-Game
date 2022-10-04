using UnityEngine;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private Transform _ground;
    [SerializeField] private GameTile _tilePrefab;

    private int _size;
    private GameTile[] _tiles;
    private Stack<GameTile> _enemySpawnPoints;
    private GameTile _targetSpawnPoint;

    private Queue<GameTile> _searchFrontier = new Queue<GameTile>();

    private GameTileContentFactory _contentFactory;

    private int _enemyCount;

    public void Init(int size, GameTileContentFactory contentFactory, int enemyCount)
    {
        _size = size;        
        _contentFactory = contentFactory;
        _ground.localScale = new Vector3(size, size, 0f);
        _enemyCount = enemyCount;

        Vector2 offset = new Vector2((_size - 1) * 0.5f, (_size - 1) * 0.5f);

        _tiles = new GameTile[_size * _size];
        _enemySpawnPoints = new Stack<GameTile>(_enemyCount);   
        
        for(int i = 0, y = 0; y < _size; y++)
        {
            for(int x = 0; x < _size; x++, i++)
            {
                GameTile tile = _tiles[i] = Instantiate(_tilePrefab, transform);
                tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);

                if(x > 0)
                {
                    GameTile.MakeEastWestNeighbors(tile, _tiles[i - 1]);
                }
                if(y > 0)
                {
                    GameTile.MakeNorthSouthNeighbors(tile, _tiles[i - _size]);
                }

                tile.Content = _contentFactory.Get(GameTileContentType.Empty);
            }
        }

        GenerateLevel();
    }

    public GameTile GetEnemySpawnPoint()
    {
        return _enemySpawnPoints.Pop();
    }

    public GameTile GetTargetSpawnPoint()
    {
        return _targetSpawnPoint;
    }

    private void Build(GameTile tile, GameTileContentType type)
    {
        if(tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = _contentFactory.Get(type);
            if(FindPath() == false)
            {
                tile.Content = _contentFactory.Get(GameTileContentType.Empty);
                FindPath();
            }

            if(type == GameTileContentType.TargetSpawnPoint)
            {
                _targetSpawnPoint = tile;
            }
            
            if(type == GameTileContentType.EnemySpawnPoint)
            {
                _enemySpawnPoints.Push(tile);
            }
        }        
    }

    private bool FindPath()
    {
        foreach(var tile in _tiles)
        {
            if (tile.Content.Type == GameTileContentType.Destination)
            {
                tile.BecomeDestination();
                _searchFrontier.Enqueue(tile);
            }
            else
            {
                tile.ClearPath();
            }
        }

        if(_searchFrontier.Count == 0)
        {
            return false;
        }

        int destinationIndex = _tiles.Length - 1;
        _tiles[destinationIndex].BecomeDestination();
        _searchFrontier.Enqueue(_tiles[destinationIndex]);

        while(_searchFrontier.Count > 0)
        {
            GameTile tile = _searchFrontier.Dequeue();
            
            if(tile != null)
            {
                _searchFrontier.Enqueue(tile.GrowPathNorth());
                _searchFrontier.Enqueue(tile.GrowPathEast());
                _searchFrontier.Enqueue(tile.GrowPathSouth());
                _searchFrontier.Enqueue(tile.GrowPathWest());
            }            
        }

        foreach(var tile in _tiles)
        {
            if(tile.HasPath == false)
            {
                return false;
            }
        }

        return true;
    }

    private void GenerateLevel()
    {
        int index = Mathf.FloorToInt((_tiles.Length - 2) / _enemyCount);        
        int countSpawnEnemy = 0;        

        for(int i = 0; i < _tiles.Length - 1; i++)
        {
            if (i == 0)
            {
                Build(_tiles[_tiles.Length - 1], GameTileContentType.Destination);
                Build(_tiles[0], GameTileContentType.TargetSpawnPoint);                
            }
            else
            {                
                if (i == index * (1 + countSpawnEnemy))
                {
                    countSpawnEnemy++;
                    Build(_tiles[i], GameTileContentType.EnemySpawnPoint);
                }
                else
                {
                    float chance = Random.value;

                    if (chance < 0.2f)
                    {
                        Build(_tiles[i], GameTileContentType.Obstacle);
                    }

                }

            }            
        }
    }
}
