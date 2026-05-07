using System.Collections.Generic;
using UnityEngine;

public static class BuilderHouseProgress
{
    static readonly Dictionary<Tower, int> Houses = new Dictionary<Tower, int>();

    public static void RegisterOrUpdateHouse(Tower tower, int level)
    {
        if (tower == null)
            return;

        if (!tower.IsBuilderHouse)
            return;

        Houses[tower] = Mathf.Max(0, level);
        CleanupNullKeys();
    }

    public static void UnregisterHouse(Tower tower)
    {
        if (tower == null)
            return;
        Houses.Remove(tower);
    }

    public static int GetCurrentLevel()
    {
        CleanupNullKeys();
        int maxLevel = 0;
        foreach (var kvp in Houses)
        {
            if (kvp.Value > maxLevel)
                maxLevel = kvp.Value;
        }
        return maxLevel;
    }

    static void CleanupNullKeys()
    {
        if (Houses.Count == 0)
            return;

        List<Tower> toRemove = null;
        foreach (var kvp in Houses)
        {
            if (kvp.Key == null)
            {
                if (toRemove == null)
                    toRemove = new List<Tower>();
                toRemove.Add(kvp.Key);
            }
        }

        if (toRemove == null)
            return;

        for (int i = 0; i < toRemove.Count; i++)
            Houses.Remove(toRemove[i]);
    }
}
