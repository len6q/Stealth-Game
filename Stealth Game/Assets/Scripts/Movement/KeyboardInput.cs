using UnityEngine;

public class KeyboardInput : MonoBehaviour
{
    [SerializeField] private Movement _movement;    

    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private Vector3 _direction;    

    public bool IsMove(float speed, Rect allowedArea)
    {
        _direction = new Vector3(Input.GetAxis(HORIZONTAL), 0f, Input.GetAxis(VERTICAL));

        if(_direction.magnitude > 0f)
        {
            _movement.Move(_direction, speed, allowedArea);
            return true;
        }

        return false;
    }
}
