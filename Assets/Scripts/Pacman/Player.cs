using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private float movingSpeed = 10.0f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float checkOffSet = 0.3f;
    [SerializeField] private float distanceToStop = 0.1f;

    private Vector2 _inputVector;
    public Vector2 _currentDirection = Vector2.right;
    private Vector2 _nextDirection;
    private Rigidbody2D _rb;

    private void Awake()
    {
        Instance = this;
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _inputVector = InputSystem.Instance.GetMovementVector();
        _inputVector = RoundToFourDirections(_inputVector);

        if (_inputVector != Vector2.zero)
        {
            _nextDirection = _inputVector;
        }
    }

    private void FixedUpdate()
    {
        if (_nextDirection != Vector2.zero && CanMoveInDirection() && _nextDirection != _currentDirection)
        {
            _currentDirection = _nextDirection;
            _nextDirection = Vector2.zero;
        }

        _rb.linearVelocity = _currentDirection * movingSpeed;
    }

    private bool CanMoveInDirection()
    {
        if (_nextDirection == Vector2.zero) return false;

        float rayDistance = distanceToStop;
        RaycastHit2D hit = Physics2D.Raycast(_rb.position - _currentDirection * checkOffSet, _nextDirection, rayDistance, wallLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(_rb.position + _currentDirection * checkOffSet, _nextDirection, rayDistance, wallLayer);

        return (hit.collider == null && hit2.collider == null);
    }

    private Vector2 RoundToFourDirections(Vector2 input)
    {
        if (input.magnitude < 0.3f) return Vector2.zero;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            return new Vector2(Mathf.Sign(input.x), 0);
        else
            return new Vector2(0, Mathf.Sign(input.y));
    }

}