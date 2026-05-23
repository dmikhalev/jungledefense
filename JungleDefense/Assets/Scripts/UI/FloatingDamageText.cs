using TMPro;
using UnityEngine;

public class FloatingDamageText : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float lifetime = 0.6f;
    [SerializeField] private float moveSpeed = 1.2f;
    [SerializeField] private float fadeSpeed = 2f;

    private Transform cachedTransform;
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

    public void Init(int damage)
    {
        if (text != null)
        {
            text.text = "-" + damage;
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
            Destroy(gameObject);
        }
    }
}
