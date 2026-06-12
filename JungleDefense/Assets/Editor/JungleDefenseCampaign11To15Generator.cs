#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class JungleDefenseCampaign11To15Generator
{
    private const string DataFolder = "Assets/Data";
    private const string LevelsFolder = "Assets/Data/Levels";
    private const string WavesFolder = "Assets/Data/Waves/Generated";
    private const string EnemiesFolder = "Assets/Data/Enemies";
    private const string PrefabsFolder = "Assets/Prefabs/Enemies";

    [MenuItem("Jungle Defense/Generate Campaign Levels 11-15")]
    public static void Generate()
    {
        EnsureFolder(DataFolder);
        EnsureFolder(LevelsFolder);
        EnsureFolder("Assets/Data/Waves");
        EnsureFolder(WavesFolder);
        EnsureFolder(EnemiesFolder);
        EnsureFolder("Assets/Prefabs");
        EnsureFolder(PrefabsFolder);

        GameObject fast = FindPrefabByName("Enemy_Fast");
        GameObject normal = FindPrefabByName("Enemy_Normal");
        GameObject tank = FindPrefabByName("Enemy_Tank");

        if (fast == null || normal == null || tank == null)
        {
            Debug.LogError("Cannot generate levels 11-15. Expected prefabs named Enemy_Fast, Enemy_Normal, Enemy_Tank.");
            return;
        }

        ReduceExistingBossRewards();

        EnemyData rhinoData = CreateOrUpdateRhinoData();
        GameObject rhinoPrefab = CreateOrUpdateRhinoPrefab(tank, rhinoData);

        CreateLevel11(fast, normal, tank);
        CreateLevel12(fast, normal, tank);
        CreateLevel13(fast, normal, tank);
        CreateLevel14(fast, normal, tank);
        CreateLevel15(fast, normal, tank, rhinoPrefab);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Generated levels 11-15 and Rhino King boss. Add Level_11-Level_15 to LevelManager Levels array.");
    }

    private static void ReduceExistingBossRewards()
    {
        SetEnemyReward("Boss_GorillaData", 25);
        SetEnemyReward("Boss_TurtleData", 30);
    }

    private static void SetEnemyReward(string assetName, int reward)
    {
        string[] guids = AssetDatabase.FindAssets($"{assetName} t:EnemyData");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            EnemyData data = AssetDatabase.LoadAssetAtPath<EnemyData>(path);

            if (data != null && data.name == assetName)
            {
                data.reward = reward;
                EditorUtility.SetDirty(data);
                return;
            }
        }
    }

    private static EnemyData CreateOrUpdateRhinoData()
    {
        string path = $"{EnemiesFolder}/Boss_RhinoData.asset";
        EnemyData data = AssetDatabase.LoadAssetAtPath<EnemyData>(path);

        if (data == null)
        {
            data = ScriptableObject.CreateInstance<EnemyData>();
            AssetDatabase.CreateAsset(data, path);
        }

        data.enemyName = "Rhino King";
        data.enemyType = EnemyType.Boss;
        data.sprite = FindSprite("rhino");
        data.maxHealth = 2600f;
        data.speed = 0.65f;
        data.reward = 35;
        data.damageToBase = 999;
        data.directDamageMultiplier = 1.05f;
        data.splashDamageMultiplier = 0.55f;
        data.stunDurationMultiplier = 0.25f;
        data.bossAbility = BossAbilityType.None;
        data.rageHealthThresholds = new[] { 0.75f, 0.5f, 0.25f };
        data.rageSpeedMultiplier = 1.2f;
        data.regenerationInterval = 5f;
        data.regenerationPercentOfMaxHealth = 0.03f;

        EditorUtility.SetDirty(data);
        return data;
    }

    private static GameObject CreateOrUpdateRhinoPrefab(GameObject sourcePrefab, EnemyData rhinoData)
    {
        string path = $"{PrefabsFolder}/Boss_Rhino.prefab";

        if (!File.Exists(path))
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(sourcePrefab);
            PrefabUtility.SaveAsPrefabAsset(instance, path);
            Object.DestroyImmediate(instance);
        }

        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);

        Enemy enemy = prefabRoot.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("Boss_Rhino prefab has no Enemy component.");
        }
        else
        {
            SerializedObject serializedEnemy = new SerializedObject(enemy);
            SerializedProperty dataProperty = serializedEnemy.FindProperty("data");

            if (dataProperty != null)
            {
                dataProperty.objectReferenceValue = rhinoData;
            }
            else
            {
                Debug.LogError("Enemy component does not contain serialized field named 'data'.");
            }

            serializedEnemy.ApplyModifiedProperties();
        }

        prefabRoot.transform.localScale = Vector3.one * 1.65f;

        SpriteRenderer spriteRenderer = prefabRoot.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Sprite rhinoSprite = FindSprite("rhino");
            if (rhinoSprite != null)
            {
                spriteRenderer.sprite = rhinoSprite;
                spriteRenderer.color = Color.white;
            }
            else
            {
                spriteRenderer.color = new Color(0.72f, 0.72f, 0.68f, 1f);
            }
        }

        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        PrefabUtility.UnloadPrefabContents(prefabRoot);

        return AssetDatabase.LoadAssetAtPath<GameObject>(path);
    }

    private static void CreateLevel11(GameObject fast, GameObject normal, GameObject tank)
    {
        WaveData[] waves =
        {
            CreateWave("Level_11_Wave_01", Group(normal, 10, 0.72f), Group(fast, 8, 0.55f)),
            CreateWave("Level_11_Wave_02", Group(tank, 6, 1.20f), Group(normal, 8, 0.72f)),
            CreateWave("Level_11_Wave_03", Group(fast, 14, 0.50f), Group(tank, 5, 1.15f)),
            CreateWave("Level_11_Wave_04", Group(normal, 12, 0.68f), Group(tank, 8, 1.08f)),
            CreateWave("Level_11_Wave_05", Group(fast, 12, 0.48f), Group(normal, 10, 0.66f), Group(tank, 7, 1.05f)),
            CreateWave("Level_11_Wave_06", Group(tank, 10, 1.00f), Group(normal, 10, 0.62f)),
            CreateWave("Level_11_Wave_07", Group(fast, 14, 0.46f), Group(tank, 11, 0.96f)),
            CreateWave("Level_11_Wave_08", Group(normal, 12, 0.58f), Group(tank, 12, 0.92f), Group(fast, 10, 0.45f)),
        };

        CreateLevel("Level_11", 285, new[]
        {
            "PPPPP11",
            "1111P11",
            "1111PPP",
            "111111P",
            "1PPPPPP",
            "1P11111",
            "1PPPPP1",
            "11111P1",
            "PPPPPP1",
            "P111111",
            "PPPPP11",
            "1111P11",
            "1111PPP",
        }, waves);
    }

    private static void CreateLevel12(GameObject fast, GameObject normal, GameObject tank)
    {
        WaveData[] waves =
        {
            CreateWave("Level_12_Wave_01", Group(tank, 7, 1.18f), Group(fast, 10, 0.52f)),
            CreateWave("Level_12_Wave_02", Group(normal, 14, 0.68f), Group(tank, 6, 1.12f)),
            CreateWave("Level_12_Wave_03", Group(fast, 16, 0.48f), Group(tank, 7, 1.08f)),
            CreateWave("Level_12_Wave_04", Group(tank, 10, 1.04f)),
            CreateWave("Level_12_Wave_05", Group(normal, 12, 0.62f), Group(fast, 12, 0.46f), Group(tank, 8, 1.00f)),
            CreateWave("Level_12_Wave_06", Group(tank, 12, 0.96f), Group(normal, 10, 0.60f)),
            CreateWave("Level_12_Wave_07", Group(fast, 16, 0.42f), Group(tank, 11, 0.94f)),
            CreateWave("Level_12_Wave_08", Group(normal, 14, 0.56f), Group(tank, 13, 0.90f)),
            CreateWave("Level_12_Wave_09", Group(fast, 12, 0.40f), Group(normal, 12, 0.55f), Group(tank, 14, 0.86f)),
        };

        CreateLevel("Level_12", 295, new[]
        {
            "P111111",
            "PPPPPP1",
            "11111P1",
            "11111PP",
            "111111P",
            "PPPPPPP",
            "P111111",
            "PPPPP11",
            "1111P11",
            "1111PPP",
            "111111P",
            "11PPPPP",
            "11P1111",
        }, waves);
    }

    private static void CreateLevel13(GameObject fast, GameObject normal, GameObject tank)
    {
        WaveData[] waves =
        {
            CreateWave("Level_13_Wave_01", Group(fast, 12, 0.50f), Group(normal, 10, 0.66f), Group(tank, 6, 1.10f)),
            CreateWave("Level_13_Wave_02", Group(tank, 9, 1.04f), Group(normal, 12, 0.62f)),
            CreateWave("Level_13_Wave_03", Group(fast, 18, 0.44f)),
            CreateWave("Level_13_Wave_04", Group(tank, 12, 0.98f)),
            CreateWave("Level_13_Wave_05", Group(normal, 12, 0.58f), Group(tank, 10, 0.94f)),
            CreateWave("Level_13_Wave_06", Group(fast, 16, 0.40f), Group(tank, 12, 0.90f)),
            CreateWave("Level_13_Wave_07", Group(normal, 14, 0.54f), Group(tank, 14, 0.86f)),
            CreateWave("Level_13_Wave_08", Group(fast, 14, 0.38f), Group(normal, 14, 0.52f), Group(tank, 14, 0.84f)),
            CreateWave("Level_13_Wave_09", Group(tank, 17, 0.80f), Group(fast, 12, 0.36f)),
        };

        CreateLevel("Level_13", 305, new[]
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
            "1111PPP",
        }, waves);
    }

    private static void CreateLevel14(GameObject fast, GameObject normal, GameObject tank)
    {
        WaveData[] waves =
        {
            CreateWave("Level_14_Wave_01", Group(tank, 9, 1.08f), Group(fast, 10, 0.48f)),
            CreateWave("Level_14_Wave_02", Group(normal, 16, 0.60f), Group(tank, 8, 1.02f)),
            CreateWave("Level_14_Wave_03", Group(tank, 12, 0.96f)),
            CreateWave("Level_14_Wave_04", Group(fast, 16, 0.40f), Group(tank, 10, 0.92f)),
            CreateWave("Level_14_Wave_05", Group(normal, 14, 0.54f), Group(tank, 13, 0.88f)),
            CreateWave("Level_14_Wave_06", Group(fast, 14, 0.36f), Group(normal, 14, 0.52f), Group(tank, 12, 0.84f)),
            CreateWave("Level_14_Wave_07", Group(tank, 16, 0.80f), Group(normal, 12, 0.50f)),
            CreateWave("Level_14_Wave_08", Group(fast, 18, 0.34f), Group(tank, 15, 0.78f)),
            CreateWave("Level_14_Wave_09", Group(normal, 16, 0.48f), Group(tank, 17, 0.74f)),
            CreateWave("Level_14_Wave_10", Group(fast, 14, 0.32f), Group(normal, 14, 0.46f), Group(tank, 18, 0.72f)),
        };

        CreateLevel("Level_14", 315, new[]
        {
            "PPPPPPP",
            "111111P",
            "11PPPPP",
            "11P1111",
            "11PPPP1",
            "11111P1",
            "PPPPPP1",
            "P111111",
            "PPPPP11",
            "1111P11",
            "1111PPP",
            "111111P",
            "111111P",
        }, waves);
    }

    private static void CreateLevel15(GameObject fast, GameObject normal, GameObject tank, GameObject rhino)
    {
        WaveData[] waves =
        {
            CreateWave("Level_15_Wave_01", Group(normal, 14, 0.58f), Group(tank, 9, 1.02f)),
            CreateWave("Level_15_Wave_02", Group(fast, 16, 0.40f), Group(tank, 10, 0.96f)),
            CreateWave("Level_15_Wave_03", Group(tank, 13, 0.90f)),
            CreateWave("Level_15_Wave_04", Group(normal, 14, 0.52f), Group(tank, 12, 0.86f)),
            CreateWave("Level_15_Wave_05", Group(fast, 14, 0.36f), Group(normal, 12, 0.50f), Group(tank, 12, 0.82f)),
            CreateWave("Level_15_Wave_06", Group(tank, 16, 0.78f), Group(normal, 12, 0.48f)),
            CreateWave("Level_15_Wave_07", Group(fast, 16, 0.34f), Group(tank, 15, 0.74f)),
            CreateWave("Level_15_Wave_08", Group(normal, 16, 0.46f), Group(tank, 17, 0.70f)),
            CreateWave("Level_15_Wave_09", Group(fast, 12, 0.32f), Group(normal, 14, 0.44f), Group(tank, 18, 0.68f)),
            CreateWave("Level_15_Wave_Boss", Group(rhino, 1, 1.00f), Group(tank, 4, 1.15f)),
        };

        CreateLevel("Level_15", 325, new[]
        {
            "PPPPP11",
            "1111P11",
            "1111PPP",
            "111111P",
            "11PPPPP",
            "11P1111",
            "11PPPPP",
            "111111P",
            "PPPPPPP",
            "P111111",
            "PPPPP11",
            "1111P11",
            "1111PPP",
        }, waves);
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

        if (groupsProperty == null)
        {
            Debug.LogError("WaveData does not contain serialized field named 'enemyGroups'.");
            return wave;
        }

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

    private static void CreateLevel(string name, int startMoney, string[] rows, WaveData[] waves)
    {
        string path = $"{LevelsFolder}/{name}.asset";
        LevelData level = AssetDatabase.LoadAssetAtPath<LevelData>(path);

        if (level == null)
        {
            level = ScriptableObject.CreateInstance<LevelData>();
            AssetDatabase.CreateAsset(level, path);
        }

        level.width = 7;
        level.height = 13;
        level.rows = NormalizeRows(rows);
        level.startMoney = startMoney;

        SerializedObject serializedLevel = new SerializedObject(level);
        SerializedProperty wavesProperty = serializedLevel.FindProperty("waves");

        if (wavesProperty == null)
        {
            Debug.LogError("LevelData does not contain serialized field named 'waves'.");
            return;
        }

        wavesProperty.arraySize = waves.Length;

        for (int i = 0; i < waves.Length; i++)
        {
            wavesProperty.GetArrayElementAtIndex(i).objectReferenceValue = waves[i];
        }

        serializedLevel.ApplyModifiedProperties();
        EditorUtility.SetDirty(level);
    }

    private static string[] NormalizeRows(string[] rows)
    {
        string[] result = new string[13];

        for (int y = 0; y < result.Length; y++)
        {
            string row = y < rows.Length && !string.IsNullOrEmpty(rows[y])
                ? rows[y]
                : "1111111";

            if (row.Length < 7)
            {
                row = row.PadRight(7, '1');
            }

            if (row.Length > 7)
            {
                row = row.Substring(0, 7);
            }

            result[y] = row;
        }

        return result;
    }

    private static GameObject FindPrefabByName(string prefabName)
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
