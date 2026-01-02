using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    [SerializeField] private CircleCollider2D circleCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == circleCollider) CollectCoin(this.gameObject);
    }

    private void CollectCoin(GameObject coin)
    {
        Player.Instance._countOfCoins++;
        LevelManager.Instance.AddScore(10);
        Destroy(coin);
    }
}
