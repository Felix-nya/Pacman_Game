using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private Animator animator;
    private static readonly int Victory = Animator.StringToHash(IsWin);
    private const string IsWin = "IfWin";

    [SerializeField] private Blinky blinky;
    [SerializeField] private Blinky pinky;
    [SerializeField] private Blinky inky;
    [SerializeField] private Blinky clyde;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private SpriteRenderer startingPan;
    [SerializeField] private SpriteRenderer endPan;

    [SerializeField] private PrefabManager levelObjectManager;

    [SerializeField] private Image life1;
    [SerializeField] private Image life2;
    [SerializeField] private Image life3;

    public int _currentLifes;
    private int _currentScore = 0;
    private int _currentLevel = 1;
    private readonly float _mainVelocity = 5f;
    private int _ghostMulti = 1;

    private bool _isPause = true;
    private bool _isTransitioning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        InputSystem.Instance.OnResetButton += Player_OnResetButton;
        _currentLifes = 4;
        endPan.enabled = false;
        UpdateScoreUI();
        Time.timeScale = 0f;
        _isPause = true;
        startingPan.enabled = true;
        SetterWithCurLevel();
    }
    private void FixedUpdate()
    {
        if (!_isTransitioning && Player.Instance._countOfCoins == 240)
        {
            _isTransitioning = true;
            _currentLevel++;
            animator.SetBool(Victory, true);
            StartCoroutine(TransitionToNextLevel());
        }
    }

    
    public void AddScore(int points)
    {
        _currentScore += points;
        UpdateScoreUI();
    }
    public void AddGhostScore()
    {
        _currentScore += 200 * _ghostMulti;
        Debug.Log(200 * _ghostMulti);
        _ghostMulti *= 2;
        if (_ghostMulti > 16) 
        {
            _ghostMulti = 1;
        }
        UpdateScoreUI();
    }
    public void ResetGhostMulti()
    {
        _ghostMulti = 1;
    }
    public void SetDeath()
    {
        if (_currentLifes == 4)
        {
            Debug.Log("1");
            _currentLifes = 3;
            life1.enabled = false;
            StartCoroutine(ResetWithoutNextLevel());
        } else if (_currentLifes == 3)
        {
            Debug.Log("2");
            _currentLifes = 2;
            life2.enabled = false;
            StartCoroutine(ResetWithoutNextLevel());
        } else if (_currentLifes == 2)
        {
            Debug.Log("3");
            _currentLifes = 1;
            life3.enabled = false;
            StartCoroutine(ResetWithoutNextLevel());
        } else
        {
            _currentLifes = 0;
            Debug.Log("4");
            endPan.enabled = true;
            StartCoroutine(Lose());
        }
    }
    public void SetPhaseAfterDeath()
    {
        blinky.ResetThisGhost();
        pinky.ResetThisGhost();
        inky.ResetThisGhost();
        clyde.ResetThisGhost();
        Player.Instance.ResetToStartPosition();
        Player.Instance._isDeath = false;
        SetterWithCurLevel();
        Player.Instance.SetControlEnable();
        PauseGame();
    }
    private void SetterWithCurLevel()
    {
        if (_currentLevel == 1)
        {
            Player.Instance.movingSpeed = _mainVelocity * 0.8f;

            if (blinky != null) blinky.SetGhostVelocity(_mainVelocity * 0.75f);
            if (pinky != null) pinky.SetGhostVelocity(_mainVelocity * 0.75f);
            if (inky != null) inky.SetGhostVelocity(_mainVelocity * 0.75f);
            if (clyde != null) clyde.SetGhostVelocity(_mainVelocity * 0.75f);

            if (blinky != null) blinky.SetGhostWavesTimer(20f, 20f, 7f, 5f, 5f);
            if (pinky != null) pinky.SetGhostWavesTimer(20f, 20f, 7f, 5f, 5f);
            if (inky != null) inky.SetGhostWavesTimer(20f, 20f, 7f, 5f, 5f);
            if (clyde != null) clyde.SetGhostWavesTimer(20f, 20f, 7f, 5f, 5f);
        }
        else if (_currentLevel <= 4)
        {
            Player.Instance.movingSpeed = _mainVelocity * 0.9f;

            if (blinky != null) blinky.SetGhostVelocity(_mainVelocity * 0.85f);
            if (pinky != null) pinky.SetGhostVelocity(_mainVelocity * 0.85f);
            if (inky != null) inky.SetGhostVelocity(_mainVelocity * 0.85f);
            if (clyde != null) clyde.SetGhostVelocity(_mainVelocity * 0.85f);

            if (blinky != null) blinky.SetGhostWavesTimer(20f, 1033f, 7f, 5f, 0.015f);
            if (pinky != null) pinky.SetGhostWavesTimer(20f, 1033f, 7f, 5f, 0.015f);
            if (inky != null) inky.SetGhostWavesTimer(20f, 1033f, 7f, 5f, 0.015f);
            if (clyde != null) clyde.SetGhostWavesTimer(20f, 1033f, 7f, 5f, 0.015f);
        }
        else
        {
            Player.Instance.movingSpeed = _mainVelocity;

            if (blinky != null) blinky.SetGhostVelocity(_mainVelocity * 0.95f);
            if (pinky != null) pinky.SetGhostVelocity(_mainVelocity * 0.95f);
            if (inky != null) inky.SetGhostVelocity(_mainVelocity * 0.95f);
            if (clyde != null) clyde.SetGhostVelocity(_mainVelocity * 0.95f);

            if (blinky != null) blinky.SetGhostWavesTimer(20f, 1037f, 5f, 5f, 0.015f);
            if (pinky != null) pinky.SetGhostWavesTimer(20f, 1037f, 5f, 5f, 0.015f);
            if (inky != null) inky.SetGhostWavesTimer(20f, 1037f, 5f, 5f, 0.015f);
            if (clyde != null) clyde.SetGhostWavesTimer(20f, 1037f, 5f, 5f, 0.015f);
        }
    }

    public int GetLevel()
    {
        return _currentLevel;
    }

    private void TogglePause()
    {
        if (_isPause)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    private void PauseGame()
    {
        _isPause = true;
        Time.timeScale = 0f;
        startingPan.enabled = true;
    }
    private void ResumeGame()
    {
        _isPause = false;
        Time.timeScale = 1f;
        startingPan.enabled = false;
    }
    private void UpdateScoreUI()
    {
        scoreText.text = $"{_currentScore}";
    }
    private void Player_OnResetButton(object sender, System.EventArgs e)
    {
        TogglePause();
    }
    private IEnumerator TransitionToNextLevel()
    {
        Player.Instance.SetControlDisable();
        levelObjectManager.ClearLevelObjects();//
        blinky.VanishThisGhost();
        pinky.VanishThisGhost();
        inky.VanishThisGhost();
        clyde.VanishThisGhost();
        yield return new WaitForSeconds(3f);//
        animator.SetBool(Victory, false);//
        yield return StartCoroutine(ClearAndGenerateLevel());
        Player.Instance.SetControlEnable();
        _isTransitioning = false;//
        PauseGame();
    }
    private IEnumerator ClearAndGenerateLevel()
    {
        blinky.ResetThisGhost();
        pinky.ResetThisGhost();
        inky.ResetThisGhost();
        clyde.ResetThisGhost();
        Player.Instance.ResetToStartPosition();
        Player.Instance.ResetCoinCount();//
        levelObjectManager.GenerateLevelObjects();//
        SetterWithCurLevel();

        yield return null;
    }
    private IEnumerator ResetWithoutNextLevel()
    {
        blinky.VanishThisGhost();
        pinky.VanishThisGhost();
        inky.VanishThisGhost();
        clyde.VanishThisGhost();
        yield return null;
    }
    private IEnumerator Lose()
    {
        Player.Instance.SetControlDisable();
        levelObjectManager.ClearLevelObjects();
        blinky.VanishThisGhost();
        pinky.VanishThisGhost();
        inky.VanishThisGhost();
        clyde.VanishThisGhost();
        yield return new WaitForSeconds(3f);
        Debug.Log("gg");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}