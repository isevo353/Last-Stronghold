using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI livesText;
    public Button startWaveButton;
    public TextMeshProUGUI waveText;


    [Header("Gate")]
    public Gate gate;

    [Header("Spawner")]
    public EnemySpawner enemySpawner; // ← добавь ссылку

    private int _currentWave = 0;
    private bool _waveActive = false;

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
        if (startWaveButton != null)
            startWaveButton.onClick.AddListener(OnStartWavePressed);

        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void OnStartWavePressed()
    {
        if (_waveActive)
        {
            Debug.Log("[UIManager] Волна уже начата!");
            return;
        }

        _currentWave++;
        _waveActive = true;
        startWaveButton.interactable = false;
        startWaveButton.gameObject.SetActive(false); // ← кнопка пропадает

        Debug.Log($"[UIManager] Начата волна {_currentWave}!");

        // Запускаем спавнер
        if (enemySpawner != null)
        {
            enemySpawner.StartSpawning();
        }

        // Волна длится 15 секунд, потом можно начать новую
        Invoke(nameof(EndWave), 15f);
    }

    void EndWave()
    {
        _waveActive = false;
        startWaveButton.interactable = true;
        startWaveButton.gameObject.SetActive(true); // ← кнопка появляется снова

        if (enemySpawner != null)
        {
            enemySpawner.StopSpawning();
        }

        Debug.Log($"[UIManager] Волна {_currentWave} закончена!");
    }

    void UpdateUI()
    {
        if (GameManager.Instance == null) return;

        if (moneyText != null)
            moneyText.text = "Монеты: " + GameManager.Instance.GetMoney();

        if (livesText != null)
            livesText.text = "Жизни: " + gate.currentHealth;

        if (waveText != null)
            waveText.text = "Волна: " + _currentWave;
    }

    public bool IsWaveActive()
    {
        return _waveActive;
    }
}