using UnityEngine;

public class GameTile : MonoBehaviour
{
    private GameTile _north, _east, _south, _west;    
    private int _distance;

    private GameTileContent _content;
    public GameTileContent Content
    {
        get => _content;
        set
        {
            if(_content != null)
            {
                _content.Recycle();
            }
            _content = value;
            _content.transform.localPosition = transform.localPosition;
        }
    }

    public bool HasPath => _distance != int.MaxValue;

    public static void MakeEastWestNeighbors(GameTile east, GameTile west)
    {
        west._east = east;
        east._west = west;
    }

    public static void MakeNorthSouthNeighbors(GameTile north, GameTile south)
    {        
        south._north = north;
        north._south = south;
    }

    public void ClearPath()
    {
        _distance = int.MaxValue;        
    }

    public void BecomeDestination()
    {
        _distance = 0;        
    }

    private GameTile GrowPathTo(GameTile neighbor)
    {
        if(HasPath == false || neighbor == null || neighbor.HasPath)
        {
            return null;
        }

        neighbor._distance = _distance + 1;        
        return neighbor.Content.Type != GameTileContentType.Obstacle ? neighbor : null;
    }

    public GameTile GrowPathNorth() => GrowPathTo(_north);

    public GameTile GrowPathEast() => GrowPathTo(_east);

    public GameTile GrowPathSouth() => GrowPathTo(_south);

    public GameTile GrowPathWest() => GrowPathTo(_west);
   
}
