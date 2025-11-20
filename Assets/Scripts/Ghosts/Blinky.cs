using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Transform cellOfScary;
    [SerializeField] private Transform blinky;
    [SerializeField] private Transform ghostTransform;
    [SerializeField] private float checkOffSet = 0.425f;
    [SerializeField] private float distanceToStop = 0.74f;
    [SerializeField] private Ghosts ghost;
    [SerializeField] private int energyTime = 10;

    private Vector2 _currentDirection = Vector2.right;
    private Vector2 _futureDirection = Vector2.zero;
    private readonly Vector2[] _dirs = { Vector2.right, Vector2.down, Vector2.left, Vector2.up };
    private State _currentState;
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Color _color;
    private bool _ChangeDirection = true;
    private int _Waves = 1;
    private float _ChasingTimer = 0f;
    private float _ScatterTimer = 0f;
    private float _FrightenedTimer = 0f;
    private bool _CanEatPacman = true;

    private enum State
    {
        House,
        Scatter,
        Chasing,
        Frightened,
        Death
    }
    private enum Ghosts
    {
        Blinky,
        Clyde,
        Inky,
        Pinky
    }
    private void Awake()
    {
        _currentState = startingState;
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _color = _sr.color;
    }
    private void Start()
    {
        EnergyCollector.OnEatingEnergy += collector_OnEatingEnergy;
    }
    private void FixedUpdate()
    {
        StateHandler();
        if (GhostChasePacman() && _CanEatPacman)
        {
#if UNITY_EDITOR                                                                  
            UnityEditor.EditorApplication.ExitPlaymode();
#endif

            Application.Quit();

            Debug.Log("Exit");
        }
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
                    _currentDirection = -_currentDirection;
                }
                if (_Waves < 5 && _ScatterTimer >= 5f)
                {
                    _ScatterTimer = 0f;
                    _currentState = State.Chasing;
                    _currentDirection = -_currentDirection;
                }
                break;
            case State.Frightened:
                GhostFrightened();
                if (_FrightenedTimer == 0f)
                {
                    _CanEatPacman = false;
                    _sr.color = Color.blueViolet;
                }
                _FrightenedTimer += Time.deltaTime;
                if (_FrightenedTimer >= energyTime)
                {
                    _FrightenedTimer = 0f;
                    _currentState = State.Chasing;
                    _CanEatPacman = true;
                    _sr.color = _color;
                }
                break;
            case State.House:
                break;
            default:
            case State.Death:
                break;
        }
    }
    private bool CanMoveInDirection(Vector2 _nextDirection, Vector2 _currentPosition)
    {
        if (_nextDirection == Vector2.zero) return false;

        float rayDistance = distanceToStop;
        RaycastHit2D hit = Physics2D.Raycast(_currentPosition - _currentDirection * checkOffSet, _nextDirection, rayDistance, wallLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(_currentPosition + _currentDirection * checkOffSet, _nextDirection, rayDistance, wallLayer);
        return (hit.collider == null && hit2.collider == null);
    }
    private void GhostChasing()
    {
        if (!_ChangeDirection && _currentDirection != Vector2.down)
        {
            _rb.linearVelocity = _currentDirection * movingSpeed;
            return;
        }

        Vector2 targetPosition = GetTargetPosition();
        Vector2 bestDirection = Vector2.zero;
        float bestDistance = float.MaxValue;

        foreach (Vector2 dir in _dirs)
        {
            if (dir == -_currentDirection) continue;

            if (CanMoveInDirection(dir, _rb.position))
            {
                Vector2 testPosition = _rb.position + dir;
                float distance = Vector2.Distance(testPosition, targetPosition);

                Debug.DrawLine(testPosition, targetPosition, new Color(1, 0, 0, 0.2f), 0.3f);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestDirection = dir;
                }
            }
        }

        if (bestDirection == Vector2.zero)
        {
            bestDirection = _currentDirection;
        }

        _currentDirection = bestDirection;
        _rb.linearVelocity = _currentDirection * movingSpeed;
}
    private void GhostScatter()
    {
        if (!_ChangeDirection && _currentDirection != Vector2.down)
        {
            _rb.linearVelocity = _currentDirection * movingSpeed;
            return;
        }

        Vector2 bestDirection = Vector2.zero;
        float bestDistance = float.MaxValue;

        foreach (Vector2 dir in _dirs)
        {
            if (dir == -_currentDirection) continue;

            if (CanMoveInDirection(dir, _rb.position))
            {
                Vector2 testPosition = _rb.position + dir;
                float distance = Vector2.Distance(testPosition, cellOfScary.position);

                Debug.DrawLine(testPosition, cellOfScary.position, new Color(1, 0, 0, 0.2f), 0.3f);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestDirection = dir;
                }
            }
        }

        if (bestDirection == Vector2.zero)
        {
            bestDirection = _currentDirection;
        }

        _currentDirection = bestDirection;
        _rb.linearVelocity = _currentDirection * movingSpeed;
    }
    private void GhostFrightened()
    {
        _futureDirection = Vector2.zero;
        List<Vector2> validDirections = new();

        foreach (Vector2 dir in _dirs)
        {
            if (dir == -_currentDirection) continue;
            if (CanMoveInDirection(dir, _rb.position))
            {
                validDirections.Add(dir);
            }
        }

        if (validDirections.Count > 0)
        {
            _futureDirection = validDirections[UnityEngine.Random.Range(0, validDirections.Count)];
        }
        else
        {
            _futureDirection = _currentDirection;
        }
        _currentDirection = _futureDirection;
        _rb.linearVelocity = _currentDirection * movingSpeed;

    }
    private bool GhostChasePacman()
    {
        Vector2 _pacmanCell;
        Vector2 _ghostCell;
        _pacmanCell.x = Mathf.Round(pacman.position.x * 2f) / 2f;
        _pacmanCell.y = Mathf.Round(pacman.position.y);
        _ghostCell.x = Mathf.Round(ghostTransform.position.x * 2f) / 2f;
        _ghostCell.y = Mathf.Round(ghostTransform.position.y);
        if (_pacmanCell == _ghostCell) return true;
        return false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == specialGrid1 || collision == specialGrid2 || collision == specialGrid3 || collision == specialGrid4) _ChangeDirection = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == specialGrid1 || collision == specialGrid2 || collision == specialGrid3 || collision == specialGrid4) _ChangeDirection = true;
    }
    private Vector2 GetTargetPosition()
    {
        switch (ghost)
        {
            case Ghosts.Blinky:
                return pacman.position;

            case Ghosts.Pinky:
                return (Vector2)pacman.position + Player.Instance._currentDirection * 4;

            case Ghosts.Inky:
                Vector2 pacmanAhead = (Vector2)pacman.position + Player.Instance._currentDirection * 2;
                return 2 * pacmanAhead - (Vector2)blinky.position;

            case Ghosts.Clyde:
                float distanceToPacman = Vector2.Distance(_rb.position, pacman.position);
                return distanceToPacman > 8f ? pacman.position : cellOfScary.position;

            default:
                return pacman.position;
        }
    }
    private void collector_OnEatingEnergy(object sender, EventArgs e)
    {
        _currentState = State.Frightened;
        _FrightenedTimer = 0f;
    }
    private void OnDestroy()
    {
        EnergyCollector.OnEatingEnergy -= collector_OnEatingEnergy;
    }
}
