using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RangeCircleRenderer : MonoBehaviour
{
    private static Material sharedMaterial;

    [SerializeField] private int segments = 72;
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

        lineRenderer.sharedMaterial = GetSharedMaterial();

        Color color = new Color(1f, 1f, 1f, 0.45f);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    private static Material GetSharedMaterial()
    {
        if (sharedMaterial == null)
        {
            sharedMaterial = new Material(Shader.Find("Sprites/Default"))
            {
                color = new Color(1f, 1f, 1f, 0.45f)
            };
        }

        return sharedMaterial;
    }
}
