using System;
using UnityEngine;

public class EnergyCollector : MonoBehaviour
{
    [SerializeField] private CircleCollider2D circleCollider;
    public static event EventHandler OnEatingEnergy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == circleCollider) CollectEnergy(this.gameObject);
    }

    private void CollectEnergy(GameObject energy)
    {
        Debug.Log("Ghost Hunt");
        OnEatingEnergy?.Invoke(this, EventArgs.Empty);
        Destroy(energy);
    }
}
