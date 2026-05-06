using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TestEnemy))]
public class TeleportSkeleton : MonoBehaviour
{
    [Header("Charge")]
    public float chargeDuration = 10f;
    public Color chargedColor = Color.white;

    [Header("Stats")]
    public int health = 350;

    private TestEnemy _enemy;
    private SpriteRenderer _spriteRenderer;
    private Color _startColor;

    void Awake()
    {
        _enemy = GetComponent<TestEnemy>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_enemy != null)
        {
            _enemy.maxHealth = health;
            _enemy.currentHealth = health;
            _enemy.RefreshHealthBar();
            _enemy.SetMovementEnabled(false);
        }

        if (_spriteRenderer != null)
            _startColor = _spriteRenderer.color;
    }

    void Start()
    {
        StartCoroutine(ChargeAndTeleport());
    }

    IEnumerator ChargeAndTeleport()
    {
        float elapsed = 0f;
        while (elapsed < chargeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / chargeDuration);

            if (_spriteRenderer != null)
                _spriteRenderer.color = Color.Lerp(_startColor, chargedColor, t);

            yield return null;
        }

        if (_enemy != null)
        {
            int waypointCount = _enemy.GetWaypointCount();
            if (waypointCount >= 2)
            {
                int secondLastIndex = waypointCount - 2;
                _enemy.SetPathProgress(secondLastIndex, true);
            }

            _enemy.SetMovementEnabled(true);
        }
    }
}
