#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class JungleDefenseLevels16To20Generator
{
    private const string LevelsFolder = "Assets/Data/Levels";
    private const string WavesFolder = "Assets/Data/Waves/Generated";
    private const string EnemiesFolder = "Assets/Data/Enemies";

    private struct Group
    {
        public GameObject prefab;
        public int count;
        public float delay;

        public Group(GameObject prefab, int count, float delay)
        {
            this.prefab = prefab;
            this.count = count;
            this.delay = delay;
        }
    }

    [MenuItem("Jungle Defense/Generate Levels 16-20")]
    public static void Generate()
    {
        EnsureFolder("Assets/Data");
        EnsureFolder(LevelsFolder);
        EnsureFolder("Assets/Data/Waves");
        EnsureFolder(WavesFolder);
        EnsureFolder(EnemiesFolder);

        GameObject fast = FindPrefabByName("Enemy_Fast");
        GameObject normal = FindPrefabByName("Enemy_Normal");
        GameObject tank = FindPrefabByName("Enemy_Tank");
        GameObject shadow = FindPrefabByName("Enemy_Shadow");
        GameObject shadowKing = FindPrefabByName("Boss_ShadowKing");

        if (fast == null || normal == null || tank == null || shadow == null || shadowKing == null)
        {
            Debug.LogError(
                "Cannot generate Levels 16-20. Required prefabs: Enemy_Fast, Enemy_Normal, Enemy_Tank, Enemy_Shadow, Boss_ShadowKing.");
            return;
        }

        TuneShadowEnemyData();
        TuneShadowKingData();

        GenerateLevel16(fast, normal, tank, shadow);
        GenerateLevel17(fast, normal, tank, shadow);
        GenerateLevel18(fast, normal, tank, shadow);
        GenerateLevel19(fast, normal, tank, shadow);
        GenerateLevel20(fast, normal, tank, shadow, shadowKing);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Generated Jungle Defense Levels 16-20 with Shadow enemies and Shadow King boss.");
    }

    private static void TuneShadowEnemyData()
    {
        EnemyData data = AssetDatabase.LoadAssetAtPath<EnemyData>($"{EnemiesFolder}/Enemy_ShadowData.asset");

        if (data == null)
        {
            return;
        }

        SerializedObject so = new SerializedObject(data);

        SetString(so, "enemyName", "Shadow");
        SetInt(so, "enemyType", 3);
        SetFloat(so, "maxHealth", 95f);
        SetFloat(so, "speed", 1f);
        SetInt(so, "reward", 8);
        SetInt(so, "damageToBase", 2);
        SetFloat(so, "directDamageMultiplier", 1f);
        SetFloat(so, "splashDamageMultiplier", 0.75f);
        SetFloat(so, "stunDurationMultiplier", 0.5f);
        SetFloat(so, "shadowPauseDuration", 1.7f);
        SetFloat(so, "shadowFinalPauseDuration", 2.6f);
        SetFloat(so, "shadowInvulnerabilityDuration", 0.65f);
        SetFloat(so, "shadowPulseScale", 0.12f);
        SetFloat(so, "shadowPulseSpeed", 1.25f);

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(data);
    }

    private static void TuneShadowKingData()
    {
        EnemyData data = AssetDatabase.LoadAssetAtPath<EnemyData>($"{EnemiesFolder}/Boss_ShadowKingData.asset");

        if (data == null)
        {
            data = ScriptableObject.CreateInstance<EnemyData>();
            AssetDatabase.CreateAsset(data, $"{EnemiesFolder}/Boss_ShadowKingData.asset");
        }

        SerializedObject so = new SerializedObject(data);

        SetString(so, "enemyName", "Shadow King");
        SetInt(so, "enemyType", 4);
        SetFloat(so, "maxHealth", 2850f);
        SetFloat(so, "speed", 0.78f);
        SetInt(so, "reward", 45);
        SetInt(so, "damageToBase", 999);
        SetFloat(so, "directDamageMultiplier", 1f);
        SetFloat(so, "splashDamageMultiplier", 0.55f);
        SetFloat(so, "stunDurationMultiplier", 0.2f);
        SetInt(so, "bossAbility", 3);
        SetFloat(so, "shadowKingTeleportHealthPercent", 0.7f);
        SetFloat(so, "shadowKingTeleportRoutePercent", 0.25f);
        SetFloat(so, "shadowKingInvulnerabilityHealthPercent", 0.15f);
        SetFloat(so, "shadowKingInvulnerabilityInterval", 3f);
        SetFloat(so, "shadowKingInvulnerabilityDuration", 1f);

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(data);
    }

    private static void GenerateLevel16(GameObject fast, GameObject normal, GameObject tank, GameObject shadow)
    {
        WaveData[] waves =
        {
            Wave("Level_16_Wave_01", G(normal, 12, 0.58f), G(fast, 10, 0.36f)),
            Wave("Level_16_Wave_02", G(tank, 8, 0.82f), G(normal, 10, 0.52f)),
            Wave("Level_16_Wave_03", G(shadow, 3, 1.25f), G(fast, 14, 0.34f)),
            Wave("Level_16_Wave_04", G(normal, 14, 0.48f), G(tank, 10, 0.74f)),
            Wave("Level_16_Wave_05", G(shadow, 4, 1.15f), G(normal, 10, 0.46f), G(fast, 12, 0.32f)),
            Wave("Level_16_Wave_06", G(tank, 13, 0.68f), G(shadow, 3, 1.10f)),
            Wave("Level_16_Wave_07", G(normal, 16, 0.42f), G(fast, 14, 0.30f), G(tank, 9, 0.66f)),
            Wave("Level_16_Wave_08", G(shadow, 5, 1.00f), G(tank, 12, 0.62f)),
            Wave("Level_16_Wave_09", G(fast, 20, 0.27f), G(normal, 14, 0.38f), G(shadow, 4, 0.95f)),
            Wave("Level_16_Wave_10", G(tank, 16, 0.58f), G(shadow, 5, 0.90f), G(normal, 14, 0.36f)),
        };

        Level("Level_16", 250, LayoutA(), waves);
    }

    private static void GenerateLevel17(GameObject fast, GameObject normal, GameObject tank, GameObject shadow)
    {
        WaveData[] waves =
        {
            Wave("Level_17_Wave_01", G(fast, 16, 0.34f), G(shadow, 3, 1.15f)),
            Wave("Level_17_Wave_02", G(normal, 14, 0.48f), G(tank, 9, 0.76f)),
            Wave("Level_17_Wave_03", G(shadow, 5, 1.05f), G(normal, 12, 0.44f)),
            Wave("Level_17_Wave_04", G(tank, 13, 0.68f), G(fast, 14, 0.30f)),
            Wave("Level_17_Wave_05", G(shadow, 6, 0.95f), G(fast, 16, 0.28f)),
            Wave("Level_17_Wave_06", G(normal, 16, 0.40f), G(tank, 12, 0.62f), G(shadow, 4, 0.92f)),
            Wave("Level_17_Wave_07", G(tank, 16, 0.58f), G(shadow, 5, 0.88f)),
            Wave("Level_17_Wave_08", G(shadow, 8, 0.78f), G(fast, 18, 0.25f)),
            Wave("Level_17_Wave_09", G(normal, 18, 0.36f), G(tank, 16, 0.52f), G(shadow, 5, 0.82f)),
            Wave("Level_17_Wave_10", G(shadow, 9, 0.72f), G(tank, 18, 0.48f), G(fast, 16, 0.24f)),
        };

        Level("Level_17", 240, LayoutB(), waves);
    }

    private static void GenerateLevel18(GameObject fast, GameObject normal, GameObject tank, GameObject shadow)
    {
        WaveData[] waves =
        {
            Wave("Level_18_Wave_01", G(tank, 10, 0.70f), G(normal, 14, 0.42f)),
            Wave("Level_18_Wave_02", G(shadow, 5, 1.00f), G(fast, 16, 0.28f)),
            Wave("Level_18_Wave_03", G(tank, 14, 0.60f), G(shadow, 4, 0.88f)),
            Wave("Level_18_Wave_04", G(shadow, 7, 0.78f)),
            Wave("Level_18_Wave_05", G(normal, 18, 0.36f), G(fast, 18, 0.24f), G(tank, 10, 0.54f)),
            Wave("Level_18_Wave_06", G(tank, 18, 0.50f), G(shadow, 5, 0.74f)),
            Wave("Level_18_Wave_07", G(shadow, 10, 0.64f), G(normal, 14, 0.34f)),
            Wave("Level_18_Wave_08", G(fast, 24, 0.22f), G(tank, 16, 0.48f)),
            Wave("Level_18_Wave_09", G(shadow, 8, 0.60f), G(tank, 18, 0.44f), G(normal, 14, 0.30f)),
            Wave("Level_18_Wave_10", G(shadow, 12, 0.52f), G(fast, 18, 0.20f)),
            Wave("Level_18_Wave_11", G(tank, 22, 0.40f), G(shadow, 7, 0.56f)),
            Wave("Level_18_Wave_12", G(shadow, 10, 0.50f), G(normal, 18, 0.28f), G(tank, 20, 0.38f)),
        };

        Level("Level_18", 230, LayoutC(), waves);
    }

    private static void GenerateLevel19(GameObject fast, GameObject normal, GameObject tank, GameObject shadow)
    {
        WaveData[] waves =
        {
            Wave("Level_19_Wave_01", G(shadow, 5, 0.90f), G(normal, 14, 0.38f)),
            Wave("Level_19_Wave_02", G(tank, 16, 0.54f), G(fast, 16, 0.24f)),
            Wave("Level_19_Wave_03", G(shadow, 8, 0.72f), G(tank, 12, 0.50f)),
            Wave("Level_19_Wave_04", G(normal, 20, 0.32f), G(shadow, 6, 0.68f)),
            Wave("Level_19_Wave_05", G(tank, 20, 0.44f)),
            Wave("Level_19_Wave_06", G(shadow, 10, 0.58f), G(fast, 18, 0.20f)),
            Wave("Level_19_Wave_07", G(tank, 20, 0.40f), G(normal, 18, 0.28f), G(shadow, 7, 0.56f)),
            Wave("Level_19_Wave_08", G(shadow, 12, 0.50f), G(tank, 18, 0.38f)),
            Wave("Level_19_Wave_09", G(fast, 24, 0.18f), G(normal, 20, 0.24f), G(shadow, 8, 0.50f)),
            Wave("Level_19_Wave_10", G(tank, 24, 0.34f), G(shadow, 10, 0.46f)),
            Wave("Level_19_Wave_11", G(shadow, 15, 0.40f), G(tank, 20, 0.32f)),
            Wave("Level_19_Wave_12", G(shadow, 12, 0.38f), G(tank, 26, 0.30f), G(fast, 18, 0.16f)),
        };

        Level("Level_19", 220, LayoutD(), waves);
    }

    private static void GenerateLevel20(GameObject fast, GameObject normal, GameObject tank, GameObject shadow, GameObject shadowKing)
    {
        WaveData[] waves =
        {
            Wave("Level_20_Wave_01", G(fast, 22, 0.22f), G(normal, 12, 0.32f)),
            Wave("Level_20_Wave_02", G(shadow, 6, 0.78f), G(normal, 16, 0.30f)),
            Wave("Level_20_Wave_03", G(tank, 18, 0.44f), G(fast, 18, 0.18f)),
            Wave("Level_20_Wave_04", G(shadow, 9, 0.58f), G(tank, 14, 0.38f)),
            Wave("Level_20_Wave_05", G(normal, 20, 0.26f), G(shadow, 8, 0.54f), G(fast, 18, 0.16f)),
            Wave("Level_20_Wave_06", G(tank, 22, 0.34f), G(shadow, 8, 0.50f)),
            Wave("Level_20_Wave_07", G(shadow, 12, 0.42f), G(normal, 18, 0.24f)),
            Wave("Level_20_Wave_08", G(fast, 26, 0.14f), G(tank, 20, 0.30f), G(shadow, 8, 0.42f)),
            Wave("Level_20_Wave_09", G(shadow, 14, 0.36f), G(tank, 22, 0.28f)),
            Wave("Level_20_Wave_10", G(normal, 24, 0.22f), G(shadow, 12, 0.34f), G(fast, 20, 0.13f)),
            Wave("Level_20_Wave_11", G(tank, 28, 0.25f), G(shadow, 15, 0.30f)),
            Wave("Level_20_Wave_Boss", G(shadowKing, 1, 1f), G(shadow, 6, 0.65f)),
        };

        Level("Level_20", 210, LayoutE(), waves);
    }

    private static Group G(GameObject prefab, int count, float delay)
    {
        return new Group(prefab, count, delay);
    }

    private static WaveData Wave(string name, params Group[] groups)
    {
        string path = $"{WavesFolder}/{name}.asset";
        WaveData wave = AssetDatabase.LoadAssetAtPath<WaveData>(path);

        if (wave == null)
        {
            wave = ScriptableObject.CreateInstance<WaveData>();
            AssetDatabase.CreateAsset(wave, path);
        }

        SerializedObject so = new SerializedObject(wave);
        SerializedProperty enemyGroups = so.FindProperty("enemyGroups");

        if (enemyGroups == null)
        {
            Debug.LogError("WaveData must contain serialized field 'enemyGroups'.");
            return wave;
        }

        enemyGroups.arraySize = groups.Length;

        for (int i = 0; i < groups.Length; i++)
        {
            SerializedProperty item = enemyGroups.GetArrayElementAtIndex(i);

            item.FindPropertyRelative("enemyPrefab").objectReferenceValue = groups[i].prefab;
            item.FindPropertyRelative("count").intValue = groups[i].count;
            item.FindPropertyRelative("delayBetweenEnemies").floatValue = groups[i].delay;
        }

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(wave);

        return wave;
    }

    private static void Level(string name, int startMoney, string[] rows, WaveData[] waves)
    {
        string path = $"{LevelsFolder}/{name}.asset";
        LevelData level = AssetDatabase.LoadAssetAtPath<LevelData>(path);

        if (level == null)
        {
            level = ScriptableObject.CreateInstance<LevelData>();
            AssetDatabase.CreateAsset(level, path);
        }

        SerializedObject so = new SerializedObject(level);

        SetInt(so, "width", 7);
        SetInt(so, "height", 13);
        SetInt(so, "startMoney", startMoney);

        SerializedProperty rowsProperty = so.FindProperty("rows");

        if (rowsProperty != null)
        {
            rowsProperty.arraySize = 13;

            for (int i = 0; i < 13; i++)
            {
                rowsProperty.GetArrayElementAtIndex(i).stringValue = NormalizeRow(rows[i]);
            }
        }

        SerializedProperty wavesProperty = so.FindProperty("waves");

        if (wavesProperty != null)
        {
            wavesProperty.arraySize = waves.Length;

            for (int i = 0; i < waves.Length; i++)
            {
                wavesProperty.GetArrayElementAtIndex(i).objectReferenceValue = waves[i];
            }
        }

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(level);
    }

    private static string[] LayoutA() => new[]
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
    };

    private static string[] LayoutB() => new[]
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
    };

    private static string[] LayoutC() => new[]
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
    };

    private static string[] LayoutD() => new[]
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
    };

    private static string[] LayoutE() => new[]
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
    };

    private static string NormalizeRow(string row)
    {
        if (string.IsNullOrEmpty(row))
        {
            return "1111111";
        }

        if (row.Length < 7)
        {
            return row.PadRight(7, '1');
        }

        if (row.Length > 7)
        {
            return row.Substring(0, 7);
        }

        return row;
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

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            return;
        }

        string parent = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
        string folder = System.IO.Path.GetFileName(path);

        if (!AssetDatabase.IsValidFolder(parent))
        {
            EnsureFolder(parent);
        }

        AssetDatabase.CreateFolder(parent, folder);
    }

    private static void SetString(SerializedObject so, string name, string value)
    {
        SerializedProperty property = so.FindProperty(name);

        if (property != null)
        {
            property.stringValue = value;
        }
    }

    private static void SetInt(SerializedObject so, string name, int value)
    {
        SerializedProperty property = so.FindProperty(name);

        if (property != null)
        {
            property.intValue = value;
        }
    }

    private static void SetFloat(SerializedObject so, string name, float value)
    {
        SerializedProperty property = so.FindProperty(name);

        if (property != null)
        {
            property.floatValue = value;
        }
    }
}
#endif
