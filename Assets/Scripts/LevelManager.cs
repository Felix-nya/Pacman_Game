using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
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
    }
    private void Reseting()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
