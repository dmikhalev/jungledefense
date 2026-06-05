using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FruitSplatDecal : MonoBehaviour
{
    [SerializeField] private float lifetime = 3.5f;
    [SerializeField] private float fadeDuration = 1.0f;

    private SpriteRenderer spriteRenderer;
    private Coroutine fadeRoutine;
    private Action<FruitSplatDecal> releaseCallback;
    private Color startColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(Color color, Action<FruitSplatDecal> onRelease)
    {
        releaseCallback = onRelease;
        startColor = color;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = startColor;
        }

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        float visibleTime = Mathf.Max(0f, lifetime - fadeDuration);

        if (visibleTime > 0f)
        {
            yield return new WaitForSeconds(visibleTime);
        }

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);

            Color color = startColor;
            color.a = Mathf.Lerp(startColor.a, 0f, t);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }

            yield return null;
        }

        fadeRoutine = null;
        releaseCallback?.Invoke(this);
    }
}
