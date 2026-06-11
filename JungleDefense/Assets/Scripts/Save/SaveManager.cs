using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const int CurrentSaveVersion = 2;
    private const string SaveFileName = "save_data.json";
    private const string BackupSaveFileName = "save_data.backup.json";
    private const string TempSaveFileName = "save_data.tmp.json";
    private const float AutoSaveIntervalSeconds = 30f;

    private SaveData saveData;
    private bool hasUnsavedChanges;
    private float nextAutoSaveTime;

    public SaveData Data => saveData;

    private string SaveFilePath =>
        Path.Combine(Application.persistentDataPath, SaveFileName);

    private string BackupSaveFilePath =>
        Path.Combine(Application.persistentDataPath, BackupSaveFileName);

    private string TempSaveFilePath =>
        Path.Combine(Application.persistentDataPath, TempSaveFileName);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    private void Update()
    {
        if (!hasUnsavedChanges || Time.unscaledTime < nextAutoSaveTime)
        {
            return;
        }

        Save();
    }

    public bool IsLevelUnlocked(int levelIndex)
    {
        EnsureData();
        return levelIndex <= saveData.highestUnlockedLevel;
    }

    public int GetStars(int levelIndex)
    {
        EnsureData();
        return saveData.GetStars(levelIndex);
    }

    public int GetHighestUnlockedLevel()
    {
        EnsureData();
        return saveData.highestUnlockedLevel;
    }

    public void CompleteLevel(int levelIndex, int stars = 1)
    {
        EnsureData();

        if (!saveData.completedLevels.Contains(levelIndex))
        {
            saveData.completedLevels.Add(levelIndex);
        }

        saveData.highestUnlockedLevel = Mathf.Max(
            saveData.highestUnlockedLevel,
            levelIndex + 1
        );

        saveData.SetStars(levelIndex, Mathf.Max(saveData.GetStars(levelIndex), stars));

        Save();
    }

    public void RecordEnemyKilled(int reward)
    {
        EnsureData();

        saveData.totalEnemiesKilled++;
        saveData.totalMoneyEarnedFromKills += Mathf.Max(0, reward);

        MarkDirty();
    }

    public void RecordEnemyReachedBase()
    {
        EnsureData();

        saveData.totalEnemiesReachedBase++;

        MarkDirty();
    }

    public void RecordTowerPlaced()
    {
        EnsureData();

        saveData.totalTowersPlaced++;

        MarkDirty();
    }

    public void RecordTowerUpgraded()
    {
        EnsureData();

        saveData.totalTowersUpgraded++;

        MarkDirty();
    }

    public void RecordWaveStarted()
    {
        EnsureData();

        saveData.totalWavesStarted++;

        MarkDirty();
    }

    public void RecordWaveCompleted()
    {
        EnsureData();

        saveData.totalWavesCompleted++;

        MarkDirty();
    }

    public void RecordLevelCompleted()
    {
        EnsureData();

        saveData.totalLevelsCompleted++;

        MarkDirty();
    }

    public void FlushIfDirty()
    {
        if (!hasUnsavedChanges)
        {
            return;
        }

        Save();
    }

    public void ResetProgress()
    {
        saveData = CreateDefaultSaveData();
        hasUnsavedChanges = false;
        Save();
    }

    public void DeleteSaveFiles()
    {
        TryDeleteFile(SaveFilePath);
        TryDeleteFile(BackupSaveFilePath);
        TryDeleteFile(TempSaveFilePath);

        saveData = CreateDefaultSaveData();
        hasUnsavedChanges = false;
        Save();
    }

    public string GetSaveFilePath()
    {
        return SaveFilePath;
    }

    public void Save()
    {
        EnsureData();

        try
        {
            Directory.CreateDirectory(Application.persistentDataPath);

            string json = JsonUtility.ToJson(saveData, true);

            File.WriteAllText(TempSaveFilePath, json);

            if (File.Exists(SaveFilePath))
            {
                File.Copy(SaveFilePath, BackupSaveFilePath, true);
            }

            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
            }

            File.Move(TempSaveFilePath, SaveFilePath);

            File.Copy(SaveFilePath, BackupSaveFilePath, true);

            hasUnsavedChanges = false;
        }
        catch (Exception exception)
        {
            Debug.LogError("Failed to save progress: " + exception.Message);
        }
    }

    private void MarkDirty()
    {
        hasUnsavedChanges = true;

        if (nextAutoSaveTime <= Time.unscaledTime)
        {
            nextAutoSaveTime = Time.unscaledTime + AutoSaveIntervalSeconds;
        }
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            FlushIfDirty();
        }
    }

    private void OnApplicationQuit()
    {
        FlushIfDirty();
    }

    private void Load()
    {
        if (TryLoadFromFile(SaveFilePath))
        {
            return;
        }

        if (TryLoadFromFile(BackupSaveFilePath))
        {
            Save();
            return;
        }

        saveData = CreateDefaultSaveData();
        Save();
    }

    private bool TryLoadFromFile(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return false;
            }

            string json = File.ReadAllText(path);
            SaveData loadedData = JsonUtility.FromJson<SaveData>(json);

            if (loadedData == null)
            {
                return false;
            }

            saveData = loadedData;
            MigrateIfNeeded();
            NormalizeData();

            return true;
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Failed to load save file: " + path + " | " + exception.Message);
            return false;
        }
    }

    private void EnsureData()
    {
        if (saveData == null)
        {
            saveData = CreateDefaultSaveData();
        }

        MigrateIfNeeded();
        NormalizeData();
    }

    private void MigrateIfNeeded()
    {
        if (saveData == null)
        {
            return;
        }

        if (saveData.version <= 0)
        {
            saveData.version = 1;
        }

        if (saveData.version < CurrentSaveVersion)
        {
            saveData.version = CurrentSaveVersion;
        }
    }

    private void NormalizeData()
    {
        if (saveData.completedLevels == null)
        {
            saveData.completedLevels = new List<int>();
        }

        if (saveData.levelStars == null)
        {
            saveData.levelStars = new List<LevelStarsData>();
        }

        saveData.highestUnlockedLevel = Mathf.Max(0, saveData.highestUnlockedLevel);
        saveData.version = CurrentSaveVersion;

        RemoveDuplicateCompletedLevels();
        NormalizeStars();
    }

    private void RemoveDuplicateCompletedLevels()
    {
        HashSet<int> uniqueLevels = new HashSet<int>();
        List<int> normalizedLevels = new List<int>();

        foreach (int levelIndex in saveData.completedLevels)
        {
            if (levelIndex < 0 || uniqueLevels.Contains(levelIndex))
            {
                continue;
            }

            uniqueLevels.Add(levelIndex);
            normalizedLevels.Add(levelIndex);
        }

        saveData.completedLevels = normalizedLevels;
    }

    private void NormalizeStars()
    {
        Dictionary<int, int> bestStarsByLevel = new Dictionary<int, int>();

        foreach (LevelStarsData entry in saveData.levelStars)
        {
            if (entry == null || entry.levelIndex < 0)
            {
                continue;
            }

            int stars = Mathf.Clamp(entry.stars, 0, 3);

            if (!bestStarsByLevel.ContainsKey(entry.levelIndex) ||
                stars > bestStarsByLevel[entry.levelIndex])
            {
                bestStarsByLevel[entry.levelIndex] = stars;
            }
        }

        saveData.levelStars = new List<LevelStarsData>();

        foreach (KeyValuePair<int, int> pair in bestStarsByLevel)
        {
            saveData.levelStars.Add(new LevelStarsData
            {
                levelIndex = pair.Key,
                stars = pair.Value
            });
        }
    }

    private static SaveData CreateDefaultSaveData()
    {
        return new SaveData
        {
            version = CurrentSaveVersion,
            highestUnlockedLevel = 0,
            completedLevels = new List<int>(),
            levelStars = new List<LevelStarsData>()
        };
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Failed to delete save file: " + path + " | " + exception.Message);
        }
    }
}
