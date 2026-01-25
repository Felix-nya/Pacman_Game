using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    [Header("Префабы")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject energizerPrefab;

    [Header("Контейнеры")]
    [SerializeField] private Transform coinsContainer;
    [SerializeField] private Transform energizersContainer;

    [Header("Позиции")]
    [SerializeField] private Transform[] coinPositions;
    [SerializeField] private Transform[] energizerPositions;

    [Header("Настройки")]
    [SerializeField] private string coinTag = "Coins";
    [SerializeField] private string energizerTag = "Energy";

    private List<GameObject> _currentCoins = new();
    private List<GameObject> _currentEnergizers = new();

    private Vector3[] _coinPositions;
    private Vector3[] _energizerPositions;

    private void Start()
    {
        CaptureExistingObjects();
        _coinPositions = new Vector3[coinPositions.Length];
        for (int i = 0; i < coinPositions.Length; i++)
        {
            if (coinPositions[i] != null)
                _coinPositions[i] = coinPositions[i].position;
        }

        _energizerPositions = new Vector3[energizerPositions.Length];
        for (int i = 0; i < energizerPositions.Length; i++)
        {
            if (energizerPositions[i] != null)
                _energizerPositions[i] = energizerPositions[i].position;
        }
    }

    public void ClearLevelObjects()
    {
        foreach (var coin in _currentCoins)
        {
            if (coin != null) Destroy(coin);
        }
        _currentCoins.Clear();

        foreach (var energizer in _currentEnergizers)
        {
            if (energizer != null) Destroy(energizer);
        }
        _currentEnergizers.Clear();
    }

    public void GenerateLevelObjects()
    {
        GenerateCoins();
        GenerateEnergizers();
    }

    private void GenerateCoins()
    {
        foreach (var position in _coinPositions)
        {
            GameObject coin = Instantiate(coinPrefab, position, Quaternion.identity, coinsContainer);
            _currentCoins.Add(coin);
        }
    }

    private void GenerateEnergizers()
    {
        foreach (var position in _energizerPositions)
        {
            GameObject energizer = Instantiate(energizerPrefab, position, Quaternion.identity, energizersContainer);
            _currentEnergizers.Add(energizer);
        }
    }

    private void CaptureExistingObjects()
    {
        CaptureCoins();
        CaptureEnergizers();
    }

    private void CaptureCoins()
    {
        GameObject[] coinObjects = GameObject.FindGameObjectsWithTag(coinTag);

        foreach (GameObject coin in coinObjects)
        {
            _currentCoins.Add(coin);

            if (coinsContainer != null)
            {
                coin.transform.SetParent(coinsContainer);
            }
        }
    }

    private void CaptureEnergizers()
    {
        GameObject[] energizerObjects = GameObject.FindGameObjectsWithTag(energizerTag);

        foreach (GameObject energizer in energizerObjects)
        {
            _currentEnergizers.Add(energizer);

            if (energizersContainer != null)
            {
                energizer.transform.SetParent(energizersContainer);
            }
        }
    }
}
