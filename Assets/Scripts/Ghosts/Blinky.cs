using System;
using UnityEngine;

public class Blinky : MonoBehaviour
{
    [SerializeField] private State startingState;
    [SerializeField] private float movingSpeed = 10.0f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Collider2D specialGrid1;
    [SerializeField] private Collider2D specialGrid2;
    [SerializeField] private Collider2D specialGrid3;
    [SerializeField] private Collider2D specialGrid4;
    [SerializeField] private Transform pacman;
    [SerializeField] private Transform cellOfScary_Blinky;
    [SerializeField] private float checkOffSet = 0.425f;
    [SerializeField] private float distanceToStop = 0.74f;

    private Vector2 _currentDirection = Vector2.right;
    private Vector2 _futureDirection = Vector2.zero;
    private readonly Vector2[] _dirs = { Vector2.right, Vector2.down, Vector2.left, Vector2.up };
    private State _currentState;
    private Rigidbody2D _rb;
    private float _DistanceToCell = 10000f;
    private Vector2 _TestPosition = Vector2.zero;
    private bool _ChangeDirection = true;
    private int _Waves = 1;
    private float _ChasingTimer = 0f;
    private float _ScatterTimer = 0f;
    private enum State
    {
        House,
        Scatter,
        Chasing,
        Frightened,
        Death
    }

    private void Awake()
    {
        _currentState = startingState;
        _rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        StateHandler();
    }
    private void StateHandler()
    {
        switch (_currentState)
        {
            case State.Chasing:
                GhostChasing();
                _ChasingTimer += Time.deltaTime;
                if (_Waves < 4 && _ChasingTimer >= 20f)
                {
                    _Waves++;
                    _ChasingTimer = 0f;
                    _currentState = State.Scatter;
                    _currentDirection = -_currentDirection;
                }
                break;
            case State.Scatter:
                GhostScatter();
                _ScatterTimer += Time.deltaTime;
                if (_Waves < 3 && _ScatterTimer >= 7f)
                {
                    _ScatterTimer = 0f;
                    _currentState = State.Chasing;
                }
                if (_Waves < 5 && _ScatterTimer >= 5f)
                {
                    _ScatterTimer = 0f;
                    _currentState = State.Chasing;
                }
                break;
            case State.Frightened:
                
                break;
            case State.House:
                break;
            default:
            case State.Death:
                break;
        }
    }
    private bool CanMoveInDirection(Vector2 _nextDirection)
    {
        if (_nextDirection == Vector2.zero) return false;

        float rayDistance = distanceToStop;
        RaycastHit2D hit = Physics2D.Raycast(_rb.position - _currentDirection * checkOffSet, _nextDirection, rayDistance, wallLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(_rb.position + _currentDirection * checkOffSet, _nextDirection, rayDistance, wallLayer);
        return (hit.collider == null && hit2.collider == null);
    }
    private void GhostChasing()
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
                    if (_DistanceToCell >= Vector2.Distance(_TestPosition, pacman.position))
                    {
                        _futureDirection = dir;
                        _DistanceToCell = Vector2.Distance(_TestPosition, pacman.position);
                        Debug.DrawLine(_TestPosition, (Vector2)pacman.position, new Color(1, 0, 0, 0.2f), 0.3f);
                    }
                }
            }
            _currentDirection = _futureDirection;
            _rb.linearVelocity = _currentDirection * movingSpeed;
        }
        else _rb.linearVelocity = _currentDirection * movingSpeed;
    }
    private void GhostScatter()
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
                    if (_DistanceToCell >= Vector2.Distance(_TestPosition, cellOfScary_Blinky.position))
                    {
                        _futureDirection = dir;
                        _DistanceToCell = Vector2.Distance(_TestPosition, cellOfScary_Blinky.position);
                        Debug.DrawLine(_TestPosition, (Vector2)cellOfScary_Blinky.position, new Color(1, 0, 0, 0.2f), 0.3f);
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
