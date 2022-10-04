using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private CharacterController _character;    

    private float _turnSmoothTime = 0.05f;
    private Vector3 _newPosition;
    private Vector3 _offset;

    public void Move(Vector3 direction, float speed, Rect allowedArea)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, _turnSmoothTime);        
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        _offset = direction.normalized * speed * Time.deltaTime;
        _newPosition = transform.localPosition + _offset;

        if (allowedArea.Contains(new Vector2(_newPosition.x, _newPosition.z)))
        {
            _character.Move(_offset);
        }
    }
}
