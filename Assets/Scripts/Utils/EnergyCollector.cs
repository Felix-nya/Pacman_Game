using System;
using UnityEngine;

public class EnergyCollector : MonoBehaviour
{
    private CircleCollider2D circleCollider;
    public static event EventHandler OnEatingEnergy;

    private void Start()
    {
        circleCollider = Player.Instance.getCol2D();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == circleCollider) CollectEnergy(this.gameObject);
    }

    private void CollectEnergy(GameObject energy)
    {
        Debug.Log("Ghost Hunt");
        LevelManager.Instance.ResetGhostMulti();
        OnEatingEnergy?.Invoke(this, EventArgs.Empty);
        Destroy(energy);
    }
}
