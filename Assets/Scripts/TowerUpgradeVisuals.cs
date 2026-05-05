using UnityEngine;

/// <summary>
/// Спрайты башни по уровню улучшения. Добавь этот компонент на тот же объект, что и Tower (или на ребёнка).
/// </summary>
[AddComponentMenu("Last Stronghold/Tower Upgrade Visuals")]
public class TowerUpgradeVisuals : MonoBehaviour
{
    [Tooltip("Пусто — берётся SpriteRenderer на этом объекте или первый у дочерних")]
    public SpriteRenderer targetRenderer;

    [Tooltip("[0] база, [1] после 1-го улучшения, [2] после 2-го. Пустой слот — используется предыдущий заданный спрайт")]
    public Sprite[] spritesByLevel = new Sprite[3];

    SpriteRenderer _cachedRenderer;

    public void ApplyUpgradeStep(int upgradeStep)
    {
        if (spritesByLevel == null || spritesByLevel.Length == 0)
            return;

        SpriteRenderer sr = ResolveRenderer();
        if (sr == null)
            return;

        int step = Mathf.Clamp(upgradeStep, 0, spritesByLevel.Length - 1);
        for (int i = step; i >= 0; i--)
        {
            if (spritesByLevel[i] != null)
            {
                sr.sprite = spritesByLevel[i];
                return;
            }
        }
    }

    SpriteRenderer ResolveRenderer()
    {
        if (targetRenderer != null)
            return targetRenderer;
        if (_cachedRenderer != null)
            return _cachedRenderer;
        _cachedRenderer = GetComponent<SpriteRenderer>();
        if (_cachedRenderer == null)
            _cachedRenderer = GetComponentInChildren<SpriteRenderer>(true);
        return _cachedRenderer;
    }

#if UNITY_EDITOR
    void Reset()
    {
        if (spritesByLevel == null || spritesByLevel.Length != 3)
            spritesByLevel = new Sprite[3];
    }
#endif
}
