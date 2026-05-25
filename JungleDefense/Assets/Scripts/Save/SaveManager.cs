using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SaveFileName = "save_data.json";

    private SaveData saveData;

    public SaveData Data => saveData;

    private string SaveFilePath =>
        Path.Combine(Application.persistentDataPath, SaveFileName);

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

    public void CompleteLevel(int levelIndex, int stars = 1)
    {
        EnsureData();

        if (saveData.completedLevels == null)
        {
            saveData.completedLevels = new System.Collections.Generic.List<int>();
        }

        if (!saveData.completedLevels.Contains(levelIndex))
        {
            saveData.completedLevels.Add(levelIndex);
        }

        saveData.highestUnlockedLevel = Mathf.Max(
            saveData.highestUnlockedLevel,
            levelIndex + 1
        );

        saveData.SetStars(levelIndex, stars);
        Save();
    }

    public void ResetProgress()
    {
        saveData = CreateDefaultSaveData();
        Save();
    }

    public void Save()
    {
        EnsureData();

        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(SaveFilePath, json);
        }
        catch (Exception exception)
        {
            Debug.LogError("Failed to save progress: " + exception.Message);
        }
    }

    private void Load()
    {
        try
        {
            if (!File.Exists(SaveFilePath))
            {
                saveData = CreateDefaultSaveData();
                Save();
                return;
            }

            string json = File.ReadAllText(SaveFilePath);
            saveData = JsonUtility.FromJson<SaveData>(json);

            if (saveData == null)
            {
                saveData = CreateDefaultSaveData();
                Save();
            }
        }
        catch (Exception exception)
        {
            Debug.LogError("Failed to load progress: " + exception.Message);
            saveData = CreateDefaultSaveData();
        }
    }

    private void EnsureData()
    {
        if (saveData == null)
        {
            saveData = CreateDefaultSaveData();
        }
    }

    private static SaveData CreateDefaultSaveData()
    {
        return new SaveData
        {
            version = 1,
            highestUnlockedLevel = 0,
            completedLevels = new System.Collections.Generic.List<int>(),
            levelStars = new System.Collections.Generic.List<LevelStarsData>()
        };
    }
}
