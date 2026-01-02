using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private Blinky blinky;
    [SerializeField] private Blinky pinky;
    [SerializeField] private Blinky inky;
    [SerializeField] private Blinky clyde;

    [SerializeField] private TextMeshProUGUI scoreText;

    private int _currentScore = 0;
    private int _currentLevel = 1;
    private readonly float _mainVelocity = 5f;
    private float _reset;

    private bool _isLevelInitialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeLevel();
        UpdateScoreUI();
    }

    private void Update()
    {
        _reset = InputSystem.Instance.GetReset();
    }

    private void FixedUpdate()
    {
        if (_reset != 0)
        {
            _currentLevel++;
            _isLevelInitialized = false;
            Reseting();
        }

        if (Player.Instance._countOfCoins == 240)
        {
            _currentLevel++;
            _isLevelInitialized = false;
            Reseting();
        }
    }

    public void AddScore(int points)
    {
        _currentScore += points;
        UpdateScoreUI();
    }

    private void Reseting()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    private void InitializeLevel()
    {
        if (!_isLevelInitialized)
        {
            FindGhosts();
            FindScore();

            SetterWithCurLevel();
            _isLevelInitialized = true;
        }
    }

    private void FindScore()
    {
        GameObject scoreObj = GameObject.Find("ScoreText");
        if (scoreObj != null) scoreText = scoreObj.GetComponent<TextMeshProUGUI>();
    }

    private void FindGhosts()
    {
        GameObject blinkyObj = GameObject.Find("Blinky");
        GameObject pinkyObj = GameObject.Find("Pinky");
        GameObject inkyObj = GameObject.Find("Inky");
        GameObject clydeObj = GameObject.Find("Clyde");

        if (blinkyObj != null) blinky = blinkyObj.GetComponent<Blinky>();
        if (pinkyObj != null) pinky = pinkyObj.GetComponent<Blinky>();
        if (inkyObj != null) inky = inkyObj.GetComponent<Blinky>();
        if (clydeObj != null) clyde = clydeObj.GetComponent<Blinky>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeLevel();
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

    public int GetCurrentLevel()
    {
        return _currentLevel;
    }

    public void ResetToLevelOne()
    {
        _currentLevel = 1;
        _isLevelInitialized = false;
        Reseting();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"{_currentScore}";
    }

}