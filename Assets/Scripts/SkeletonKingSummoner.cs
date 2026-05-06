using UnityEngine;

[RequireComponent(typeof(TestEnemy))]
public class SkeletonKingSummoner : MonoBehaviour
{
    [Header("Summon")]
    public GameObject weakSkeletonPrefab;
    public float summonInterval = 5f;
    public float summonDistance = 0.8f;

    [Header("King Stats")]
    public int health = 1200;

    private TestEnemy _enemy;
    private float _nextSummonTime;
    private Vector3 _lastPosition;

    void Awake()
    {
        _enemy = GetComponent<TestEnemy>();
        if (_enemy != null)
        {
            _enemy.maxHealth = health;
            _enemy.currentHealth = health;
        }
    }

    void Start()
    {
        _lastPosition = transform.position;
        _nextSummonTime = Time.time + summonInterval;
    }

    void Update()
    {
        if (weakSkeletonPrefab == null) return;
        if (Time.time < _nextSummonTime) return;

        Vector3 currentPosition = transform.position;
        Vector2 direction = (currentPosition - _lastPosition);
        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector2.right;
        direction.Normalize();

        Instantiate(weakSkeletonPrefab, currentPosition + (Vector3)(direction * summonDistance), Quaternion.identity);
        Instantiate(weakSkeletonPrefab, currentPosition - (Vector3)(direction * summonDistance), Quaternion.identity);

        _nextSummonTime = Time.time + summonInterval;
        _lastPosition = currentPosition;
    }

    void LateUpdate()
    {
        _lastPosition = transform.position;
    }
}
