using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CampaignSettings", menuName = "LastStronghold/Campaign Settings")]
public class CampaignSettings : ScriptableObject
{
    [Serializable]
    public class LevelSettings
    {
        public string sceneName;
        public int maxWavesToWin = 10;
        public string victoryTitle = "ПОБЕДА";
        public string victorySubtitle = "Вы прошли уровень!";
    }

    public int defaultMaxWavesToWin = 10;
    public string defaultVictoryTitle = "ПОБЕДА";
    public string defaultVictorySubtitle = "Вы прошли 10 волн!";
    public LevelSettings[] levelSettings;

    public LevelSettings GetSettingsForScene(string sceneName)
    {
        if (levelSettings == null) return null;

        for (int i = 0; i < levelSettings.Length; i++)
        {
            LevelSettings settings = levelSettings[i];
            if (settings != null && settings.sceneName == sceneName)
            {
                return settings;
            }
        }

        return null;
    }
}
