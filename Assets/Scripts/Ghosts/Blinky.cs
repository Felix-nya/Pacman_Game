using System;
using System.Collections;
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
    private float _DistanceToCell = 10000f;
    private Vector2 _TestPosition = Vector2.zero;
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
            if (ghost == Ghosts.Clyde)
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
                        }
                        else
                        {
                            if (_DistanceToCell >= Vector2.Distance(_TestPosition, cellOfScary.position))
                            {
                                _futureDirection = dir;
                                _DistanceToCell = Vector2.Distance(_TestPosition, cellOfScary.position);
                                //Debug.DrawLine(_TestPosition, (Vector2)cellOfScary.position, new Color(1, 0.5f, 0, 0.2f), 0.3f);
                            }
                        }
                    }
                }
                _currentDirection = _futureDirection;
                _rb.linearVelocity = _currentDirection * movingSpeed;
            }
            else
            {
                _DistanceToCell = 10000f;
                foreach (Vector2 dir in _dirs)
                {
                    if (dir == -_currentDirection) continue;
                    if (CanMoveInDirection(dir))
                    {
                        Vector2 _cellPosition = (ghost == Ghosts.Blinky) ? (Vector2)pacman.position : (ghost == Ghosts.Pinky) ? (Vector2)pacman.position + Player.Instance._currentDirection * 4 : (ghost == Ghosts.Inky) ? 2 * ((Vector2)pacman.position + Player.Instance._currentDirection * 2) - (Vector2)blinky.position : Vector2.zero;
                        _TestPosition = _rb.position + dir;
                        if (_DistanceToCell >= Vector2.Distance(_TestPosition, _cellPosition))
                        {
                            _futureDirection = dir;
                            _DistanceToCell = Vector2.Distance(_TestPosition, _cellPosition);
                            //Debug.DrawLine(_TestPosition, _cellPosition, new Color(1, 0, 0, 0.2f), 0.3f);
                        }
                    }
                }
                _currentDirection = _futureDirection;
                _rb.linearVelocity = _currentDirection * movingSpeed;
            }
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
                    if (_DistanceToCell >= Vector2.Distance(_TestPosition, cellOfScary.position))
                    {
                        _futureDirection = dir;
                        _DistanceToCell = Vector2.Distance(_TestPosition, cellOfScary.position);
                        //Debug.DrawLine(_TestPosition, (Vector2)cellOfScary.position, new Color(1, 0, 0, 0.2f), 0.3f);
                    }
                }
            }
            _currentDirection = _futureDirection;
            _rb.linearVelocity = _currentDirection * movingSpeed;
        }
        else _rb.linearVelocity = _currentDirection * movingSpeed;
    }
    private void GhostFrightened()
    {
        _futureDirection = Vector2.zero;
        foreach (Vector2 dir in _dirs)
        {
            if (dir == -_currentDirection) continue;
            if (CanMoveInDirection(dir))
            {
                if (_futureDirection == Vector2.zero)
                {
                    _futureDirection = dir;
                }
                else
                {
                    if (UnityEngine.Random.Range(1, 3) == 1)
                    {
                        _futureDirection = dir;
                    }
                }

            }
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
