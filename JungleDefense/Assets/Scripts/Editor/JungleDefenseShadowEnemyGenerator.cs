#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class JungleDefenseShadowEnemyGenerator
{
    private const string EnemiesFolder = "Assets/Data/Enemies";
    private const string PrefabsFolder = "Assets/Prefabs/Enemies";

    [MenuItem("Jungle Defense/Generate Shadow Enemy")]
    public static void Generate()
    {
        EnsureFolder("Assets/Data");
        EnsureFolder(EnemiesFolder);
        EnsureFolder("Assets/Prefabs");
        EnsureFolder(PrefabsFolder);

        GameObject sourcePrefab = FindPrefabByName("Enemy_Fast");

        if (sourcePrefab == null)
        {
            sourcePrefab = FindPrefabByName("Enemy_Normal");
        }

        if (sourcePrefab == null)
        {
            Debug.LogError("Cannot generate Shadow enemy. Expected prefab named Enemy_Fast or Enemy_Normal.");
            return;
        }

        EnemyData shadowData = CreateOrUpdateShadowData();
        CreateOrUpdateShadowPrefab(sourcePrefab, shadowData);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Generated Enemy_ShadowData and Enemy_Shadow prefab.");
    }

    private static EnemyData CreateOrUpdateShadowData()
    {
        string path = $"{EnemiesFolder}/Enemy_ShadowData.asset";

        EnemyData data = AssetDatabase.LoadAssetAtPath<EnemyData>(path);

        if (data == null)
        {
            data = ScriptableObject.CreateInstance<EnemyData>();
            AssetDatabase.CreateAsset(data, path);
        }

        data.enemyName = "Shadow";
        data.enemyType = EnemyType.Shadow;
        data.sprite = FindSprite("shadow");
        data.maxHealth = 45f;
        data.speed = 1f;
        data.reward = 12;
        data.damageToBase = 1;

        data.directDamageMultiplier = 1f;
        data.splashDamageMultiplier = 0.75f;
        data.stunDurationMultiplier = 0.5f;

        data.shadowPauseDuration = 2f;
        data.shadowFinalPauseDuration = 3f;
        data.shadowInvulnerabilityDuration = 0.75f;
        data.shadowPulseScale = 0.12f;
        data.shadowPulseSpeed = 1f;

        EditorUtility.SetDirty(data);
        return data;
    }

    private static void CreateOrUpdateShadowPrefab(GameObject sourcePrefab, EnemyData shadowData)
    {
        string path = $"{PrefabsFolder}/Enemy_Shadow.prefab";

        if (!File.Exists(path))
        {
            PrefabUtility.SaveAsPrefabAsset(sourcePrefab, path);
        }

        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);

        Enemy enemy = prefabRoot.GetComponent<Enemy>();

        if (enemy == null)
        {
            Debug.LogError("Enemy_Shadow prefab has no Enemy component.");
            PrefabUtility.UnloadPrefabContents(prefabRoot);
            return;
        }

        SerializedObject serializedEnemy = new SerializedObject(enemy);
        SerializedProperty dataProperty = serializedEnemy.FindProperty("data");

        if (dataProperty != null)
        {
            dataProperty.objectReferenceValue = shadowData;
        }
        else
        {
            Debug.LogError("Enemy component does not have serialized field named 'data'.");
        }

        serializedEnemy.ApplyModifiedProperties();

        SpriteRenderer spriteRenderer = prefabRoot.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.45f, 0.2f, 0.85f, 1f);
        }

        prefabRoot.transform.localScale = Vector3.one * 0.32f;

        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
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
