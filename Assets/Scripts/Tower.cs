using UnityEngine;
using UnityEngine.SceneManagement;

public class Tower : MonoBehaviour
{
    public const int MaxUpgradeSteps = 2;

    [Tooltip("Ключ для CampaignSettings → towerUpgradePrices (если пусто — имя объекта / префаба без (Clone))")]
    public string campaignTowerId = "";

    [Header("Targeting")]
    public float range = 3f;
    public float fireCooldown = 0.5f;
    public LayerMask enemyLayer;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform shootPoint;

    [Header("Upgrades (fallback if CampaignSettings missing)")]
    [Tooltip("Used only when Resources/CampaignSettings is absent; otherwise prices come from the asset")]
    public int[] upgradeCosts = new int[2] { 30, 50 };

    [Tooltip("Range multiplier per step (0.12 = +12% per upgrade)")]
    [Range(0f, 1f)]
    public float rangeBonusPerStep = 0.12f;

    [Tooltip("Fire cooldown reduction per step (0.1 = -10% CD per upgrade)")]
    [Range(0f, 0.5f)]
    public float fireCooldownReductionPerStep = 0.1f;

    [Tooltip("Projectile damage bonus per step (Projectile / SplashProjectile only)")]
    [Range(0f, 1f)]
    public float projectileDamageBonusPerStep = 0.15f;

    [Header("Visuals")]
    [Tooltip("Не обязателен: добавь компонент Tower Upgrade Visuals на башню и назначь спрайты там")]
    public TowerUpgradeVisuals upgradeVisuals;

    float _lastShotTime;
    int _placedCost;
    int _totalInvested;
    int _upgradeStep;
    TowerSlot _placedSlot;

    float _baseRange;
    float _baseFireCooldown;
    int _baseProjectileDamage;
    bool _projectileDamageApplies;

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

    public int GetSellRefund() => _totalInvested / 2;

    public int GetUpgradeCost()
    {
        if (!CanUpgrade || upgradeCosts == null || _upgradeStep >= upgradeCosts.Length)
            return 0;
        return upgradeCosts[_upgradeStep];
    }

    public bool TryUpgrade()
    {
        if (!CanUpgrade)
            return false;
        int cost = GetUpgradeCost();
        if (cost <= 0 || GameManager.Instance == null || !GameManager.Instance.TrySpendMoney(cost))
            return false;

        _totalInvested += cost;
        _upgradeStep++;
        ApplyUpgradeStats();
        RefreshUpgradeSprite();
        return true;
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

        Destroy(gameObject);
    }

    void CacheCombatBases()
    {
        _baseRange = range;
        _baseFireCooldown = fireCooldown;
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
    }

    void ApplyUpgradeStats()
    {
        range = _baseRange * (1f + rangeBonusPerStep * _upgradeStep);
        float cdMul = 1f - fireCooldownReductionPerStep * _upgradeStep;
        fireCooldown = Mathf.Max(0.05f, _baseFireCooldown * Mathf.Max(0.2f, cdMul));
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
        if (shootPoint == null)
        {
            shootPoint = transform.Find("shootPoint");

            if (shootPoint == null)
            {
                Debug.LogError("[Tower] shootPoint not found. Add child transform 'shootPoint'.");
            }
        }

        RefreshUpgradeSprite();
    }

    void Update()
    {
        TestEnemy target = FindTarget();
        if (target == null) return;

        if (Time.time >= _lastShotTime + fireCooldown)
        {
            Shoot(target);
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

    void Shoot(TestEnemy target)
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
            slow.Init(target.transform);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
