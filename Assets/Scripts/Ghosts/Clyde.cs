using UnityEngine;

public class Clyde : MonoBehaviour
{
    [SerializeField] private float movingSpeed = 10.0f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Collider2D specialGrid1;
    [SerializeField] private Collider2D specialGrid2;
    [SerializeField] private Collider2D specialGrid3;
    [SerializeField] private Collider2D specialGrid4;
    [SerializeField] private Transform pacman;
    [SerializeField] private Transform cellOfScary_Clyde;
    [SerializeField] private float checkOffSet = 0.425f;
    [SerializeField] private float distanceToStop = 0.74f;

    private Vector2 _currentDirection = Vector2.left;
    private Vector2 _futureDirection = Vector2.zero;
    private readonly Vector2[] _dirs = { Vector2.right, Vector2.down, Vector2.left, Vector2.up };
    private Rigidbody2D _rb;
    private float _DistanceToCell = 10000f;
    private Vector2 _TestPosition = Vector2.zero;
    private bool _ChangeDirection = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        MovingGhost();
    }

    private bool CanMoveInDirection(Vector2 _nextDirection)
    {
        if (_nextDirection == Vector2.zero) return false;

        float rayDistance = distanceToStop;
        RaycastHit2D hit = Physics2D.Raycast(_rb.position - _currentDirection * checkOffSet, _nextDirection, rayDistance, wallLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(_rb.position + _currentDirection * checkOffSet, _nextDirection, rayDistance, wallLayer);
        return (hit.collider == null && hit2.collider == null);
    }
    private void MovingGhost()
    {
        if (_ChangeDirection || _currentDirection == Vector2.down)
        {
            _DistanceToCell = 10000f;
            foreach (Vector2 dir in _dirs)
            {
                if (dir == -_currentDirection) continue;
                if (CanMoveInDirection(dir))
                {
                    _TestPosition = _rb.position + dir;
                    if (Vector2.Distance(_TestPosition, pacman.position) > 8f)
                    {
                        if (_DistanceToCell >= Vector2.Distance(_TestPosition, pacman.position))
                        {
                            _futureDirection = dir;
                            _DistanceToCell = Vector2.Distance(_TestPosition, pacman.position);
                            //Debug.DrawLine(_TestPosition, (Vector2)pacman.position, new Color(1, 0.5f, 0, 0.2f), 0.3f);
                        }
                    } else
                    {
                        if (_DistanceToCell >= Vector2.Distance(_TestPosition, cellOfScary_Clyde.position))
                        {
                            _futureDirection = dir;
                            _DistanceToCell = Vector2.Distance(_TestPosition, cellOfScary_Clyde.position);
                            //Debug.DrawLine(_TestPosition, (Vector2)cellOfScary_Clyde.position, new Color(1, 0.5f, 0, 0.2f), 0.3f);
                        }
                    }
                }
            }
            _currentDirection = _futureDirection;
            _rb.linearVelocity = _currentDirection * movingSpeed;
        }
        else _rb.linearVelocity = _currentDirection * movingSpeed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == specialGrid1 || collision == specialGrid2 || collision == specialGrid3 || collision == specialGrid4) _ChangeDirection = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == specialGrid1 || collision == specialGrid2 || collision == specialGrid3 || collision == specialGrid4) _ChangeDirection = true;
    }
}
