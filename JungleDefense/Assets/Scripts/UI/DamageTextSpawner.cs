using System.Collections.Generic;
using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance { get; private set; }

    [SerializeField] private FloatingDamageText damageTextPrefab;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 0.6f, 0f);
    [SerializeField] private int prewarmCount = 12;

    private readonly Queue<FloatingDamageText> pool = new();
    private Transform poolRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        poolRoot = transform;

        Prewarm();
    }

    private void Prewarm()
    {
        if (damageTextPrefab == null)
        {
            return;
        }

        for (int i = 0; i < prewarmCount; i++)
        {
            FloatingDamageText text = CreateNewText();
            Release(text);
        }
    }

    public void Spawn(Vector3 position, int damage)
    {
        if (damageTextPrefab == null || damage <= 0)
        {
            return;
        }

        FloatingDamageText text = Get();
        text.transform.position = position + spawnOffset;
        text.transform.rotation = Quaternion.identity;
        text.gameObject.SetActive(true);
        text.Init(damage, Release);
    }

    private FloatingDamageText Get()
    {
        while (pool.Count > 0)
        {
            FloatingDamageText text = pool.Dequeue();

            if (text != null)
            {
                return text;
            }
        }

        return CreateNewText();
    }

    private FloatingDamageText CreateNewText()
    {
        FloatingDamageText text = Instantiate(
            damageTextPrefab,
            poolRoot
        );

        text.gameObject.SetActive(false);
        return text;
    }

    private void Release(FloatingDamageText text)
    {
        if (text == null)
        {
            return;
        }

        text.gameObject.SetActive(false);
        text.transform.SetParent(poolRoot, false);
        pool.Enqueue(text);
    }
}
