using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public int startMoney = 50;
    public int startLives = 10;

    [Header("Spawning")]
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public int maxEnemiesOnWave = 10;

    [Header("References")]
    public Gate targetGate;
    public PathManager pathManager;

    // Game state
    private int _currentMoney;
    private int _currentLives;
    private float _spawnTimer;
    private int _enemiesSpawned;
    private bool _gameOver = false;

    void Awake()
    {
        // Синглтон
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        _currentMoney = startMoney;
        _currentLives = startLives;
        _enemiesSpawned = 0;
        _gameOver = false;

        if (pathManager == null)
            pathManager = FindObjectOfType<PathManager>();

        if (targetGate == null)
            targetGate = FindObjectOfType<Gate>();

        Debug.Log($"[GameManager] Игра начата! Деньги: {_currentMoney}, Жизни: {_currentLives}");

        Debug.Log($"[GameManager] Подсказка: Кликай на круги чтобы ставить башни (50 монет)");
    }

    void Update()
    {
        if (_gameOver) return;


        if (UIManager.Instance != null && !UIManager.Instance.IsWaveActive())
            return;

        // Проверка проигрыша (ворота уничтожены)
        if (targetGate == null || targetGate.currentHealth <= 0)
        {
            GameOver();
        }

        // Проверка проигрыша (жизни закончились)
        if (_currentLives <= 0)
        {
            GameOver();
        }


    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("[GameManager] Enemy prefab не назначен!");
            return;
        }

        GameObject go = Instantiate(enemyPrefab);
        TestEnemy enemy = go.GetComponent<TestEnemy>();
        if (enemy != null)
        {
            _enemiesSpawned++;
            Debug.Log($"[GameManager] Враг спаунен! Всего спаунено: {_enemiesSpawned}");
        }
    }

    /// <summary>
    /// Попытка потратить деньги (при постройке башни)
    /// </summary>
    public bool TrySpendMoney(int amount)
    {
        if (_currentMoney < amount)
        {
            Debug.LogWarning($"[GameManager] Недостаточно денег! Есть: {_currentMoney}, нужно: {amount}");
            return false;
        }

        _currentMoney -= amount;
        Debug.Log($"[GameManager] Потрачено {amount} денег. Осталось: {_currentMoney}");
        return true;
    }

    /// <summary>
    /// Добавить деньги (при убийстве врага)
    /// </summary>
    public void AddMoney(int amount)
    {
        _currentMoney += amount;
        Debug.Log($"[GameManager] +{amount} денег! Всего: {_currentMoney}");
    }

    /// <summary>
    /// Потерять жизнь (враг дошёл до ворот)
    /// </summary>
    public void TakeLives(int amount)
    {
        _currentLives -= amount;
        Debug.Log($"[GameManager] -{amount} жизней! Осталось: {_currentLives}");

        if (_currentLives <= 0)
        {
            GameOver();
        }
    }

    /// <summary>
    /// Получить текущее количество денег
    /// </summary>
    public int GetMoney()
    {
        return _currentMoney;
    }

    /// <summary>
    /// Получить текущее количество жизней
    /// </summary>
    public int GetLives()
    {
        return _currentLives;
    }

    /// <summary>
    /// Завершить игру (поражение)
    /// </summary>
    void GameOver()
    {
        _gameOver = true;
        Debug.Log("[GameManager] GAME OVER!");
        // Здесь можно показать UI экран поражения
        // или перезагрузить сцену через пару секунд
        Invoke(nameof(RestartLevel), 2f);
    }

    /// <summary>
    /// Перезагрузить уровень
    /// </summary>
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Выход в главное меню (если есть)
    /// </summary>
    public void BackToMenu()
    {
        SceneManager.LoadScene(0); // или имя сцены меню
    }

    /// <summary>
    /// Выход из игры
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}