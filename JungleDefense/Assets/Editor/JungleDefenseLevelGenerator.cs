#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class JungleDefenseLevelGenerator
{
    private const string LevelsFolder = "Assets/Data/Levels";
    private const string WavesFolder = "Assets/Data/Waves/Generated";

    [MenuItem("Jungle Defense/Generate Clean Levels 1-3")]
    public static void Generate()
    {
        EnsureFolder("Assets/Data");
        EnsureFolder(LevelsFolder);
        EnsureFolder("Assets/Data/Waves");
        EnsureFolder(WavesFolder);

        GameObject fast = FindPrefab("Enemy_Fast");
        GameObject normal = FindPrefab("Enemy_Normal");
        GameObject tank = FindPrefab("Enemy_Tank");

        if (fast == null || normal == null || tank == null)
        {
            Debug.LogError(
                "Cannot generate levels. Missing enemy prefabs. " +
                "Expected prefabs named Enemy_Fast, Enemy_Normal, Enemy_Tank."
            );
            return;
        }

        CreateLevel1(fast, normal, tank);
        CreateLevel2(fast, normal, tank);
        CreateLevel3(fast, normal, tank);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Generated Level_1, Level_2, Level_3 and WaveData assets.");
    }

    private static void CreateLevel1(GameObject fast, GameObject normal, GameObject tank)
    {
        WaveData[] waves =
        {
            CreateWave("Level_1_Wave_01", Group(fast, 6, 0.55f), Group(normal, 3, 0.85f)),
            CreateWave("Level_1_Wave_02", Group(fast, 8, 0.45f), Group(normal, 5, 0.75f)),
            CreateWave("Level_1_Wave_03", Group(normal, 7, 0.65f), Group(fast, 8, 0.38f)),
            CreateWave("Level_1_Wave_04", Group(tank, 2, 1.15f), Group(normal, 8, 0.58f)),
            CreateWave("Level_1_Wave_05", Group(fast, 10, 0.34f), Group(normal, 8, 0.52f), Group(tank, 3, 0.95f)),
        };

        CreateLevel(
            "Level_1",
            7,
            14,
            170,
            new[]
            {
                "PPPPP11",
                "1111P11",
                "11PPP11",
                "11P1111",
                "11PPPP1",
                "11111P1",
                "1PPPPP1",
                "1P11111",
                "1PPPPP1",
                "11111P1",
                "11PPPP1",
                "11P1111",
                "11PPPP1",
                "11111P1",
            },
            waves
        );
    }

    private static void CreateLevel2(GameObject fast, GameObject normal, GameObject tank)
    {
        WaveData[] waves =
        {
            CreateWave("Level_2_Wave_01", Group(fast, 8, 0.48f), Group(normal, 5, 0.72f)),
            CreateWave("Level_2_Wave_02", Group(normal, 8, 0.60f), Group(fast, 10, 0.34f)),
            CreateWave("Level_2_Wave_03", Group(tank, 3, 1.05f), Group(normal, 9, 0.52f)),
            CreateWave("Level_2_Wave_04", Group(fast, 12, 0.28f), Group(normal, 10, 0.45f)),
            CreateWave("Level_2_Wave_05", Group(tank, 4, 0.90f), Group(normal, 10, 0.42f), Group(fast, 10, 0.26f)),
            CreateWave("Level_2_Wave_06", Group(tank, 5, 0.82f), Group(normal, 12, 0.38f), Group(fast, 12, 0.22f)),
        };

        CreateLevel(
            "Level_2",
            7,
            14,
            210,
            new[]
            {
                "P111111",
                "PPPPPP1",
                "11111P1",
                "11111P1",
                "11PPPP1",
                "11P1111",
                "11PPPPP",
                "111111P",
                "1PPPPPP",
                "1P11111",
                "1PPPPP1",
                "11111P1",
                "11111PP",
                "111111P",
            },
            waves
        );
    }

    private static void CreateLevel3(GameObject fast, GameObject normal, GameObject tank)
    {
        WaveData[] waves =
        {
            CreateWave("Level_3_Wave_01", Group(normal, 8, 0.58f), Group(fast, 10, 0.32f)),
            CreateWave("Level_3_Wave_02", Group(tank, 3, 0.95f), Group(fast, 14, 0.24f)),
            CreateWave("Level_3_Wave_03", Group(normal, 12, 0.42f), Group(tank, 4, 0.82f)),
            CreateWave("Level_3_Wave_04", Group(fast, 16, 0.20f), Group(normal, 12, 0.36f), Group(tank, 4, 0.76f)),
            CreateWave("Level_3_Wave_05", Group(tank, 6, 0.70f), Group(normal, 14, 0.32f)),
            CreateWave("Level_3_Wave_06", Group(fast, 18, 0.18f), Group(tank, 7, 0.62f), Group(normal, 14, 0.30f)),
        };

        CreateLevel(
            "Level_3",
            7,
            14,
            230,
            new[]
            {
                "PPPP111",
                "111P111",
                "111PPPP",
                "111111P",
                "1PPPPPP",
                "1P11111",
                "1PPPPP1",
                "11111P1",
                "11111P1",
                "PPPPPP1",
                "P111111",
                "PPPPP11",
                "1111P11",
                "1111PPP",
            },
            waves
        );
    }

    private static WaveEnemyGroup Group(GameObject enemyPrefab, int count, float delay)
    {
        return new WaveEnemyGroup
        {
            enemyPrefab = enemyPrefab,
            count = count,
            delayBetweenEnemies = delay
        };
    }

    private static WaveData CreateWave(string name, params WaveEnemyGroup[] groups)
    {
        string path = $"{WavesFolder}/{name}.asset";

        WaveData wave = AssetDatabase.LoadAssetAtPath<WaveData>(path);
        if (wave == null)
        {
            wave = ScriptableObject.CreateInstance<WaveData>();
            AssetDatabase.CreateAsset(wave, path);
        }

        SerializedObject serializedWave = new SerializedObject(wave);
        SerializedProperty groupsProperty = serializedWave.FindProperty("enemyGroups");

        groupsProperty.arraySize = groups.Length;

        for (int i = 0; i < groups.Length; i++)
        {
            SerializedProperty item = groupsProperty.GetArrayElementAtIndex(i);
            item.FindPropertyRelative("enemyPrefab").objectReferenceValue = groups[i].enemyPrefab;
            item.FindPropertyRelative("count").intValue = groups[i].count;
            item.FindPropertyRelative("delayBetweenEnemies").floatValue = groups[i].delayBetweenEnemies;
        }

        serializedWave.ApplyModifiedProperties();
        EditorUtility.SetDirty(wave);

        return wave;
    }

    private static void CreateLevel(
        string name,
        int width,
        int height,
        int startMoney,
        string[] rows,
        WaveData[] waves)
    {
        string path = $"{LevelsFolder}/{name}.asset";

        LevelData level = AssetDatabase.LoadAssetAtPath<LevelData>(path);
        if (level == null)
        {
            level = ScriptableObject.CreateInstance<LevelData>();
            AssetDatabase.CreateAsset(level, path);
        }

        level.width = width;
        level.height = height;
        level.rows = rows;
        level.startMoney = startMoney;

        SerializedObject serializedLevel = new SerializedObject(level);
        SerializedProperty wavesProperty = serializedLevel.FindProperty("waves");

        wavesProperty.arraySize = waves.Length;

        for (int i = 0; i < waves.Length; i++)
        {
            wavesProperty.GetArrayElementAtIndex(i).objectReferenceValue = waves[i];
        }

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
