#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class JungleDefenseShadowCampaignInjector
{
    private const string WavesFolder = "Assets/Data/Waves/Generated";
    private const string LevelsFolder = "Assets/Data/Levels";

    [MenuItem("Jungle Defense/Apply Shadow Balance Levels 11-15")]
    public static void Apply()
    {
        GameObject normal = FindPrefab("Enemy_Normal");
        GameObject fast = FindPrefab("Enemy_Fast");
        GameObject tank = FindPrefab("Enemy_Tank");
        GameObject shadow = FindPrefab("Enemy_Shadow");
        GameObject rhino = FindPrefab("Boss_Rhino");

        if (normal == null || fast == null || tank == null)
        {
            Debug.LogError("Cannot apply level balance. Missing Enemy_Normal, Enemy_Fast, or Enemy_Tank prefab.");
            return;
        }

        if (shadow == null)
        {
            Debug.LogWarning("Enemy_Shadow prefab was not found. Levels will be strengthened, but Shadow groups will be skipped.");
        }

        SetStartMoney(11, 210);
        SetStartMoney(12, 215);
        SetStartMoney(13, 220);
        SetStartMoney(14, 225);
        SetStartMoney(15, 230);

        SetWave(11, 1, G(normal, 8, 1.00f), G(tank, 6, 1.25f));
        SetWave(11, 2, G(fast, 10, 0.65f), G(tank, 8, 1.20f));
        SetWave(11, 3, G(normal, 10, 0.95f), G(tank, 9, 1.15f));
        SetWave(11, 4, G(tank, 12, 1.10f));
        SetWave(11, 5, G(fast, 8, 0.65f), G(normal, 8, 0.95f), G(tank, 10, 1.05f), GS(shadow, 1, 2.0f));
        SetWave(11, 6, G(tank, 14, 1.00f), G(normal, 8, 0.90f));
        SetWave(11, 7, G(fast, 10, 0.58f), G(tank, 13, 0.98f));
        SetWave(11, 8, G(normal, 10, 0.85f), G(tank, 15, 0.92f), GS(shadow, 1, 2.2f));

        SetWave(12, 1, G(tank, 8, 1.20f), G(fast, 8, 0.65f));
        SetWave(12, 2, G(normal, 10, 0.95f), G(tank, 9, 1.15f));
        SetWave(12, 3, G(fast, 10, 0.60f), G(tank, 11, 1.05f));
        SetWave(12, 4, G(tank, 14, 1.00f));
        SetWave(12, 5, G(normal, 9, 0.88f), G(fast, 8, 0.55f), G(tank, 12, 0.98f), GS(shadow, 1, 2.0f));
        SetWave(12, 6, G(tank, 15, 0.94f), G(normal, 8, 0.86f));
        SetWave(12, 7, G(fast, 10, 0.52f), G(tank, 15, 0.90f));
        SetWave(12, 8, G(normal, 10, 0.82f), G(tank, 16, 0.86f));
        SetWave(12, 9, G(fast, 8, 0.50f), G(normal, 8, 0.78f), G(tank, 18, 0.82f), GS(shadow, 1, 2.1f));

        SetWave(13, 1, G(fast, 8, 0.58f), G(normal, 8, 0.88f), G(tank, 10, 1.05f));
        SetWave(13, 2, G(tank, 13, 0.98f), G(normal, 8, 0.82f));
        SetWave(13, 3, G(fast, 12, 0.52f), G(tank, 10, 0.95f), GS(shadow, 1, 2.0f));
        SetWave(13, 4, G(tank, 16, 0.90f));
        SetWave(13, 5, G(normal, 10, 0.78f), G(tank, 15, 0.86f));
        SetWave(13, 6, G(fast, 10, 0.48f), G(tank, 16, 0.82f));
        SetWave(13, 7, G(normal, 10, 0.74f), G(tank, 18, 0.78f));
        SetWave(13, 8, G(fast, 8, 0.45f), G(normal, 8, 0.70f), G(tank, 18, 0.74f), GS(shadow, 1, 2.1f));
        SetWave(13, 9, G(tank, 22, 0.70f), G(fast, 8, 0.42f));

        SetWave(14, 1, G(tank, 12, 1.00f), G(fast, 8, 0.55f));
        SetWave(14, 2, G(normal, 10, 0.82f), G(tank, 13, 0.92f));
        SetWave(14, 3, G(tank, 17, 0.86f), GS(shadow, 1, 2.0f));
        SetWave(14, 4, G(fast, 10, 0.48f), G(tank, 16, 0.82f));
        SetWave(14, 5, G(normal, 10, 0.74f), G(tank, 18, 0.78f));
        SetWave(14, 6, G(fast, 8, 0.44f), G(normal, 8, 0.70f), G(tank, 18, 0.74f), GS(shadow, 1, 2.1f));
        SetWave(14, 7, G(tank, 22, 0.68f), G(normal, 8, 0.66f));
        SetWave(14, 8, G(fast, 10, 0.40f), G(tank, 21, 0.66f));
        SetWave(14, 9, G(normal, 10, 0.62f), G(tank, 24, 0.62f));
        SetWave(14, 10, G(fast, 8, 0.38f), G(normal, 8, 0.58f), G(tank, 24, 0.60f), GS(shadow, 1, 2.2f));

        SetWave(15, 1, G(normal, 10, 0.80f), G(tank, 14, 0.90f));
        SetWave(15, 2, G(fast, 10, 0.48f), G(tank, 15, 0.82f));
        SetWave(15, 3, G(tank, 20, 0.76f), GS(shadow, 1, 2.0f));
        SetWave(15, 4, G(normal, 10, 0.70f), G(tank, 18, 0.72f));
        SetWave(15, 5, G(fast, 8, 0.42f), G(normal, 8, 0.66f), G(tank, 19, 0.68f));
        SetWave(15, 6, G(tank, 24, 0.62f), G(normal, 8, 0.60f));
        SetWave(15, 7, G(fast, 10, 0.38f), G(tank, 23, 0.58f), GS(shadow, 1, 2.1f));
        SetWave(15, 8, G(normal, 10, 0.56f), G(tank, 26, 0.54f));
        SetWave(15, 9, G(fast, 8, 0.34f), G(normal, 8, 0.52f), G(tank, 26, 0.50f));

        if (rhino != null)
        {
            SetWave("Level_15_Wave_Boss", G(rhino, 1, 1.0f), G(tank, 6, 1.05f));
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Applied stronger levels 11-15 with Shadow enemy groups.");
    }

    private static WaveEnemyGroup G(GameObject prefab, int count, float delay)
    {
        return new WaveEnemyGroup
        {
            enemyPrefab = prefab,
            count = count,
            delayBetweenEnemies = delay
        };
    }

    private static WaveEnemyGroup GS(GameObject prefab, int count, float delay)
    {
        if (prefab == null)
        {
            return null;
        }

        return G(prefab, count, delay);
    }

    private static void SetStartMoney(int levelNumber, int startMoney)
    {
        LevelData level = AssetDatabase.LoadAssetAtPath<LevelData>($"{LevelsFolder}/Level_{levelNumber}.asset");

        if (level == null)
        {
            Debug.LogWarning($"Level_{levelNumber}.asset not found.");
            return;
        }

        level.startMoney = startMoney;
        EditorUtility.SetDirty(level);
    }

    private static void SetWave(int levelNumber, int waveNumber, params WaveEnemyGroup[] groups)
    {
        SetWave($"Level_{levelNumber}_Wave_{waveNumber:00}", groups);
    }

    private static void SetWave(string waveName, params WaveEnemyGroup[] groups)
    {
        WaveData wave = AssetDatabase.LoadAssetAtPath<WaveData>($"{WavesFolder}/{waveName}.asset");

        if (wave == null)
        {
            Debug.LogWarning($"{waveName}.asset not found.");
            return;
        }

        List<WaveEnemyGroup> validGroups = new();

        foreach (WaveEnemyGroup group in groups)
        {
            if (group != null && group.enemyPrefab != null)
            {
                validGroups.Add(group);
            }
        }

        SerializedObject serializedWave = new SerializedObject(wave);
        SerializedProperty groupsProperty = serializedWave.FindProperty("enemyGroups");

        groupsProperty.arraySize = validGroups.Count;

        for (int i = 0; i < validGroups.Count; i++)
        {
            SerializedProperty item = groupsProperty.GetArrayElementAtIndex(i);
            item.FindPropertyRelative("enemyPrefab").objectReferenceValue = validGroups[i].enemyPrefab;
            item.FindPropertyRelative("count").intValue = validGroups[i].count;
            item.FindPropertyRelative("delayBetweenEnemies").floatValue = validGroups[i].delayBetweenEnemies;
        }

        serializedWave.ApplyModifiedProperties();
        EditorUtility.SetDirty(wave);
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
}
#endif
