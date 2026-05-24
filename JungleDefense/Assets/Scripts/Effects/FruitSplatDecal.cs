using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FruitSplatDecal : MonoBehaviour
{
    [SerializeField] private float lifetime = 3.5f;
    [SerializeField] private float fadeDuration = 1.0f;

    private SpriteRenderer spriteRenderer;
    private Color startColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
    }

    private void Start()
    {
        StartCoroutine(FadeRoutine());
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
            spriteRenderer.color = color;

            yield return null;
        }

        Destroy(gameObject);
    }
}
