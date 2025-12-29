using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private int _currentLevel = 1;
    private float _reset;
    private void Update()
    {
        _reset = InputSystem.Instance.GetReset();
    }
    private void FixedUpdate()
    {
        if (_reset != 0)
        {
            Reseting();
        }
        if (Player.Instance._countOfCoins == 240)
        {
            _currentLevel++;
            Reseting();
        }
    }
    private void Reseting()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    //нужно добавить переход на некст уровень который изменит характеристики

}
