using System;
using System.Collections;
using UnityEngine;

public class EnemyDeathFeedback : MonoBehaviour
{
    [SerializeField] private float duration = 0.22f;
    [SerializeField] private float squashX = 1.18f;
    [SerializeField] private float squashY = 0.72f;

    private SpriteRenderer[] renderers;
    private Color[] originalColors;
    private Vector3 originalScale;
    private Coroutine routine;

    private void Awake()
    {
        CacheVisualState();
    }

    public void Play()
    {
        Play(null);
    }

    public void Play(Action onComplete)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }

        CacheVisualState();
        DisableColliders();
        routine = StartCoroutine(PlayRoutine(onComplete));
    }

    private IEnumerator PlayRoutine(Action onComplete)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            float eased = 1f - Mathf.Pow(1f - t, 2f);

            transform.localScale = new Vector3(
                originalScale.x * Mathf.Lerp(1f, squashX, eased),
                originalScale.y * Mathf.Lerp(1f, squashY, eased),
                originalScale.z
            );

            float alpha = 1f - eased;

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null)
                {
                    continue;
                }

                Color color = originalColors[i];
                color.a *= alpha;
                renderers[i].color = color;
            }

            yield return null;
        }

        routine = null;

        if (onComplete != null)
        {
            onComplete.Invoke();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CacheVisualState()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].color;
        }

        originalScale = transform.localScale;
    }

    private void DisableColliders()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
    }
}
