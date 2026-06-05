using System;
using TMPro;
using UnityEngine;

public class FloatingDamageText : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float lifetime = 0.6f;
    [SerializeField] private float moveSpeed = 1.2f;
    [SerializeField] private float fadeSpeed = 2f;

    private Transform cachedTransform;
    private Action<FloatingDamageText> releaseCallback;
    private float timer;
    private Color startColor;

    private void Awake()
    {
        cachedTransform = transform;

        if (text == null)
        {
            text = GetComponentInChildren<TextMeshPro>();
        }

        if (text != null)
        {
            startColor = text.color;
        }
    }

    public void Init(int damage, Action<FloatingDamageText> onRelease)
    {
        releaseCallback = onRelease;

        if (text != null)
        {
            text.text = "-" + damage;
            text.color = startColor;
        }

        timer = lifetime;
    }

    private void Update()
    {
        cachedTransform.position += Vector3.up * moveSpeed * Time.deltaTime;

        timer -= Time.deltaTime;

        if (text != null)
        {
            float alpha = Mathf.Clamp01(timer * fadeSpeed);
            Color color = startColor;
            color.a = alpha;
            text.color = color;
        }

        if (timer <= 0f)
        {
            releaseCallback?.Invoke(this);
        }
    }
}
