using UnityEngine;
using UnityEngine.SceneManagement;

public class Tower : MonoBehaviour
{
    public const int MaxUpgradeSteps = 2;
    public enum AttackType
    {
        Projectile,
        Beam
    }

    [Tooltip("Ключ для CampaignSettings → towerUpgradePrices (если пусто — имя объекта / префаба без (Clone))")]
    public string campaignTowerId = "";

    [Header("Targeting")]
    public float range = 3f;
    public float fireCooldown = 0.5f;
    public LayerMask enemyLayer;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform shootPoint;

    [Header("Attack Mode")]
    public AttackType attackType = AttackType.Projectile;

    [Header("Beam Settings")]
    [Tooltip("Урон в секунду в начале фокуса")]
    public float beamBaseDps = 12f;
    [Tooltip("Насколько быстро растет множитель урона во время непрерывного фокуса")]
    public float beamRampPerSecond = 0.45f;
    [Tooltip("Максимальный множитель урона луча")]
    public float beamMaxMultiplier = 3f;
    [Tooltip("Линия для визуала луча (LineRenderer)")]
    public LineRenderer beamLine;

    [Header("Upgrades (fallback if CampaignSettings missing)")]
    [Tooltip("Used only when Resources/CampaignSettings is absent; otherwise prices come from the asset")]
    public int[] upgradeCosts = new int[2] { 30, 50 };

    [Tooltip("Range multiplier per step (0.12 = +12% per upgrade)")]
    [Range(0f, 1f)]
    public float rangeBonusPerStep = 0.12f;

    [Tooltip("Fire cooldown reduction per step (0.1 = -10% CD per upgrade)")]
    [Range(0f, 0.5f)]
    public float fireCooldownReductionPerStep = 0.1f;

    [Tooltip("Бонус урона за шаг улучшения (снаряды и базовый DPS луча)")]
    [Range(0f, 1f)]
    public float projectileDamageBonusPerStep = 0.15f;

    [Header("Visuals")]
    [Tooltip("Не обязателен: добавь компонент Tower Upgrade Visuals на башню и назначь спрайты там")]
    public TowerUpgradeVisuals upgradeVisuals;

    [Header("Builder House Rules")]
    [Tooltip("Отметь для домика строителя. Его уровень открывает улучшения обычных башен.")]
    public bool isBuilderHouse;
    [Tooltip("Для обычной башни: минимальный уровень домика для 1-го улучшения башни.")]
    public int requiredBuilderHouseLevelForUpgrade1 = 1;
    [Tooltip("Для обычной башни: минимальный уровень домика для 2-го улучшения башни.")]
    public int requiredBuilderHouseLevelForUpgrade2 = 2;

    float _lastShotTime;
    int _placedCost;
    int _totalInvested;
    int _upgradeStep;
    TowerSlot _placedSlot;

    float _baseRange;
    float _baseFireCooldown;
    int _baseProjectileDamage;
    bool _projectileDamageApplies;
    float _baseBeamDps;
    bool _combatBasesCached;

    TestEnemy _beamTarget;
    float _beamFocusTime;
    float _beamDamageRemainder;

    void Awake()
    {
        if (upgradeVisuals == null)
            upgradeVisuals = GetComponent<TowerUpgradeVisuals>() ?? GetComponentInChildren<TowerUpgradeVisuals>(true);
    }

    public void SetPlacedInfo(TowerSlot slot, int cost)
    {
        _placedSlot = slot;
        _placedCost = cost;
        _totalInvested = cost;
        _upgradeStep = 0;
        ApplyUpgradeCostsFromCampaign();
        CacheCombatBases();
        ApplyUpgradeStats();
        RefreshUpgradeSprite();
        NotifyBuilderHouseLevelChanged();
    }

    void ApplyUpgradeCostsFromCampaign()
    {
        CampaignSettings settings = Resources.Load<CampaignSettings>("CampaignSettings");
        if (settings != null)
        {
            string scene = SceneManager.GetActiveScene().name;
            int[] c = settings.GetUpgradeCostsForSceneTower(scene, GetCampaignTowerKey());
            if (c != null && c.Length >= MaxUpgradeSteps)
            {
                upgradeCosts = new int[MaxUpgradeSteps];
                for (int i = 0; i < MaxUpgradeSteps; i++)
                    upgradeCosts[i] = c[i];
            }
        }
    }

    string GetCampaignTowerKey()
    {
        if (!string.IsNullOrWhiteSpace(campaignTowerId))
            return campaignTowerId.Trim();

        string n = gameObject.name;
        if (n.EndsWith("(Clone)"))
            n = n.Substring(0, n.Length - 7).Trim();
        return n;
    }

    public int PlacedCost => _placedCost;
    public TowerSlot PlacedSlot => _placedSlot;

    public int UpgradeStep => _upgradeStep;
    public bool CanUpgrade => _upgradeStep < MaxUpgradeSteps;
    public bool IsBuilderHouse => isBuilderHouse;

    public int GetSellRefund() => _totalInvested / 2;

    public int GetUpgradeCost()
    {
        if (!CanUpgrade || upgradeCosts == null || _upgradeStep >= upgradeCosts.Length)
            return 0;
        return upgradeCosts[_upgradeStep];
    }

    public bool TryUpgrade()
    {
        EnsureCombatBasesCached();

        if (!CanUpgrade)
            return false;
        if (!CanUpgradeUnlocked(out _))
            return false;
        int cost = GetUpgradeCost();
        if (cost <= 0 || GameManager.Instance == null || !GameManager.Instance.TrySpendMoney(cost))
            return false;

        _totalInvested += cost;
        _upgradeStep++;
        ApplyUpgradeStats();
        RefreshUpgradeSprite();
        NotifyBuilderHouseLevelChanged();
        return true;
    }

    public bool CanUpgradeUnlocked(out string blockedReason)
    {
        blockedReason = "";
        if (!CanUpgrade)
        {
            blockedReason = "Макс. уровень";
            return false;
        }

        if (isBuilderHouse)
            return true;

        int requiredLevel = GetRequiredBuilderHouseLevelForNextUpgrade();
        if (requiredLevel <= 0)
            return true;

        int houseLevel = BuilderHouseProgress.GetCurrentLevel();
        if (houseLevel >= requiredLevel)
            return true;

        blockedReason = requiredLevel <= 1
            ? "Нужен улучшенный домик строителя"
            : $"Нужен домик строителя ур. {requiredLevel}";
        return false;
    }

    void RefreshUpgradeSprite()
    {
        if (upgradeVisuals == null)
            upgradeVisuals = GetComponent<TowerUpgradeVisuals>() ?? GetComponentInChildren<TowerUpgradeVisuals>(true);
        if (upgradeVisuals != null)
            upgradeVisuals.ApplyUpgradeStep(_upgradeStep);
    }

    public void SellAndRemove()
    {
        int refund = GetSellRefund();
        if (GameManager.Instance != null)
            GameManager.Instance.AddMoney(refund);

        if (_placedSlot != null)
            _placedSlot.FreeSlot();

        if (isBuilderHouse)
            BuilderHouseProgress.UnregisterHouse(this);

        Destroy(gameObject);
    }

    /// <summary>
    /// Вызывается врагом-саботажником: снимает 1 апгрейд, а если апгрейдов нет — уничтожает башню без возврата денег.
    /// </summary>
    public void DowngradeOrDestroyByEnemy()
    {
        if (_upgradeStep > 0)
        {
            _upgradeStep--;

            if (upgradeCosts != null && _upgradeStep >= 0 && _upgradeStep < upgradeCosts.Length)
                _totalInvested = Mathf.Max(_placedCost, _totalInvested - upgradeCosts[_upgradeStep]);

            ApplyUpgradeStats();
            RefreshUpgradeSprite();
            return;
        }

        if (_placedSlot != null)
            _placedSlot.FreeSlot();

        if (isBuilderHouse)
            BuilderHouseProgress.UnregisterHouse(this);

        Destroy(gameObject);
    }

    void CacheCombatBases()
    {
        _baseRange = range;
        _baseFireCooldown = fireCooldown;
        _baseBeamDps = beamBaseDps;
        _baseProjectileDamage = 0;
        _projectileDamageApplies = false;
        if (projectilePrefab == null)
            return;

        var direct = projectilePrefab.GetComponent<Projectile>();
        if (direct != null)
        {
            _baseProjectileDamage = direct.damage;
            _projectileDamageApplies = true;
            return;
        }

        var splash = projectilePrefab.GetComponent<SplashProjectile>();
        if (splash != null)
        {
            _baseProjectileDamage = splash.damage;
            _projectileDamageApplies = true;
        }

        _combatBasesCached = true;
    }

    void EnsureCombatBasesCached()
    {
        if (!_combatBasesCached)
            CacheCombatBases();
    }

    void ApplyUpgradeStats()
    {
        range = _baseRange * (1f + rangeBonusPerStep * _upgradeStep);
        float cdMul = 1f - fireCooldownReductionPerStep * _upgradeStep;
        fireCooldown = Mathf.Max(0.05f, _baseFireCooldown * Mathf.Max(0.2f, cdMul));
        beamBaseDps = Mathf.Max(0.1f, _baseBeamDps * (1f + projectileDamageBonusPerStep * _upgradeStep));
    }

    int GetCurrentProjectileDamage()
    {
        if (!_projectileDamageApplies)
            return -1;
        float mul = 1f + projectileDamageBonusPerStep * _upgradeStep;
        return Mathf.Max(1, Mathf.RoundToInt(_baseProjectileDamage * mul));
    }

    void Start()
    {
        EnsureCombatBasesCached();

        if (shootPoint == null)
        {
            shootPoint = transform.Find("shootPoint");

            if (shootPoint == null)
            {
                Debug.LogError("[Tower] shootPoint not found. Add child transform 'shootPoint'.");
            }
        }

        if (beamLine != null)
            beamLine.enabled = false;

        RefreshUpgradeSprite();
    }

    void Update()
    {
        EnsureCombatBasesCached();

        if (attackType == AttackType.Beam)
        {
            UpdateBeamAttack();
            return;
        }

        TestEnemy target = FindTarget();
        if (target == null)
            return;

        if (Time.time >= _lastShotTime + fireCooldown)
        {
            ShootProjectile(target);
            _lastShotTime = Time.time;
        }
    }

    TestEnemy FindTarget()
    {
        TestEnemy[] allEnemies = FindObjectsOfType<TestEnemy>();

        float bestDist = Mathf.Infinity;
        TestEnemy best = null;

        foreach (TestEnemy enemy in allEnemies)
        {
            float d = Vector2.Distance(transform.position, enemy.transform.position);

            if (d <= range && d < bestDist)
            {
                bestDist = d;
                best = enemy;
            }
        }

        return best;
    }

    void ShootProjectile(TestEnemy target)
    {
        if (projectilePrefab == null || shootPoint == null) return;

        int dmg = GetCurrentProjectileDamage();

        GameObject go = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Projectile proj = go.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.Init(target.transform, dmg);
            return;
        }

        SplashProjectile splash = go.GetComponent<SplashProjectile>();
        if (splash != null)
        {
            splash.Init(target.transform, dmg);
            return;
        }

        SlowProjectile slow = go.GetComponent<SlowProjectile>();
        if (slow != null)
        {
            slow.Init(target.transform);
            return;
        }

        PoisonStopProjectile poisonStop = go.GetComponent<PoisonStopProjectile>();
        if (poisonStop != null)
            poisonStop.Init(target.transform);
    }

    void UpdateBeamAttack()
    {
        TestEnemy target = FindTarget();
        if (target == null)
        {
            ResetBeamFocus();
            return;
        }

        if (_beamTarget != target)
        {
            _beamTarget = target;
            _beamFocusTime = 0f;
            _beamDamageRemainder = 0f;
        }
        else
        {
            _beamFocusTime += Time.deltaTime;
        }

        float multiplier = Mathf.Min(beamMaxMultiplier, 1f + _beamFocusTime * beamRampPerSecond);
        float damageThisFrame = beamBaseDps * multiplier * Time.deltaTime + _beamDamageRemainder;
        int wholeDamage = Mathf.FloorToInt(damageThisFrame);
        _beamDamageRemainder = damageThisFrame - wholeDamage;
        if (wholeDamage > 0 && _beamTarget != null)
            _beamTarget.TakeDamage(wholeDamage);

        UpdateBeamVisual(_beamTarget);
    }

    void ResetBeamFocus()
    {
        _beamTarget = null;
        _beamFocusTime = 0f;
        _beamDamageRemainder = 0f;
        if (beamLine != null)
            beamLine.enabled = false;
    }

    void UpdateBeamVisual(TestEnemy target)
    {
        if (beamLine == null || shootPoint == null || target == null)
            return;

        beamLine.enabled = true;
        beamLine.positionCount = 2;
        beamLine.SetPosition(0, shootPoint.position);
        beamLine.SetPosition(1, target.transform.position);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    void OnDisable()
    {
        if (isBuilderHouse)
            BuilderHouseProgress.UnregisterHouse(this);

        if (beamLine != null)
            beamLine.enabled = false;
    }

    int GetRequiredBuilderHouseLevelForNextUpgrade()
    {
        if (_upgradeStep <= 0)
            return Mathf.Max(0, requiredBuilderHouseLevelForUpgrade1);
        return Mathf.Max(0, requiredBuilderHouseLevelForUpgrade2);
    }

    void NotifyBuilderHouseLevelChanged()
    {
        if (isBuilderHouse)
            BuilderHouseProgress.RegisterOrUpdateHouse(this, _upgradeStep);
    }
}
