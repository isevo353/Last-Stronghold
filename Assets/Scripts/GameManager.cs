using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public int startMoney = 50;

    [Header("Spawning")]
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public int maxEnemiesOnWave = 10;

    [Header("References")]
    public Gate targetGate;
    public PathManager pathManager;

    // Game state
    private int _currentMoney;
    private float _spawnTimer;
    private int _enemiesSpawned;
    private bool _gameOver = false;

    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        ApplyCampaignBalance();
        _currentMoney = startMoney;
        _enemiesSpawned = 0;
        _gameOver = false;

        if (pathManager == null)
            pathManager = FindObjectOfType<PathManager>();

        if (targetGate == null)
            targetGate = FindObjectOfType<Gate>();

       
    }

    void ApplyCampaignBalance()
    {
        CampaignSettings settings = Resources.Load<CampaignSettings>("CampaignSettings");
        if (settings == null) return;

        var data = settings.GetBalanceForScene(SceneManager.GetActiveScene().name);
        startMoney = Mathf.Max(0, data.startMoney);
    }

    void Update()
    {
        if (_gameOver) return;


        if (UIManager.Instance != null && !UIManager.Instance.IsWaveActive())
            return;

       
        if (targetGate == null || targetGate.currentHealth <= 0)
        {
            GameOver();
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("[GameManager] Enemy prefab ");
            return;
        }

        GameObject go = Instantiate(enemyPrefab);
        TestEnemy enemy = go.GetComponent<TestEnemy>();
        if (enemy != null)
        {
            _enemiesSpawned++;
            Debug.Log($"[GameManager]  {_enemiesSpawned}");
        }
    }

   
    public bool TrySpendMoney(int amount)
    {
        if (_currentMoney < amount)
        {
            Debug.LogWarning($"[GameManager]  {_currentMoney},  {amount}");
            return false;
        }

        _currentMoney -= amount;
        Debug.Log($"[GameManager]  {amount}  {_currentMoney}");
        return true;
    }

    
    public void AddMoney(int amount)
    {
        _currentMoney += amount;
        Debug.Log($"[GameManager] +{amount}  {_currentMoney}");
    }

    
    public int GetMoney()
    {
        return _currentMoney;
    }

   
    void GameOver()
    {
        _gameOver = true;
        Debug.Log("[GameManager] GAME OVER!");
    }

   
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}