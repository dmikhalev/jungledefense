using UnityEngine;

public class LevelBackgroundManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundRenderer;

    public void SetBackground(Sprite sprite)
    {
        if (backgroundRenderer == null)
        {
            Debug.LogError("Background Renderer is not assigned");
            return;
        }

        backgroundRenderer.sprite = sprite;
        backgroundRenderer.gameObject.SetActive(sprite != null);
    }
}