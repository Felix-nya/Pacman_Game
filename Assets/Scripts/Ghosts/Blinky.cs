using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : MonoBehaviour
{
    [SerializeField] private State startingState;
    [SerializeField] private float movingSpeed = 10.0f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask wallWithHouseLayer;
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
    [SerializeField] private int blockSomeSwithingDirs = 5;

    private Vector2 _currentDirection = Vector2.right;
    private Vector2 _futureDirection = Vector2.zero;
    private readonly Vector2[] _dirs = { Vector2.right, Vector2.down, Vector2.left, Vector2.up };
    private State _currentState;
    private CircleCollider2D _cd;
    private Rigidbody2D _rb;
    private bool _ChangeDirection = true;
    private int _Waves = 1;
    private float _ChasingTimer = -1f;
    private float _ScatterTimer = 0f;
    private float _FrightenedTimer = 0f;
    private int _WaitForChoose = 0;
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
        _cd = GetComponent<CircleCollider2D>();
    }
    private void Start()
    {
        EnergyCollector.OnEatingEnergy += Collector_OnEatingEnergy;
    }
    private void FixedUpdate()
    {
        UpdateWaves();
        StateHandler();
        if (GhostChasePacman())
        {
            if (_CanEatPacman)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
#endif

                Application.Quit();

                Debug.Log("Exit");
            }
            else
            {
                _currentState = State.Death;
            }
        }
    }
    private void StateHandler()
    {
        switch (_currentState)
        {
            case State.Chasing:
                GhostChasing();
                break;
            case State.Scatter:
                GhostScatter();
                break;
            case State.Frightened:
                GhostFrightened();
                if (_FrightenedTimer == 0f)
                {
                    _CanEatPacman = false;
                }
                _FrightenedTimer += Time.deltaTime;
                if (_FrightenedTimer >= energyTime)
                {
                    _FrightenedTimer = 0f;
                    _currentState = State.Chasing;
                    _CanEatPacman = true;
                }
                break;
            case State.Death:
                if (_rb.position.x > -0.5f && _rb.position.y > 3.7f && _rb.position.x < 0.5f && _rb.position.y < 4.1f) _cd.isTrigger = true;
                MovingToHouse();
                break;
            default:
            case State.House:
                _CanEatPacman = true;
                _cd.isTrigger = true;
                if (CanLeaveHome())
                {
                    MovingFromHouse();
                }
                else
                {
                    _rb.linearVelocity = Vector2.zero;
                }
                break;
        }
    }
    private bool CanMoveInDirection(Vector2 _nextDirection, Vector2 _currentPosition, LayerMask _wallLayer)
    {
        if (_nextDirection == Vector2.zero) return false;

        float rayDistance = distanceToStop;
        RaycastHit2D hit = Physics2D.Raycast(_currentPosition - _currentDirection * checkOffSet, _nextDirection, rayDistance, _wallLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(_currentPosition + _currentDirection * checkOffSet, _nextDirection, rayDistance, _wallLayer);
        return (hit.collider == null && hit2.collider == null);
    }
    private void GhostChasing()
    {
        if (_WaitForChoose > 0)
        {
            _rb.linearVelocity = _currentDirection * movingSpeed;
            _WaitForChoose--;
            return;
        }

        if (!_ChangeDirection && _currentDirection != Vector2.down)
        {
            _rb.linearVelocity = _currentDirection * movingSpeed;
            return;
        }

        Vector2 targetPosition = GetTargetPosition();
        Vector2 bestDirection = Vector2.zero;
        float bestDistance = float.MaxValue;
        int countOfDirs = 0;

        foreach (Vector2 dir in _dirs)
        {
            if (dir == -_currentDirection) continue;

            if (CanMoveInDirection(dir, _rb.position, wallLayer))
            {
                countOfDirs++;
                Vector2 testPosition = _rb.position + dir;
                float distance = Vector2.Distance(testPosition, targetPosition);

                //Debug.DrawLine(testPosition, targetPosition, new Color(1, 0, 0, 0.2f), 0.3f);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestDirection = dir;
                }
            }
        }

        if (countOfDirs > 1) _WaitForChoose = blockSomeSwithingDirs;

        if (bestDirection == Vector2.zero)
        {
            bestDirection = _currentDirection;
        }

        _currentDirection = bestDirection;
        _rb.linearVelocity = _currentDirection * movingSpeed;
    }
    private void GhostScatter()
    {
        if (_WaitForChoose > 0)
        {
            _rb.linearVelocity = _currentDirection * movingSpeed;
            _WaitForChoose--;
            return;
        }

        if (!_ChangeDirection && _currentDirection != Vector2.down)
        {
            _rb.linearVelocity = _currentDirection * movingSpeed;
            return;
        }

        Vector2 bestDirection = Vector2.zero;
        float bestDistance = float.MaxValue;
        int countOfDirs = 0;

        foreach (Vector2 dir in _dirs)
        {
            if (dir == -_currentDirection) continue;

            if (CanMoveInDirection(dir, _rb.position, wallLayer))
            {
                countOfDirs++;
                Vector2 testPosition = _rb.position + dir;
                float distance = Vector2.Distance(testPosition, cellOfScary.position);

                //Debug.DrawLine(testPosition, cellOfScary.position, new Color(1, 0, 0, 0.2f), 0.3f);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestDirection = dir;
                }
            }
        }

        if (countOfDirs > 1) _WaitForChoose = blockSomeSwithingDirs;

        if (bestDirection == Vector2.zero)
        {
            bestDirection = _currentDirection;
        }

        _currentDirection = bestDirection;
        _rb.linearVelocity = _currentDirection * movingSpeed;
    }
    private void GhostFrightened()
    {
        if (_WaitForChoose > 0)
        {
            _rb.linearVelocity = _currentDirection * movingSpeed;
            _WaitForChoose--;
            return;
        }

        _futureDirection = Vector2.zero;
        List<Vector2> validDirections = new();

        foreach (Vector2 dir in _dirs)
        {
            if (dir == -_currentDirection) continue;
            if (CanMoveInDirection(dir, _rb.position, wallLayer))
            {
                validDirections.Add(dir);
            }
        }

        if (validDirections.Count > 1)
        {
            _WaitForChoose = blockSomeSwithingDirs;
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
    private void MovingToHouse()
    {
        if (_rb.position.x > -2f && _rb.position.y > 0.5f && _rb.position.x < 2f && _rb.position.y < 1.5f) _currentState = State.House;

        Vector2 bestDirection = Vector2.zero;
        float bestDistance = float.MaxValue;

        foreach (Vector2 dir in _dirs)
        {
            if (dir == -_currentDirection) continue;

            if (CanMoveInDirection(dir, _rb.position, wallWithHouseLayer))
            {
                Vector2 testPosition = _rb.position + dir;
                float distance = Vector2.Distance(testPosition, new Vector2(0, 1));

                //Debug.DrawLine(testPosition, new Vector2(0, 1), new Color(1, 0, 0, 0.2f), 0.3f);

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
        _rb.linearVelocity = 3 * movingSpeed * _currentDirection;
    }
    private void MovingFromHouse()
    {
        if (_rb.position.x > -0.2f && _rb.position.y > 3.85f && _rb.position.x < 0.2f && _rb.position.y < 4.1f)
        {
            _cd.isTrigger = false;
            _currentState = State.Chasing;
        }

        Vector2 bestDirection = Vector2.zero;
        float bestDistance = float.MaxValue;

        if (_rb.position.x > -0.1f && _rb.position.x < 0.1f)
        {
            bestDirection = Vector2.up;
        }
        else
        {
            foreach (Vector2 dir in _dirs)
            {
                if (CanMoveInDirection(dir, _rb.position, wallWithHouseLayer))
                {
                    Vector2 testPosition = _rb.position + dir;
                    float distance = Vector2.Distance(testPosition, new Vector2(0f, 4.1f));

                    //Debug.DrawLine(testPosition, new Vector2(0, 4), new Color(1, 0, 0, 0.2f), 0.3f);

                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestDirection = dir;
                    }
                }
            }
        }
        if (bestDirection == Vector2.zero)
        {
            bestDirection = _currentDirection;
        }

        _currentDirection = bestDirection;
        _rb.linearVelocity = movingSpeed * _currentDirection;

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
    private void UpdateWaves()
    {
        _ChasingTimer = (_ChasingTimer != -1f) ? _ChasingTimer + Time.deltaTime : -1f;
        _ScatterTimer = (_ScatterTimer != -1f) ? _ScatterTimer + Time.deltaTime : -1f;
        if (_ChasingTimer == -1f)
        {
            if (!IsUnActiveState()) _currentState = State.Scatter;
        }
        if (_ScatterTimer == -1f)
        {
            if (!IsUnActiveState()) _currentState = State.Chasing;
        }
        if (_Waves < 4 && _ChasingTimer >= 20f)
        {
            _Waves++;
            _ChasingTimer = -1f;
            _ScatterTimer = 0f;
            if (IsUnActiveState()) return;
            _currentState = State.Scatter;
            _currentDirection = -_currentDirection;
        }
        if (_Waves < 3 && _ScatterTimer >= 7f)
        {
            _ScatterTimer = -1f;
            _ChasingTimer = 0f;
            if (IsUnActiveState()) return;
            _currentState = State.Chasing;
            _currentDirection = -_currentDirection;
        }
        if (_Waves < 5 && _ScatterTimer >= 5f)
        {
            _ScatterTimer = -1f;
            _ChasingTimer = 0f;
            if (IsUnActiveState()) return;
            _currentState = State.Chasing;
            _currentDirection = -_currentDirection;
        }
    }
    private bool IsUnActiveState()
    {
        if (_currentState == State.Death || _currentState == State.House || _currentState == State.Frightened) return true;
        return false;
    }
    private bool CanLeaveHome()
    {
        int AmountOfCoins = Player.Instance._countOfCoins;
        if (ghost == Ghosts.Blinky || ghost == Ghosts.Pinky) return true;
        else if (ghost == Ghosts.Inky && AmountOfCoins >= 30) return true;
        else if (AmountOfCoins >= 60) return true;
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
    private void Collector_OnEatingEnergy(object sender, EventArgs e)
    {
        if (_currentState == State.Death || _currentState == State.House) return;
        _currentState = State.Frightened;
        _CanEatPacman = false;
        _FrightenedTimer = 0f;
    }
    private void OnDestroy()
    {
        EnergyCollector.OnEatingEnergy -= Collector_OnEatingEnergy;
    }
}
