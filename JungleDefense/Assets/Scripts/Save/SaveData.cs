using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int version = 2;
    public int highestUnlockedLevel = 0;
    public List<int> completedLevels = new List<int>();
    public List<LevelStarsData> levelStars = new List<LevelStarsData>();

    public bool IsLevelCompleted(int levelIndex)
    {
        return completedLevels != null && completedLevels.Contains(levelIndex);
    }

    public int GetStars(int levelIndex)
    {
        if (levelStars == null)
        {
            return 0;
        }

        foreach (LevelStarsData entry in levelStars)
        {
            if (entry.levelIndex == levelIndex)
            {
                return entry.stars;
            }
        }

        return 0;
    }

    public void SetStars(int levelIndex, int stars)
    {
        stars = Math.Max(0, Math.Min(3, stars));

        if (levelStars == null)
        {
            levelStars = new List<LevelStarsData>();
        }

        foreach (LevelStarsData entry in levelStars)
        {
            if (entry.levelIndex == levelIndex)
            {
                entry.stars = Math.Max(entry.stars, stars);
                return;
            }
        }

        levelStars.Add(new LevelStarsData
        {
            levelIndex = levelIndex,
            stars = stars
        });
    }
}

[Serializable]
public class LevelStarsData
{
    public int levelIndex;
    public int stars;
}
