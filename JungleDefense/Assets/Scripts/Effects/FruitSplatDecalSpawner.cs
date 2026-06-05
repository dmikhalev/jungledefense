using System.Collections.Generic;
using UnityEngine;

public static class FruitSplatDecalSpawner
{
    private static readonly Queue<FruitSplatDecal> pool = new();
    private static Sprite splatSprite;
    private static Transform poolRoot;

    public static void Spawn(Vector3 position, Color color, float size)
    {
        EnsureSprite();
        EnsureRoot();

        FruitSplatDecal decal = Get();
        Transform decalTransform = decal.transform;

        decalTransform.SetParent(poolRoot, false);
        decalTransform.position = new Vector3(position.x, position.y, 0f);
        decalTransform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        decalTransform.localScale = Vector3.one * Random.Range(size * 0.8f, size * 1.2f);

        decal.gameObject.SetActive(true);
        decal.Init(color, Release);
    }

    private static FruitSplatDecal Get()
    {
        while (pool.Count > 0)
        {
            FruitSplatDecal decal = pool.Dequeue();

            if (decal != null)
            {
                return decal;
            }
        }

        return CreateNewDecal();
    }

    private static FruitSplatDecal CreateNewDecal()
    {
        GameObject decalObject = new GameObject("FruitSplatDecal");

        SpriteRenderer renderer = decalObject.AddComponent<SpriteRenderer>();
        renderer.sprite = splatSprite;
        renderer.sortingLayerName = "Effects";
        renderer.sortingOrder = -10;

        FruitSplatDecal decal = decalObject.AddComponent<FruitSplatDecal>();
        decalObject.SetActive(false);

        return decal;
    }

    private static void Release(FruitSplatDecal decal)
    {
        if (decal == null)
        {
            return;
        }

        decal.gameObject.SetActive(false);
        decal.transform.SetParent(poolRoot, false);
        pool.Enqueue(decal);
    }

    private static void EnsureRoot()
    {
        if (poolRoot != null)
        {
            return;
        }

        GameObject root = new GameObject("FruitSplatDecalPool");
        poolRoot = root.transform;
    }

    private static void EnsureSprite()
    {
        if (splatSprite != null)
        {
            return;
        }

        const int size = 32;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;

        Vector2 center = new Vector2((size - 1) / 2f, (size - 1) / 2f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 point = new Vector2(x, y);
                float distance = Vector2.Distance(point, center);
                float normalized = distance / (size * 0.5f);

                float noise = Mathf.PerlinNoise(x * 0.22f, y * 0.22f) * 0.25f;
                float alpha = normalized < 0.62f + noise ? 1f : 0f;

                if (normalized > 0.9f)
                {
                    alpha = 0f;
                }

                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();

        splatSprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, size, size),
            new Vector2(0.5f, 0.5f),
            100f
        );
    }
}
