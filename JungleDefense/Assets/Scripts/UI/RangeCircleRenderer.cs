using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RangeCircleRenderer : MonoBehaviour
{
    [SerializeField] private int segments = 96;
    [SerializeField] private float lineWidth = 0.04f;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        ConfigureLineRenderer();
    }

    public void Draw(float radius)
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            ConfigureLineRenderer();
        }

        lineRenderer.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 point = new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0f
            );

            lineRenderer.SetPosition(i, point);
        }
    }

    private void ConfigureLineRenderer()
    {
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.sortingLayerName = "Projectiles";
        lineRenderer.sortingOrder = 20;

        Material material = new Material(Shader.Find("Sprites/Default"));
        material.color = new Color(1f, 1f, 1f, 0.45f);
        lineRenderer.material = material;

        lineRenderer.startColor = new Color(1f, 1f, 1f, 0.45f);
        lineRenderer.endColor = new Color(1f, 1f, 1f, 0.45f);
    }
}
