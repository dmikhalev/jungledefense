using TMPro;
using UnityEngine;

public class LivesUI : MonoBehaviour
{
    public TextMeshProUGUI livesText;

    void Update()
    {
        livesText.text = "Lives: " + GameManager.Instance.lives;
    }
}