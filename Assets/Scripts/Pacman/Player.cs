using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] public float movingSpeed = 10.0f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float checkOffSet = 0.3f;
    [SerializeField] private float distanceToStop = 0.1f;

    [SerializeField] public int _countOfCoins = 0;
    private Vector2 _inputVector;
    public Vector2 _currentDirection = Vector2.right;
    private Vector2 _nextDirection;
    private Vector2 _startPosition;
    private Rigidbody2D _rb;
    private bool _canMove = true;
    public bool _isDeath = false;
    public bool _isMoving = true;

    private void Awake()
    {
        Instance = this;
        _rb = GetComponent<Rigidbody2D>();
        _startPosition = new Vector2(0, -8);
    }

    private void Update()
    {
        if (!_isDeath)
        {
            if (_canMove)
            {
                _inputVector = InputSystem.Instance.GetMovementVector();
                _inputVector = RoundToFourDirections(_inputVector);

                if (_inputVector != Vector2.zero)
                {
                    _nextDirection = _inputVector;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (MathF.Abs(_rb.linearVelocity.x) < 0.1f && MathF.Abs(_rb.linearVelocity.y) < 0.1f)
        {
            _isMoving = false;
        }
        else
        {
            _isMoving = true;
        }

        if (!_isDeath)
        {
            if (_nextDirection != Vector2.zero && CanMoveInDirection() && _nextDirection != _currentDirection)
            {
                _currentDirection = _nextDirection;
                _nextDirection = Vector2.zero;
            }

            _rb.linearVelocity = _currentDirection * movingSpeed;
        }
        else if (_rb.linearVelocity != Vector2.zero)
        {
            _rb.linearVelocity = Vector2.zero;
        }
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

    public void Death()
    {
        LevelManager.Instance.SetDeath();
        _isDeath = true;
        _rb.linearVelocity = Vector2.zero;
        _nextDirection = Vector2.right;
    }

    public void SetControlEnable()
    {
        _canMove = true;
    }

    public void SetControlDisable() 
    { 
        _canMove = false;
        movingSpeed = 0f;
    }

    public void ResetToStartPosition()
    {
        transform.position = _startPosition;
        transform.rotation = Quaternion.identity;
    }

    public void ResetCoinCount()
    {
        _countOfCoins = 0;
    }

    public CircleCollider2D getCol2D()
    {
        return GetComponent<CircleCollider2D>();
    }

}