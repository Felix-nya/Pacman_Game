using System.Collections;
using UnityEngine;

public class FruitCollector : MonoBehaviour
{
    private CircleCollider2D _circleCollider;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    private bool _flag1;
    private bool _flag2;
    private int _level;
    private int _coins;

    private void Awake()
    {
        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_boxCollider == null) _boxCollider = GetComponent<BoxCollider2D>();
        _flag1 = true;
        _flag2 = true;
    }

    private void Start()
    {
        _circleCollider = Player.Instance.getCol2D();
        _spriteRenderer.enabled = false;
        _boxCollider.enabled = false;
        _level = LevelManager.Instance.GetLevel();
    }

    private void FixedUpdate()
    {
        _coins = Player.Instance._countOfCoins;
        if (_coins == 70 && _flag1)
        {
            _flag1 = false;
            _spriteRenderer.enabled = true;
            _boxCollider.enabled = true;
            StartCoroutine(lifeTimer());
        }
        if (_coins == 170 && _flag2)
        {
            _flag2 = false;
            _spriteRenderer.enabled = true;
            _boxCollider.enabled = true;
            StartCoroutine(lifeTimer());
        }
        if (_coins == 0)
        {
            _flag1 = true;
            _flag2 = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == _circleCollider) CollectFruit();
    }

    private void CollectFruit()
    {
        _spriteRenderer.enabled = false;
        _boxCollider.enabled = false;
        _level = LevelManager.Instance.GetLevel();
        if (_level == 1) LevelManager.Instance.AddScore(100);
        else if (_level == 2) LevelManager.Instance.AddScore(300);
        else if (_level <= 4) LevelManager.Instance.AddScore(500);
        else if (_level <= 6) LevelManager.Instance.AddScore(700);
        else if (_level <= 8) LevelManager.Instance.AddScore(1000);
        else if (_level <= 10) LevelManager.Instance.AddScore(2000);
        else if (_level <= 12) LevelManager.Instance.AddScore(3000);
        else LevelManager.Instance.AddScore(5000);
    }

    private IEnumerator lifeTimer()
    {
        yield return new WaitForSeconds(9f + Random.Range(0f,1f));
        _spriteRenderer.enabled = false;
        _boxCollider.enabled = false;
    }
}
