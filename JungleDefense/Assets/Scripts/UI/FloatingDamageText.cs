using TMPro;
using UnityEngine;

public class FloatingDamageText : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float lifetime = 0.6f;
    [SerializeField] private float moveSpeed = 1.2f;
    [SerializeField] private float fadeSpeed = 2f;

    private float timer;
    private Color startColor;

    private void Awake()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshPro>();
        }

        startColor = text.color;
    }

    public void Init(int damage)
    {
        text.text = "-" + damage;
        timer = lifetime;
    }

    private void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        timer -= Time.deltaTime;

        float alpha = Mathf.Clamp01(timer * fadeSpeed);

        Color color = startColor;
        color.a = alpha;
        text.color = color;

        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }
}