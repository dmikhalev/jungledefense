#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class JungleDefenseBossGenerator
{
    private const string DataEnemiesFolder = "Assets/Data/Enemies";
    private const string PrefabsEnemiesFolder = "Assets/Prefabs/Enemies";
    private const string WavesFolder = "Assets/Data/Waves/Generated";
    private const string LevelsFolder = "Assets/Data/Levels";

    [MenuItem("Jungle Defense/Generate Gorilla Boss")]
    public static void Generate()
    {
        EnsureFolder("Assets/Data");
        EnsureFolder(DataEnemiesFolder);
        EnsureFolder("Assets/Prefabs");
        EnsureFolder(PrefabsEnemiesFolder);
        EnsureFolder("Assets/Data/Waves");
        EnsureFolder(WavesFolder);

        GameObject tankPrefab = FindPrefab("Enemy_Tank");

        if (tankPrefab == null)
        {
            Debug.LogError("Cannot generate Gorilla Boss. Prefab named Enemy_Tank was not found.");
            return;
        }

        EnemyData bossData = CreateOrUpdateBossData();
        GameObject bossPrefab = CreateOrUpdateBossPrefab(tankPrefab, bossData);

        WaveData bossWave = CreateOrUpdateBossWave(bossPrefab);
        AppendBossWaveToLevel("Level_3", bossWave);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Generated Boss_GorillaData, Boss_Gorilla prefab, and Level_3 boss wave.");
    }

    private static EnemyData CreateOrUpdateBossData()
    {
        string path = $"{DataEnemiesFolder}/Boss_GorillaData.asset";

        EnemyData data = AssetDatabase.LoadAssetAtPath<EnemyData>(path);

        if (data == null)
        {
            data = ScriptableObject.CreateInstance<EnemyData>();
            AssetDatabase.CreateAsset(data, path);
        }

        data.enemyName = "Gorilla Boss";
        data.enemyType = EnemyType.Boss;
        data.sprite = FindSprite("gorilla");
        data.maxHealth = 950f;
        data.speed = 0.8f;
        data.reward = 120;
        data.damageToBase = 5;

        data.directDamageMultiplier = 1f;
        data.splashDamageMultiplier = 0.55f;
        data.stunDurationMultiplier = 0.25f;

        EditorUtility.SetDirty(data);

        return data;
    }

    private static GameObject CreateOrUpdateBossPrefab(GameObject sourcePrefab, EnemyData bossData)
    {
        string path = $"{PrefabsEnemiesFolder}/Boss_Gorilla.prefab";

        if (!File.Exists(path))
        {
            PrefabUtility.SaveAsPrefabAsset(sourcePrefab, path);
        }

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        if (prefab == null)
        {
            Debug.LogError("Failed to create/load Boss_Gorilla prefab.");
            return null;
        }

        Enemy enemy = prefab.GetComponent<Enemy>();

        if (enemy == null)
        {
            Debug.LogError("Boss_Gorilla prefab has no Enemy component.");
            return prefab;
        }

        SerializedObject serializedEnemy = new SerializedObject(enemy);

        SerializedProperty dataProperty = serializedEnemy.FindProperty("data");
        if (dataProperty != null)
        {
            dataProperty.objectReferenceValue = bossData;
        }

        serializedEnemy.ApplyModifiedProperties();

        prefab.transform.localScale = Vector3.one * 1.55f;

        EditorUtility.SetDirty(prefab);

        return prefab;
    }

    private static WaveData CreateOrUpdateBossWave(GameObject bossPrefab)
    {
        string path = $"{WavesFolder}/Level_3_Wave_Boss.asset";

        WaveData wave = AssetDatabase.LoadAssetAtPath<WaveData>(path);

        if (wave == null)
        {
            wave = ScriptableObject.CreateInstance<WaveData>();
            AssetDatabase.CreateAsset(wave, path);
        }

        SerializedObject serializedWave = new SerializedObject(wave);
        SerializedProperty groupsProperty = serializedWave.FindProperty("enemyGroups");

        groupsProperty.arraySize = 2;

        SetGroup(groupsProperty.GetArrayElementAtIndex(0), bossPrefab, 1, 1.0f);

        GameObject normalPrefab = FindPrefab("Enemy_Normal");
        if (normalPrefab != null)
        {
            SetGroup(groupsProperty.GetArrayElementAtIndex(1), normalPrefab, 8, 0.65f);
        }

        serializedWave.ApplyModifiedProperties();
        EditorUtility.SetDirty(wave);

        return wave;
    }

    private static void SetGroup(SerializedProperty group, GameObject prefab, int count, float delay)
    {
        group.FindPropertyRelative("enemyPrefab").objectReferenceValue = prefab;
        group.FindPropertyRelative("count").intValue = count;
        group.FindPropertyRelative("delayBetweenEnemies").floatValue = delay;
    }

    private static void AppendBossWaveToLevel(string levelName, WaveData bossWave)
    {
        string path = $"{LevelsFolder}/{levelName}.asset";

        LevelData level = AssetDatabase.LoadAssetAtPath<LevelData>(path);

        if (level == null)
        {
            Debug.LogWarning($"Level asset not found: {path}. Boss wave was created but not assigned to a level.");
            return;
        }

        SerializedObject serializedLevel = new SerializedObject(level);
        SerializedProperty wavesProperty = serializedLevel.FindProperty("waves");

        if (wavesProperty == null)
        {
            Debug.LogError("LevelData does not contain serialized field named 'waves'.");
            return;
        }

        for (int i = 0; i < wavesProperty.arraySize; i++)
        {
            if (wavesProperty.GetArrayElementAtIndex(i).objectReferenceValue == bossWave)
            {
                Debug.Log($"{levelName} already contains boss wave.");
                return;
            }
        }

        int index = wavesProperty.arraySize;
        wavesProperty.arraySize++;
        wavesProperty.GetArrayElementAtIndex(index).objectReferenceValue = bossWave;

        serializedLevel.ApplyModifiedProperties();
        EditorUtility.SetDirty(level);
    }

    private static GameObject FindPrefab(string prefabName)
    {
        string[] guids = AssetDatabase.FindAssets($"{prefabName} t:Prefab");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null && prefab.name == prefabName)
            {
                return prefab;
            }
        }

        return null;
    }

    private static Sprite FindSprite(string namePart)
    {
        string[] guids = AssetDatabase.FindAssets($"{namePart} t:Sprite");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (sprite != null)
            {
                return sprite;
            }
        }

        return null;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            return;
        }

        string parent = Path.GetDirectoryName(path).Replace("\\", "/");
        string folder = Path.GetFileName(path);

        if (!AssetDatabase.IsValidFolder(parent))
        {
            EnsureFolder(parent);
        }

        AssetDatabase.CreateFolder(parent, folder);
    }
}
#endif
