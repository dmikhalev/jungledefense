using TMPro;
using UnityEngine;

public class LivesUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI livesText;

    private void Update()
    {
        if (GameManager.Instance == null || livesText == null)
        {
            return;
        }

        livesText.text = $"Lives: {GameManager.Instance.lives}";
    }
}
