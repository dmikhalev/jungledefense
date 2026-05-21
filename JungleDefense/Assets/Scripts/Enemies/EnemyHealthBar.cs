using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Transform fill;

    private float originalFillScaleX;
    private float originalFillPositionX;

    private void Awake()
    {
        if (fill != null)
        {
            originalFillScaleX = fill.transform.localScale.x;
            originalFillPositionX = fill.transform.localPosition.x;
        }

        Hide();
    }

    public void SetHealth(float currentHealth, float maxHealth)
    {
        if (root == null || fill == null || maxHealth <= 0f)
        {
            return;
        }

        float normalized = Mathf.Clamp01(currentHealth / maxHealth);

        if (normalized >= 1f)
        {
            Hide();
            return;
        }

        root.SetActive(true);

        Vector3 scale = fill.transform.localScale;
        scale.x = originalFillScaleX * normalized;
        fill.transform.localScale = scale;

        Vector3 position = fill.transform.localPosition;
        position.x = originalFillPositionX - (originalFillScaleX * (1f - normalized)) / 2f;
        fill.transform.localPosition = position;
    }

    public void Hide()
    {
        if (root != null)
        {
            root.SetActive(false);
        }
    }
}