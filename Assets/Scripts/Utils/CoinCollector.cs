using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    private CircleCollider2D circleCollider;
    public AudioSource coinEat;
    private void Start()
    {
        circleCollider = Player.Instance.getCol2D();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == circleCollider) CollectCoin(this.gameObject);
    }

    private void CollectCoin(GameObject coin)
    {
        coinEat.Play();
        Player.Instance._countOfCoins++;
        LevelManager.Instance.AddScore(10);
        Destroy(coin);
    }
}
